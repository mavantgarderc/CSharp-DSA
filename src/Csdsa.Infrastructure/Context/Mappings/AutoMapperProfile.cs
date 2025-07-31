using AutoMapper;
using Csdsa.Application.DTOs.Entities;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Domain.Models.Enums;
using Csdsa.Domain.Models.UserEntities;

namespace Csdsa.Infrastructure.Services.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ReverseMap();

        CreateMap<Role, RoleDto>().ReverseMap();

        CreateMap<User, RoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src.Role))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => GetRoleDescription(src.Role))
            )
            .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.UserCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<(User user, int userCount), RoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src.user.Role))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.user.Role.ToString()))
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => GetRoleDescription(src.user.Role))
            )
            .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.user.Role))
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.userCount))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

    private static string GetRoleDescription(UserRole role)
    {
        return role switch
        {
            UserRole.User => "Standard user with basic permissions",
            UserRole.Admin => "Administrator with elevated permissions",
            UserRole.SuperAdmin => "Super administrator with full system access",
            _ => "Unknown role",
        };
    }
}
