using Ebank.API.Data;
using Ebank.API.Entities;
using Microsoft.EntityFrameworkCore;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string name) =>
        await _dbSet.FirstOrDefaultAsync(r => r.Name == name);

    public async Task<bool> ExistsByNameAsync(string name) =>
        await _dbSet.AnyAsync(r => r.Name == name);
}