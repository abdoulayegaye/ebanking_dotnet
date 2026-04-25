using Ebank.API.Entities;

public interface IGenericService<TEntity, TDto, TCreateDto, TUpdateDto> where TEntity : BaseEntity
{
    Task<TDto> GetByIdAsync(long id);
    Task<List<TDto>> GetAllAsync();
    Task<TDto> CreateAsync(TCreateDto dto);
    Task<TDto> UpdateAsync(long id, TUpdateDto dto);
    Task DeleteAsync(long id);
}