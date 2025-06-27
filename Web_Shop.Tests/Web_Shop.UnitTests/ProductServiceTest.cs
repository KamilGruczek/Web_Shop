using Moq;
using Web_Shop.Application.Common;
using Web_Shop.Application.CustomQueries;
using Web_Shop.Application.DTOs;
using Web_Shop.Application.Mappings.PropertiesMappings;
using Web_Shop.Application.Services;
using Web_Shop.Application.Services.Interfaces;
using Web_Shop.Persistence.Repositories.Interfaces;
using Web_Shop.Persistence.UOW.Interfaces;
using Web_Shop.Tests.Common.Sieve;
using WWSI_Shop.Persistence.MySQL.Model;

namespace Web_Shop.UnitTests;

public class ProductServiceTest
{
    private readonly Mock<SieveOptionsAccessor> _optionsAccessorMock;
    private readonly Mock<ApplicationSieveProcessor> _processorMock;
    private readonly Mock<IWrapperService> _wrapperMock;

    public ProductServiceTest()
    {
        _wrapperMock = new Mock<IWrapperService>();
        _wrapperMock.Setup(m => m.ExecuteMethodAsync(It.IsAny<Func<Task<ServiceResponse<Product>>>>()))
            .Returns((Func<Task<ServiceResponse<Product>>> func) => func());
        _optionsAccessorMock = new Mock<SieveOptionsAccessor>();

        _processorMock = new Mock<ApplicationSieveProcessor>(_optionsAccessorMock.Object,
            new Mock<SieveCustomSortMethods>().Object,
            new Mock<SieveCustomFilterMethods>().Object);
    }

    [Theory]
    [InlineData(false)]
    public async Task ProductService_CreateNewProductAsync_ReturnsTrue(bool productNameExists)
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(m => m.ProductNameExistsAsync(It.IsAny<string>())).ReturnsAsync(productNameExists);
        productRepositoryMock.Setup(m => m.AddAsync(It.IsAny<Product>())).ReturnsAsync(new Product());

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.ProductRepository).Returns(productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.Repository<Product>()).Returns(() => productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var productService = new ProductService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var dto = new AddUpdateProductDTO { Name = "TestProduct", Description = "Test Description", Price = 100.0m, Sku = "TEST123" };

        var result = await productService.CreateNewProductAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Theory]
    [InlineData(true)]
    public async Task ProductService_CreateNewProductAsync_ReturnsFalse(bool productNameExists)
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(m => m.ProductNameExistsAsync(It.IsAny<string>())).ReturnsAsync(productNameExists);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.ProductRepository).Returns(productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.Repository<Product>()).Returns(() => productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var productService = new ProductService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var dto = new AddUpdateProductDTO { Name = "ExistingProduct", Description = "Test Description", Price = 100.0m, Sku = "TEST123" };

        var result = await productService.CreateNewProductAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product: ExistingProduct already exists.", result.Message);
    }

    [Fact]
    public async Task ProductService_UpdateExistingProductAsync_ReturnsFalse_WhenProductNotFound()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(m => m.IsProductNameEditAllowedAsync(It.IsAny<string>(), It.IsAny<ulong>())).ReturnsAsync(false);
        productRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<ulong>())).ReturnsAsync(new Product { IdProduct = 1, Name = "ExistingProduct" });
        productRepositoryMock.Setup(m => m.WithoutTracking()).Returns(productRepositoryMock.Object);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.ProductRepository).Returns(productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.Repository<Product>()).Returns(() => productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.ProductRepository.GetByIdAsync(It.IsAny<ulong>())).ReturnsAsync((Product?)null);

        var productService = new ProductService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var dto = new AddUpdateProductDTO { Name = "ExistingProduct", Description = "Test Description", Price = 100.0m, Sku = "TEST123" };


        var result = await productService.UpdateExistingProductAsync(dto, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Object not found in database.", result.Message);
    }

    [Fact]
    public async Task ProductService_UpdateExistingProductAsync_ReturnsFalse_WhenProductNameExists()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(m => m.IsProductNameEditAllowedAsync(It.IsAny<string>(), It.IsAny<ulong>())).ReturnsAsync(true);
        productRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<ulong>())).ReturnsAsync(new Product { IdProduct = 1, Name = "ExistingProduct" });
        productRepositoryMock.Setup(m => m.WithoutTracking()).Returns(productRepositoryMock.Object);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.ProductRepository).Returns(productRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.Repository<Product>()).Returns(() => productRepositoryMock.Object);

        var productService = new ProductService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var dto = new AddUpdateProductDTO { Name = "ExistingProduct", Description = "Test Description", Price = 100.0m, Sku = "TEST123" };

        var result = await productService.UpdateExistingProductAsync(dto, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product: ExistingProduct already exists.", result.Message);
    }
}