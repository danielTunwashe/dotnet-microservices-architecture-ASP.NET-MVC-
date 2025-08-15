using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using MediatR;

namespace Mango.Services.ProductAPI.Products.Commands.UpdateProduct;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, ProductResponseDto>
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    public UpdateCommandHandler(IMapper mapper, IProductRepository productRepository)
    {
        _mapper = mapper;
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetById(request.ProductId);
        if(product == null)
        {
            throw new Exception("Product not found..");
        }

        var productMapped = _mapper.Map(request, product);
        var productResponse = await _productRepository.Update(productMapped);

        var response = _mapper.Map<ProductResponseDto>(productResponse);
        return response;
    }
}
