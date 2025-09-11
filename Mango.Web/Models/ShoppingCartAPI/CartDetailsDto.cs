using Mango.Web.Models.ProductAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Web.Models.ShoppingCartAPI;

public class CartDetailsDto
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public CartHeaderDto? CartHeader { get; set; }
    public int ProductId { get; set; }
    public ProductResponseDto? Product { get; set; }
    public int Count { get; set; }
}
