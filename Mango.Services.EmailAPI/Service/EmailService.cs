using Mango.Services.EmailAPI.Models.CustomEmailEntity;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Service.IService;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using Mango.Services.EmailAPI.Models;

namespace Mango.Services.EmailAPI.Service;

public class EmailService : IEmailService
{
    private readonly IViewRenderService _viewRenderService;
    private readonly EmailSettings _emailSettings;
    public EmailService(IViewRenderService viewRenderService, IOptions<EmailSettings> emailSettings)
    {
        _viewRenderService = viewRenderService;
        _emailSettings = emailSettings.Value;
    }

    public async Task<EmailResult> SendOrderConfirmationEmailAsync(SendConfirmationEmailInput input)
    {
        try
        {
            var body = await _viewRenderService.RenderToStringAsync("EmailTemplates/OrderConfirmation", input.Order);

            using (SmtpClient client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
            {
                client.EnableSsl = _emailSettings.EnableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.AppPassword);

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = $"Order Confirmation - Order #{input.Order.OrderHeaderId}",
                    IsBodyHtml = true,
                    Body = body
                };

                mailMessage.To.Add(input.ToEmail);

                await client.SendMailAsync(mailMessage);

                return new EmailResult { Success = true, Message = "Email sent successfully" };
            }
        }
        catch (Exception ex)
        {

            return new EmailResult { Success = false, Message = ex.Message };
        }

    }

}
