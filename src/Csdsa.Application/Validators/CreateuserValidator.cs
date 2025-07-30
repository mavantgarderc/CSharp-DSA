using Csdsa.Application.Commands.Users.CreateUser;
using FluentValidation;

namespace Application.Users.Commands;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        // RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        // RuleFor(x => x.Email).NotEmpty().EmailAddress();
        // RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        //

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 30)
            .WithMessage("Username must be between 3 and 30 characters")
            .Matches(@"^[a-zA-Z0-9_.-]*$")
            .WithMessage(
                "Username can only contain letters, numbers, underscores, dots, and hyphens"
            )
            // .MustAsync(BeUniqueUserName)
            .WithMessage("Username already exists");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters")
            // .MustAsync(BeUniqueEmail)
            .WithMessage("Email already exists");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&)"
            );
    }

    // private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    // {
    //     if (string.IsNullOrWhiteSpace(email))
    //         return true; // Let the NotEmpty rule handle this
    //
    //     var existingUser = await _userRepository.GetByEmailAsync(
    //         email.ToLowerInvariant(),
    //         cancellationToken
    //     );
    //     return existingUser == null;
    // }
    //
    // private async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    // {
    //     if (string.IsNullOrWhiteSpace(userName))
    //         return true; // Let the NotEmpty rule handle this
    //
    //     var existingUser = await _userRepository.GetByUserNameAsync(
    //         userName.ToLowerInvariant(),
    //         cancellationToken
    //     );
    //     return existingUser == null;
    // }

}
