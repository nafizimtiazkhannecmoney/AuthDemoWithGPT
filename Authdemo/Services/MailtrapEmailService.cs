
using Authdemo.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Authdemo.Services
{
    public class MailtrapEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public MailtrapEmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _emailSettings.FromName,
                    _emailSettings.FromEmail));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject = "Password Reset Request";

            var resetLink =
                $"https://localhost:5236/reset-password?token={Uri.EscapeDataString(resetToken)}";  //http://localhost:5236/swagger/index.html

            //message.Body = new BodyBuilder
            //{
            //    HtmlBody = $"""
            //    <h2>Password Reset</h2>

            //    <p>You requested to reset your password.</p>

            //    <p>
            //        Click the link below to reset your password:
            //    </p>

            //    <p>
            //        <a href="{resetLink}">
            //            Reset Password
            //        </a>
            //        <p">
            //            Reset Token: {resetToken}
            //        </p>
            //    </p>

            //    <p>
            //        This link will expire in 15 minutes.
            //    </p>

            //    <p>
            //        If you did not request this, you can safely ignore this email.
            //    </p>
            //    """
            //}.ToMessageBody();

            message.Body = new BodyBuilder
            {
                HtmlBody = $"""
                            <h2>Password Reset Request</h2>

                            <p>You requested to reset your password.</p>

                            <p>Your password reset token is:</p>

                            <p>
                                <strong>{resetToken}</strong>
                            </p>

                            <p>
                                This token will expire in 15 minutes.
                            </p>

                            <p>
                                Use this token with the password reset API.<strong>'http://localhost:5236/api/Auth/reset-password'</strong>
                            </p>

                            <p>
                                If you did not request this password reset,
                                you can safely ignore this email.
                            </p>
                            """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _emailSettings.Host,
                _emailSettings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _emailSettings.UserName,
                _emailSettings.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}
