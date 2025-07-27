using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Shared.Application;
using System.Reflection;

namespace Shared.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob<T> : IJob where T : DbContext
{
    private readonly T _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly Assembly[] _handlerAssemblies;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob<T>> _logger;

    public ProcessOutboxMessagesJob(
        T dbContext,
        IServiceProvider serviceProvider,
        Assembly[] handlerAssemblies,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob<T>> logger)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _handlerAssemblies = handlerAssemblies;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _dbContext.Set<OutboxMessage>()
                                .Where(x => x.ProcessedOn == null)
                                .OrderBy(x => x.OccurredOn)
                                .Take(_outboxOptions.MessagesPerPoll)
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

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var eventType = DomainEventTypeRegistry.Resolve(message.EventType);
            if (eventType == null)
            {
                _logger.LogWarning($"Event type '{message.EventType}' could not be resolved for message Id '{message.Id}'.");
                return;
            }

            var method = typeof(DomainEventHandlerFactory).GetMethod(nameof(DomainEventHandlerFactory.GetHandlers))!
                                                        .MakeGenericMethod(eventType);
            var handlers = (IEnumerable<object>)method.Invoke(null, [scope.ServiceProvider, _handlerAssemblies])!;
            var handlerList = handlers.Cast<IDomainEventHandler>()
                .GroupBy(h => h.GetType().GetProperty("InnerHandler")?.GetValue(h)?.GetType() ?? h.GetType())
                .Select(g => g.First())
                .ToList();

            var domainEvent = message.DeserializeContent();
            foreach (var handler in handlerList)
            {
                var innerHandlerType = handler.GetType().GetProperty("InnerHandler")?.GetValue(handler)?.GetType()?.Name ?? handler.GetType().Name;
                await handler.Handle(domainEvent, cancellationToken);
            }

            message.ProcessedOn = DateTime.UtcNow;
            _logger.LogInformation("Processed outbox message with Id '{MessageId}'.", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process outbox message with Id '{MessageId}'.", message.Id);
            throw;
        }
    }
}
