using Web_Shop.Application.Common;
using Web_Shop.Application.DTOs;
using WWSI_Shop.Persistence.MySQL.Model;

namespace Web_Shop.Application.Services.Interfaces;

public interface ICustomerService : IBaseService<Customer>
{
    Task<ServiceResponse<Customer>> CreateNewCustomerAsync(AddUpdateCustomerDTO dto);
    Task<ServiceResponse<Customer>> UpdateExistingCustomerAsync(AddUpdateCustomerDTO dto, ulong id);

    Task<ServiceResponse<Customer>> VerifyPasswordByEmail(string email, string password);
    //Task<ServiceResponse<Customer>> SearchCustomersAsync(SieveModel paginationParams);
}