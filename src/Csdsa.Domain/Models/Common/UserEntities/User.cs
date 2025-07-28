namespace Csdsa.Domain.Models.Common.UserEntities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName() => FirstName + LastName;
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string UserName { get; set; }
    }
}
