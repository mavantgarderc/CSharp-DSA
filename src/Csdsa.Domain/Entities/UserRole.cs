namespace Csdsa.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public Guid? RoleId { get; private set; }
    public Enums.UserRole? PredefinedRole { get; private set; }
    public User User { get; private set; }
    public Role? Role { get; private set; }

    public UserRole(User user, Role? role = null, Enums.UserRole? predefinedRole = null)
    {
        if (role == null && predefinedRole == null)
            throw new ArgumentException("Either role or predefinedRole must be provided.");
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserId = user.Id;
        Role = role;
        RoleId = role?.Id;
        PredefinedRole = predefinedRole;
    }
}
