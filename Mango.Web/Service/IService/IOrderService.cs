using Mango.Web.Models;
using Mango.Web.Models.ShoppingCartAPI;

namespace Mango.Web.Service.IService;

public interface IOrderService
{
    Task<ResponseDto?> CreateOrder(CartDto cartDto);
}
