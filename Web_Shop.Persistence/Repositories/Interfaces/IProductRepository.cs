using WWSI_Shop.Persistence.MySQL.Model;

namespace Web_Shop.Persistence.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<bool> ProductNameExistsAsync(string name);
    Task<bool> IsProductNameEditAllowedAsync(string name, ulong id);
}