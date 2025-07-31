using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : BaseController
    {
        public FeatureController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
            : base(unitOfWork, logger, mediator) { }

        // GET: /api/features => GetAllFeatureToggles
        // POST: /api/features => CreateFeatureFlag
        // PUT: /api/features/{id} => UpdateFeatureFlagState
        // DELETE: /api/feature/{id} => RemoveFeatureFlag
    }
}
