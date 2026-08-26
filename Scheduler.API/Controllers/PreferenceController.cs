using Scheduler.API.Common;
using Scheduler.API.Models.Preference;
using Scheduler.API.Services.Preference;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PreferenceController : BaseController
    {
        private readonly IPreference _preferenceService;

        public PreferenceController(IPreference preferenceService, ILogger<PreferenceController> logger)
            : base(logger)
        {
            _preferenceService = preferenceService;
        }

        #region Client Preferences

        /// <summary>
        /// Get all preferences for a client
        /// </summary>
        [HttpGet]
        [Route("Client/{clientId}")]
        public async Task<IActionResult> GetClientPreferences(Guid clientId)
        {
            if (clientId == Guid.Empty)
                return ValidationError("Valid Client ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var preferences = await _preferenceService.GetClientPreferencesAsync(clientId);
                    return new GetClientPreferencesResponse { Preferences = preferences };
                },
                "Client preferences retrieved successfully!"
            );
        }

        /// <summary>
        /// Create or update a client preference
        /// </summary>
        [HttpPost]
        [Route("Client/Upsert")]
        public async Task<IActionResult> UpsertClientPreference([FromBody] UpsertClientPreferenceRequest request)
        {
            if (request == null)
                return ValidationError("Preference data is required");

            if (request.ClientId == Guid.Empty)
                return ValidationError("Valid Client ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var userId = GetCurrentUserId();
                    var preferenceId = await _preferenceService.UpsertClientPreferenceAsync(request, userId);
                    return preferenceId;
                },
                "Client preference saved successfully!"
            );
        }

        /// <summary>
        /// Delete a client preference
        /// </summary>
        [HttpDelete]
        [Route("Client/{id}")]
        public async Task<IActionResult> DeleteClientPreference(Guid id)
        {
            if (id == Guid.Empty)
                return ValidationError("Valid Preference ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var userId = GetCurrentUserId();
                    var result = await _preferenceService.DeleteClientPreferenceAsync(id, userId);
                    return result;
                },
                "Client preference deleted successfully!"
            );
        }

        #endregion

        #region Service Provider Attributes

        /// <summary>
        /// Get all attributes for a service provider
        /// </summary>
        [HttpGet]
        [Route("ServiceProvider/{serviceProviderId}")]
        public async Task<IActionResult> GetServiceProviderAttributes(Guid serviceProviderId)
        {
            if (serviceProviderId == Guid.Empty)
                return ValidationError("Valid Service Provider ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var attributes = await _preferenceService.GetServiceProviderAttributesAsync(serviceProviderId);
                    return new GetServiceProviderAttributesResponse { Attributes = attributes };
                },
                "Service provider attributes retrieved successfully!"
            );
        }

        /// <summary>
        /// Create or update a service provider attribute
        /// </summary>
        [HttpPost]
        [Route("ServiceProvider/Upsert")]
        public async Task<IActionResult> UpsertServiceProviderAttribute([FromBody] UpsertServiceProviderAttributeRequest request)
        {
            if (request == null)
                return ValidationError("Attribute data is required");

            if (request.ServiceProviderId == Guid.Empty)
                return ValidationError("Valid Service Provider ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var userId = GetCurrentUserId();
                    var attributeId = await _preferenceService.UpsertServiceProviderAttributeAsync(request, userId);
                    return attributeId;
                },
                "Service provider attribute saved successfully!"
            );
        }

        /// <summary>
        /// Delete a service provider attribute
        /// </summary>
        [HttpDelete]
        [Route("ServiceProvider/{id}")]
        public async Task<IActionResult> DeleteServiceProviderAttribute(Guid id)
        {
            if (id == Guid.Empty)
                return ValidationError("Valid Attribute ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var userId = GetCurrentUserId();
                    var result = await _preferenceService.DeleteServiceProviderAttributeAsync(id, userId);
                    return result;
                },
                "Service provider attribute deleted successfully!"
            );
        }

        #endregion

        #region Matching

        /// <summary>
        /// Get service providers matching client preferences
        /// </summary>
        [HttpPost]
        [Route("MatchingServiceProviders")]
        public async Task<IActionResult> GetMatchingServiceProviders([FromBody] GetMatchingServiceProvidersRequest request)
        {
            if (request == null || request.ClientId == Guid.Empty)
                return ValidationError("Valid Client ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var serviceProviders = await _preferenceService.GetMatchingServiceProvidersAsync(
                        request.ClientId,
                        request.FranchiseId
                    );
                    return new GetMatchingServiceProvidersResponse { ServiceProviders = serviceProviders };
                },
                "Matching service providers retrieved successfully!"
            );
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get current user ID from JWT claims
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("sub")?.Value
                ?? User?.FindFirst("userId")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }

        #endregion
    }
}

