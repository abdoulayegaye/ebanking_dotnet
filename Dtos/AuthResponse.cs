namespace Ebank.API.Dtos;
public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    string Username,
    string Firstname,
    string Lastname,
    string Role
);