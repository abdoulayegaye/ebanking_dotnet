using Ebank.API.Data;
using Ebank.API.Entities;
using Microsoft.EntityFrameworkCore;

public class OperationRepository : GenericRepository<Operation>, IOperationRepository
{
    public OperationRepository(AppDbContext context) : base(context) { }

    public async Task<List<Operation>> GetByTypeAsync(TypeOperation type) =>
        await _dbSet.Where(o => o.Type == type).ToListAsync();

    public async Task<Operation?> GetByNumeroAsync(string numero) =>
        await _dbSet.FirstOrDefaultAsync(o => o.Numero == numero);
}
