using BookingManagement.Application.Common.Exceptions;
using BookingManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            BookingOverlapException => (StatusCodes.Status409Conflict, "Booking conflict"),

            BookingAlreadyCancelledException => (StatusCodes.Status409Conflict, "Booking already cancelled"),

            EntityNotFoundException => (StatusCodes.Status404NotFound, "Not found"),

            InvalidBookingTimeRangeException or ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),

            ResourceLockUnavailableException => (StatusCodes.Status503ServiceUnavailable, "Booking service temporarily busy"),

            _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
        };

        if (statusCode >= 500)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == 500
                ? "An unexpected error occurred."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}