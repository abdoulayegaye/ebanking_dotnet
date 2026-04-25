using Ebank.API.Data;
using Ebank.API.Entities;
using Microsoft.EntityFrameworkCore;
namespace Ebank.API.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context) { }

    public async Task<Account?> GetByNumeroAsync(string numero) =>
        await _dbSet.FirstOrDefaultAsync(a => a.Numero == numero);

    public async Task<List<Operation>> GetOperationsAsync(long accountId) =>
        await _context.Operations
            .Where(o => o.AccountId == accountId)
            .ToListAsync();
}