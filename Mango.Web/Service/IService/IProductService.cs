using Mango.Web.Models;
using Mango.Web.Models.ProductAPI;

namespace Mango.Web.Service.IService;

public interface IProductService
{
    Task<ResponseDto?> CreateProductAsync(CreateProductRequestDto createProduct);
    Task<ResponseDto?> GetAll();
    Task<ResponseDto?> GetById(int id);
    Task<ResponseDto?> Update(UpdateProductRequestDto updateProduct);
    Task<ResponseDto?> Delete(int id);
}
