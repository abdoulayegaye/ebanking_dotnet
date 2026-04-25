using Ebank.API.Entities;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
}