namespace Mango.Services.OrderAPI.Models.Dto;

public class PaystackWebhookResponseDto
{
    public string Event { get; set; }
    public dynamic Data { get; set; }
}
