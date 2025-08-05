using Microsoft.Extensions.DependencyInjection;

namespace Csdsa.Application.Behaviors;

public static class MediatorEntryPoint
{
    public static IServiceCollection AddApplicaton(this IServiceCollection services)
    {
        // MediatR Configuration
        // services.AddMediatR(config => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        // Repository Injections
        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services.AddScoped<IGenericRepository, GenericRepository>();

        // External API/Services Injection
        // services.AddScoped<IExternalAPICaller, ExternalAPICaller>();

        return services;
    }
}
