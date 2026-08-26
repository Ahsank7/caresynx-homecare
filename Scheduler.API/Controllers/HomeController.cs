using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Scheduler.API.Common;
using Scheduler.API.Services.Email;
using Scheduler.API.Models.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IEmailService emailService, IConfiguration configuration, ILogger<HomeController> logger)
        {
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("Get String")]
        [Authorize]
        public string getString()
        {
            return "heello";
        }

        [HttpPost("Contact")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitContactRequest([FromBody] ContactRequestViewModel model)
        {
            if (model == null)
                return BadRequest(Response<object>.BadRequest("Contact request data is required"));

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(Response<object>.BadRequest("Name is required"));

            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(Response<object>.BadRequest("Email is required"));

            try
            {
                var adminEmail = _configuration["Email:AdminEmail"] ?? "admin@caresynx.com";
                
                var emailBody = $@"
                    <h2>New Contact Request</h2>
                    <p><strong>Name:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Phone:</strong> {model.Phone ?? "Not provided"}</p>
                    <p><strong>Message:</strong></p>
                    <p>{model.Message}</p>
                ";

                var emailMessage = new EmailMessage
                {
                    To = adminEmail,
                    Subject = $"New Contact Request from {model.Name}",
                    Body = emailBody,
                    IsHtml = true
                };

                var emailSent = await _emailService.SendEmailAsync(emailMessage);

                if (emailSent)
                {
                    _logger.LogInformation($"Contact request submitted successfully from {model.Email}");
                    return Ok(Response<object>.Success(null, "Thank you for contacting us! We'll get back to you soon."));
                }
                else
                {
                    _logger.LogWarning($"Failed to send contact request email from {model.Email}");
                    return StatusCode(500, Response<object>.InternalServerError(null, "Failed to send contact request. Please try again later."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing contact request from {model.Email}");
                return StatusCode(500, Response<object>.InternalServerError(ex, "An error occurred while processing your request"));
            }
        }

        [HttpPost("RequestDemo")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestDemo([FromBody] DemoRequestViewModel model)
        {
            if (model == null)
                return BadRequest(Response<object>.BadRequest("Demo request data is required"));

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(Response<object>.BadRequest("Name is required"));

            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(Response<object>.BadRequest("Email is required"));

            if (string.IsNullOrWhiteSpace(model.Company))
                return BadRequest(Response<object>.BadRequest("Company name is required"));

            try
            {
                var adminEmail = _configuration["Email:AdminEmail"] ?? "admin@caresynx.com";
                
                var emailBody = $@"
                    <h2>New Demo Request</h2>
                    <p><strong>Name:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Phone:</strong> {model.Phone ?? "Not provided"}</p>
                    <p><strong>Company:</strong> {model.Company}</p>
                    <p><strong>Preferred Date/Time:</strong> {model.PreferredDate ?? "Not specified"}</p>
                    <p><strong>Additional Notes:</strong></p>
                    <p>{model.Message ?? "None"}</p>
                ";

                var emailMessage = new EmailMessage
                {
                    To = adminEmail,
                    Subject = $"New Demo Request from {model.Company} - {model.Name}",
                    Body = emailBody,
                    IsHtml = true
                };

                var emailSent = await _emailService.SendEmailAsync(emailMessage);

                if (emailSent)
                {
                    _logger.LogInformation($"Demo request submitted successfully from {model.Email}");
                    return Ok(Response<object>.Success(null, "Demo request submitted! We'll contact you to schedule a demo."));
                }
                else
                {
                    _logger.LogWarning($"Failed to send demo request email from {model.Email}");
                    return StatusCode(500, Response<object>.InternalServerError(null, "Failed to submit demo request. Please try again later."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing demo request from {model.Email}");
                return StatusCode(500, Response<object>.InternalServerError(ex, "An error occurred while processing your request"));
            }
        }
    }

    public class ContactRequestViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DemoRequestViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Company { get; set; } = string.Empty;
        public string? PreferredDate { get; set; }
        public string? Message { get; set; }
    }
}
