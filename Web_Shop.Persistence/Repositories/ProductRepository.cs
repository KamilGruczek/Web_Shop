using Microsoft.EntityFrameworkCore;
using Web_Shop.Persistence.Repositories.Interfaces;
using WWSI_Shop.Persistence.MySQL.Context;
using WWSI_Shop.Persistence.MySQL.Model;

namespace Web_Shop.Persistence.Repositories;

public class ProductRepository(WwsishopContext context) : GenericRepository<Product>(context), IProductRepository
{
    public async Task<bool> ProductNameExistsAsync(string name)
    {
        return await Entities.AnyAsync(e => e.Name == name);
    }

    public async Task<bool> IsProductNameEditAllowedAsync(string name, ulong id)
    {
        return !await Entities.AnyAsync(e => e.Name == name && e.IdProduct != id);
    }
}