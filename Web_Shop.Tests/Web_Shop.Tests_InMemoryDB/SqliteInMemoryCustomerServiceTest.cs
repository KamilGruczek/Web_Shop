using Moq;
using Sieve.Models;
using Sieve.Services;
using Web_Shop.Application.CustomQueries;
using Web_Shop.Application.DTOs;
using Web_Shop.Application.Mappings;
using Web_Shop.Application.Mappings.PropertiesMappings;
using Web_Shop.Application.Services;
using Web_Shop.Application.Services.Interfaces;
using Web_Shop.Persistence.UOW;
using Web_Shop.Tests.Common.Sieve;

namespace Web_Shop.Tests_InMemoryDB;

public class SqliteInMemoryCustomerServiceTest : IDisposable
{
    private readonly SqliteDatabaseFixture _databaseFixture;
    private readonly SieveOptionsAccessor _optionsAccessor;

    private readonly SieveProcessor _processor;

    private readonly Mock<IWrapperService> _wrapperMock;

    public SqliteInMemoryCustomerServiceTest()
    {
        _databaseFixture = new SqliteDatabaseFixture();

        _wrapperMock = new Mock<IWrapperService>();

        _optionsAccessor = new SieveOptionsAccessor();

        _processor = new ApplicationSieveProcessor(_optionsAccessor,
            new SieveCustomSortMethods(),
            new SieveCustomFilterMethods());
    }

    public void Dispose()
    {
        _databaseFixture.Dispose();
    }

    [Fact]
    public async Task CustomerService_CreateNewCustomerAsync_ReturnsTrue()
    {
        {
            using var context = _databaseFixture.CreateContext();

            var unitOfWork = new UnitOfWork(context);

            var customerService = new CustomerService(_wrapperMock.Object, _processor, _optionsAccessor, unitOfWork);

            var addUpdateCustomerDTO = new AddUpdateCustomerDTO
            {
                Name = "TestName",
                Surname = "TestSurname",
                Password = "TestPassword",
                Email = "test@domain.com"
            };

            var verifyResult = await customerService.CreateNewCustomerAsync(addUpdateCustomerDTO);

            Assert.True(verifyResult.IsSuccess);
            Assert.NotNull(verifyResult.Data);
            Assert.Equal("test@domain.com", verifyResult.Data!.Email);
        }
    }

    [Fact]
    public async Task CustomerService_SearchAsync_ReturnsTrue()
    {
        using var context = _databaseFixture.CreateContext();

        var unitOfWork = new UnitOfWork(context);

        var customerService = new CustomerService(_wrapperMock.Object, _processor, _optionsAccessor, unitOfWork);

        var model = new SieveModel
        {
            Filters = "Name@=Mic"
        };

        var searchResult = await customerService.SearchAsync(model, resultEntity => resultEntity.MapGetSingleCustomerDTO());

        Assert.True(searchResult.IsSuccess);
        Assert.Equal(1, searchResult.Data!.TotalItemCount);
    }
}