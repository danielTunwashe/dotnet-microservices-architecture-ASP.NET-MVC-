using Mango.Services.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.PayStackModel;

public class InitializePaymentRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public decimal Amount { get; set; } 
    [Required]
    public string CallbackUrl { get; set; }
    [Required]
    public OrderHeaderDto OrderHeader { get; set; }
}
