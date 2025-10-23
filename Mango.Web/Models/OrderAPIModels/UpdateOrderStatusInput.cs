namespace Mango.Web.Models.OrderAPIModels;

public class UpdateOrderStatusInput
{
    public int OrderId { get; set; }
    public string NewStatus { get; set; }
}
