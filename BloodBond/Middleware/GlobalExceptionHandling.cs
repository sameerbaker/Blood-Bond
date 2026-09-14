using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BloodBond.Middleware
{
    
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandling> _logger;

        public GlobalExceptionHandling(RequestDelegate next, ILogger<GlobalExceptionHandling> logger)
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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Forbidden: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode status, string message)
        {
            if (context.Response.HasStarted) return;
            context.Response.Clear();
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            var error = new ErrorDetails
            {
                StatusCode = (int)status,
                Message = message
            };
            var json = JsonSerializer.Serialize(error);
            await context.Response.WriteAsync(json);
        }
    }
}
