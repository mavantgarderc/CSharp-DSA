using MediatR;
using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Common.Interfaces;

namespace Csdsa.Application.Users.Queries;

public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUserByEmailHandler(IUnitOfWork uow, IMapper mapper)
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
