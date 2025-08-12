namespace Csdsa.Domain.Models.Auth;

public class ApiClient : BaseEntity
{
    public string Name { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
