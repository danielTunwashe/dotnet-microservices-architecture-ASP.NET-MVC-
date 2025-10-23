using Mango.Services.OrderAPI.Models.Dto.EmailDto;
using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IEmailService
{
    Task<ResponseDto?> SendOrderConfirmation(SendConfirmationEmailInput request);
}
