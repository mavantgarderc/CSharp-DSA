using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Csdsa.Application.Behaviors;

public static class ValidatorEntryPoint
{
    public static IServiceCollection AddValidator(this IServiceCollection services)
    {
        // services.AddValidatorsFromAssemblyContaining<ValiatorClasses>();
        // services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

        return services;
    }
}
