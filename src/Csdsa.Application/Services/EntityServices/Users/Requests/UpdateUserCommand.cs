using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public class UpdateUserCommand : IRequest<bool>
{
    public Guid userId { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
}
