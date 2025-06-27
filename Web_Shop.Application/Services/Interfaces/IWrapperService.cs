using Web_Shop.Application.Common;

namespace Web_Shop.Application.Services.Interfaces;

public interface IWrapperService
{
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<Task<ServiceResponse<T>>> action);
    Task<ServiceResponse> ExecuteMethodAsync(Func<Task<ServiceResponse>> action);
}