using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        public RoleController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
            : base(unitOfWork, logger, mediator) { }

        // GET: /api/roles => GetAllRoles
        // GET: /api/roles/{id} => GetRoleById
        // POST: /api/roles => CreateNewRole
        // PUT: /api/roles/{id} => UpdateExistingRole
        // DELETE: /api/roles/{id} => DeleteRole
        // GET: /api/roles/{id}/users => ListUsersAssignedToRole
        // POST: /api/roles/{id}/permissions => AssignPermissionsToRole
    }
}
