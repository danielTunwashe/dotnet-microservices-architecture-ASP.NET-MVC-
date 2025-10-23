using Mango.Web.Models.OrderAPIModels;

namespace Mango.Web.Models.PayStackModel;

public class InitializePaymentRequestDto
{
    public string Email { get; set; }
    public decimal Amount { get; set; } // in Naira
    public string CallbackUrl { get; set; }
    public OrderHeaderDto OrderHeader { get; set; }
}
