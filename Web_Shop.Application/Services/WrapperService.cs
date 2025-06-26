using System.Text;
using Microsoft.EntityFrameworkCore;
using Web_Shop.Application.Common;
using Web_Shop.Application.Extensions;
using Web_Shop.Application.Services.Interfaces;
using WWSI_Shop.Persistence.MySQL.Context;

namespace Web_Shop.Application.Services;

public class WrapperService(ILogService logService, WwsishopContext dbContext) : IWrapperService
{
    private const string INTERNAL_SERVER_ERROR = "Internal Server Error";

    private string GetStackTrace(Exception exception, StringBuilder? sb = null)
    {
        if (sb == null)
            sb = new StringBuilder();

        sb.AppendLine(exception.Message);
        sb.AppendLine("Stack Trace:");
        sb.AppendLine(exception.StackTrace);

        if (exception.InnerException != null)
        {
            sb.AppendLine("Inner Exception:");
            GetStackTrace(exception.InnerException, sb);
        }

        return sb.ToString();
    }

    #region ExecuteMethod

    #region WithDbContext

    public async Task<ServiceResponse> ExecuteMethodAsync(Func<DbContext, Task<ServiceResponse>> action)
    {
        try
        {
            var actionResult = await action(dbContext);

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, Task<T>> action)
    {
        try
        {
            var actionResult = await action(dbContext);

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, T> action)
    {
        try
        {
            var actionResult = action(dbContext);

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, Task<ServiceResponse<T>>> action)
    {
        try
        {
            var actionResult = await action(dbContext);

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, ServiceResponse<T>> action)
    {
        try
        {
            var actionResult = action(dbContext);

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    #endregion

    #region WithoutDbContext

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<Task<T>> action)
    {
        try
        {
            var actionResult = await action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<T> action)
    {
        try
        {
            var actionResult = action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<Task<ServiceResponse<T>>> action)
    {
        try
        {
            var actionResult = await action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse> ExecuteMethodAsync(Func<Task<ServiceResponse>> action)
    {
        try
        {
            var actionResult = await action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse> ExecuteMethodAsync(Func<ServiceResponse> action)
    {
        try
        {
            var actionResult = action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse(false, INTERNAL_SERVER_ERROR);
        }
    }

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<ServiceResponse<T>> action)
    {
        try
        {
            var actionResult = action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex));

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }
    }

    #endregion

    #endregion

    #region ReturnResult

    private async Task<ServiceResponse<T>> ReturnResultAsync<T>(ServiceResponse<T>? serviceResponse)
    {
        if (serviceResponse == null)
        {
            await logService.AddErrorLogAsync("Service response is null", INTERNAL_SERVER_ERROR);

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }

        if (serviceResponse.Message.IsNotNull() &&
            serviceResponse.LogMessage)
        {
            if (serviceResponse.IsSuccess)
                await logService.AddInformationLogAsync(serviceResponse.Message);
            else
                await logService.AddErrorLogAsync(serviceResponse.Message);
        }

        if (serviceResponse.IsSuccess)
        {
            if (serviceResponse.Message.IsNotNull())
                return new ServiceResponse<T>(true, serviceResponse.Message);

            return new ServiceResponse<T>(serviceResponse.Data!);
        }

        return new ServiceResponse<T>(false, serviceResponse.Message ?? string.Empty);
    }

    private async Task<ServiceResponse<T>> ReturnResultAsync<T>(T? actionResult)
    {
        if (actionResult == null)
        {
            await logService.AddErrorLogAsync("Action Result is null", INTERNAL_SERVER_ERROR);

            return new ServiceResponse<T>(false, INTERNAL_SERVER_ERROR);
        }

        return new ServiceResponse<T>(actionResult);
    }

    private async Task<ServiceResponse> ReturnResultAsync(ServiceResponse? serviceResponse)
    {
        if (serviceResponse == null)
        {
            await logService.AddErrorLogAsync("Service response is null", INTERNAL_SERVER_ERROR);

            return new ServiceResponse(false, INTERNAL_SERVER_ERROR);
        }

        if (serviceResponse.Message.IsNotNull() &&
            serviceResponse.LogMessage)
        {
            if (serviceResponse.IsSuccess)
                await logService.AddInformationLogAsync(serviceResponse.Message);
            else
                await logService.AddErrorLogAsync(serviceResponse.Message);
        }

        return serviceResponse;
    }

    #endregion
}