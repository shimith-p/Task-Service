namespace TaskService.Application.Settings
{
    /// <summary>
    /// Strongly-typed options bound from the "Jwt" section in appsettings.json.
    /// Injected via IOptions&lt;JwtSettings&gt; — never hardcode secrets in code.
    /// </summary>
    public sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        /// <summary>HMAC-SHA256 signing secret. Min 32 chars. Store in secrets/vault in production.</summary>
        public string SecretKey { get; init; } = string.Empty;

        /// <summary>Token issuer — identifies who created the token (your API).</summary>
        public string Issuer { get; init; } = string.Empty;

        /// <summary>Token audience — identifies who the token is intended for (your clients).</summary>
        public string Audience { get; init; } = string.Empty;

        /// <summary>How long the access token is valid. Default: 60 minutes.</summary>
        public int ExpiryMinutes { get; init; } = 60;

        /// <summary>How long the refresh token is valid. Default: 7 days.</summary>
        public int RefreshExpiryDays { get; init; } = 7;
    }
}
