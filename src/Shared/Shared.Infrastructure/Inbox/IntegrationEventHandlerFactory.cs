using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts;
using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Infrastructure.Inbox;

public static class IntegrationEventHandlerFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> _handlersDict = new();

    public static IEnumerable<IIntegrationEventHandler<TEvent>> GetHandlers<TEvent>(
        IServiceProvider serviceProvider,
        params Assembly[] assemblies)
        where TEvent : IntegrationEvent
    {
        var eventType = typeof(TEvent);
        var handlerTypes = _handlersDict.GetOrAdd(
            eventType.FullName ?? throw new ArgumentException("Type has no full name.", nameof(eventType)),
            _ =>
            {
                var handlerInterfaceType = typeof(IIntegrationEventHandler<TEvent>);
                var types = assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && handlerInterfaceType.IsAssignableFrom(t))
                    .Distinct()
                    .ToArray();
                return types;
            });

        if (!handlerTypes.Any())
        {
            return Enumerable.Empty<IIntegrationEventHandler<TEvent>>();
        }

        var handlers = new List<IIntegrationEventHandler<TEvent>>();
        var handlerTypeSet = new HashSet<Type>();
        foreach (var handlerType in handlerTypes)
        {
            if (!handlerTypeSet.Add(handlerType))
            {
                continue;
            }

            var handlerInterface = typeof(IIntegrationEventHandler<TEvent>);
            var handlerInstances = serviceProvider.GetServices(handlerInterface)
                .OfType<IIntegrationEventHandler<TEvent>>()
                .ToList();

            if (!handlerInstances.Any())
            {
                continue;
            }

            foreach (var handler in handlerInstances)
            {
                handlers.Add(handler);
            }
        }

        var uniqueHandlers = handlers
            .GroupBy(h => h.GetType().GetProperty("InnerHandler")?.GetValue(h)?.GetType() ?? h.GetType())
            .Select(g => g.First())
            .ToList();

        return uniqueHandlers;
    }
}