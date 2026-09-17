using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;
using photo_album.Infrastructure.PostgreSql;
using photo_album.Infrastructure.Repositories;

namespace photo_album.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPhotoAlbumContextConfiguration()
        {
            var connectionString = PostgresSettings.FromEnvironment().ConnectionString;

            services.AddDbContext<PhotoAlbumContext>(opt =>
                opt.UseNpgsql(connectionString)
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

            return services;
        }
    }
}
