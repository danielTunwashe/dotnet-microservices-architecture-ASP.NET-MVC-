using Mango.Services.EmailAPI.Models.Dto;

namespace Mango.Services.EmailAPI.Service.IService;

public interface IViewRenderService
{
    Task<string> RenderToStringAsync(string viewName, OrderEmailRequest model);
}
