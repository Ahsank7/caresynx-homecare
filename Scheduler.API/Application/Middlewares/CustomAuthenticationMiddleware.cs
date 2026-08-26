using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Scheduler.API.Common;
using System.Net;
using System.Text.Json;
using System.Linq;

namespace Scheduler.API.Application.Middlewares
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomAuthenticationMiddleware> _logger;

        public CustomAuthenticationMiddleware(RequestDelegate next, ILogger<CustomAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication check for login endpoint and other public endpoints
            if (IsPublicEndpoint(context))
            {
                await _next(context);
                return;
            }

            // Check if the endpoint requires authorization
            var endpoint = context.GetEndpoint();
            var requiresAuth = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;

            if (requiresAuth)
            {
                // Check if user is authenticated
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    // Check if response has already started to prevent conflicts
                    if (context.Response.HasStarted)
                    {
                        return; // Cannot modify response that has already started
                    }

                    // Check if there's an Authorization header to determine the error message
                    string message;
                    if (context.Request.Headers.ContainsKey("Authorization"))
                    {
                        message = "Invalid or expired token. Please login again.";
                    }
                    else
                    {
                        message = "Authentication required. Please provide a valid token.";
                    }

                    // Return our custom response format for unauthorized access
                    var response = Response<object>.Unauthorized(message);
                    
                    // Clear any existing response content
                    context.Response.Clear();
                    
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    
                    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    await context.Response.WriteAsync(jsonResponse);
                    return;
                }
            }

            await _next(context);
        }

        private static bool IsPublicEndpoint(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();
            
            // List of public endpoints that don't require authentication
            var publicEndpoints = new[]
            {
                "/api/authenticate/login",
                "/api/health/database",
                "/api/health/config",
                "/swagger",
                "/swagger/index.html",
                "/swagger/v1/swagger.json"
            };

            return publicEndpoints.Any(endpoint => path?.StartsWith(endpoint) == true);
        }
    }
}
