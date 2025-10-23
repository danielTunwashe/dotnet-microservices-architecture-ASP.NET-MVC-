using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Models.Dto.EmailDto;
using Mango.Services.OrderAPI.Service.IService;

namespace Mango.Services.OrderAPI.Service;

public class EmailService : IEmailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public EmailService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<EmailResult> SendOrderConfirmationEmailAsync(SendConfirmationEmailInput input)
    {
        //create the client that will be making the request
        var client = _httpClientFactory.CreateClient("Email");

        //Create the email payload required..

        var emailPayload = new SendConfirmationEmailInput
        {
            ToEmail = input.ToEmail,
            Order = new OrderEmailRequest
            {
                OrderHeaderId = input.Order.OrderHeaderId,
                OrderTotal = input.Order.OrderTotal,
                Name = input.Order.Name,
                OrderDetails = input.Order.OrderDetails.Select(d => new OrderItemDto
                {
                    Count = d.Count,
                    Price = d.Price,
                    ProductName = d.ProductName
                }).ToList()
            }
        };

        try
        {
            var response = await client.PostAsJsonAsync("/api/email/send-order-confirmation", emailPayload);

            if (response.IsSuccessStatusCode)
            {
                return new EmailResult { Success = true, Message = "Email request sent successfully." };
            }

            var error = await response.Content.ReadAsStringAsync();
            return new EmailResult { Success = false, Message = $"Email API failed: {error}" };
        }
        catch (Exception ex)
        {
            return new EmailResult { Success = false, Message = $"Exception: {ex.Message}" };
        }
    }
}
