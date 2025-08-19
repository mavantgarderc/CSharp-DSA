using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.SoftDeleteUser;

public class SoftDeleteUserCommand : IRequest<OperationResult>
{
    public Guid UserId { get; set; }
    public string? Email { get; set; } = string.Empty;
}
