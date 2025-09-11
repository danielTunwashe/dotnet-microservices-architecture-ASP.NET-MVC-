using Mango.Services.CouponAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;


namespace Mango.Services.ShoppingCartAPI.Service;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Coupon> GetCoupon(string couponCode)
    {
        //create the client that will be making the request
        var client = _httpClientFactory.CreateClient("Coupon");
        //Get the response from the product api using the client created
        var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
        //Get the API content from the response created it coming as string
        var apiContent = await response.Content.ReadAsStringAsync();
        //Finally get the response back by deserializing to our response dto
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<Coupon>(Convert.ToString(resp.Result));
        }
        return new Coupon();
    }
}
