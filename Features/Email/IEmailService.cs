using System.Threading.Tasks;
using CACI_Life.Features.Discovery;

namespace CACI_Life.Features.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, byte[] attachmentBytes, string filename);
    Task SendSpiritualGiftsResultsAsync(SpiritualGiftsEntry entry);
}
