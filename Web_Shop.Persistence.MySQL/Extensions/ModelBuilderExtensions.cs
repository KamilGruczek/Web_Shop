using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

namespace Web_Shop.Persistence.MySQL.Extensions
{
    internal static class ModelBuilderExtensions
    {
        // Fix issue with extension conflict Microsoft.EntityFrameworkCore and MySql.EntityFrameworkCore
        internal static ModelBuilder HasCharSets(this ModelBuilder modelBuilder, string charSet)
        {
            return modelBuilder.HasCharSet(charSet);
        }
    }
}
