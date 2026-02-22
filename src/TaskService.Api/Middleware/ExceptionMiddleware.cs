using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using TaskService.API.Models;
using TaskService.Application.Exceptions;

namespace TaskService.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (UnauthorizedAccessException e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, e);
        }
        catch (AuthenticationException e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Forbidden, e);
        } 
        catch (InvalidOperationException e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Conflict, e);
        }
        catch (InvalidInputException e)
        {
            await HandleExceptionWithErrorContentAsync(context, HttpStatusCode.BadRequest, e);
        }
        catch (ValidationException e)
        {
            await HandleExceptionWithErrorContentAsync(context, HttpStatusCode.UnprocessableEntity, e);
        }
        catch (NotFoundException e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotFound, e);
        }
        catch (ArgumentNullException e)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, e);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex, messageOverride: "An unexpected error occurred.");
        }
    }

    private Task HandleExceptionWithErrorContentAsync(HttpContext context, HttpStatusCode statusCode, Exception exception)
    {
        var errors = TryGetErrors(exception);
        return HandleExceptionAsync(context, statusCode, exception, errors);
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        Exception exception,
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? messageOverride = null)
    {
        if ((int)statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("{ExceptionType}: {Message}", exception.GetType().Name, exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new ApiErrorResponse((int)statusCode, messageOverride ?? exception.Message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static IReadOnlyDictionary<string, string[]>? TryGetErrors(Exception exception)
    {
        if (exception is ValidationException ve)
            return ve.Errors;

        // Avoid compile-time dependency on an `Errors` member for other exception types.
        var prop = exception.GetType().GetProperty("Errors");
        if (prop is null)
            return null;

        if (!typeof(IReadOnlyDictionary<string, string[]>).IsAssignableFrom(prop.PropertyType))
            return null;

        return prop.GetValue(exception) as IReadOnlyDictionary<string, string[]>;
    }
}