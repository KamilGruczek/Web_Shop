using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Web_Shop.Application.Common;
using Web_Shop.Application.DTOs;
using Web_Shop.Application.Mappings;
using Web_Shop.Application.Services.Interfaces;
using Web_Shop.Persistence.UOW.Interfaces;
using WWSI_Shop.Persistence.MySQL.Model;
using BC = BCrypt.Net.BCrypt;

namespace Web_Shop.Application.Services;

public class CustomerService(IWrapperService wrapperService, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions, IUnitOfWork unitOfWork) : BaseService<Customer>(wrapperService, sieveProcessor, sieveOptions, unitOfWork), ICustomerService
{
    public async Task<ServiceResponse<Customer>> CreateNewCustomerAsync(AddUpdateCustomerDTO dto)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            if (await unitOfWork.CustomerRepository.EmailExistsAsync(dto.Email))
                return new ServiceResponse<Customer>(false, "Email: " + dto.Email + " already registered.");

            var newEntity = dto.MapCustomer();

            var result = await AddAndSaveAsync(newEntity);
            if (!result.IsSuccess)
                return new ServiceResponse<Customer>(false, result.Message ?? "Failed to create new customer.");

            return new ServiceResponse<Customer>(result.Data);
        });
    }

    public async Task<ServiceResponse<Customer>> UpdateExistingCustomerAsync(AddUpdateCustomerDTO dto, ulong id)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var existingEntityResult = await WithoutTracking().GetByIdAsync(id);

            if (!existingEntityResult.IsSuccess)
                return existingEntityResult;

            if (!await unitOfWork.CustomerRepository.IsEmailEditAllowedAsync(dto.Email, id))
                return new ServiceResponse<Customer>(false, "Email: " + dto.Email + " already registered.");

            var domainEntity = dto.MapCustomer();

            domainEntity.IdCustomer = id;
            if (!dto.IsPasswordUpdate)
                domainEntity.PasswordHash = existingEntityResult!.Data!.PasswordHash;

            return await UpdateAndSaveAsync(domainEntity, id);
        });
    }

    public async Task<ServiceResponse<Customer>> VerifyPasswordByEmail(string email, string password)
    {
        return await wrapperService.ExecuteMethodAsync(async () =>
        {
            var existingEntity = await unitOfWork.CustomerRepository.GetByEmailAsync(email);

            if (existingEntity == null ||
                !BC.Verify(password, existingEntity.PasswordHash))
                return new ServiceResponse<Customer>(false, "Invalid email or password.");

            return new ServiceResponse<Customer>(existingEntity);
        });
    }
}