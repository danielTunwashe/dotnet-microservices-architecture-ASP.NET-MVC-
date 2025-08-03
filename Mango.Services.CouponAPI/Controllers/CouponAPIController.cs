using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[ApiController]
public class CouponAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private ResponseDto _response;

    public CouponAPIController(AppDbContext db)
    {
        _db = db;
        _response = new ResponseDto();
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            IEnumerable<Coupon> objList = _db.Coupons.ToList(); 
            _response.Result = objList;
            
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;    
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpGet("{id}")]
    public object Get([FromRoute]int id)
    {
        try
        {
            Coupon objList = _db.Coupons.First(u => u.CouponId == id);
            _response.Result = objList;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpGet("GetByCode/{code}")]
    public ResponseDto Get([FromRoute]string code)
    {
        try
        {
            Coupon objList = _db.Coupons.First(u => u.CouponCode.ToLower() == code.ToLower());
            _response.Result = objList;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost]
    public ResponseDto Post([FromBody] CouponDto couponDto)
    {
        try
        {
            Coupon obj = new Coupon()
            {
                CouponCode = couponDto.CouponCode,
                DiscountAmount = couponDto.DiscountAmount,
                MinAmount = couponDto.MinAmount
            };
            _db.Coupons.Add(obj);
            _db.SaveChanges();
            _response.Result = obj;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPut]
    public ResponseDto put([FromBody] CouponDto couponDto)
    {

        try
        {
            var coupon = _db.Coupons.FirstOrDefault(c=> c.CouponId == couponDto.CouponId);
            if (coupon == null) { throw new Exception("Coupon Not Found.."); }

            coupon.CouponCode = couponDto.CouponCode;
            coupon.DiscountAmount = couponDto.DiscountAmount;
            coupon.MinAmount = couponDto.MinAmount;
            
            _db.Coupons.Update(coupon);
            _db.SaveChanges();
            _response.Result = coupon;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpDelete("{id}")]
    public ResponseDto Delete([FromRoute]int id)
    {

        try
        {
            var coupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
            if (coupon == null) { throw new Exception("Coupon Not Found.."); }

          
            _db.Coupons.Remove(coupon);
            _db.SaveChanges();
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

}
