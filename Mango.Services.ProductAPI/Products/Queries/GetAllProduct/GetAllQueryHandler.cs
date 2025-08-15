using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using MediatR;

namespace Mango.Services.ProductAPI.Products.Queries.GetAllProduct;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery,IEnumerable<ProductResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    public GetAllQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }


    public async Task<IEnumerable<ProductResponseDto>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var allProducts = await _productRepository.GetAll();
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        var allProductResponse = _mapper.Map<IEnumerable<ProductResponseDto>>(allProducts);
        return allProductResponse;
    }

}
