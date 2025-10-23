namespace Mango.Services.OrderAPI.Models.Dto.EmailDto;

public class SendConfirmationEmailInput
{
    public string ToEmail { get; set; }
    public OrderEmailRequest Order { get; set; }
}
