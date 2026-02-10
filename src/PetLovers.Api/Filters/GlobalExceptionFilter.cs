using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PetLovers.Domain.Exceptions;

namespace PetLovers.Api.Filters;

/// <summary>
/// Global exception filter that handles domain exceptions and returns appropriate HTTP responses.
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            PetNotFoundException => (StatusCodes.Status404NotFound, "Pet Not Found"),
            InvalidPetStateException => (StatusCodes.Status400BadRequest, "Invalid Operation"),
            DomainException => (StatusCodes.Status400BadRequest, "Domain Error"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Argument"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = _environment.IsDevelopment() ? exception.Message : GetSafeErrorMessage(exception),
            Instance = context.HttpContext.Request.Path
        };

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }

    private static string GetSafeErrorMessage(Exception exception)
    {
        return exception switch
        {
            DomainException => exception.Message,
            ArgumentException => exception.Message,
            _ => "An unexpected error occurred. Please try again later."
        };
    }
}
