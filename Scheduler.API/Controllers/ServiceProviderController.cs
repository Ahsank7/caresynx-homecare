using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.ServiceProvider;
using Scheduler.API.Services.ServiceProvider;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceProviderController : BaseController
    {
        Services.ServiceProvider.IServiceProvider _ServiceProvider;
        public ServiceProviderController(Services.ServiceProvider.IServiceProvider ServiceProvider, ILogger<ServiceProviderController> logger) : base(logger)
        {
            _ServiceProvider = ServiceProvider;
        }

        [HttpPost]
        [Route("SaveUpdateServiceProvider")]
        public async Task<IActionResult> SaveUpdateServiceProvider(SaveServiceProviderInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Service provider data is required");

            return await ExecuteAsync(
                () => _ServiceProvider.CreateUpdateServiceProviderAsync(model),
                "ServiceProvider created/Updated successfully!"
            );
        }

        [HttpGet]
        [Route("GetServiceProviderDetails")]
        public async Task<IActionResult> GetServiceProviderDetails(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid user ID is required");

            return await ExecuteAsync(
                () => _ServiceProvider.GetServiceProviderInfoAsync(UserId),
                "ServiceProvider details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("GetServiceProviderList")]
        public async Task<IActionResult> GetServiceProviderList(ServiceProviderSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search request data is required");

            return await ExecuteAsync(
                () => _ServiceProvider.GetServiceProvidersAsync(model),
                "Service provider list retrieved successfully!"
            );
        }

        [HttpDelete]
        [Route("DeleteServiceProvider")]
        public IActionResult DeleteServiceProvider(Guid ServiceProviderID)
        {
            if (ServiceProviderID == Guid.Empty)
                return ValidationError("Valid service provider ID is required");

            return Execute(() =>
            {
                var result = _ServiceProvider.DeleteServiceProvider(ServiceProviderID);
                return result;
            }, "ServiceProvider deleted successfully!");
        }

        [HttpPost]
        [Route("Available")]
        public async Task<IActionResult> GetAvailableServiceProvidersAsync(AvailableServiceProviderSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search request data is required");

            return await ExecuteAsync(
                () => _ServiceProvider.GetAvailableServiceProvidersAsync(model),
                "Available ServiceProviders returned successfully!"
            );
        }

        [HttpPost]
        [Route("WithAvailability")]
        public async Task<IActionResult> GetServiceProvidersWithAvailabilityAsync(ServiceProviderWithAvailabilityRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _ServiceProvider.GetServiceProvidersWithAvailabilityAsync(model),
                "Service Providers with availability returned successfully!"
            );
        }

        [HttpPost]
        [Route("UpsertContractInfo")]
        public async Task<IActionResult> UpsertContractInfo(UpsertContractInfoViewModel upsertCardInfoViewModel)
        {
            if (upsertCardInfoViewModel == null)
                return ValidationError("Contract data is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _ServiceProvider.UpsertContractInfo(upsertCardInfoViewModel);

                if (result == null)
                    throw new InvalidOperationException("Failed to add/update contract");

                return result;
            }, "User Contract added/updated successfully!");
        }

        [HttpGet]
        [Route("GetContractInfo")]
        public async Task<IActionResult> GetContractInfoAsync(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid user ID is required");

            return await ExecuteAsync(
                () => _ServiceProvider.GetContractInfoAsync(UserId),
                "Contract info retrieved successfully!"
            );
        }
    }
}
