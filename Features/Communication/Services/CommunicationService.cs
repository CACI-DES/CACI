using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CACI_Life.Features.Email;

namespace CACI_Life.Features.Communication.Services;

public class ContactSubmission
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class BibleStudySubmission
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CityState { get; set; }
    public string? Interest { get; set; }
    public string? AboutYou { get; set; }
    public string? Source { get; set; }
}

public interface ICommunicationService
{
    Task SubmitContactAsync(ContactSubmission request);
    Task SubmitBibleStudyAsync(BibleStudySubmission request);
}

public class CommunicationService : ICommunicationService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<CommunicationService> _logger;

    public CommunicationService(IEmailService emailService, ILogger<CommunicationService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SubmitContactAsync(ContactSubmission request)
    {
        try
        {
            var subject = $"New Contact Request: {request.RequestType}";
            var details = $@"
                <p><strong>Name:</strong> {request.Name}</p>
                <p><strong>Email:</strong> <a href='mailto:{request.Email}'>{request.Email}</a></p>
                <p><strong>Type:</strong> {request.RequestType}</p>
                <p><strong>Message:</strong></p>
                <blockquote style='border-left: 3px solid #174477; padding-left: 10px; color: #555;'>
                    {(request.Message ?? "").Replace("\n", "<br>")}
                </blockquote>";

            var adminBody = EmailTemplates.GetAdminNotificationBody(subject, details);
            await _emailService.SendEmailAsync("Contact@caci.life", subject, adminBody);

            var confirmBody = EmailTemplates.GetUserConfirmationBody(request.Name, request.RequestType);
            await _emailService.SendEmailAsync(request.Email, "We received your message - CACI", confirmBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing contact submission for {Email}", request.Email);
            throw;
        }
    }

    public async Task SubmitBibleStudyAsync(BibleStudySubmission request)
    {
        try
        {
            var subject = $"New Bible Study Request: {request.Interest ?? "General"}";
            var details = $@"
                <p><strong>Name:</strong> {request.Name}</p>
                <p><strong>Email:</strong> <a href='mailto:{request.Email}'>{request.Email}</a></p>
                <p><strong>Location:</strong> {request.CityState}</p>
                <p><strong>Source / Funnel:</strong> {request.Source ?? "Direct"}</p>
                <p><strong>Interest:</strong> {request.Interest}</p>
                <p><strong>About / Questions:</strong></p>
                <blockquote style='border-left: 3px solid #174477; padding-left: 10px; color: #555;'>
                    {(request.AboutYou ?? "").Replace("\n", "<br>")}
                </blockquote>";

            var adminBody = EmailTemplates.GetAdminNotificationBody("New Bible Study / Small Group Request", details);
            await _emailService.SendEmailAsync("Contact@caci.life", subject, adminBody);

            var confirmBody = EmailTemplates.GetUserConfirmationBody(request.Name, $"Bible Study: {request.Interest}");
            await _emailService.SendEmailAsync(request.Email, "We received your Bible study request - CACI", confirmBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Bible study submission for {Email}", request.Email);
            throw;
        }
    }
}
