using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Models.Common.UserEntities
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
