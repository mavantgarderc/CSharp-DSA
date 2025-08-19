using AutoMapper;
using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Auth;
using MediatR;
using Serilog;

namespace Csdsa.Application.Services.Auth.SoftDeleteUser;

public class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, OperationResult>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<SoftDeleteUserCommandHandler>();

    public SoftDeleteUserCommandHandler(
        IGenericRepository<User> userRepository,
        IUnitOfWork uow,
        IMapper mapper
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<OperationResult> Handle(
        SoftDeleteUserCommand request,
        CancellationToken cancellationToken
    )
    {
        await _uow.BeginTransactionAsync();
        try
        {
            _logger.Information(
                "Attempting to soft delete user with ID: {UserId} or Email: {Email}",
                request.UserId,
                request.Email
            );

            User? user = null;

            if (request.UserId != Guid.Empty)
            {
                user = await _userRepository.GetByIdAsync(request.UserId, u => u.RefreshTokens);
            }
            else if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user = await _userRepository.FirstOrDefaultAsync(
                    u => u.Email == request.Email,
                    u => u.RefreshTokens
                );
            }

            if (user == null)
            {
                var errorMessage =
                    request.UserId != Guid.Empty
                        ? $"User with ID {request.UserId} not found."
                        : $"User with email {request.Email} not found.";

                _logger.Warning("User not found: {ErrorMessage}", errorMessage);
                throw new KeyNotFoundException(errorMessage);
            }

            if (user.SoftDelete)
            {
                _logger.Warning("User {UserId} is already soft deleted", user.Id);
                return (OperationResult)OperationResult.ErrorResult("User is already deleted.");
            }

            if (!user.IsActive)
            {
                _logger.Warning("User {UserId} is already inactive", user.Id);
                return (OperationResult)OperationResult.ErrorResult("User is already inactive.");
            }

            var softDeleteResult = await _userRepository.SoftDeleteAsync(user.Id);

            if (!softDeleteResult)
            {
                _logger.Error("Failed to soft delete user {UserId}", user.Id);
                return (OperationResult)OperationResult.ErrorResult("Failed to delete user.");
            }

            if (user.RefreshTokens.Any())
            {
                user = await _userRepository.GetByIdAsync(user.Id, u => u.RefreshTokens);

                if (user != null)
                {
                    foreach (var refreshToken in user.RefreshTokens)
                    {
                        refreshToken.IsRevoked = true;
                        refreshToken.RevokedAt = DateTime.UtcNow;
                    }

                    user.EmailVerificationToken = null;
                    user.EmailVerificationTokenExpires = null;
                    user.PasswordResetToken = null;
                    user.PasswordResetTokenExpires = null;

                    await _userRepository.UpdateAsync(user);
                    _logger.Information("Successfully soft deleted user {UserId}", user.Id);
                }
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync();

            return OperationResult.SuccessResult("User successfully deleted.");
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(
                ex,
                "Error occurred while soft deleting user with ID: {UserId} or Email: {Email}",
                request.UserId,
                request.Email
            );
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }
}
