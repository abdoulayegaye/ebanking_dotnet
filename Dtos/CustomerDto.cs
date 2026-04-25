public record CustomerDto(long Id, string Name, string Email, bool State);
public record CreateCustomerDto(string Name, string Email);
public record UpdateCustomerDto(string Name, string Email, bool State);