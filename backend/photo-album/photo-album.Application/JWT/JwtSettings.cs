namespace photo_album.Application.JWT;

internal sealed class JwtSettings
{
    private const string SecurityKeyEnvVar = "JWT_SECURITY_KEY";
    private const string ExpiresInMinutesEnvVar = "JWT_EXPIRES_IN_MINUTES";

    public string SecurityKey { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; }

    public static JwtSettings FromEnvironment()
    {
        var expiresInMinutesValue = GetRequired(ExpiresInMinutesEnvVar);

        if (!int.TryParse(expiresInMinutesValue, out var expiresInMinutes))
        {
            throw new InvalidOperationException(
                $"Environment variable '{ExpiresInMinutesEnvVar}' must be a valid integer");
        }

        return new JwtSettings
        {
            SecurityKey = GetRequired(SecurityKeyEnvVar),
            ExpiresInMinutes = expiresInMinutes
        };
    }

    private static string GetRequired(string name)
    {
        return Environment.GetEnvironmentVariable(name)
               ?? throw new InvalidOperationException($"Environment variable '{name}' is not set");
    }
}
