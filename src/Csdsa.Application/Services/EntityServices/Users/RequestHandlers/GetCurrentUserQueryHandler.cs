using System.Security.Claims;
using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Csdsa.Application.Services.EntityServices.Users.QueryHandlers;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork uow,
        IMapper mapper
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = _httpContextAccessor
            .HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _uow.Users.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return _mapper.Map<UserDto>(user);
    }
}
