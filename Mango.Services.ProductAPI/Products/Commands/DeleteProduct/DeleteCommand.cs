using MediatR;

namespace Mango.Services.ProductAPI.Products.Commands.DeleteProduct;

public class DeleteCommand : IRequest<bool>
{
    public DeleteCommand(int id)
    {
        Id = id;
    }
    public int Id { get; set; }
}
