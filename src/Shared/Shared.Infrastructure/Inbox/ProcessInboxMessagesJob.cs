using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Reflection;

namespace Shared.Infrastructure.Inbox;

[DisallowConcurrentExecution]
public class ProcessInboxMessagesJob<T> : IJob where T : DbContext
{
    private readonly T _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly Assembly[] _handlerAssemblies;
    private readonly InboxOptions _inboxOptions;
    private readonly ILogger<ProcessInboxMessagesJob<T>> _logger;

    public ProcessInboxMessagesJob(
        T dbContext,
        IServiceProvider serviceProvider,
        Assembly[] handlerAssemblies,
        IOptions<InboxOptions> inboxOptions,
        ILogger<ProcessInboxMessagesJob<T>> logger)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _handlerAssemblies = handlerAssemblies;
        _inboxOptions = inboxOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _dbContext.Set<InboxMessage>()
                                .Where(x => x.ProcessedOn == null)
                                .OrderBy(x => x.OccurredOn)
                                .Take(_inboxOptions.MessagesPerPoll)
                                .ToListAsync(context.CancellationToken);

        if (!messages.Any())
        {
            return;
        }
        using var transaction = await _dbContext.Database.BeginTransactionAsync(context.CancellationToken);

        try
        {
            foreach (var message in messages)
            {
                await ProcessMessageAsync(message, context.CancellationToken);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            await transaction.CommitAsync(context.CancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(context.CancellationToken);
            throw;
        }
    }

    private async Task ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var integrationEvent = message.DeserializeContent();
            var eventType = integrationEvent.GetType();

            var method = typeof(IntegrationEventHandlerFactory).GetMethod(nameof(IntegrationEventHandlerFactory.GetHandlers))!
                .MakeGenericMethod(eventType);
            var handlers = (IEnumerable<object>)method.Invoke(null, [_serviceProvider, _handlerAssemblies])!;
            var handlerList = handlers.Cast<IIntegrationEventHandler>()
                .GroupBy(h => h.GetType().GetProperty("InnerHandler")?.GetValue(h)?.GetType() ?? h.GetType())
                .Select(g => g.First())
                .ToList();

            if (!handlerList.Any())
            {
                _logger.LogWarning($"No handlers found for event type '{eventType.Name}' for message Id '{message.Id}'.");
                return;
            }

            foreach (var handler in handlerList)
            {
                var innerHandlerType = handler.GetType().GetProperty("InnerHandler")?.GetValue(handler)?.GetType()?.Name ?? handler.GetType().Name;
                _logger.LogInformation($"Calling handler {innerHandlerType} for event {eventType.Name} (Id: {((dynamic)integrationEvent).Id})");
                await handler.Handle(integrationEvent, cancellationToken);
            }

            message.ProcessedOn = DateTime.UtcNow;
            _logger.LogInformation("Processed inbox message with Id '{MessageId}'.", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process inbox message with Id '{MessageId}'.", message.Id);
            throw;
        }
    }
}
