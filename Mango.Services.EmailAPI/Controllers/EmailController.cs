using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.EmailAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private ResponseDto _response;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
            _response = new ResponseDto();
        }

        [HttpPost("send-order-confirmation")] //Need to create to email and order header dto...
        public async Task<ResponseDto> SendOrderConfirmation([FromBody] SendConfirmationEmailInput request)
        {
            if (request == null)
                throw new Exception("Invalid request payload.");

            var result = await _emailService.SendOrderConfirmationEmailAsync(request);

            if (result.Success)
            {
                _response.Message = result.Message;
                _response.IsSuccess = result.Success;
            }
            else
            {
                _response.Message = result.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
