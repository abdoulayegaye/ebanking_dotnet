using Ebank.API.Data;
using Ebank.API.Entities;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(long id) =>
        await _dbSet.FindAsync(id);

    public async Task<List<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new NotFoundException($"Entité {typeof(T).Name} {id} introuvable");
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(long id) =>
        await _dbSet.AnyAsync(e => e.Id == id);
}
