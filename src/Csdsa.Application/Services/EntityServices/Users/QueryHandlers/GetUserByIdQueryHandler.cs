using AutoMapper;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Users.Queries;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.QueryHandlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _uow.Users.FirstOrDefaultAsync(u => u.Id == request.userId);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}
