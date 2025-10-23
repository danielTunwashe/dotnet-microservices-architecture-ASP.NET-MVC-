using Mango.Web.Models.OrderAPIModels;

namespace Mango.Web.Models.ShoppingCartAPI;

public class ConfirmationInput
{
    public int OrderId { get; set; }
    public string Reference { get; set; }

    public UpdateOrderDto UpdateOrder { get; set; }
}
