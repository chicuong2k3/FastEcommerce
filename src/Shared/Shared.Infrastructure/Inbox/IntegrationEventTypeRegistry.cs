using Shared.Contracts;
using System.Reflection;

namespace Shared.Infrastructure.Inbox;

public static class IntegrationEventTypeRegistry
{
    private static readonly Dictionary<string, Type> EventTypes = new();

    public static void RegisterAllIntegrationEvents(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var eventTypes = assembly.GetTypes()
                .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t) && !t.IsAbstract);

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