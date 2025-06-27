using Web_Shop.Application.Common;
using Web_Shop.Application.DTOs;
using WWSI_Shop.Persistence.MySQL.Model;

namespace Web_Shop.Application.Services.Interfaces;

public interface IProductService : IBaseService<Product>
{
    Task<ServiceResponse<Product>> CreateNewProductAsync(AddUpdateProductDTO dto);
    Task<ServiceResponse<Product>> UpdateExistingProductAsync(AddUpdateProductDTO dto, ulong productId);
}