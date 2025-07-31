using System.Linq;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Services.EntityServices.Roles.Queries;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.QueryHandlers;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, List<UserDto>>
{
    private readonly IUnitOfWork _uow;

    public GetUsersByRoleQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
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

        return users
            .Select(u => new UserDto(u.Id, u.UserName, u.Email, u.Role.ToString(), u.IsActive))
            .ToList();
    }
}
