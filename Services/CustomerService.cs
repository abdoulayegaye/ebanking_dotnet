using Ebank.API.Entities;

public class CustomerService
    : GenericService<Customer, CustomerDto, CreateCustomerDto, UpdateCustomerDto>,
      ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository repository) : base(repository)
    {
        _customerRepository = repository;
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);
        return customer is null ? null : MapToDto(customer);
    }

    public async Task<List<AccountDto>> GetAccountsAsync(long customerId)
    {
        var accounts = await _customerRepository.GetAccountsAsync(customerId);
        return accounts.Select(a => new AccountDto(
            a.Id, a.Numero, a.Balance, a.Currency, a.Active, a.CustomerId)).ToList();
    }

    protected override CustomerDto MapToDto(Customer e) =>
        new(e.Id, e.Name, e.Email, e.State);

    protected override Customer MapToEntity(CreateCustomerDto dto) =>
        new() { Name = dto.Name, Email = dto.Email };

    protected override void ApplyUpdate(Customer entity, UpdateCustomerDto dto)
    {
        entity.Name = dto.Name;
        entity.Email = dto.Email;
        entity.State = dto.State;
    }
}