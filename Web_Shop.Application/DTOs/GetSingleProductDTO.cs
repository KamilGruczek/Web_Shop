namespace Web_Shop.Application.DTOs;

public class GetSingleProductDTO
{
    public required ulong IdProduct { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required decimal Price { get; set; }

    public required string Sku { get; set; }
}