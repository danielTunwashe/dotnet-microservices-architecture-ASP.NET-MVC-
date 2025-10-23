namespace Mango.Services.OrderAPI.Models.Dto;

public class UpdateOrderStatusInput
{
    public int OrderId { get; set; }
    public string NewStatus { get; set; }
}
