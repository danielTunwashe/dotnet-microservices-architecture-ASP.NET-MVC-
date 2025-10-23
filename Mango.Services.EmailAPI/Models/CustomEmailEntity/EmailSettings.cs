namespace Mango.Services.EmailAPI.Models.CustomEmailEntity;

public class EmailSettings
{
    public string SmtpServer { get; set; } = default!;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string Username { get; set; } = default!;
    public string AppPassword { get; set; } = default!;
    public string SenderEmail { get; set; } = default!;
}
