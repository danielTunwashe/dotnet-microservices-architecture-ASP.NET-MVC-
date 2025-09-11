using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService;

//Help us retrieve all the product in the product api
//Then we add the url of the product api in our appsettings;
public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetProducts();
}
