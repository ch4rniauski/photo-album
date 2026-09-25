using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using photo_album.Application.Contracts.Storage;

namespace photo_album.Infrastructure.Storage;

internal sealed class LocalImageStorage : IImageStorage
{
    private const int ThumbnailMaxSize = 300;

    private readonly ImageStorageSettings _settings;

    public LocalImageStorage(ImageStorageSettings settings)
    {
        _settings = settings;
    }

    public async Task SaveAsync(
        Stream content,
        string storedFileName,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_settings.OriginalsPath);
        Directory.CreateDirectory(_settings.ThumbnailsPath);

        var originalPath = Path.Combine(_settings.OriginalsPath, storedFileName);
        var thumbnailPath = Path.Combine(_settings.ThumbnailsPath, storedFileName);

        await using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        await using (var originalFileStream = File.Create(originalPath))
        {
            await memoryStream.CopyToAsync(originalFileStream, cancellationToken);
        }

        memoryStream.Position = 0;

        var thumbnailSaved = false;

        try
        {
            using var image = await Image.LoadAsync(memoryStream, cancellationToken);

            image.Mutate(context => context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(ThumbnailMaxSize, ThumbnailMaxSize)
            }));

            await image.SaveAsync(thumbnailPath, cancellationToken);
            thumbnailSaved = true;
        }
        catch (UnknownImageFormatException exception)
        {
            throw new InvalidDataException("Uploaded file is not a valid image", exception);
        }
        finally
        {
            if (!thumbnailSaved)
            {
                if (File.Exists(originalPath))
                {
                    File.Delete(originalPath);
                }

                if (File.Exists(thumbnailPath))
                {
                    File.Delete(thumbnailPath);
                }
            }
        }
    }

    public Stream? OpenOriginal(string storedFileName)
    {
        var originalPath = Path.Combine(_settings.OriginalsPath, storedFileName);

        if (!File.Exists(originalPath))
        {
            return null;
        }

        return File.OpenRead(originalPath);
    }

    public Stream? OpenThumbnail(string storedFileName)
    {
        var thumbnailPath = Path.Combine(_settings.ThumbnailsPath, storedFileName);

        if (!File.Exists(thumbnailPath))
        {
            return null;
        }

        return File.OpenRead(thumbnailPath);
    }
}
