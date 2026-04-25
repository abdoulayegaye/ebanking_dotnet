namespace Ebank.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Tags("Authentification")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger      = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [EndpointSummary("Inscription")]
    [EndpointDescription("Crée un nouveau compte utilisateur et retourne un token JWT.")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Register request : {Username}", request.Username);
        var response = await _authService.RegisterAsync(request);
        return Created(string.Empty, response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Connexion")]
    [EndpointDescription("Authentifie un utilisateur et retourne un token JWT valable 60 minutes.")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        _logger.LogInformation("Login request : {Username}", request.Username);
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [EndpointSummary("Profil connecté")]
    [EndpointDescription("Retourne les informations de l'utilisateur authentifié depuis le token JWT.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<object> Me()
    {
        var username = User.Identity?.Name;
        var roles    = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return Ok(new { Username = username, Roles = roles });
    }
}