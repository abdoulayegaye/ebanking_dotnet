using Ebank.API.Data;
using Ebank.API.Entities;
using Microsoft.EntityFrameworkCore;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<Customer?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<List<Account>> GetAccountsAsync(long customerId) =>
        await _context.Accounts
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
}