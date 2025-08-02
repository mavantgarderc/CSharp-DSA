namespace Csdsa.Application.Auth.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string email, string token);
        Task SendPasswordResetAsync(string email, string token);
        Task SendWelcomeEmailAsync(string email, string firstName);
        Task SendAccountLockedEmailAsync(string email, DateTime lockoutEnd);
    }
}
