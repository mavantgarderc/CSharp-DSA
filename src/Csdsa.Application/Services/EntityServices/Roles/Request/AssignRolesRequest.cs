using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Enums;

namespace Csdsa.Application.Services.EntityServices.Roles.Request;

public record AssignRolesRequest(List<Guid> UserIds, UserRole Role);
