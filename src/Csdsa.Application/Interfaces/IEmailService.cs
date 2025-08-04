namespace Csdsa.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string verificationToken);
    Task SendPasswordResetAsync(string email, string resetToken);
    Task SendAccountLockedAsync(string email, DateTime lockoutEnd);
}
