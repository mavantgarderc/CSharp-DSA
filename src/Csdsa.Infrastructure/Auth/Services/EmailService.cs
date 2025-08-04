using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using Csdsa.Infrastructure.Auth.Configuration;
using Csdsa.Application.Interfaces;

namespace Csdsa.Infrastructure.Auth.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailVerificationAsync(string email, string token)
        {
            var subject = "Verify Your Email Address";
            var verificationUrl = $"{_emailSettings.WebAppUrl}/verify-email?token={token}&email={email}";

            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Email Verification Required</h2>
                    <p>Thank you for registering with us! Please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationUrl}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Verify Email Address</a></p>
                    <p>If you cannot click the link, please copy and paste this URL into your browser:</p>
                    <p>{verificationUrl}</p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not create an account, please ignore this email.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetAsync(string email, string token)
        {
            var subject = "Reset Your Password";
            var resetUrl = $"{_emailSettings.WebAppUrl}/reset-password?token={token}&email={email}";

            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Password Reset Request</h2>
                    <p>You have requested to reset your password. Click the link below to set a new password:</p>
                    <p><a href='{resetUrl}' style='background-color: #28a745; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Reset Password</a></p>
                    <p>If you cannot click the link, please copy and paste this URL into your browser:</p>
                    <p>{resetUrl}</p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request a password reset, please ignore this email.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string firstName)
        {
            var subject = "Welcome!";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Welcome{(!string.IsNullOrEmpty(firstName) ? $", {firstName}" : "")}!</h2>
                    <p>Your email has been verified successfully. You can now enjoy all the features of our application.</p>
                    <p>If you have any questions, please don't hesitate to contact our support team.</p>
                    <p>Thank you for joining us!</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendAccountLockedEmailAsync(string email, DateTime lockoutEnd)
        {
            var subject = "Account Temporarily Locked";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Account Security Notice</h2>
                    <p>Your account has been temporarily locked due to multiple failed login attempts.</p>
                    <p>Your account will be automatically unlocked at: <strong>{lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC</strong></p>
                    <p>If you believe this was not you, please contact our support team immediately.</p>
                    <p>For security reasons, you can also reset your password using the 'Forgot Password' feature.</p>
                </div>";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
                client.EnableSsl = _emailSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject: {Subject}", toEmail, subject);
                throw;
            }
        }

        public Task SendAccountLockedAsync(string email, DateTime lockoutEnd)
        {
            throw new NotImplementedException();
        }
    }
}
