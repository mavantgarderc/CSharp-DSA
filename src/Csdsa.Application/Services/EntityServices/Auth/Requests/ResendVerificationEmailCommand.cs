using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Requests;

public class ResendVerificationEmailCommand : IRequest<ApiResponse>
{
    public string Email { get; set; } = string.Empty;
}
