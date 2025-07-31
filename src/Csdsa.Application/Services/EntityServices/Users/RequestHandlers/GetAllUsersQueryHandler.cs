using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.QueryHandlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto?>>
{
    private readonly IUnitOfWork _uow;

    public GetAllUsersQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<UserDto?>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var users = await _uow.Users.GetAllAsync();
            return (IEnumerable<UserDto?>)users;
        }
        catch
        {
            throw;
        }
    }
}
