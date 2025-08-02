using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Models.Enums;

namespace Csdsa.Domain.Models.UserEntities
{
    public class Role : BaseEntity
    {
        [Required]
        public string Name { get; set;} = string.Empty;
        public string? Description {get; set;}
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
