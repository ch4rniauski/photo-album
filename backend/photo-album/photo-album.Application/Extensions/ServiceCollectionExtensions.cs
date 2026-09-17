using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using photo_album.Application.Contracts.Jwt;
using photo_album.Application.JWT;

namespace photo_album.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddJwtConfiguration()
        {
            services.AddScoped<ITokenProvider, JwtTokenProvider>();

            services.Configure<JwtSettings>(options =>
            {
                var settings = JwtSettings.FromEnvironment();

                options.SecurityKey = settings.SecurityKey;
                options.ExpiresInMinutes = settings.ExpiresInMinutes;
            });

            return services;
        }

        public IServiceCollection AddMediatrConfiguration()
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            });

            return services;
        }

        public IServiceCollection AddValidationConfiguration()
        {
            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }

        public IServiceCollection AddAutoMapperConfiguration()
        {
            services.AddAutoMapper(_ => {}, typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}
