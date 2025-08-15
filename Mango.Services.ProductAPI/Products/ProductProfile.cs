using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Products.Commands.CreateProduct;
using Mango.Services.ProductAPI.Products.Commands.UpdateProduct;

namespace Mango.Services.ProductAPI.Products;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore());
        CreateMap<UpdateCommand, Product>();

        


        CreateMap<Product, ProductResponseDto>();
    }
}
