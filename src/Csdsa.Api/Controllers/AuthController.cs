using Application.Users.Commands;
using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Application.Users.Queries;
using Csdsa.Domain.ViewModel.EntityViewModel.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    public AuthController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
        : base(unitOfWork, logger, mediator) { }

    [HttpPost("UserRegister")]
    public async Task<IActionResult> RegisterUser(CreateUserCommand cmd)
    {
        var user = await _mediator.Send(cmd);
        return Ok(user);
    }

    [HttpGet("LoginUser")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password));
        return Ok(result);
    }
}
