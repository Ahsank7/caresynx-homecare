using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Scheduler.API.Common;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scheduler.API.Application.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                
                // Only handle the exception if the response hasn't started
                if (!context.Response.HasStarted)
                {
                    await HandleExceptionAsync(context, ex);
                }
                else
                {
                    // If response has already started, we can't modify it
                    // Just log the exception and let it bubble up
                    _logger.LogWarning("Exception occurred but response has already started, cannot modify response: {Message}", ex.Message);
                }
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set content type and status code
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ArgumentException => Response.BadRequest(exception.Message),
                //ArgumentNullException => Response.BadRequest(exception.Message),
                InvalidOperationException => Response.BadRequest(exception.Message),
                UnauthorizedAccessException => Response.Error("Unauthorized access", StatusCodes.Status401Unauthorized),
                _ => Response.InternalServerError("An unexpected error occurred. Please try again later.")
            };

            // Add trace ID for debugging
            response.TraceId = context.TraceIdentifier;

            // Set status code before writing response
            context.Response.StatusCode = response.Status;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
} 