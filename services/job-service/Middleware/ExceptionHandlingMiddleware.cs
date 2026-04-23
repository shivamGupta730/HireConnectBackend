using System.Net;

namespace HireConnect.JobService.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = exception switch
        {
            ArgumentException => new
            {
                success = false,
                message = "Invalid input provided",
                error = exception.Message
            },
            UnauthorizedAccessException => new
            {
                success = false,
                message = "Unauthorized access",
                error = exception.Message
            },
            InvalidOperationException => new
            {
                success = false,
                message = "Invalid operation",
                error = exception.Message
            },
            _ => new
            {
                success = false,
                message = "An internal server error occurred",
                error = "Please try again later"
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
