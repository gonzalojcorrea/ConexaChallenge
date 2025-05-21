namespace Infrastructure.Configurations.Authentication;

/// <summary>
/// Configuration settings for JWT authentication.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string SecretKey { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int ExpiresInMinutes { get; init; }
}
