namespace Ebank.API.Dtos;
public record JwtResponse(
    string Token,
    DateTime ExpiresAt
);