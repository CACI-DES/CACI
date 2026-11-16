using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using CACI_Life.Features.Discovery;

namespace CACI_Life.Features.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.SmtpServer) || string.IsNullOrEmpty(_settings.Username))
            {
                _logger.LogWarning("Email sending skipped: SMTP settings are missing.");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, 465, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}. Error: {Message}", to, ex.Message);
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, byte[] attachmentBytes, string filename)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.SmtpServer) || string.IsNullOrEmpty(_settings.Username))
            {
                _logger.LogWarning("Email sending with attachment skipped: SMTP settings are missing.");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            builder.Attachments.Add(filename, attachmentBytes);
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, 465, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent with attachment {Filename} to {To}", filename, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To}. Error: {Message}", to, ex.Message);
        }
    }

    public async Task SendSpiritualGiftsResultsAsync(SpiritualGiftsEntry entry)
    {
        try
        {
            var subject = $"Your Spiritual Gifts Discovery Results - {DateTime.UtcNow.Year}";
            var body = EmailTemplates.GetSpiritualGiftsResultsBody(entry);

            // Send to user
            await SendEmailAsync(entry.Email, subject, body);

            // Send to admin
            var adminEmail = _settings.SenderEmail;
            var adminSubject = $"New Spiritual Gifts Discovery: {entry.FirstName} {entry.LastName}";
            await SendEmailAsync(adminEmail, adminSubject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending discovery email for {Email}", entry.Email);
        }
    }
}
