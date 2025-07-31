using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Models.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Request;

public record AssignRoleToUserCommand(List<Guid> UserIds, UserRole Role) : IRequest<bool>;
