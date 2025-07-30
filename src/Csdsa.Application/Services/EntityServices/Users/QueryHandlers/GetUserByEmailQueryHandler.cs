using MediatR;
using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.Users.Queries;

namespace Csdsa.Application.Services.EntityServices.Users.QueryHandlers;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUserByEmailQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken ct)
    {
        var user = await _uow.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}
