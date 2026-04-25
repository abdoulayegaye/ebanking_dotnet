using Ebank.API.Entities;

public class AccountService
    : GenericService<Account, AccountDto, CreateAccountDto, UpdateAccountDto>,
      IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository repository) : base(repository)
    {
        _accountRepository = repository;
    }

    public async Task<AccountDto?> GetByNumeroAsync(string numero)
    {
        var account = await _accountRepository.GetByNumeroAsync(numero);
        return account is null ? null : MapToDto(account);
    }

    public async Task<List<OperationDto>> GetOperationsAsync(long accountId)
    {
        var operations = await _accountRepository.GetOperationsAsync(accountId);
        return operations.Select(o => new OperationDto(
            o.Id, o.Numero, o.Type, o.Amount, o.AccountId)).ToList();
    }

    protected override AccountDto MapToDto(Account e) =>
        new(e.Id, e.Numero, e.Balance, e.Currency, e.Active, e.CustomerId);

    protected override Account MapToEntity(CreateAccountDto dto) =>
        new() { Numero = dto.Numero, Balance = dto.Balance, Currency = dto.Currency, CustomerId = dto.CustomerId };

    protected override void ApplyUpdate(Account entity, UpdateAccountDto dto)
    {
        entity.Balance = dto.Balance;
        entity.Currency = dto.Currency;
        entity.Active = dto.Active;
    }
}