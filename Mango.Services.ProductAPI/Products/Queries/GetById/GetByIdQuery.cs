using Mango.Services.ProductAPI.Models.Dto;
using MediatR;

namespace Mango.Services.ProductAPI.Products.Queries.GetById;

public class GetByIdQuery : IRequest<ProductResponseDto>
{
    public GetByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get;}
}
