﻿namespace Shared.Infrastructure.Inbox;

internal class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;
    public string VHost { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
