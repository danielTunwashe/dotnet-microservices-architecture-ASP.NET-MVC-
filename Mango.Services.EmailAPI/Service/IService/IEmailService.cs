using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;

namespace Mango.Services.EmailAPI.Service.IService;

public interface IEmailService
{
    Task<EmailResult> SendOrderConfirmationEmailAsync(SendConfirmationEmailInput input);
}
