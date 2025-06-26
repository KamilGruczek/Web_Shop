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
using BC = BCrypt.Net.BCrypt;

namespace Web_Shop.UnitTests;

public class CustomerServiceTest
{
    private readonly Mock<SieveOptionsAccessor> _optionsAccessorMock;
    private readonly Mock<ApplicationSieveProcessor> _processorMock;
    private readonly Mock<IWrapperService> _wrapperMock;

    public CustomerServiceTest()
    {
        _wrapperMock = new Mock<IWrapperService>();
        _wrapperMock.Setup(m => m.ExecuteMethodAsync(It.IsAny<Func<Task<ServiceResponse<Customer>>>>()))
            .Returns((Func<Task<ServiceResponse<Customer>>> func) => func());
        _optionsAccessorMock = new Mock<SieveOptionsAccessor>();

        _processorMock = new Mock<ApplicationSieveProcessor>(_optionsAccessorMock.Object,
            new Mock<SieveCustomSortMethods>().Object,
            new Mock<SieveCustomFilterMethods>().Object);
    }

    [Theory]
    [InlineData(false)]
    public async Task CustomerService_CreateNewCustomerAsync_ReturnsTrue(bool emailExists)
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock.Setup(m => m.EmailExistsAsync(It.IsAny<string>())).Returns((string email) => Task.FromResult(emailExists));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.CustomerRepository).Returns(() => customerRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.Repository<Customer>()).Returns(() => customerRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(0));

        var customerService = new CustomerService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var addUpdateCustomerDTO = new AddUpdateCustomerDTO
        {
            Name = "TestName",
            Surname = "TestSurname",
            Password = "TestPassword",
            Email = "test@domain.com"
        };

        var verifyResult = await customerService.CreateNewCustomerAsync(addUpdateCustomerDTO);

        Assert.True(verifyResult.IsSuccess);
    }

    [Theory]
    [InlineData(true)]
    public async Task CustomerService_CreateNewCustomerAsync_ReturnsFalse(bool emailExists)
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock.Setup(m => m.EmailExistsAsync(It.IsAny<string>())).Returns((string email) => Task.FromResult(emailExists));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.CustomerRepository).Returns(() => customerRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.Repository<Customer>()).Returns(() => customerRepositoryMock.Object);
        unitOfWorkMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(0));

        var customerService = new CustomerService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var addUpdateCustomerDTO = new AddUpdateCustomerDTO
        {
            Name = "Test",
            Surname = "Test",
            Password = "Test",
            Email = "test@domain.com"
        };

        var verifyResult = await customerService.CreateNewCustomerAsync(addUpdateCustomerDTO);

        Assert.False(verifyResult.IsSuccess);
    }

    [Fact]
    public async Task CustomerService_VerifyPasswordByEmailTest_ReturnsTrue()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).Returns((string email) => Task.FromResult(new Customer { IdCustomer = 2, Name = "Jan", Surname = "Kowalski", Email = "jan.kowalski@o2.pl", PasswordHash = BC.HashPassword("Test222") }));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.CustomerRepository).Returns(() => customerRepositoryMock.Object);

        var customerService = new CustomerService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var verifyResult = await customerService.VerifyPasswordByEmail("jan.kowalski@o2.pl", "Test222");

        Assert.True(verifyResult.IsSuccess);
        Assert.NotNull(verifyResult.Data);
        Assert.Equal("jan.kowalski@o2.pl", verifyResult.Data!.Email);
    }

    [Fact]
    public async Task CustomerService_VerifyPasswordByEmailTest_ReturnsFalse()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).Returns((string email) => Task.FromResult(new Customer { IdCustomer = 2, Name = "Jan", Surname = "Kowalski", Email = "jan.kowalski@o2.pl", PasswordHash = BC.HashPassword("Test222") }));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(m => m.CustomerRepository).Returns(() => customerRepositoryMock.Object);

        var customerService = new CustomerService(_wrapperMock.Object, _processorMock.Object, _optionsAccessorMock.Object, unitOfWorkMock.Object);

        var verifyResult = await customerService.VerifyPasswordByEmail("jan.kowalski@o2.pl", "Test211");

        Assert.False(verifyResult.IsSuccess);
        Assert.Null(verifyResult.Data);
    }
}