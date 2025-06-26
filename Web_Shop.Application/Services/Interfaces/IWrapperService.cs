using Microsoft.EntityFrameworkCore;
using Web_Shop.Application.Common;

namespace Web_Shop.Application.Services.Interfaces;

public interface IWrapperService
{
    Task<ServiceResponse> ExecuteMethodAsync(Func<DbContext, Task<ServiceResponse>> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, Task<T>> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, T> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, Task<ServiceResponse<T>>> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<DbContext, ServiceResponse<T>> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<Task<T>> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<T> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<Task<ServiceResponse<T>>> action);
    Task<ServiceResponse> ExecuteMethodAsync(Func<Task<ServiceResponse>> action);
    Task<ServiceResponse> ExecuteMethodAsync(Func<ServiceResponse> action);
    Task<ServiceResponse<T>> ExecuteMethodAsync<T>(Func<ServiceResponse<T>> action);
}