using Ebank.API.Data;
using Ebank.API.Entities;
using Microsoft.EntityFrameworkCore;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _dbSet.Include(u => u.Role)             // ← FetchType.EAGER
                    .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);

    public async Task<bool> ExistsByUsernameAsync(string username) =>
        await _dbSet.AnyAsync(u => u.Username == username);
}