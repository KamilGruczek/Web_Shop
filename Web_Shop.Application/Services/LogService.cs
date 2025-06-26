using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Web_Shop.Application.Services.Interfaces;

namespace Web_Shop.Application.Services;

public class LogService(ILogger<LogService> logger) : ILogService
{
    public Task AddErrorLogAsync(string errorMessage, string? stackTrace = null)
    {
        var traceId = Activity.Current?.TraceId;
        var spanId = Activity.Current?.SpanId;

        logger.LogError("Exception: " + errorMessage + " TraceID: " + traceId + "-" + spanId);

        return Task.CompletedTask;
    }

    public Task AddInformationLogAsync(string message)
    {
        var traceId = Activity.Current?.TraceId;
        var spanId = Activity.Current?.SpanId;

        logger.LogInformation("Information: " + message + " TraceID: " + traceId + "-" + spanId);

        return Task.CompletedTask;
    }
}