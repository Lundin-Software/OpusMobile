using Opus.Mobile.API.Models.Exceptions;
using Opus.Mobile.API.Services.Logging;
using Opus.Mobile.Shared.API;
using System.Text.Json;

namespace Opus.Mobile.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (BadRequestException ex)
        {
            await HandleExceptionAsync(httpContext, StatusCodes.Status400BadRequest, ex, ExceptionType.BadRequest);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, StatusCodes.Status404NotFound, ex, ExceptionType.NotFound);
        }
        catch (UnauthorizedException ex)
        {
            await HandleExceptionAsync(httpContext, StatusCodes.Status401Unauthorized, ex, ExceptionType.Unauthorized);
        }
        catch (GeneralException ex)
        {
            await HandleExceptionAsync(httpContext, StatusCodes.Status400BadRequest, ex, ExceptionType.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, StatusCodes.Status500InternalServerError, ex, unknownException: true);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext httpContext,
        int statusCode,
        Exception exception,
        ExceptionType? exceptionType = null,
        bool unknownException = false)
    {
        logger.Error(exceptionType switch
        {
            ExceptionType.BadRequest => $"Business bad request: {exception}",
            ExceptionType.NotFound => $"Business not found: {exception}",
            ExceptionType.Unauthorized => $"Business unauthorized: {exception}",
            ExceptionType.Message => $"Business message: {exception}",
            _ => $"Something went wrong: {exception}"
        });

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new APIResponse
        {
            Success = false,
            Message = unknownException ? "Something went wrong" : exception.Message
        }));
    }

    private enum ExceptionType
    {
        BadRequest,
        NotFound,
        Unauthorized,
        Message
    }
}
