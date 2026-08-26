using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scheduler.API.Common;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Scheduler.API.Application.Middlewares
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly ILogger<CustomJwtBearerEvents> _logger;

        public CustomJwtBearerEvents(ILogger<CustomJwtBearerEvents> logger)
        {
            _logger = logger;
        }

        private static bool IsAnonymousEndpoint(JwtBearerChallengeContext? context)
        {
            var endpoint = context?.HttpContext.GetEndpoint();
            if (endpoint == null)
                return false;

            // Check for AllowAnonymous attribute on the endpoint
            var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();
            if (allowAnonymous != null)
                return true;

            // Check for AllowAnonymous attribute on the controller action
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor != null)
            {
                // Check action for AllowAnonymous
                if (actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
                    return true;

                // Check controller for AllowAnonymous
                if (actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
                    return true;
            }

            return false;
        }

        private static bool IsAnonymousEndpoint(AuthenticationFailedContext? context)
        {
            var endpoint = context?.HttpContext.GetEndpoint();
            if (endpoint == null)
                return false;

            // Check for AllowAnonymous attribute on the endpoint
            var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();
            if (allowAnonymous != null)
                return true;

            // Check for AllowAnonymous attribute on the controller action
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor != null)
            {
                // Check action for AllowAnonymous
                if (actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
                    return true;

                // Check controller for AllowAnonymous
                if (actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
                    return true;
            }

            return false;
        }

        public override async Task Challenge(JwtBearerChallengeContext context)
        {
            _logger.LogInformation("JWT Challenge triggered for path: {Path}", context.Request.Path);
            
            // Skip challenge for endpoints with AllowAnonymous attribute
            if (IsAnonymousEndpoint(context))
            {
                _logger.LogInformation("Skipping JWT challenge for anonymous endpoint: {Path}", context.Request.Path);
                return;
            }
            
            // Check if response has already started to prevent conflicts
            if (context.Response.HasStarted)
            {
                context.HandleResponse(); // Stop further processing
                return;
            }
            
            // Handle authentication challenge (401 Unauthorized)
            var response = Response<object>.Unauthorized("Authentication required. Please provide a valid token.");
            
            // Clear any existing response content
            context.Response.Clear();
            
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(jsonResponse);
            context.HandleResponse(); // Stop further processing
        }

        public override async Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context?.HttpContext?.Request?.Path.Value?.Contains("UpdateLogoutTime", StringComparison.OrdinalIgnoreCase) == true)
            {
                return;
            }

            // Skip authentication failure for endpoints with AllowAnonymous attribute
            if (IsAnonymousEndpoint(context))
            {
                _logger.LogInformation("Skipping JWT authentication failure for anonymous endpoint: {Path}", context.Request.Path);
                return;
            }

            _logger.LogInformation("JWT Authentication Failed for path: {Path}, Exception: {Exception}", 
                context.Request.Path, context.Exception?.Message);

            // Check if response has already started to prevent conflicts
            if (context.Response.HasStarted)
            {
                return; // Cannot modify response that has already started
            }
            
            // Handle authentication failure (invalid token, expired, etc.)
            string message;
            
            if (context.Exception?.GetType().Name.Contains("SecurityTokenExpiredException") == true)
            {
                message = "Token has expired. Please login again.";
            }
            else if (context.Exception?.GetType().Name.Contains("SecurityTokenInvalidSignatureException") == true)
            {
                message = "Invalid token signature. Please login again.";
            }
            else if (context.Exception?.GetType().Name.Contains("SecurityTokenMalformedException") == true)
            {
                message = "Malformed token. Please provide a valid token.";
            }
            else
            {
                message = "Invalid or expired token. Please login again.";
            }

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
            // Note: AuthenticationFailedContext doesn't have HandleResponse()
            // The response will be sent and processing will continue
        }
    }
}
