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
            var connectionString = BuildPostgresConnectionString(configuration);

            services.AddDbContext<PhotoAlbumContext>(opt =>
                opt.UseNpgsql(connectionString)
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

            return services;
        }
    }

    private static string BuildPostgresConnectionString(IConfiguration configuration)
    {
        var host = configuration["POSTGRES_HOST"]
            ?? throw new InvalidOperationException("Environment variable POSTGRES_HOST is not set.");
        var port = configuration["POSTGRES_PORT"]
            ?? throw new InvalidOperationException("Environment variable POSTGRES_PORT is not set.");
        var database = configuration["POSTGRES_DB"]
            ?? throw new InvalidOperationException("Environment variable POSTGRES_DB is not set.");
        var username = configuration["POSTGRES_USER"]
            ?? throw new InvalidOperationException("Environment variable POSTGRES_USER is not set.");
        var password = configuration["POSTGRES_PASSWORD"]
            ?? throw new InvalidOperationException("Environment variable POSTGRES_PASSWORD is not set.");

        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }
}
