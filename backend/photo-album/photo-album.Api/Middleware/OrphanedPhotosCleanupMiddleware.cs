using photo_album.Application.Contracts.Repositories;
using photo_album.Application.Contracts.Storage;

namespace photo_album.Api.Middleware;

public static class OrphanedPhotosCleanupMiddleware
{
    public static async Task RunOnceAsync(
        IApplicationBuilder app,
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var photoRepository = scope.ServiceProvider.GetRequiredService<IPhotoRepository>();
        var imageStorage = scope.ServiceProvider.GetRequiredService<IImageStorage>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(OrphanedPhotosCleanupMiddleware));

        var photos = await photoRepository.GetAllAsync(cancellationToken);

        var orphanedPhotos = photos
            .Where(photo => !imageStorage.ExistsOriginal(photo.OriginalFileName))
            .ToList();

        if (orphanedPhotos.Count == 0)
        {
            logger.LogInformation("Orphaned photos cleanup: no missing files found");
            return;
        }

        var deletedCount = await photoRepository.DeleteRangeAsync(
            orphanedPhotos,
            cancellationToken);

        logger.LogInformation(
            "Orphaned photos cleanup: removed {DeletedCount} photo record(s) with missing files",
            deletedCount);
    }
}
