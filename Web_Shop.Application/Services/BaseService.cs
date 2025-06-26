using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Web_Shop.Application.Common;
using Web_Shop.Application.Extensions;
using Web_Shop.Application.Helpers.PagedList;
using Web_Shop.Application.Services.Interfaces;
using Web_Shop.Persistence.UOW.Interfaces;

namespace Web_Shop.Application.Services;

public class BaseService<T>(IWrapperService wrapperService, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions, IUnitOfWork unitOfWork) : IBaseService<T> where T : class
{
    private bool _tracking = true;

    public IBaseService<T> WithTracking()
    {
        _tracking = true;
        return this;
    }

    public IBaseService<T> WithoutTracking()
    {
        _tracking = false;
        return this;
    }

    public async Task<ServiceResponse<T>> GetByIdAsync(params object?[]? id)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var result = _tracking
                ? await unitOfWork.Repository<T>().GetByIdAsync(id)
                : await unitOfWork.Repository<T>().WithoutTracking().GetByIdAsync(id);

            if (result == null) return new ServiceResponse<T>(false, "Object not found in database.");

            return new ServiceResponse<T>(result);
        });
    }

    public async Task<ServiceResponse<IPagedList<TOut>>> SearchAsync<TOut>(SieveModel paginationParams,
        Func<T, TOut> formatterCallback)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var query = unitOfWork.Repository<T>().Entities.AsNoTracking();

            var result = await query.ToPagedListAsync(sieveProcessor,
                sieveOptions,
                paginationParams,
                formatterCallback);

            return new ServiceResponse<IPagedList<TOut>>(result);
        });
    }

    public async Task<ServiceResponse<T>> AddAsync(T entity)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var result = await unitOfWork.Repository<T>().AddAsync(entity);

            return new ServiceResponse<T>(result);
        });
    }

    public async Task<ServiceResponse<T>> AddAndSaveAsync(T entity)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var result = await unitOfWork.Repository<T>().AddAsync(entity);
            await SaveChangesAsync(CancellationToken.None);

            return new ServiceResponse<T>(result);
        });
    }

    public async Task<ServiceResponse<T>> UpdateAndSaveAsync(T entity, params object?[]? id)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            if (id == null) return new ServiceResponse<T>(false, "Object ID cannot be null or empty.");

            if (!await unitOfWork.Repository<T>().Exists(id))
                return new ServiceResponse<T>(false,
                    "Object " + typeof(T) + " with ID " + id[0] + " does not exists in database.");

            var result = await unitOfWork.Repository<T>().UpdateAsync(entity, id);
            await SaveChangesAsync(CancellationToken.None);

            return new ServiceResponse<T>(result);
        });
    }

    public async Task<ServiceResponse> DeleteAsync(params object?[]? id)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var entity = await unitOfWork.Repository<T>().GetByIdAsync(id);

            if (entity == null)
                return new ServiceResponse(false, "Object not found.");

            await unitOfWork.Repository<T>().DeleteAsync(entity);

            return new ServiceResponse(true, "Object deleted successfully.");
        });
    }

    public async Task<ServiceResponse<T>> DeleteAndSaveAsync(params object?[]? id)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            if (id == null) return new ServiceResponse<T>(false, "Object ID cannot be null or empty.");

            if (!await unitOfWork.Repository<T>().Exists(id))
                return new ServiceResponse<T>(false,
                    "Object " + typeof(T) + " with ID " + id[0] + " does not exists in database.");

            await DeleteAsync(id);
            await SaveChangesAsync(CancellationToken.None);

            return new ServiceResponse<T>(true, "Object deleted successfully.");
        });
    }

    public async Task<ServiceResponse<int>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var savedChangesCount = await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ServiceResponse<int>(savedChangesCount);
        });
    }

    public async Task<ServiceResponse<T>> UpdateAsync(T entity, params object?[]? id)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var updatedEntity = await unitOfWork.Repository<T>().UpdateAsync(entity, id);

            return new ServiceResponse<T>(updatedEntity);
        });
    }
}