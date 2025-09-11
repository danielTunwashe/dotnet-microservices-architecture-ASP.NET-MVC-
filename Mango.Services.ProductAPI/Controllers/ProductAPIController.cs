using Mango.Services.ProductAPI.Constants;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Products.Commands.CreateProduct;
using Mango.Services.ProductAPI.Products.Commands.DeleteProduct;
using Mango.Services.ProductAPI.Products.Commands.UpdateProduct;
using Mango.Services.ProductAPI.Products.Queries.GetAllProduct;
using Mango.Services.ProductAPI.Products.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Mango.Services.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly IMediator _mediator;
    private ResponseDto _response;
    public ProductAPIController(IMediator mediator)
    {
        _mediator = mediator;
        _response = new ResponseDto();
    }

    [Authorize(Roles = SD.ADMIN)]
    [HttpPost("CreateProduct")]
    public async Task<ActionResult<ProductResponseDto?>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        if(product == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Product not created an error occured";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        _response.Message = "Product created successfully..";
        _response.Result = product;

        return Ok(_response);
    }


    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto?>>> GetAll()
    {
        var allProducts = await _mediator.Send(new GetAllQuery());
        if (allProducts == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Product could not be retrieved";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        _response.Message = "Product retrieved successfully..";
        _response.Result = allProducts;

        return Ok(_response);

    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto?>>> GetById([FromRoute] int id)
    {
        var product = await _mediator.Send(new GetByIdQuery(id));

        if (product == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Product not gotten";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        _response.Message = "Product gotton successfully..";
        _response.Result = product;


        return Ok(_response);

    }
    [Authorize(Roles = SD.ADMIN)]
    [HttpPut("UpdateProduct")]
    public async Task<ActionResult<ProductResponseDto>> Update([FromBody] UpdateCommand command)
    {
        var updatedProduct = await _mediator.Send(command);
        if (updatedProduct == null)
        {
            _response.IsSuccess = false;
            _response.Message = "Product not gotten";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        _response.Message = "Product gotton successfully..";
        _response.Result = updatedProduct;


        return Ok(_response);
    }

    [Authorize(Roles = SD.ADMIN)]
    [HttpDelete("DeleteProduct/{id}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] int id, DeleteCommand command)
    {
        command.Id = id;
        var isDeleted = await _mediator.Send(command);
        if(!isDeleted == false)
        {
            _response.IsSuccess = false;
            _response.Message = "Product not deleted";
            return BadRequest(_response);
        }

        _response.IsSuccess = true;
        _response.Message = "Product deleted successfully..";
        return Ok(_response);
    }
}
