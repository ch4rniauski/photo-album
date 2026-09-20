namespace photo_album.Infrastructure.Storage;

internal sealed class ImageStorageSettings
{
    private const string BasePathEnvVar = "IMAGE_STORAGE_PATH";

    public string BasePath { get; init; } = string.Empty;

    public string OriginalsPath => Path.Combine(BasePath, "originals");

    public string ThumbnailsPath => Path.Combine(BasePath, "thumbnails");

    public static ImageStorageSettings FromEnvironment()
    {
        var configuredPath = Environment.GetEnvironmentVariable(BasePathEnvVar);

        return new ImageStorageSettings
        {
            BasePath = string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "storage", "images")
                : configuredPath
        };
    }
}
