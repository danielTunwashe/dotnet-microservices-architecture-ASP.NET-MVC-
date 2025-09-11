using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;


namespace Mango.Services.ShoppingCartAPI.Application;

public class ShoppingCartProfile : Profile
{
    public ShoppingCartProfile()
    {
        CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
        CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
    }
}
