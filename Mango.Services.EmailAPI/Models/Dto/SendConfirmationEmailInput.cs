namespace Mango.Services.EmailAPI.Models.Dto;

public class SendConfirmationEmailInput
{
    public string ToEmail { get; set; }
    public OrderEmailRequest Order { get; set; }
}
