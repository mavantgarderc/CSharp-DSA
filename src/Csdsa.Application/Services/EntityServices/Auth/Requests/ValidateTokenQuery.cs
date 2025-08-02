using Csdsa.Application.DTOs;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Queries;

public class ValidateTokenQuery : IRequest<ApiResponse<bool>>
{
    public string Token { get; set; } = string.Empty;
}
