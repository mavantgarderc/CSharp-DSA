using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Enums;

namespace Csdsa.Application.DTOs.Entities.Role;

public record BulkAssignRolesRequest(List<Guid> UserIds, UserRole Role);
