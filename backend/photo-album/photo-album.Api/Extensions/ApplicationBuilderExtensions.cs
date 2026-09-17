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
    }
}
