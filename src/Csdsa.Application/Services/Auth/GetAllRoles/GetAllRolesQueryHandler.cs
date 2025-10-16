using System.Linq.Expressions;
using Csdsa.Contracts.Dtos.Auth;
using Csdsa.Contracts.Repositories;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Auth;
using Serilog;

namespace Csdsa.Application.Services.Auth.GetAllRoles;

public class GetAllRolesQueryHandler
    : IRequestHandler<GetAllRolesQuery, OperationResult<List<RoleDto>>>
{
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<GetAllRolesQueryHandler>();

    public GetAllRolesQueryHandler(IGenericRepository<Role> roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<OperationResult<List<RoleDto>>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            _logger.Information(
                "Retrieving all roles. IncludeInactive: {IncludeInactive}, IncludePermissions: {IncludePermissions}",
                request.IncludeInactive,
                request.IncludePermissions
            );

            Expression<Func<Role, bool>>? filter = null;
            if (!request.IncludeInactive)
            {
                filter = r => !r.SoftDelete && r.IsActive;
            }

            var includes = new List<Expression<Func<Role, object>>> { r => r.UserRoles };

            if (request.IncludePermissions)
            {
                includes.Add(r => r.RolePermissions);
            }

            var roles = await _roleRepository.GetAllAsync(
                filter: filter,
                orderBy: query => query.OrderBy(r => r.Name),
                includes: includes.ToArray()
            );

            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
            {
                var roleDto = new RoleDto
                {
                    Id = (int)role.Id.GetHashCode(),
                    Name = role.Name,
                    Description = role.Description ?? string.Empty,
                    UserCount = role.UserRoles?.Count ?? 0,
                    CreatedAt = role.CreatedAt,
                    UpdatedAt = role.UpdatedAt,
                };

                roleDtos.Add(roleDto);
            }

            _logger.Information("Successfully retrieved {RoleCount} roles", roleDtos.Count);
            return OperationResult<List<RoleDto>>.SuccessResult(
                roleDtos,
                "Roles retrieved successfully."
            );
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while retrieving all roles");
            return OperationResult<List<RoleDto>>.ErrorResult(
                "Failed to retrieve roles.",
                ex.Message
            );
        }
    }
}
