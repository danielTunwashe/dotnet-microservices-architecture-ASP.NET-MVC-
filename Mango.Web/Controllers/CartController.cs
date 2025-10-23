using Mango.Services.OrderAPI.Models.Dto.EmailDto;
using Mango.Web.Models;
using Mango.Web.Models.OrderAPIModels;
using Mango.Web.Models.PayStackModel;
using Mango.Web.Models.ShoppingCartAPI;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IEmailService _emailService;
    public CartController(ICartService cartService, IOrderService orderService, IEmailService emailService)
    {
        _cartService = cartService;
        _orderService = orderService;
        _emailService = emailService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();

        return View(cartDto);
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();

        return View(cartDto);
    }


    [HttpPost]
    [ActionName("Checkout")]
    public async Task<IActionResult> Checkout(CartDto cartDto)
    {
        CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
        cart.CartHeader.Phone = cartDto.CartHeader.Phone;
        cart.CartHeader.Email = cartDto.CartHeader.Email;
        cart.CartHeader.Name = cartDto.CartHeader.Name;


        var response = await _orderService.CreateOrder(cart);
        var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(JsonConvert.SerializeObject(response?.Result));

        if(response!=null && response.IsSuccess)
        {
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            InitializePaymentRequestDto initializePaymentRequestDto = new()
            {
                CallbackUrl = domain + "cart/Confirmation?orderId=" + orderHeader.OrderHeaderId,
                Email = cart.CartHeader.Email,
                Amount = (decimal)cart.CartHeader.CartTotal,
                OrderHeader = orderHeader
            };

            var initializePaymentResponseDto = await _orderService.Initialize(initializePaymentRequestDto);
            InitializePaymentResponseDto initializePaymentResponseResult = JsonConvert.DeserializeObject<InitializePaymentResponseDto>(JsonConvert.SerializeObject(initializePaymentResponseDto?.Result));

            Response.Headers.Add("Location", initializePaymentResponseResult.AuthorizationUrl);

            //signifies there is a redirect to another page..
            return new StatusCodeResult(303);
        }
        return View();
    }

    
    public async Task<IActionResult> Confirmation(ConfirmationInput input)
    {
        var verifyReference = await _orderService.Verify(input.Reference);
        var verifyReferenceResult = JsonConvert.DeserializeObject<VerifyPaymentResponseDto>(JsonConvert.SerializeObject(verifyReference.Result));

        if (verifyReferenceResult.Status == true)
        {
           
            var updateOrder = new UpdateOrderDto
            {
                OrderId = input.OrderId,
                Reference = input.Reference,
            };

            var updatedOrderHeader = await _orderService.UpdateOrder(updateOrder);
            var updateOrderHeaderResult = JsonConvert.DeserializeObject<OrderHeaderDto>(JsonConvert.SerializeObject(updatedOrderHeader.Result));
            
            if (updatedOrderHeader.IsSuccess == true && updateOrderHeaderResult!= null)
            {

                var emailPayload = new SendConfirmationEmailInput
                {
                    ToEmail = updateOrderHeaderResult.Email,
                    Order = new OrderEmailRequest
                    {
                        OrderHeaderId = updateOrderHeaderResult.OrderHeaderId,
                        OrderTotal = updateOrderHeaderResult.OrderTotal,
                        Name = updateOrderHeaderResult.Name,
                        OrderDetails = updateOrderHeaderResult.OrderDetails.Select(d => new OrderItemDto
                        {
                            Count = d.Count,
                            Price = d.Price,
                            ProductName = d.ProductName
                        }).ToList()
                    }
                };

                var email = await _emailService.SendOrderConfirmation(emailPayload);
                var emailResult = JsonConvert.DeserializeObject<EmailResult>(JsonConvert.SerializeObject(email));

                if(emailResult.Success == true)
                {
                    TempData["success"] = "Payment Verification Successful, OrderHeader Updated Successfully... && EmailSent..";
                }
            }
        }   
        else
        {   
            TempData["error"] = "Payment Verification Failed";
        }
        return View(input);
    }

    public async Task<IActionResult> Remove(RemoveCartInput input)
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
        ResponseDto? response = await _cartService.RemoveFromCartAsync(input.CartDetailsId);
        if (response.Result != null && response.IsSuccess)
        {
            TempData["success"] = "Item removed from cart successfully.";
            return RedirectToAction(nameof(CartIndex)); 
        }
        return View();
    }

    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto!);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon Applied";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        cartDto.CartHeader.CouponCode = "";
        ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto!);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon Removed";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
        ResponseDto?  response = await _cartService.GetCartByUserIdAsync(userId!);
        if(response != null && response.IsSuccess)
        {
            CartDto? cartDto = JsonConvert.DeserializeObject<CartDto>(JsonConvert.SerializeObject((response.Result)));
            return cartDto!;
        }
        return new CartDto();
    }
}
