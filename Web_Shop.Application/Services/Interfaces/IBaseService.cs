using Sieve.Models;
using Web_Shop.Application.Common;
using Web_Shop.Application.Helpers.PagedList;

namespace Web_Shop.Application.Services.Interfaces;

public interface IBaseService<T> where T : class
{
    IBaseService<T> WithTracking();
    IBaseService<T> WithoutTracking();
    Task<ServiceResponse<T>> GetByIdAsync(params object?[]? id);
    Task<ServiceResponse<IPagedList<TOut>>> SearchAsync<TOut>(SieveModel paginationParams, Func<T, TOut> formatterCallback);
    Task<ServiceResponse<T>> AddAsync(T entity);
    Task<ServiceResponse<T>> AddAndSaveAsync(T entity);
    Task<ServiceResponse<T>> UpdateAsync(T entity, params object?[]? id);
    Task<ServiceResponse<T>> UpdateAndSaveAsync(T entity, params object?[]? id);
    Task<ServiceResponse> DeleteAsync(params object?[]? id);
    Task<ServiceResponse<T>> DeleteAndSaveAsync(params object?[]? id);
    Task<ServiceResponse<int>> SaveChangesAsync(CancellationToken cancellationToken);
}