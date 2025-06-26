using System.Net;
using Microsoft.AspNetCore.Mvc;
using Web_Shop.Application.Common;
using Web_Shop.Application.Extensions;

namespace Web_Shop.RestAPI.Controllers;

public class BaseController : ControllerBase
{
    private const string INTERNAL_SERVER_ERROR = "Internal Server Error";
    protected HttpStatusCode SuccessCode => HttpStatusCode.OK;

    protected ObjectResult Problem<T>(ServiceResponse<T> response, string title)
    {
        if (response.IsSuccess)
            return StatusCode((int)SuccessCode, response);

        var errorCode = response.Message.IsNotNull() && response.Message == INTERNAL_SERVER_ERROR
            ? (int)HttpStatusCode.InternalServerError
            : (int)HttpStatusCode.BadRequest;
        return Problem(statusCode: errorCode, title: title, detail: response.Message);
    }
}