using Mango.Services.CouponAPI.Models;

namespace Mango.Services.ShoppingCartAPI.Service.IService;

//Help us retrieve all the product in the product api
//Then we add the url of the product api in our appsettings;
public interface ICouponService
{
    Task<Coupon> GetCoupon(string couponCode);
}
