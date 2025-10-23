using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Models.Dto;

public class PaystackWebhookRequestDto
{
    [Required]
    public string RequestBody { get; set; }
    [Required]
    public string Signature { get; set; }
}
