using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Organization;
using Scheduler.API.Services.Organization;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationBillingSettingsController : BaseController
    {
        private readonly IOrganizationBillingSettingsService _billingSettingsService;

        public OrganizationBillingSettingsController(
            IOrganizationBillingSettingsService billingSettingsService, 
            ILogger<OrganizationBillingSettingsController> logger) 
            : base(logger)
        {
            _billingSettingsService = billingSettingsService;
        }

        [HttpGet]
        [Route("{organizationId}")]
        public async Task<IActionResult> GetBillingSettings(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _billingSettingsService.GetBillingSettingsAsync(organizationId),
                "Billing settings retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> SaveBillingSettings(OrganizationBillingSettingsRequest request)
        {
            if (request == null)
                return ValidationError("Billing settings data is required");

            if (request.OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _billingSettingsService.SaveBillingSettingsAsync(request),
                "Billing settings saved successfully!"
            );
        }

        [HttpGet]
        [Route("timebasedrates/{organizationId}")]
        public async Task<IActionResult> GetTimeBasedRates(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _billingSettingsService.GetTimeBasedRatesAsync(organizationId),
                "Time-based rates retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("timebasedrates")]
        public async Task<IActionResult> SaveTimeBasedRate(OrganizationTimeBasedRateRequest request)
        {
            if (request == null)
                return ValidationError("Time-based rate data is required");

            if (request.OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _billingSettingsService.SaveTimeBasedRateAsync(request),
                "Time-based rate saved successfully!"
            );
        }

        [HttpDelete]
        [Route("timebasedrates/{id}")]
        public async Task<IActionResult> DeleteTimeBasedRate(int id, [FromQuery] Guid organizationId)
        {
            if (id <= 0)
                return ValidationError("Valid rate ID is required");

            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _billingSettingsService.DeleteTimeBasedRateAsync(id, organizationId),
                "Time-based rate deleted successfully!"
            );
        }
    }
}
