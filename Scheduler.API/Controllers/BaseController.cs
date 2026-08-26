using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> operation, string successMessage = null)
        {
            try
            {
                var result = await operation();
                
                if (result == null)
                {
                    _logger.LogWarning("Operation returned null result");
                    return NotFound(Response<T>.NotFound("Resource not found"));
                }

                return Ok(Response<T>.Success(result, successMessage ?? "Operation completed successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided: {Message}", ex.Message);
                return BadRequest(Response<T>.BadRequest(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(Response<T>.BadRequest(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access: {Message}", ex.Message);
                return Unauthorized(Response<T>.Unauthorized(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during operation execution: {Message}", ex.Message);
                return StatusCode(500, Response<T>.InternalServerError(ex, "An internal server error occurred"));
            }
        }


        protected IActionResult Execute<T>(Func<T> operation, string successMessage = null)
        {
            try
            {
                var result = operation();
                
                if (result == null)
                {
                    _logger.LogWarning("Operation returned null result");
                    return NotFound(Response<T>.NotFound("Resource not found"));
                }

                return Ok(Response<T>.Success(result, successMessage ?? "Operation completed successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided: {Message}", ex.Message);
                return BadRequest(Response<T>.BadRequest(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(Response<T>.BadRequest(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access: {Message}", ex.Message);
                return Unauthorized(Response<T>.Unauthorized(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during operation execution: {Message}", ex.Message);
                return StatusCode(500, Response<T>.InternalServerError(ex, "An internal server error occurred"));
            }
        }

        protected IActionResult ValidationError(string message, List<string> errors = null)
        {
            _logger.LogWarning("Validation error: {Message}", message);
            return BadRequest(Response<object>.BadRequest(message, errors));
        }

        protected IActionResult NotFoundError(string message = "Resource not found")
        {
            _logger.LogWarning("Resource not found: {Message}", message);
            return NotFound(Response<object>.NotFound(message));
        }
    }
} 