using System.Linq;
using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Roles.Request;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.QueryHandlers;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, List<UserDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUsersByRoleQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(
        GetUsersByRoleQuery request,
        CancellationToken cancellationToken
    )
    {
        var users = await _uow.Users.GetAllAsync(
            filter: u => u.Role == request.Role,
            orderBy: q => q.OrderBy(u => u.UserName)
        );

        return _mapper.Map<List<UserDto>>(users);
    }
}
