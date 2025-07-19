using FlatFlow.BLL.DTOs;
using FlatFlow.BLL.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FlatFlow.BLL.Services.Classes
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email, _emailSettings.DisplayName),
                    Subject = subject,
                    Body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #7c3aed;'>FlatFlow Password Reset</h2>
                    <p>{message}</p>
                    <p style='color: #6b7280; font-size: 0.9rem;'>
                        If you didn't request this code, you can safely ignore this email.
                    </p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb;'>
                    <p style='color: #6b7280; font-size: 0.8rem;'>
                        © {DateTime.Now.Year} FlatFlow. All rights reserved.
                    </p>
                </div>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                    EnableSsl = true
                };

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to send email: {ex.Message}");
            }
        }
    }
}