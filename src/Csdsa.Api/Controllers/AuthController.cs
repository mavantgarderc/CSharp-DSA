using Application.Users.Commands;
using Csdsa.Application.Commands.Users.CreateUser;
using Csdsa.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserCommand cmd)
    {
        var user = await _mediator.Send(cmd);
        return Ok(user);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        var user = await _mediator.Send(new GetUserByEmailQuery(email));
        return user == null ? NotFound() : Ok(user);
    }
}
