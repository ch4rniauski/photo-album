using Microsoft.EntityFrameworkCore;
using photo_album.Infrastructure;

namespace photo_album.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public async Task ApplyMigrationsAsync()
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();

            await using var db = scope.ServiceProvider.GetRequiredService<PhotoAlbumContext>();

            await db.Database.MigrateAsync();
        }
        
        public async Task CheckDbPendingMigrationsAsync()
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();
            
            await using var db = scope.ServiceProvider.GetRequiredService<PhotoAlbumContext>();

            var hasPendingChanges = db.Database.HasPendingModelChanges();

            if (hasPendingChanges)
            {
                throw new InvalidOperationException("There are migrations that have not been applied");
            }
        }
    }
}
