using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Swashbuckle.AspNetCore.Annotations;
using Web_Shop.Application.DTOs;
using Web_Shop.Application.Helpers.PagedList;
using Web_Shop.Application.Mappings;
using Web_Shop.Application.Services.Interfaces;

namespace Web_Shop.RestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : BaseController
{
    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "GetProductById")]
    public async Task<ActionResult<GetSingleProductDTO>> GetProduct(ulong id)
    {
        var result = await productService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return Problem(result, "Read error.");

        return StatusCode((int)SuccessCode, result.Data!.MapGetSingleProductDTO());
    }

    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetProducts")]
    public async Task<ActionResult<IPagedList<GetSingleProductDTO>>> GetProducts([FromQuery] SieveModel paginationParams)
    {
        var result = await productService.SearchAsync(paginationParams, resultEntity => resultEntity.MapGetSingleProductDTO());

        if (!result.IsSuccess)
            return Problem(result, "Read error.");

        return Ok(result.Data);
    }

    [HttpPost("add")]
    [SwaggerOperation(OperationId = "AddProduct")]
    public async Task<ActionResult<GetSingleProductDTO>> AddProduct([FromBody] AddUpdateProductDTO dto)
    {
        var result = await productService.CreateNewProductAsync(dto);

        if (!result.IsSuccess)
            return Problem(result, "Add error.");

        return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.IdProduct }, result.Data.MapGetSingleProductDTO());
    }

    [HttpPut("update/{id}")]
    [SwaggerOperation(OperationId = "UpdateProduct")]
    public async Task<ActionResult<GetSingleProductDTO>> UpdateProduct(ulong id, [FromBody] AddUpdateProductDTO dto)
    {
        var result = await productService.UpdateExistingProductAsync(dto, id);

        if (!result.IsSuccess)
            return Problem(result, "Update error.");

        return StatusCode((int)SuccessCode, result.Data!.MapGetSingleProductDTO());
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(OperationId = "DeleteProduct")]
    public async Task<IActionResult> DeleteProduct(ulong id)
    {
        var result = await productService.DeleteAndSaveAsync(id);

        if (!result.IsSuccess)
            return Problem(result, "Delete error.");

        return NoContent();
    }
}