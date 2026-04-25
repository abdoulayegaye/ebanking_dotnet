using Ebank.API.Entities;

public abstract class GenericService<TEntity, TDto, TCreateDto, TUpdateDto>
    : IGenericService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
{
    protected readonly IGenericRepository<TEntity> _repository;

    protected GenericService(IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async Task<TDto> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} {id} introuvable");
        return MapToDto(entity);
    }

    public virtual async Task<List<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto dto)
    {
        var entity = MapToEntity(dto);
        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public virtual async Task<TDto> UpdateAsync(long id, TUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"{typeof(TEntity).Name} {id} introuvable");
        ApplyUpdate(entity, dto);
        var updated = await _repository.UpdateAsync(entity);
        return MapToDto(updated);
    }

    public virtual async Task DeleteAsync(long id) =>
        await _repository.DeleteAsync(id);

    // Méthodes abstraites à implémenter dans chaque service
    protected abstract TDto MapToDto(TEntity entity);
    protected abstract TEntity MapToEntity(TCreateDto dto);
    protected abstract void ApplyUpdate(TEntity entity, TUpdateDto dto);
}