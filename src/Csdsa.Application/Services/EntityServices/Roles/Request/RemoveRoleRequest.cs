using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public record RemoveRoleFromUserCommand(Guid UserId) : IRequest<bool>;
