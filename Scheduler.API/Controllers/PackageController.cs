using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Scheduler.API.Common;
using Scheduler.API.Common.Extensions;
using Scheduler.API.Models.Package;
using Scheduler.API.Models.Payment;
using Scheduler.API.Services.Package;
using Scheduler.API.Services.Payment;
using Scheduler.API.Services.Security;
using Microsoft.Extensions.Logging;
using Scheduler.API.Helper;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : BaseController
    {
        private readonly IPackage _package;
        private readonly ICrypto _crypto;
        private readonly IStripeConnectedAccountService _stripeService;

        public PackageController(
            IPackage package, 
            ICrypto crypto, 
            IStripeConnectedAccountService stripeService,
            ILogger<PackageController> logger)
            : base(logger)
        {
            _package = package;
            _crypto = crypto;
            _stripeService = stripeService;
        }

        [HttpPost]
        [Route("SaveUpdate")]
        public async Task<IActionResult> SaveUpdatePackage(PackageViewModel model)
        {
            if (model == null)
                return ValidationError("Package data is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                return ValidationError("Package name is required");

            return await ExecuteAsync(
                () => _package.SaveUpdatePackageAsync(model),
                "Package saved successfully!"
            );
        }

        [HttpGet]
        [Route("All")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackages([FromQuery] bool includeInactive = false)
        {
            try
            {
                var result = await _package.GetAllPackagesAsync(includeInactive);
                // Tuple is serialized as { Item1: [...], Item2: count }
                // We'll return it as an array [list, count] for easier frontend handling
                var response = new { data = result.Item1, count = result.Item2 };
                return Ok(Response<object>.Success(response, "Packages retrieved successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages");
                return StatusCode(500, Response<object>.InternalServerError(ex, "An error occurred while retrieving packages"));
            }
        }

        [HttpGet]
        [Route("{packageId}")]
        public async Task<IActionResult> GetPackageById(Guid packageId)
        {
            if (packageId == Guid.Empty)
                return ValidationError("Valid Package ID is required");

            return await ExecuteAsync(
                () => _package.GetPackageByIdAsync(packageId),
                "Package retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("AssignToOrganization")]
        public async Task<IActionResult> AssignPackageToOrganization(AssignPackageToOrganizationViewModel model)
        {
            if (model == null)
                return ValidationError("Assignment data is required");

            if (model.OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            if (model.PackageId == Guid.Empty)
                return ValidationError("Valid Package ID is required");

            return await ExecuteAsync(
                () => _package.AssignPackageToOrganizationAsync(model),
                "Package assigned to organization successfully!"
            );
        }

        [HttpGet]
        [Route("Organization/{organizationId}/History")]
        public async Task<IActionResult> GetOrganizationPackageHistory(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _package.GetOrganizationPackageHistoryAsync(organizationId),
                "Package history retrieved successfully!"
            );
        }

        [HttpGet]
        [Route("Organization/{organizationId}/Current")]
        public async Task<IActionResult> GetCurrentOrganizationPackage(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            try
            {
                var result = await _package.GetCurrentOrganizationPackageAsync(organizationId);
                // Return null if no current package (not 404)
                if (result == null)
                {
                    return Ok(Response<OrganizationPackageHistoryViewModel>.Success(null, "No current package assigned"));
                }
                return Ok(Response<OrganizationPackageHistoryViewModel>.Success(result, "Current package retrieved successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current package");
                return StatusCode(500, Response<OrganizationPackageHistoryViewModel>.InternalServerError(ex, "An error occurred while retrieving current package"));
            }
        }

        [HttpPut]
        [Route("Organization/Package/UpdatePricing")]
        public async Task<IActionResult> UpdateOrganizationPackagePricing(UpdateOrganizationPackagePricingViewModel model)
        {
            if (model == null)
                return ValidationError("Pricing data is required");

            if (model.OrganizationPackageId == Guid.Empty)
                return ValidationError("Valid Organization Package ID is required");

            try
            {
                var result = await _package.UpdateOrganizationPackagePricingAsync(model);
                if (!result)
                {
                    return BadRequest(Response<bool>.BadRequest("Failed to update package pricing. Only active packages can be edited."));
                }
                return Ok(Response<bool>.Success(result, "Package pricing updated successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization package pricing");
                return StatusCode(500, Response<bool>.InternalServerError(ex, "An error occurred while updating package pricing"));
            }
        }

        [HttpPost]
        [Route("Organization/{organizationId}/Card")]
        public async Task<IActionResult> SaveUpdateOrganizationCard(Guid organizationId, OrganizationCardInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Card data is required");

            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            if (string.IsNullOrEmpty(model.CardNumber))
                return ValidationError("Card number is required");

            if (string.IsNullOrEmpty(model.CVV))
                return ValidationError("CVV is required");

            model.OrganizationId = organizationId;

            return await ExecuteAsync(async () =>
            {
                // Step 1: Validate card with test charge/refund BEFORE encrypting
                var cardTokenRequest = new CardTokenRequest
                {
                    CardNumber = model.CardNumber,
                    ExpiryMonth = model.ExpiryMonth.ToString("00"),
                    ExpiryYear = model.ExpiryYear.ToString(),
                    Cvc = model.CVV,
                    CardHolderName = model.CardHolderName
                };

                var validationResult = await _stripeService.ValidateCardWithTestChargeAsync(cardTokenRequest);

                if (!validationResult.Success)
                {
                    throw new InvalidOperationException($"Card validation failed: {validationResult.ErrorMessage}");
                }

                _logger.LogInformation($"Card validated successfully for organization {organizationId}. Charge ID: {validationResult.ReferenceId}");

                // Step 2: Encrypt sensitive card data before saving
                model.CardNumber = _crypto.Encrypt(model.CardNumber);
                model.CVV = _crypto.Encrypt(model.CVV);

                // Step 3: Save to database
                var result = await _package.SaveUpdateOrganizationCardAsync(model);

                if (result == null)
                    throw new InvalidOperationException("Failed to save/update card information");

                // Step 4: Mask card number for response
                if (!string.IsNullOrEmpty(result.CardNumber))
                {
                    result.CardNumber = _crypto.Decrypt(result.CardNumber).MaskString();
                }
                if (!string.IsNullOrEmpty(result.CVV))
                {
                    result.CVV = _crypto.Decrypt(result.CVV).MaskString();
                }

                return result;
            }, "Card validated and saved successfully! A $1.00 test charge was made and immediately refunded.");
        }

        [HttpGet]
        [Route("Organization/{organizationId}/Card")]
        public async Task<IActionResult> GetOrganizationCard(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _package.GetOrganizationCardAsync(organizationId);

                if (result != null)
                {
                    // Decrypt and mask card number for display
                    if (!string.IsNullOrEmpty(result.CardNumber))
                    {
                        result.CardNumber = _crypto.Decrypt(result.CardNumber).MaskString();
                    }
                    // Decrypt and mask CVV for display
                    if (!string.IsNullOrEmpty(result.CVV))
                    {
                        result.CVV = _crypto.Decrypt(result.CVV).MaskString();
                    }
                }

                return result;
            }, "Card information retrieved successfully!");
        }

        [HttpPost]
        [Route("Organization/{organizationId}/Card/Validate")]
        public async Task<IActionResult> ValidateOrganizationCard(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _package.ValidateOrganizationCardAsync(organizationId),
                "Card validation completed!"
            );
        }

        [HttpGet]
        [Route("Organization/{organizationId}/Invoices")]
        public async Task<IActionResult> GetOrganizationInvoices(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _package.GetOrganizationInvoicesAsync(organizationId),
                "Invoices retrieved successfully!"
            );
        }

        [HttpGet]
        [Route("Invoice/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceById(int invoiceId)
        {
            if (invoiceId <= 0)
                return ValidationError("Valid Invoice ID is required");

            return await ExecuteAsync(
                () => _package.GetInvoiceByIdAsync(invoiceId),
                "Invoice retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("GenerateMonthlyInvoices")]
        public async Task<IActionResult> GenerateMonthlyInvoices(GenerateMonthlyInvoicesRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            if (request.BillingMonth < 1 || request.BillingMonth > 12)
                return ValidationError("Billing month must be between 1 and 12");

            if (request.BillingYear < 2000 || request.BillingYear > 2100)
                return ValidationError("Invalid billing year");

            return await ExecuteAsync(
                () => _package.GenerateMonthlyInvoicesAsync(request),
                "Monthly invoices generated successfully!"
            );
        }

        [HttpPost]
        [Route("ProcessInvoicePayment")]
        public async Task<IActionResult> ProcessInvoicePayment(ProcessPackageInvoicePaymentRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            if (request.InvoiceId <= 0)
                return ValidationError("Valid Invoice ID is required");

            if (request.OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _package.ProcessInvoicePaymentAsync(request),
                "Payment processed successfully!"
            );
        }
    }
}

