using Ebank.API.Entities;

public interface ICustomerService
    : IGenericService<Customer, CustomerDto, CreateCustomerDto, UpdateCustomerDto>
{
    Task<CustomerDto?> GetByEmailAsync(string email);
    Task<List<AccountDto>> GetAccountsAsync(long customerId);
}