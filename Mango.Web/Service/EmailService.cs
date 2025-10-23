using Mango.Services.OrderAPI.Models.Dto.EmailDto;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class EmailService : IEmailService
{
    private readonly IBaseService _baseService;
    public EmailService(IBaseService baseService)
    {
        _baseService = baseService;
    }


    public async Task<ResponseDto?> SendOrderConfirmation(SendConfirmationEmailInput request)
    {
        var email = await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = request,
            Url = SD.EmailAPIBase + "/api/email/send-order-confirmation"
        });

        return email;
    }
}
