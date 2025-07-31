using AutoMapper;
using Csdsa.Application.DTOs.Entities;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Roles.Request;
using Csdsa.Domain.Models.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.QueryHandlers;

public class GetUserRoleQueryHandler : IRequestHandler<GetUserRoleQuery, RoleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUserRoleQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<RoleDto> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        var userCount = await _uow.Users.CountAsync(u => u.Role == user.Role);

        return _mapper.Map<RoleDto>((user, userCount));
    }
}
