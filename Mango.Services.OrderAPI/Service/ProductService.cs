using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Service;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<IEnumerable<ProductResponseDto>> GetProducts()
    {
        //create the client that will be making the request
        var client = _httpClientFactory.CreateClient("Product");
        //Get the response from the product api using the client created
        var response = await client.GetAsync($"/api/product/GetAll");
        //Get the API content from the response created it coming as string
        var apiContent = await response.Content.ReadAsStringAsync();
        //Finally get the response back by deserializing to our response dto
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductResponseDto>>(Convert.ToString(resp.Result));
        }
        return new List<ProductResponseDto>();
    }
}
