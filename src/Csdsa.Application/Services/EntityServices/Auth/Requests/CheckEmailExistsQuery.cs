using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Queries;

public class CheckEmailExistsQuery : IRequest<bool>
{
    public string Email { get; set; } = string.Empty;
}
