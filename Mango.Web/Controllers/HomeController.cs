using Mango.Services.ProductAPI.Models;
using Mango.Web.Models;
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

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
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
        CartDto cartDto = new CartDto()
        {
            CartHeader = new CartHeaderDto
            {
                UserId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value
            },
           
        };

        CartDetailsDto cartDetails = new CartDetailsDto()
        {
            Count = productResponseDto.Count,
            ProductId = productResponseDto.ProductId,
        };

        List<CartDetailsDto> cartDetailsDto = new() { cartDetails };

        cartDto.CartDetails = cartDetailsDto;

        ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);



        if (response.Result != null && response.IsSuccess)
        {
            TempData["success"] = "Item has been added to the shopping cart";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(productResponseDto);
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
