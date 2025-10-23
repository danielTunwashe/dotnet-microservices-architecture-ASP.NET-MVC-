using Mango.Web.Models;
using Mango.Web.Models.ShoppingCartAPI;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class CartService : ICartService
{
    private readonly IBaseService _baseService;   
    public CartService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/ApplyCoupon"
        });
    }


    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ShoppingCartAPIBase + $"/api/Cart/GetCart/{userId}"
        });
    }

     
    public async Task<ResponseDto?> RemoveFromCartAsync(int CartDetailsId)
    {
        var response = await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = CartDetailsId,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/RemoveCart"
        });

        return response;
    }


    public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
    {
        var response =  await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/CartUpsert"
        });

        return response;
    }
}
