using AutoMapper;
using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Services.Auth.Register;
using Csdsa.Domain.Models.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Csdsa.Application.Mapping.Profiles;

/// <summary>
/// autoMapper profile for authentication-related mappings
/// </summary>
public class AuthMappingProfile : Profile
{
    private const string DEFAULT_USER_ROLE = "User";

    public AuthMappingProfile()
    {
        CreateMap<RegisterCommand, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.EmailVerificationToken, opt => opt.Ignore())
            .ForMember(dest => dest.EmailVerificationTokenExpires, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        // .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore());

        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.AccessTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));

        CreateMap<User, UserProfileDto>()
            .ForMember(
                dest => dest.Roles,
                opt =>
                    opt.MapFrom(src =>
                        src.Role.Any()
                            ? src.Role.Select(ur => ur.Role.Name).ToList()
                            : new List<string> { DEFAULT_USER_ROLE }
                    )
            );

        //     CreateMap<Role, RoleDto>()
        //         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src.Id.GetHashCode()))
        //         .ForMember(
        //             dest => dest.Description,
        //             opt => opt.MapFrom(src => src.Description ?? string.Empty)
        //         )
        //         .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.UserRoles.Count))
        //         .ForMember(
        //             dest => dest.RoleType,
        //             opt => opt.MapFrom(src => DetermineRoleType(src.Name))
        //         );
        // }
        // private static UserRole DetermineRoleType(string roleName)
        // {
        //     return roleName.ToLower() switch
        //     {
        //         "superadmin" or "super admin" or "super_admin" => UserRole.SuperAdmin,
        //         "admin" or "administrator" => UserRole.Admin,
        //         "dev" or "developer" => UserRole.Dev,
        //         "user" or "member" or "standard" => UserRole.User,
        //         _ => UserRole.User,
        //     };
        // }
    }
}

/// <summary>
/// extension methods for AutoMapper configuration
/// </summary>
public static class AutoMapperExtensions
{
    /// <summary>
    /// configures AutoMapper for the application
    /// </summary>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AuthMappingProfile));
        return services;
    }
}
