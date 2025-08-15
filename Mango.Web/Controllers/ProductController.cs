using Mango.Services.ProductAPI.Models;
using Mango.Web.Models;
using Mango.Web.Models.ProductAPI;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> ProductIndex()
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



    public async Task<IActionResult> ProductUpdate(int productId)
    {
        ResponseDto? response = await _productService.GetById(productId);

        if (response != null && response.IsSuccess)
        {
            Product? model = JsonConvert.DeserializeObject<Product>(JsonConvert.SerializeObject(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
            
        }
        return View();

    }



    [HttpPost]
    public async Task<IActionResult> ProductUpdate(UpdateProductRequestDto updateProduct)
    {
        ResponseDto? response = await _productService.Update(updateProduct);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product updated successfully..";
            return RedirectToAction(nameof(ProductIndex));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NoContent();
    }


}
