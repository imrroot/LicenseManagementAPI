using System.Net;
using System.Text.Json;
using LicenseManagementAPI.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementAPI.Presentation.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Proceed to the next middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Handle any exceptions
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new { Error = "An unexpected error occurred.", Details = "Something went wrong." };
        var statusCode = StatusCodes.Status500InternalServerError;

     
        if (exception is DbUpdateException dbUpdateException)
        {
            response = new { Error = "Database Error", Details = dbUpdateException.Message };
            statusCode = StatusCodes.Status500InternalServerError;
        }
        else
        {
            response = new { Error = exception.Message, Details = exception.ToString() };
            statusCode = StatusCodes.Status500InternalServerError;
        }

        // Set the response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
