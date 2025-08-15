using Mango.Services.ProductAPI.Models.Dto;
using MediatR;

namespace Mango.Services.ProductAPI.Products.Queries.GetAllProduct;

public class GetAllQuery : IRequest<IEnumerable<ProductResponseDto>>
{

}
