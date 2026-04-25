using Ebank.API.Entities;

public interface IAccountService
    : IGenericService<Account, AccountDto, CreateAccountDto, UpdateAccountDto>
{
    Task<AccountDto?> GetByNumeroAsync(string numero);
    Task<List<OperationDto>> GetOperationsAsync(long accountId);
}