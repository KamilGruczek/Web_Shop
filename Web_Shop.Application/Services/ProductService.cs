using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Web_Shop.Application.Common;
using Web_Shop.Application.DTOs;
using Web_Shop.Application.Mappings;
using Web_Shop.Application.Services.Interfaces;
using Web_Shop.Persistence.UOW.Interfaces;
using WWSI_Shop.Persistence.MySQL.Model;

namespace Web_Shop.Application.Services;

public class ProductService(IWrapperService wrapperService, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions, IUnitOfWork unitOfWork) : BaseService<Product>(wrapperService, sieveProcessor, sieveOptions, unitOfWork), IProductService
{
    private readonly IOptions<SieveOptions> _sieveOptions = sieveOptions;
    private readonly ISieveProcessor _sieveProcessor = sieveProcessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWrapperService _wrapperService = wrapperService;

    public async Task<ServiceResponse<Product>> CreateNewProductAsync(AddUpdateProductDTO dto)
    {
        return await _wrapperService.ExecuteMethodAsync(async () =>
        {
            if (await _unitOfWork.ProductRepository.ProductNameExistsAsync(dto.Name))
                return new ServiceResponse<Product>(false, "Product: " + dto.Name + " already exists.");

            var newEntity = dto.MapProduct();

            var result = await AddAndSaveAsync(newEntity);
            if (!result.IsSuccess)
                return new ServiceResponse<Product>(false, result.Message ?? "Failed to create new Product.");

            return new ServiceResponse<Product>(result.Data);
        });
    }

    public async Task<ServiceResponse<Product>> UpdateExistingProductAsync(AddUpdateProductDTO dto, ulong productId)
    {
        return await _wrapperService.ExecuteMethodAsync(async () =>
        {
            var existingEntityResult = await WithoutTracking().GetByIdAsync(productId);

            if (!existingEntityResult.IsSuccess)
                return existingEntityResult;

            if (await _unitOfWork.ProductRepository.IsProductNameEditAllowedAsync(dto.Name, productId))
                return new ServiceResponse<Product>(false, "Product: " + dto.Name + " already exists.");

            var domainEntity = dto.MapProduct();

            domainEntity.IdProduct = productId;

            return await UpdateAndSaveAsync(domainEntity, productId);
        });
    }
}