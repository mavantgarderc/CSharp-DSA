using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Csdsa.Application.Behaviors;

/// <summary>
/// configuration entry point for MediatR setup and registration
/// </summary>
public static class MediatorEntryPoint
{
    /// <summary>
    /// registers MediatR services and handlers from the current assembly
    /// </summary>
    public static IServiceCollection AddApplicationMediatR(this IServiceCollection services)
    {
        services.AddMediatR(typeof(MediatorEntryPoint).Assembly);
        return services;
    }
}
