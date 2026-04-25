using Ebank.API.Entities;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByNumeroAsync(string numero);
    Task<List<Operation>> GetOperationsAsync(long accountId);
}
