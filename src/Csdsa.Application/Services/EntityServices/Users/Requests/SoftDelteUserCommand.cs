using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public record SoftDeleteUserCommand(string Email) : IRequest<bool>;
