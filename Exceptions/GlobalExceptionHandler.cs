using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException ex        => (StatusCodes.Status404NotFound, ex.Message),
            BadRequestException ex      => (StatusCodes.Status400BadRequest, ex.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Non autorisé"),
            _                           => (StatusCodes.Status500InternalServerError, "Erreur interne")
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            StatusCode = statusCode,
            Message    = message,
            Timestamp  = DateTime.UtcNow
        }, cancellationToken);

        return true;
    }
}