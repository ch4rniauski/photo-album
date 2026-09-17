using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;
using photo_album.Infrastructure.Repositories;

namespace photo_album.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPhotoAlbumContextConfiguration(IConfiguration configuration)
        {
            services.AddDbContext<PhotoAlbumContext>(opt =>
                opt.UseNpgsql(configuration.GetConnectionString("PhotoAlbumDb"))
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

            return services;
        }
    }
}
