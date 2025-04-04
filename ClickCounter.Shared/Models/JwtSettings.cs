﻿namespace ClickCounter.Shared.Models;

public sealed class JwtSettings {
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenLifetimeInMinutes { get; set; }
    public int RefreshTokenLifetimeInDays { get; set; }
}
