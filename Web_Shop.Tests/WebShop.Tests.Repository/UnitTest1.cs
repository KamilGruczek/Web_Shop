using Moq;
using Web_Shop.Persistence.Repositories.Interfaces;
using WWSI_Shop.Persistence.MySQL.Model;

namespace WebShop.Tests.Repository
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetCustomers_ShouldReturnLists()
        {
            // Arrange
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            mockCustomerRepository.Setup(repo => repo.GetCustomers())
                .Returns(Task.FromResult(new List<Customer>
                {
            new Customer { IdCustomer = 1 },
            new Customer { IdCustomer = 2 }
                }));

            // Act
            var customers = await mockCustomerRepository.Object.GetCustomers();

            // Assert
            Assert.NotNull(customers);
            Assert.Equal(2, customers.Count);
        }
    }
}