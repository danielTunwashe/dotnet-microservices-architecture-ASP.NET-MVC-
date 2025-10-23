using Mango.Services.ProductAPI.Models;
using Mango.Web.Models;
using Mango.Web.Models.AuthApiModels;
using Mango.Web.Models.ProductAPI;
using Mango.Web.Models.ShoppingCartAPI;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService, IAuthService authService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Product>? product = new List<Product>();

        ResponseDto? response = await _productService.GetAll();

        if (response != null && response.IsSuccess)
        {
            product = JsonConvert.DeserializeObject<IEnumerable<Product>>(JsonConvert.SerializeObject(response.Result));
        }
        else
        {
            //Assing the error to temp data in order to display the notification..
            //So tempdata value is the error message at any point in time..
            TempData["error"] = response?.Message;
        }

        return View(product);
    }

    [Authorize]
    public async Task<IActionResult> ProductDetails(int ProductId)
    {
        ProductResponseDto? model = new();
        ResponseDto? response = await _productService.GetById(ProductId);

        if (response != null && response.IsSuccess)
        {
            model = JsonConvert.DeserializeObject<ProductResponseDto>(JsonConvert.SerializeObject(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> ProductDetails(ProductResponseDto productResponseDto)
    {
        try
        {
            // ✅ Stage 1: Validate incoming DTO
            if (productResponseDto == null)
            {
                TempData["error"] = "Invalid product data.";
                return View(productResponseDto);
            }

            if (productResponseDto.ProductId <= 0 || productResponseDto.Count <= 0)
            {
                TempData["error"] = "Invalid product selection or count.";
                return View(productResponseDto);
            }

            // Extract logged-in user
            var userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "User not authenticated.";
                return RedirectToAction("Login", "Account");
            }

            var input = new GetUserByIdInput
            {
                UserId = userId,
            };

            var userDetails = await _authService.GetUserByIdAsync(input);
            var userDetailsResult =  JsonConvert.DeserializeObject<GetUserByIdOutput>(JsonConvert.SerializeObject(userDetails.Result));

            if(userDetailsResult.User == null)
            {
                throw new Exception("User Not found.. (API returns null..)");
            }

            // Build cart DTO
            CartDto cartDto = new CartDto
            {
                CartHeader = new CartHeaderDto { UserId = userId,Name = userDetailsResult.User.Name, Email = userDetailsResult.User.Email, Phone = userDetailsResult.User.PhoneNumber },
                CartDetails = new List<CartDetailsDto>
            {
                new CartDetailsDto
                {
                    Count = productResponseDto.Count,
                    ProductId = productResponseDto.ProductId
                }
            }  };

            // Call service
            ResponseDto response = await _cartService.UpsertCartAsync(cartDto);
            var responseResult = JsonConvert.DeserializeObject<CartDto>(JsonConvert.SerializeObject(response.Result));

            if (response == null)
            {
                TempData["error"] = "Unexpected error: cart service returned null.";
                return View(productResponseDto);
            }

            // Handle service response
            if (response.IsSuccess && response.Result != null)
            {
                TempData["success"] = "Item has been added to the shopping cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message ?? "Failed to update cart.";
                return View(productResponseDto);
            }
        }
        catch (Exception ex)
        {
            // Unexpected exception handling
            TempData["error"] = $"An unexpected error occurred: {ex.Message}";
            return View(productResponseDto);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
