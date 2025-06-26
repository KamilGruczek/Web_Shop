namespace Web_Shop.Application.Services.Interfaces;

public interface ILogService
{
    Task AddErrorLogAsync(string errorMessage, string? stackTrace = null);
    Task AddInformationLogAsync(string message);
}