using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace UnitAPI.Application.Utilities;

public static class ValidatorEntryPoint
{
    public static IServiceCollection AddValidator(this IServiceCollection services)
    {
        //services.AddValidatorsFromAssemblyContaining<ValiatorClasses>();
        // services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

        return services;
    }
}
