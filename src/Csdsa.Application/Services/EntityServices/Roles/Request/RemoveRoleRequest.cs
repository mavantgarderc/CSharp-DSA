using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Request;

public record RemoveRoleFromUserCommand(Guid UserId) : IRequest<bool>;
