using Csdsa.Domain.Models.Enums;

namespace Csdsa.Domain.Models.UserEntities
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
