public record OperationDto(long Id, string Numero, TypeOperation Type, double Amount, long AccountId);
public record CreateOperationDto(string Numero, TypeOperation Type, double Amount, long AccountId);

