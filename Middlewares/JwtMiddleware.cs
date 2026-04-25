namespace Ebank.API.Middlewares;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService, IUserService userService)
    {
        var path = context.Request.Path;
        _logger.LogInformation("JWT Middleware → {Path}", path);

        if (!jwtService.IsBearer(context.Request))
        {
            await _next(context);
            return;
        }

        var token    = jwtService.GetToken(context.Request);
        var username = token != null ? jwtService.ExtractUsername(token) : null;

        if (username != null && context.User.Identity?.IsAuthenticated == false)
        {
            if (jwtService.IsTokenValid(token!, username))
            {
                // authentification déjà gérée par le middleware JWT Bearer
                // ce middleware sert uniquement au logging et à la validation custom
                _logger.LogInformation("Token valide pour {Username}", username);
            }
        }

        await _next(context);
    }
}