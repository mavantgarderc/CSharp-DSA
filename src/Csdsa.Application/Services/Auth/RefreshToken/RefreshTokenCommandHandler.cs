using AutoMapper;
using Csdsa.Application.DTOs.Auth;
using Csdsa.Contracts.Interfaces;
using Csdsa.Contracts.Repositories;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Csdsa.Application.Services.Auth.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, OperationResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    private const int ACCESS_TOKEN_EXPIRY_MINUTES = 15;
    private const int REFRESH_TOKEN_EXPIRY_DAYS = 7;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IGenericRepository<Role> roleRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<RefreshTokenCommandHandler> logger
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OperationResult<AuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return OperationResult<AuthResponse>.ErrorResult("Refresh token is required.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
            {
                return OperationResult<AuthResponse>.ErrorResult("Invalid refresh token.");
            }

            var existingToken = user.RefreshTokens.FirstOrDefault(rt =>
                rt.Token == request.RefreshToken
            );
            if (existingToken == null || !existingToken.IsValidForRefresh())
            {
                return OperationResult<AuthResponse>.ErrorResult(
                    "Refresh token is expired or revoked."
                );
            }

            existingToken.Revoke(request.IpAddress);

            var newRefreshTokenValue = await _jwtService.GenerateRefreshTokenAsync(
                request.IpAddress
            );
            var newRefreshToken = new Csdsa.Domain.Models.Auth.RefreshToken
            {
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRY_DAYS),
                CreatedByIp = request.IpAddress,
                UserId = user.Id,
            };
            user.RefreshTokens.Add(newRefreshToken);

            var accessToken = await _jwtService.GenerateAccessTokenAsync(user, request.IpAddress);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(ACCESS_TOKEN_EXPIRY_MINUTES),
                RefreshTokenExpiry = newRefreshToken.ExpiresAt,
                User = _mapper.Map<UserProfileDto>(user),
            };

            return OperationResult<AuthResponse>.SuccessResult(authResponse);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
