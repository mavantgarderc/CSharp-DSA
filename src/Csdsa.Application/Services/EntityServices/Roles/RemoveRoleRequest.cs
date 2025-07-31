using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public record RemoveRoleCommand(Guid UserId) : IRequest<bool>;
