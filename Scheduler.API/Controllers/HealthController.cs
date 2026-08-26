using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Scheduler.API.Common;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [ApiController]
    public class HealthController : BaseController
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration, ILogger<HealthController> logger) : base(logger)
        {
            _configuration = configuration;
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            return await ExecuteAsync(async () =>
            {
                var connectionString = _configuration.GetConnectionString("default");
                
                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Connection string is null or empty");

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                
                var command = new SqlCommand("SELECT 1 as TestValue", connection);
                var result = await command.ExecuteScalarAsync();
                
                return new { 
                    testValue = result,
                    connectionString = connectionString.Substring(0, 50) + "..."
                };
            }, "Database connection successful");
        }

        [HttpGet("config")]
        public IActionResult GetConfiguration()
        {
            return Execute(() => new
            {
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                connectionString = _configuration.GetConnectionString("default")?.Substring(0, 50) + "...",
                allConnectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren().Select(x => x.Key).ToList()
            }, "Configuration retrieved successfully");
        }

        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuthentication()
        {
            return Execute(() => new
            {
                message = "Authentication successful!",
                user = User.Identity?.Name,
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            }, "Authentication test successful");
        }

    }
}
