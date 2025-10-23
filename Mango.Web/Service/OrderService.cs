using Mango.Web.Models;
using Mango.Web.Models.OrderAPIModels;
using Mango.Web.Models.PayStackModel;
using Mango.Web.Models.ShoppingCartAPI;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using System.Security.Cryptography.Xml;

namespace Mango.Web.Service;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;   
    public OrderService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.OrderAPIBase + "/api/Order/CreateOrder"
        });
    }

    public async Task<ResponseDto?> GetAllOrders(string? UserId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.OrderAPIBase + $"/api/Order/GetOrders/{UserId}"
        });
    }

    public async Task<ResponseDto?> GetOrderById(int OrderId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.OrderAPIBase + $"/api/Order/GetOrderById/{OrderId}"
        });
    }

    public async Task<ResponseDto?> Initialize(InitializePaymentRequestDto request)
    {
        var intial =  await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = request,
            Url = SD.OrderAPIBase + "/api/Order/Initialize"
        });

        return intial;
    }

    public async Task<ResponseDto?> UpdateOrder(UpdateOrderDto updateOrder)
    {
        var update =  await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.PUT,
            Data = updateOrder,
            Url = SD.OrderAPIBase + $"/api/Order/UpdateOrder"
        });

        return update;
    }

    public async Task<ResponseDto?> UpdateOrderStatus(UpdateOrderStatusInput input)
    {
        var updateStatus = await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.PUT,
            Data = input,
            Url = SD.OrderAPIBase + $"/api/Order/UpdateOrderStatus"
        });

        return updateStatus;
    }

    public async Task<ResponseDto?> Verify(string reference)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.OrderAPIBase + $"/api/Order/verify/{reference}"
        });
    }

    
}
