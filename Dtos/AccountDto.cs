public record AccountDto(long Id, string Numero, double Balance, string Currency, bool Active, long CustomerId);
public record CreateAccountDto(string Numero, double Balance, string Currency, long CustomerId);
public record UpdateAccountDto(double Balance, string Currency, bool Active);