using AutoMapper;
using Csdsa.Domain.Models.Common.UserEntities;

namespace Csdsa.Infrastructure.Services.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // CreateMap<User, Csdsaa.Application.DTOs.Entities.User.UserDto>()
        //     .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}
