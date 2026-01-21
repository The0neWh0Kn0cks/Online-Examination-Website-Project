using Microsoft.AspNetCore.Identity;
using Online_Examination.Domain;
using System.Net;
using System.Net.Mail;

namespace Online_Examination.Services
{
    public class GmailEmailSender : IEmailSender<Online_ExaminationUser>
    {
        private readonly ILogger<GmailEmailSender> _logger;

        // Gmail SMTP Configuration
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SenderEmail = "booosama0113@gmail.com";
        private const string AppPassword = "icalblkpylorqwwu"; // 16-char Google App Password

        public GmailEmailSender(ILogger<GmailEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendConfirmationLinkAsync(Online_ExaminationUser user, string email, string confirmationLink)
        {
            return SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetLinkAsync(Online_ExaminationUser user, string email, string resetLink)
        {
            return SendEmailAsync(
                email,
                "Reset your password",
                $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetCodeAsync(Online_ExaminationUser user, string email, string resetCode)
        {
            return SendEmailAsync(
                email,
                "Reset your password",
                $"Please reset your password using the following code: {resetCode}");
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Sanitize app password by removing any spaces
            var cleanPassword = AppPassword.Replace(" ", "");

            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine($"[SMTP] Starting email send to {toEmail}...");
            System.Diagnostics.Debug.WriteLine($"[SMTP] Server: {SmtpServer} Port: {SmtpPort}");
            System.Diagnostics.Debug.WriteLine($"[SMTP] From: {SenderEmail}");
            System.Diagnostics.Debug.WriteLine($"[SMTP] To: {toEmail}");
            System.Diagnostics.Debug.WriteLine($"[SMTP] Subject: {subject}");
            System.Diagnostics.Debug.WriteLine($"[SMTP] SSL Enabled: True");
            System.Diagnostics.Debug.WriteLine($"[SMTP] Password Length: {cleanPassword.Length} chars");
            System.Diagnostics.Debug.WriteLine("========================================");

            try
            {
                System.Diagnostics.Debug.WriteLine($"[SMTP] Configuring SMTP client...");

                using var smtpClient = new SmtpClient(SmtpServer, SmtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(SenderEmail, cleanPassword),
                    Timeout = 30000 // 30 seconds timeout
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                System.Diagnostics.Debug.WriteLine("[SMTP] Sending email message...");
                await smtpClient.SendMailAsync(mailMessage);

                System.Diagnostics.Debug.WriteLine("[SMTP] ? Email sent successfully!");
                System.Diagnostics.Debug.WriteLine("========================================");
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (SmtpException smtpEx)
            {
                System.Diagnostics.Debug.WriteLine("[SMTP] ? SMTP ERROR occurred!");
                System.Diagnostics.Debug.WriteLine($"[SMTP] Status Code: {smtpEx.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[SMTP] ERROR: {smtpEx.Message}");
                System.Diagnostics.Debug.WriteLine($"[SMTP] Inner Exception: {smtpEx.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"[SMTP] Stack Trace: {smtpEx.StackTrace}");
                System.Diagnostics.Debug.WriteLine("========================================");
                _logger.LogError(smtpEx, "SMTP Error sending email to {Email}. Status: {Status}", toEmail, smtpEx.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[SMTP] ? ERROR occurred!");
                System.Diagnostics.Debug.WriteLine($"[SMTP] Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[SMTP] ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SMTP] Inner Exception: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"[SMTP] Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("========================================");
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }
    }
}
