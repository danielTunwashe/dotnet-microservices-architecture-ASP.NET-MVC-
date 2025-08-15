using Mango.Web.Models;
using Mango.Web.Models.ProductAPI;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class ProductService : IProductService
{
    private readonly IBaseService _baseService;
    public ProductService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> CreateProductAsync(CreateProductRequestDto createProduct)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = createProduct,
            Url = SD.ProductAPIBase + "/api/product/CreateProduct"
        });
    }

    public async Task<ResponseDto?> Delete(int id)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.DELETE,
            Url = SD.ProductAPIBase + "/api/product/DeleteProduct/" + id
        });
    }

    public async Task<ResponseDto?> GetAll()
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ProductAPIBase + "/api/product/GetAll"
        });
    }

    public async Task<ResponseDto?> GetById(int id)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ProductAPIBase + "/api/product/GetById/" + id
        });
    }

    public async Task<ResponseDto?> Update(UpdateProductRequestDto updateProduct)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.PUT,
            Data = updateProduct,
            Url = SD.ProductAPIBase + "/api/product/UpdateProduct"
        });
    }
}
