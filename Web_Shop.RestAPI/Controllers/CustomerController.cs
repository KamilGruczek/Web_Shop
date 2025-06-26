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
public class CustomerController(ICustomerService customerService) : BaseController
{
    [HttpGet("{id}")]
    [SwaggerOperation(OperationId = "GetCustomerById")]
    public async Task<ActionResult<GetSingleCustomerDTO>> GetCustomer(ulong id)
    {
        var result = await customerService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return Problem(result, "Read error.");

        return StatusCode((int)SuccessCode, result.Data!.MapGetSingleCustomerDTO());
    }

    [HttpGet("list")]
    [SwaggerOperation(OperationId = "GetCustomers")]
    public async Task<ActionResult<IPagedList<GetSingleCustomerDTO>>> GetCustomers([FromQuery] SieveModel paginationParams)
    {
        var result = await customerService.SearchAsync(paginationParams, resultEntity => resultEntity.MapGetSingleCustomerDTO());

        if (!result.IsSuccess)
            return Problem(result, "Read error.");

        return Ok(result.Data);
    }

    [HttpPost("add")]
    [SwaggerOperation(OperationId = "AddCustomer")]
    public async Task<ActionResult<GetSingleCustomerDTO>> AddCustomer([FromBody] AddUpdateCustomerDTO dto)
    {
        var result = await customerService.CreateNewCustomerAsync(dto);

        if (!result.IsSuccess)
            return Problem(result, "Add error.");

        return CreatedAtAction(nameof(GetCustomer), new { id = result.Data!.IdCustomer }, result.Data.MapGetSingleCustomerDTO());
    }

    [HttpPut("update/{id}")]
    [SwaggerOperation(OperationId = "UpdateCustomer")]
    public async Task<ActionResult<GetSingleCustomerDTO>> UpdateCustomer(ulong id, [FromBody] AddUpdateCustomerDTO dto)
    {
        var result = await customerService.UpdateExistingCustomerAsync(dto, id);

        if (!result.IsSuccess)
            return Problem(result, "Update error.");

        return StatusCode((int)SuccessCode, result.Data!.MapGetSingleCustomerDTO());
    }

    [HttpGet("verifyPassword/{email}/{password}")]
    [SwaggerOperation(OperationId = "VerifyPasswordByEmail")]
    public async Task<ActionResult<GetSingleCustomerDTO>> VerifyPasswordByEmail(string email, string password)
    {
        var result = await customerService.VerifyPasswordByEmail(email, password);

        if (!result.IsSuccess)
            return Problem(result, "Read error.");
        ;

        return StatusCode((int)SuccessCode, result.Data!.MapGetSingleCustomerDTO());
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(OperationId = "DeleteCustomer")]
    public async Task<IActionResult> DeleteCustomer(ulong id)
    {
        var result = await customerService.DeleteAndSaveAsync(id);

        if (!result.IsSuccess)
            return Problem(result, "Delete error.");

        return NoContent();
    }
}