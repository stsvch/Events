using Events.Application.Mappings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Events.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            // Register pipeline behavior for automatic validation
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>)
            );

            // Register AutoMapper profiles
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            // Register MediatR handlers (CQRS)
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly)
            );

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
