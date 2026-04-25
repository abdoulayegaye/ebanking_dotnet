using Ebank.API.Entities;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}