using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduler.API.Common;
using Scheduler.API.Models.Payer;
using Scheduler.API.Services.Payer;
using Scheduler.API.Services.Security;
using Scheduler.API.Helper;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientPayerController : BaseController
    {
        private readonly IClientPayerService _clientPayerService;
        private readonly ICrypto _crypto;

        public ClientPayerController(IClientPayerService clientPayerService, ICrypto crypto, ILogger<ClientPayerController> logger)
            : base(logger)
        {
            _clientPayerService = clientPayerService;
            _crypto = crypto;
        }

        [HttpGet("Payers")]
        public async Task<IActionResult> GetPayers([FromQuery] Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("organizationId is required");
            return await ExecuteAsync(
                () => _clientPayerService.GetPayersByOrganizationAsync(organizationId),
                "Payers loaded");
        }

        [HttpPost("Payer")]
        public async Task<IActionResult> SavePayer([FromBody] PayerDto model)
        {
            if (model == null)
                return ValidationError("Body is required");
            if (model.OrganizationId == Guid.Empty)
                return ValidationError("OrganizationId is required");
            return await ExecuteAsync(
                () => _clientPayerService.SavePayerAsync(model),
                "Payer saved");
        }

        [HttpGet("Coverage")]
        public async Task<IActionResult> GetCoverage([FromQuery] Guid clientId)
        {
            if (clientId == Guid.Empty)
                return ValidationError("clientId is required");
            return await ExecuteAsync(
                () => _clientPayerService.GetClientCoverageAsync(clientId),
                "Coverage loaded");
        }

        [HttpPost("Coverage")]
        public async Task<IActionResult> SaveCoverage([FromBody] ClientPayerCoverageDto model)
        {
            if (model == null)
                return ValidationError("Body is required");
            if (model.ClientId == Guid.Empty || model.PayerId == Guid.Empty)
                return ValidationError("ClientId and PayerId are required");
            return await ExecuteAsync(
                () => _clientPayerService.SaveClientCoverageAsync(model),
                "Coverage saved");
        }

        [HttpGet("Preference")]
        public async Task<IActionResult> GetPreference([FromQuery] Guid clientId)
        {
            if (clientId == Guid.Empty)
                return ValidationError("clientId is required");
            return await ExecuteAsync(
                () => _clientPayerService.GetClientBillingPreferenceAsync(clientId),
                "Preference loaded");
        }

        [HttpPost("Preference")]
        public async Task<IActionResult> SavePreference([FromBody] ClientBillingPreferenceDto model)
        {
            if (model == null)
                return ValidationError("Body is required");
            if (model.ClientId == Guid.Empty)
                return ValidationError("ClientId is required");
            return await ExecuteAsync(
                async () =>
                {
                    await _clientPayerService.SaveClientBillingPreferenceAsync(model);
                    return true;
                },
                "Preference saved");
        }

        [HttpGet("Funding")]
        public async Task<IActionResult> GetFunding([FromQuery] Guid clientId, [FromQuery] Guid organizationId)
        {
            if (clientId == Guid.Empty || organizationId == Guid.Empty)
                return ValidationError("clientId and organizationId are required");
            return await ExecuteAsync(
                () => _clientPayerService.GetFundingRulesAsync(clientId, organizationId),
                "Funding rules loaded");
        }

        [HttpPost("Funding")]
        public async Task<IActionResult> SaveFunding([FromBody] ClientPayerServiceFundingDto model)
        {
            if (model == null)
                return ValidationError("Body is required");
            if (model.ClientId == Guid.Empty || model.OrganizationId == Guid.Empty || model.PayerId == Guid.Empty)
                return ValidationError("ClientId, OrganizationId, and PayerId are required");
            return await ExecuteAsync(
                () => _clientPayerService.SaveFundingRuleAsync(model),
                "Funding rule saved");
        }

        [HttpDelete("Funding/{id}")]
        public async Task<IActionResult> DeleteFunding(int id, [FromQuery] Guid organizationId)
        {
            if (id <= 0 || organizationId == Guid.Empty)
                return ValidationError("id and organizationId are required");
            return await ExecuteAsync(
                async () =>
                {
                    await _clientPayerService.DeleteFundingRuleAsync(id, organizationId);
                    return true;
                },
                "Funding rule deleted");
        }

        [HttpGet("OrgFunding")]
        public async Task<IActionResult> GetOrgFunding([FromQuery] Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("organizationId is required");
            return await ExecuteAsync(
                () => _clientPayerService.GetOrganizationFundingRulesAsync(organizationId),
                "Organization funding rules loaded");
        }

        [HttpPost("OrgFunding")]
        public async Task<IActionResult> SaveOrgFunding([FromBody] OrganizationPayerServiceFundingDto model)
        {
            if (model == null)
                return ValidationError("Body is required");
            if (model.OrganizationId == Guid.Empty || model.PayerId == Guid.Empty)
                return ValidationError("OrganizationId and PayerId are required");
            return await ExecuteAsync(
                () => _clientPayerService.SaveOrganizationFundingRuleAsync(model),
                "Organization funding rule saved");
        }

        [HttpDelete("OrgFunding/{id}")]
        public async Task<IActionResult> DeleteOrgFunding(int id, [FromQuery] Guid organizationId)
        {
            if (id <= 0 || organizationId == Guid.Empty)
                return ValidationError("id and organizationId are required");
            return await ExecuteAsync(
                async () =>
                {
                    await _clientPayerService.DeleteOrganizationFundingRuleAsync(id, organizationId);
                    return true;
                },
                "Organization funding rule deleted");
        }

        /// <summary>Get organization payer card on file (masked). Data is null when payer is invalid or no card is stored yet.</summary>
        [HttpGet("PayerCard")]
        public async Task<IActionResult> GetPayerCard([FromQuery] Guid organizationId, [FromQuery] Guid payerId)
        {
            if (organizationId == Guid.Empty || payerId == Guid.Empty)
                return ValidationError("organizationId and payerId are required");

            try
            {
                var result = await _clientPayerService.GetPayerCardAsync(organizationId, payerId);
                if (result == null)
                    return Ok(Response<PayerCardInfoDto?>.Success(null, "No payer card on file"));

                if (!string.IsNullOrEmpty(result.CardNumber))
                    result.CardNumber = _crypto.Decrypt(result.CardNumber).MaskString();
                if (!string.IsNullOrEmpty(result.CVV))
                    result.CVV = _crypto.Decrypt(result.CVV).MaskString();

                return Ok(Response<PayerCardInfoDto>.Success(result, "Payer card loaded"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(Response<PayerCardInfoDto>.Error(ex.Message, StatusCodes.Status404NotFound));
            }
        }

        /// <summary>Save or update encrypted payer card for auto-charge (BillToType = organization payer).</summary>
        [HttpPost("PayerCard")]
        public async Task<IActionResult> UpsertPayerCard([FromBody] UpsertPayerCardViewModel model)
        {
            if (model == null)
                return ValidationError("Body is required");
            if (model.OrganizationId == Guid.Empty || model.PayerId == Guid.Empty)
                return ValidationError("OrganizationId and PayerId are required");

            if (model.CardId == Guid.Empty)
                model.CardId = null;

            var isNewCard = !model.CardId.HasValue;
            if (isNewCard &&
                (string.IsNullOrWhiteSpace(model.CardNumber) || string.IsNullOrWhiteSpace(model.CVV)))
                return ValidationError("Card number and CVV are required when adding a new payer payment method.");

            return await ExecuteAsync(async () =>
            {
                if (!string.IsNullOrWhiteSpace(model.CardNumber))
                    model.CardNumber = _crypto.Encrypt(model.CardNumber!);
                else
                    model.CardNumber = null;

                if (!string.IsNullOrWhiteSpace(model.CVV))
                    model.CVV = _crypto.Encrypt(model.CVV!);
                else
                    model.CVV = null;

                return await _clientPayerService.UpsertPayerCardAsync(model);
            }, "Payer card saved");
        }
    }
}
