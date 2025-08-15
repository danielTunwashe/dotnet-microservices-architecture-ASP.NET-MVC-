using Mango.Services.ProductAPI.Models;
using MediatR;

namespace Mango.Services.ProductAPI.Products.Commands.DeleteProduct;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
{
    private readonly IProductRepository _productRepository;
    public DeleteCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetById(request.Id);
        if (product == null)
        {
            throw new Exception($"Product with Id {product.ProductId} not found");
        }
        var isProductDeleted = await _productRepository.Delete(product);

        return isProductDeleted;
    }
}
