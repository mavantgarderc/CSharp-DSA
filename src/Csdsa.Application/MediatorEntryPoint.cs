using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Utilities;
public static class MediatorEntryPoint
{
    public static IServiceCollection AddApplicaton(this IServiceCollection services)
    {
        // MediatR Configuration
        // services.AddMediatR(config => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        // Repository Injections
        // services.AddScoped<IUnitOfWork, UnitOfWork>();

        // External API/Services Injection
        // services.AddScoped<IExternalAPICaller, ExternalAPICaller>();

        return services;
    }
}
