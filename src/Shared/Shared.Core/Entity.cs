using System.Text.Json.Serialization;

namespace Shared.Core;

public class Entity<T>
{
    [JsonInclude]
    public T Id { get; protected set; }
}
