using Domain.Enums;

namespace Csdsa.Domain.Models.Common.UserEntities

{
    public class Role : BaseEntity
    {
        public required string RoleName { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
