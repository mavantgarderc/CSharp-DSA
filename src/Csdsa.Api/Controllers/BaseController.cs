using System.Diagnostics;
using System.Security.Claims;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Models.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers.Base
{
    [ApiController]
    [Route("api[controller]")]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger _logger;

        protected BaseController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        #region User CLaims Helper Methods

        protected Guid GetUserId()
        {
            var userIdClaim =
                User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        protected string GetUserEmail()
        {
            return User.FindFirst("Email")?.Value
                ?? User.FindFirst(ClaimTypes.Email)?.Value
                ?? string.Empty;
        }

        protected string GetUserRole()
        {
            return User.FindFirst("Role")?.Value
                ?? User.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty;
        }

        protected List<string> GetUserPermissions()
        {
            var permissions = User.FindAll("Permissions").Select(x => x.Value).ToList();
            if (!permissions.Any())
            {
                permissions = User.FindAll("permissions").Select(x => x.Value).ToList();
            }
            return permissions;
        }

        protected bool HasPermission(string permission)
        {
            return GetUserPermissions().Contains(permission);
        }

        protected bool IsInRole(string role)
        {
            return User.IsInRole(role)
                || GetUserRole().Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Response Helper Methods

        protected ActionResult<ApiResponse<T>> SuccessResponse<T>(
            T data,
            string message = "Operation successful"
        )
        {
            _logger.LogInformation("Success: {Message}", message);
            return Ok(ApiResponse<T>.SuccessResult(data, message));
        }

        protected ActionResult<ApiResponse> SuccessResponse(string message = "Operation successful")
        {
            _logger.LogInformation("Success: {Message}", message);
            return Ok(ApiResponse.SuccessResult(message));
        }

        protected ActionResult<ApiResponse<T>> ErrorResponse<T>(
            string message,
            object? errors = null
        )
        {
            _logger.LogError("Error: {Message}", message);
            return BadRequest(ApiResponse<T>.ErrorResult(message, errors));
        }

        protected ActionResult<ApiResponse> ErrorResponse(string message, object? errors = null)
        {
            _logger.LogError("Error: {Message}", message);
            return BadRequest(ApiResponse.ErrorResult(message, errors));
        }

        protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(
            string message = "Resource not found"
        )
        {
            _logger.LogWarning("Not Found: {Message}", message);
            return NotFound(ApiResponse<T>.ErrorResult(message));
        }

        protected ActionResult<ApiResponse> NotFoundResponse(string message = "Resource not found")
        {
            _logger.LogWarning("Not Found: {Message}", message);
            return NotFound(ApiResponse.ErrorResult(message));
        }

        protected ActionResult<ApiResponse<T>> UnauthorizedResponse<T>(
            string message = "Unauthorized access"
        )
        {
            _logger.LogWarning("Unauthorized: {Message}", message);
            return Unauthorized(ApiResponse<T>.ErrorResult(message));
        }

        protected ActionResult<ApiResponse> UnauthorizedResponse(
            string message = "Unauthorized access"
        )
        {
            _logger.LogWarning("Unauthorized: {Message}", message);
            return Unauthorized(ApiResponse.ErrorResult(message));
        }

        protected ActionResult<ApiResponse<IEnumerable<T>>> PaginatedResponse<T>(
            IEnumerable<T> data,
            int page,
            int pageSize,
            int totalCount,
            string message = "Data retrieved successfully"
        )
        {
            _logger.LogInformation(
                "Paginated data retrieved: Page {Page}, PageSize {PageSize}, Total {Total}",
                page,
                pageSize,
                totalCount
            );
            return PaginatedResponse(data, page, pageSize, totalCount, message);
        }

        #endregion

        #region Validation Helper Methods

        protected async Task<ActionResult<ApiResponse<T>>?> ValidateModelAsync<T, TModel>(
            TModel model,
            IValidator<TModel> validator
        )
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => new
                {
                    Field = x.PropertyName,
                    Error = x.ErrorMessage,
                });
                return ErrorResponse<T>("Validation failed", errors);
            }
            return null;
        }

        protected ActionResult<ApiResponse<T>>? ValidateModel<T>()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp =>
                            kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                            ?? Array.Empty<string>()
                    );
                return ErrorResponse<T>("Validation failed", errors);
            }
            return null;
        }

        protected async Task<ActionResult<ApiResponse>?> ValidateModelAsync<TModel>(
            TModel model,
            IValidator<TModel> validator
        )
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => new
                {
                    Field = x.PropertyName,
                    Error = x.ErrorMessage,
                });
                return ErrorResponse("Validation failed", errors);
            }
            return null;
        }

        protected ActionResult<ApiResponse>? ValidateModel()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp =>
                            (object)(
                                kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                                ?? Array.Empty<string>()
                            )
                    );
                return ErrorResponse("Validation failed", errors);
            }
            return null;
        }

        #endregion

        #region Algorithm Helper Methods

        protected AlgorithmResponse<T> CreateAlgorithmResponse<T>(
            T result,
            string algorithmName,
            string timeComplexity,
            string spaceComplexity,
            long executionTime,
            int dataSize,
            string? description = null,
            Dictionary<string, object>? metadata = null
        )
        {
            return new AlgorithmResponse<T>
            {
                Result = result,
                AlgorithmName = algorithmName,
                Complexity = new ComplexityAnalysis
                {
                    TimeComplexity = timeComplexity,
                    SpaceComplexity = spaceComplexity,
                    ExecutionTimeMs = executionTime,
                    DataSize = dataSize,
                    Description = description ?? string.Empty,
                },
                Metadata =
                    metadata?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.ToString() ?? string.Empty
                    ) ?? new Dictionary<string, string>(),
            };
        }

        protected async Task<AlgorithmResponse<T>> ExecuteWithAnalysisAsync<T>(
            Func<Task<T>> algorithm,
            string algorithmName,
            string timeComplexity,
            string spaceComplexity,
            int dataSize,
            string? description = null,
            Dictionary<string, object>? metadata = null
        )
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await algorithm();
                stopwatch.Stop();

                return CreateAlgorithmResponse(
                    result,
                    algorithmName,
                    timeComplexity,
                    spaceComplexity,
                    stopwatch.ElapsedMilliseconds,
                    dataSize,
                    description,
                    metadata
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error executing algorithm {AlgorithmName}", algorithmName);
                throw;
            }
        }

        protected AlgorithmResponse<T> ExecuteWithAnalysis<T>(
            Func<T> algorithm,
            string algorithmName,
            string timeComplexity,
            string spaceComplexity,
            int dataSize,
            string? description = null,
            Dictionary<string, object>? metadata = null
        )
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = algorithm();
                stopwatch.Stop();

                return CreateAlgorithmResponse(
                    result,
                    algorithmName,
                    timeComplexity,
                    spaceComplexity,
                    stopwatch.ElapsedMilliseconds,
                    dataSize,
                    description,
                    metadata
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error executing algorithm {AlgorithmName}", algorithmName);
                throw;
            }
        }

        #endregion

        #region CRUD Helper Methods

        protected async Task<ActionResult<ApiResponse<TResponse>>> GetByIdAsync<TEntity, TResponse>(
            Guid id,
            Func<TEntity, TResponse> mapper,
            string entityName = "Entity"
        )
            where TEntity : BaseEntity
        {
            try
            {
                var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFoundResponse<TResponse>($"{entityName} not found");
                }

                var response = mapper(entity);
                return SuccessResponse(response, $"{entityName} retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving {EntityName} with ID {Id}", entityName, id);
                return ErrorResponse<TResponse>($"Error retrieving {entityName}");
            }
        }

        protected async Task<ActionResult<ApiResponse<IEnumerable<TResponse>>>> GetPagedAsync<
            TEntity,
            TResponse
        >(
            int page = 1,
            int pageSize = 10,
            Func<TEntity, TResponse>? mapper = null,
            string entityName = "Entity"
        )
            where TEntity : BaseEntity
        {
            try
            {
                if (page < 1)
                    page = 1;
                if (pageSize < 1)
                    pageSize = 10;
                if (pageSize > 100)
                    pageSize = 100;

                var (items, totalCount) = await _unitOfWork
                    .Repository<TEntity>()
                    .GetPagedAsync(page, pageSize);

                var response = mapper != null ? items.Select(mapper) : items.Cast<TResponse>();

                return PaginatedResponse(
                    response,
                    page,
                    pageSize,
                    totalCount,
                    $"{entityName} list retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged {EntityName}", entityName);

                return PaginatedResponse(
                    Enumerable.Empty<TResponse>(),
                    page,
                    pageSize,
                    0,
                    $"Error retrieving {entityName}"
                );
            }
        }

        protected async Task<ActionResult<ApiResponse<TResponse>>> CreateAsync<
            TEntity,
            TRequest,
            TResponse
        >(
            TRequest request,
            Func<TRequest, TEntity> mapper,
            Func<TEntity, TResponse> responseMapper,
            IValidator<TRequest>? validator = null,
            string entityName = "Entity"
        )
            where TEntity : BaseEntity
        {
            try
            {
                if (validator != null)
                {
                    var validationError = await ValidateModelAsync<TResponse, TRequest>(
                        request,
                        validator
                    );
                    if (validationError != null)
                        return validationError;
                }

                var modelError = ValidateModel<TResponse>();
                if (modelError != null)
                    return modelError;

                var entity = mapper(request);
                entity.CreatedBy = GetUserId().ToString();

                var createdEntity = await _unitOfWork.Repository<TEntity>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var response = responseMapper(createdEntity);
                return SuccessResponse(response, $"{entityName} created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {EntityName}", entityName);
                return ErrorResponse<TResponse>($"Error creating {entityName}");
            }
        }

        protected async Task<ActionResult<ApiResponse<TResponse>>> UpdateAsync<
            TEntity,
            TRequest,
            TResponse
        >(
            Guid id,
            TRequest request,
            Func<TEntity, TRequest, TEntity> mapper,
            Func<TEntity, TResponse> responseMapper,
            IValidator<TRequest>? validator = null,
            string entityName = "Entity"
        )
            where TEntity : BaseEntity
        {
            try
            {
                if (validator != null)
                {
                    var validationError = await ValidateModelAsync<TResponse, TRequest>(
                        request,
                        validator
                    );
                    if (validationError != null)
                        return validationError;
                }

                var modelError = ValidateModel<TResponse>();
                if (modelError != null)
                    return modelError;

                var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFoundResponse<TResponse>($"{entityName} not found");
                }

                entity = mapper(entity, request);
                entity.UpdatedBy = GetUserId().ToString();

                var updatedEntity = await _unitOfWork.Repository<TEntity>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var response = responseMapper(updatedEntity);
                return SuccessResponse(response, $"{entityName} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityName} with ID {Id}", entityName, id);
                return ErrorResponse<TResponse>($"Error updating {entityName}");
            }
        }

        protected async Task<ActionResult<ApiResponse>> DeleteAsync<TEntity>(
            Guid id,
            bool softDelete = true,
            string entityName = "Entity"
        )
            where TEntity : BaseEntity
        {
            try
            {
                if (softDelete)
                {
                    var deleted = await _unitOfWork.Repository<TEntity>().SoftDeleteAsync(id);
                    if (!deleted)
                    {
                        return NotFoundResponse($"{entityName} not found");
                    }
                }
                else
                {
                    var entity = await _unitOfWork.Repository<TEntity>().DeleteAsync(id);
                    if (entity == null)
                    {
                        return NotFoundResponse($"{entityName} not found");
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return SuccessResponse($"{entityName} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityName} with ID {Id}", entityName, id);
                return ErrorResponse($"Error deleting {entityName}");
            }
        }

        #endregion

        #region Exception Handling

        protected ActionResult<ApiResponse<T>> HandleException<T>(
            Exception ex,
            string operation = "Operation"
        )
        {
            _logger.LogError(ex, "Error during {Operation}", operation);

            return ex switch
            {
                ArgumentException => ErrorResponse<T>("Invalid argument provided"),
                UnauthorizedAccessException => UnauthorizedResponse<T>("Access denied"),
                KeyNotFoundException => NotFoundResponse<T>("Resource not found"),
                InvalidOperationException => ErrorResponse<T>("Invalid operation"),
                _ => ErrorResponse<T>($"An error occurred during {operation}"),
            };
        }

        #endregion
    }
}
