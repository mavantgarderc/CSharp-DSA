using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Models.Enums;

namespace Csdsa.Domain.Models.UserEntities
{
    public class Role : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public UserRole UserRoles { get; set; }

        public required string RoleName { get; set; }
    }
}
