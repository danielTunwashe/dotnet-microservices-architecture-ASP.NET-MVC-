namespace Mango.Services.OrderAPI.Models.Dto.EmailDto;

public class OrderEmailRequest
{
    public int OrderHeaderId { get; set; }
    public double OrderTotal { get; set; }
    public string? Name { get; set; }

    public List<OrderItemDto> OrderDetails { get; set; }
}
