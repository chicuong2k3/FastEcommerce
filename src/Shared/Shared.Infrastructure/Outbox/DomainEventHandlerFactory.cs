using Microsoft.Extensions.DependencyInjection;
using Shared.Application;
using Shared.Core;
using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Infrastructure.Outbox;

public static class DomainEventHandlerFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> _handlersDict = new();

    public static IEnumerable<IDomainEventHandler> GetHandlers<TEvent>(
        IServiceProvider serviceProvider,
        params Assembly[] assemblies)
        where TEvent : DomainEvent
    {
        var eventType = typeof(TEvent);
        var handlerTypes = _handlersDict.GetOrAdd(
            eventType.FullName ?? throw new ArgumentException("Type has no full name.", nameof(eventType)),
            _ =>
            {
                var handlerInterfaceType = typeof(IDomainEventHandler<TEvent>);
                return assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && handlerInterfaceType.IsAssignableFrom(t))
                    .Distinct()
                    .ToArray();
            });

        if (!handlerTypes.Any())
        {
            return Enumerable.Empty<IDomainEventHandler<TEvent>>();
        }

        var handlers = new List<IDomainEventHandler>();
        var handlerTypeSet = new HashSet<Type>();
        foreach (var handlerType in handlerTypes)
        {
            if (!handlerTypeSet.Add(handlerType))
            {
                continue;
            }

            var handlerInterface = typeof(IDomainEventHandler<TEvent>);
            var handlerInstances = serviceProvider.GetServices(handlerInterface)
                .OfType<IDomainEventHandler<TEvent>>()
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
