namespace Mango.Web.Models.PayStackModel;

public class PaystackWebhookResponseDto
{
    public string Event { get; set; }
    public dynamic Data { get; set; }
}
