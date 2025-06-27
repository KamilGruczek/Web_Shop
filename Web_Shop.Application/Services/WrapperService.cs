using System.Diagnostics;
using System.Text;
using Web_Shop.Application.Common;
using Web_Shop.Application.Extensions;
using Web_Shop.Application.Services.Interfaces;
using WWSI_Shop.Persistence.MySQL.Context;

namespace Web_Shop.Application.Services;

public class WrapperService(ILogService logService, WwsishopContext dbContext) : IWrapperService
{
    private const string INTERNAL_SERVER_ERROR = "Internal Server Error";

    public async Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<Task<ServiceResponse<T>>> action)
    {
        try
        {
            var actionResult = await action();

            return await ReturnResultAsync(actionResult);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId;
            var spanId = Activity.Current?.SpanId;

            var traceIdString = $"TraceID: {traceId}-{spanId}";
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex, traceIdString));

            return new ServiceResponse<T>(false, $"{traceIdString}{Environment.NewLine}Error: {INTERNAL_SERVER_ERROR}");
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
            var traceId = Activity.Current?.TraceId;
            var spanId = Activity.Current?.SpanId;

            var traceIdString = $"TraceID: {traceId}-{spanId}";
            await logService.AddErrorLogAsync(ex.Message, GetStackTrace(ex, traceIdString));

            return new ServiceResponse(false, $"{traceIdString}{Environment.NewLine}Error: {INTERNAL_SERVER_ERROR}");
        }
    }

    private string GetStackTrace(Exception exception, string? traceId, StringBuilder? sb = null)
    {
        if (sb == null)
            sb = new StringBuilder();

        if (traceId.IsNotNull())
            sb.AppendLine(traceId);

        sb.AppendLine(exception.Message);
        sb.AppendLine("Stack Trace:");
        sb.AppendLine(exception.StackTrace);

        if (exception.InnerException != null)
        {
            sb.AppendLine("Inner Exception:");
            GetStackTrace(exception.InnerException, null, sb);
        }

        return sb.ToString();
    }

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
}