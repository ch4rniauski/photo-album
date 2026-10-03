using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Jwt;
using photo_album.Application.JWT;
using photo_album.Application.Services;
using photo_album.Domain.Constants;

namespace photo_album.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddJwtConfiguration()
        {
            var settings = JwtSettings.FromEnvironment();

            services.AddScoped<ITokenProvider, JwtTokenProvider>();
            services.AddScoped<IUserActivityLogService, UserActivityLogService>();

            services.Configure<JwtSettings>(options =>
            {
                options.SecurityKey = settings.SecurityKey;
                options.ExpiresInMinutes = settings.ExpiresInMinutes;
            });

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(settings.SecurityKey)),
                        NameClaimType = JwtRegisteredClaimNames.Sub,
                        RoleClaimType = "role",
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "Admin",
                    policy => policy.RequireRole(UserRoles.Admin));
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
