﻿namespace NotificationService.Core;

public class EmailSettings
{
    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string? FromName { get; set; }
}
