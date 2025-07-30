using Domain.Enums;

namespace Csdsa.Domain.Models.Common.UserEntities.User
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
    }
}
