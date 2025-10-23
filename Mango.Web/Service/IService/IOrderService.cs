using Mango.Web.Models;
using Mango.Web.Models.OrderAPIModels;
using Mango.Web.Models.PayStackModel;
using Mango.Web.Models.ShoppingCartAPI;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Service.IService;

public interface IOrderService
{
    Task<ResponseDto?> CreateOrder(CartDto cartDto);
    Task<ResponseDto?> Initialize(InitializePaymentRequestDto request);
    Task<ResponseDto?> Verify(string reference);
    Task<ResponseDto?> GetOrderById(int OrderId);
    Task<ResponseDto?> UpdateOrder(UpdateOrderDto updateOrder);
    Task<ResponseDto?> UpdateOrderStatus(UpdateOrderStatusInput input);
    Task<ResponseDto?> GetAllOrders(string? UserId);
}
