using Ebank.API.Entities;

public interface IOperationRepository : IGenericRepository<Operation>
{
    Task<List<Operation>> GetByTypeAsync(TypeOperation type);
    Task<Operation?> GetByNumeroAsync(string numero);
}