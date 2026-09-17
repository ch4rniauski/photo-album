namespace photo_album.Infrastructure.PostgreSql;

internal sealed class PostgresSettings
{
    private const string HostEnvVar = "POSTGRES_HOST";
    private const string PortEnvVar = "POSTGRES_PORT";
    private const string DatabaseEnvVar = "POSTGRES_DB";
    private const string UsernameEnvVar = "POSTGRES_USER";
    private const string PasswordEnvVar = "POSTGRES_PASSWORD";

    private string Host { get; init; } = string.Empty;
    private string Port { get; init; } = string.Empty;
    private string Database { get; init; } = string.Empty;
    private string Username { get; init; } = string.Empty;
    private string Password { get; init; } = string.Empty;

    public string ConnectionString 
        => $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";

    public static PostgresSettings FromEnvironment()
    {
        return new PostgresSettings
        {
            Host = GetRequired(HostEnvVar),
            Port = GetRequired(PortEnvVar),
            Database = GetRequired(DatabaseEnvVar),
            Username = GetRequired(UsernameEnvVar),
            Password = GetRequired(PasswordEnvVar)
        };
    }

    private static string GetRequired(string name)
    {
        return Environment.GetEnvironmentVariable(name)
               ?? throw new InvalidOperationException($"Environment variable '{name}' is not set");
    }
}
