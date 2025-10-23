namespace Mango.Web.Models.ShoppingCartAPI;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; }
    public List<CartDetailsDto>? CartDetails { get; set; } 

}
