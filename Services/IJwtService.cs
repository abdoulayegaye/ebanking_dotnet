namespace Ebank.API.Services;
public interface IJwtService
{
    JwtResponse GenerateToken(string username, IList<string> roles);
    string? ExtractUsername(string token);
    bool IsTokenValid(string token, string username);
    bool IsBearer(HttpRequest request);
    string? GetToken(HttpRequest request);
}