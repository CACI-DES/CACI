namespace CACI_Life.Features.Email;

public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = "Contact@caci.life";
    public string SenderName { get; set; } = "CACI Life";
}
