using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Domain.Repository.IRepositories;
using Csdsa.Domain.ViewModel.EntityViewModel.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
            : base(unitOfWork, logger, mediator)
        {
        }

        [HttpPost]
        [Route(nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _mediator.Send(new CreateUserCommand(request));
            return Ok(result);
        }
    }
}
