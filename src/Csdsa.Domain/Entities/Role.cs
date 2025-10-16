using Csdsa.Domain.Common;

namespace Csdsa.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool SoftDelete { get; private set; }
    public List<UserRole> UserRoles { get; private set; }
    public List<RolePermission> RolePermissions { get; private set; }

    public Role(string name, string? description = null)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Role name cannot be empty.", nameof(name));
        Name = name;
        Description = description;
        IsActive = true;
        SoftDelete = false;
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
    }

    public OperationResult<Role> AddPermission(RolePermission permission)
    {
        if (!IsActive || SoftDelete) return OperationResult<Role>.ErrorResult("Role is inactive or deleted.");
        if (RolePermissions.Any(p => p.Permission == permission.Permission))
            return OperationResult<Role>.ErrorResult("Permission already exists.");
        RolePermissions.Add(permission);
        return OperationResult<Role>.SuccessResult(this);
    }
}
