using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[Route("api/Cart")]
[ApiController]
public class CartAPIController : ControllerBase
{
    private ResponseDto _response;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private IProductService _productService;
    private readonly ICouponService _couponService;
    public CartAPIController(AppDbContext context, IMapper mapper, IProductService productService, ICouponService couponService)
    {
        this._response = new ResponseDto();
        _context = context;
        _mapper = mapper;
        _productService = productService;
        _couponService = couponService;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartFromDb = await _context.cartHeaders.FirstAsync(ch => ch.UserId == cartDto.CartHeader.UserId);
            cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
            _context.cartHeaders.Update(cartFromDb);
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }


    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartFromDb = await _context.cartHeaders.FirstAsync(ch => ch.UserId == cartDto.CartHeader.UserId);
            cartFromDb.CouponCode = "";
            _context.cartHeaders.Update(cartFromDb);
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }


    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDto> GetCart(string userId)
    {
        try
        {
            CartDto cart = new()
            {
                CartHeader = _mapper.Map<CartHeaderDto>(_context.cartHeaders.First(u => u.UserId == userId))
            };
            cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.cartDetails
                .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

            IEnumerable<ProductResponseDto> productResponseDto = await _productService.GetProducts();

            foreach (var item in cart.CartDetails)
            {
                item.Product = productResponseDto.FirstOrDefault(u => u.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
            }

            //apply coupon if any
            if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                {
                    cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cart.CartHeader.Discount = coupon.DiscountAmount;
                }
            }

            _response.Result = cart;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }


    [HttpPost("CartUpsert")]
    public async Task<ResponseDto> CartUpsert([FromBody] CartDto cartDto)
    {
        try
        {
            var cartHeaderFromDb = await _context.cartHeaders.AsNoTracking().FirstOrDefaultAsync(ch => ch.UserId == cartDto.CartHeader.UserId);
            if (cartHeaderFromDb == null)
            {
                //create cart header and details
                CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                _context.cartHeaders.Add(cartHeader);
                await _context.SaveChangesAsync();

                cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                _context.cartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                await _context.SaveChangesAsync();
            }
            else
            {
                //if header is not null
                //check if details has the same product
                var cartDetailsFromDb = await _context.cartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                    u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if(cartDetailsFromDb == null)
                {
                    //create cartDetails
                    cartDto.CartDetails.First().CartHeaderId= cartHeaderFromDb.CartHeaderId;
                    _context.cartDetails.Add(_mapper.Map <CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //update count in cart details
                    cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                    cartDto.CartDetails.First().CartHeaderId =  cartDetailsFromDb.CartHeaderId;
                    cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                    _context.cartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
            }
            _response.Result = cartDto;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }

    
    [HttpPost("RemoveCart")]
    public async Task<ResponseDto> RemoveCart([FromBody] int CartDetailsId)
    {
        try
        {
            var cartDetails = await _context.cartDetails.FirstOrDefaultAsync(cd => cd.CartDetailsId == CartDetailsId);

            if (cartDetails == null)
            {
                _response.Message = "CartDetails not found";
                throw new Exception("CartDetails not found");
            }

            int totalCountofCartItems = _context.cartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
            _context.cartDetails.Remove(cartDetails);

            if(totalCountofCartItems == 1)
            {
                var cartHeaderToRemove = await _context.cartHeaders
                    .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                _context.cartHeaders.Remove(cartHeaderToRemove);
                _response.Message = $"Cart Header with Id {cartDetails.CartHeaderId} Removed successfully..";
                _response.IsSuccess = true;
            }
            await _context.SaveChangesAsync();

            _response.Result = true;
            _response.Message = $"Cart Details with Id {cartDetails.CartDetailsId} Removed successfully..";
            _response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }
}
