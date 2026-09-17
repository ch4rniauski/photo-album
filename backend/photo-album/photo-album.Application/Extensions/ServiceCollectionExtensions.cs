using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace photo_album.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension (IServiceCollection services)
    {
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
