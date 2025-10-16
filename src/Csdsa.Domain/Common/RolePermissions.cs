namespace Csdsa.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; private set; }
    public string Permission { get; private set; }
    public Role Role { get; private set; }

    public RolePermission(Role role, string permission)
    {
        Role = role ?? throw new ArgumentNullException(nameof(role));
        RoleId = role.Id;
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}
