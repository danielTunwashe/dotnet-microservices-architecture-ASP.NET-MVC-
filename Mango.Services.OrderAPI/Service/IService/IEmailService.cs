using Mango.Services.OrderAPI.Models.Dto.EmailDto;

namespace Mango.Services.OrderAPI.Service.IService;

public interface IEmailService
{
    Task<EmailResult> SendOrderConfirmationEmailAsync(SendConfirmationEmailInput input);
}
