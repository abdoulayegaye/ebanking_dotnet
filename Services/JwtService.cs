namespace Ebank.API.Services;
public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration config)
    {
        _config          = config;
        _secret          = config["Jwt:Secret"]!;
        _issuer          = config["Jwt:Issuer"]!;
        _audience        = config["Jwt:Audience"]!;
        _expirationMinutes = int.Parse(config["Jwt:ExpirationMinutes"]!);
    }

    // ← jwtService.generateToken()
    public JwtResponse GenerateToken(string username, IList<string> roles)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_expirationMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Sub, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            expires:            expires,
            signingCredentials: creds
        );

        return new JwtResponse(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    // ← jwtService.extractUsername()
    public string? ExtractUsername(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt     = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch { return null; }
    }
    
    // ← jwtService.isTokenValid()
    public bool IsTokenValid(string token, string username)
    {
        var extractedUsername = ExtractUsername(token);
        return extractedUsername == username && !IsTokenExpired(token);
    }

    // ← jwtService.isBearer()
    public bool IsBearer(HttpRequest request)
    {
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        return authHeader != null && authHeader.StartsWith("Bearer ");
    }

    // ← jwtService.getToken()
    public string? GetToken(HttpRequest request)
    {
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        return authHeader?.Substring(7);
    }

    private bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt     = handler.ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow;
    }
}