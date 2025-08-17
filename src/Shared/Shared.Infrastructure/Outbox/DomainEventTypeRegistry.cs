using Shared.Core;
using System.Reflection;

namespace Shared.Infrastructure.Outbox;

public static class DomainEventTypeRegistry
{
    private static readonly Dictionary<string, Type> EventTypes = new();

    public static void RegisterAllDomainEvents(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var eventTypes = assembly.GetTypes()
                .Where(t => typeof(DomainEvent).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var eventType in eventTypes)
            {
                EventTypes[eventType.AssemblyQualifiedName!] = eventType;
            }
        }
    }

    public static Type Resolve(string typeName)
    {
        return EventTypes.TryGetValue(typeName, out var eventType)
               ? eventType
               : throw new InvalidOperationException($"Type '{typeName}' is not registered");
    }
}