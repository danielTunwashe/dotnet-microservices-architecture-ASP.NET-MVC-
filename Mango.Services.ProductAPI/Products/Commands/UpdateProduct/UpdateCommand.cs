using Mango.Services.ProductAPI.Models.Dto;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductAPI.Products.Commands.UpdateProduct;

public class UpdateCommand : IRequest<ProductResponseDto>
{
    public int ProductId { get; set; }
    [Required]
    public string Name { get; set; }
    [Range(1, 1000)]
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
}
