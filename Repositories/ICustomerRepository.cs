using Ebank.API.Entities;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<List<Account>> GetAccountsAsync(long customerId);
}