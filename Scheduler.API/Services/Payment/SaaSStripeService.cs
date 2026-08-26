using Scheduler.API.Models.Payment;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Scheduler.API.Services.Payment
{
    /// <summary>
    /// Stripe service specifically for SaaS Admin operations (charging organizations for packages)
    /// Separate from organization Stripe accounts which charge clients/service providers
    /// </summary>
    public class SaaSStripeService : ISaaSStripeService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SaaSStripeService> _logger;
        private readonly string _saasStripeSecretKey;

        public SaaSStripeService(IConfiguration configuration, ILogger<SaaSStripeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _saasStripeSecretKey = _configuration["Stripe:SaaSAdmin:SecretKey"] 
                ?? _configuration["Stripe:SecretKey"]; // Fallback to main key if SaaS key not configured
        }

        public async Task<PaymentResult> CreateChargeForPackageInvoiceAsync(InvoiceChargeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.StripeToken))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Stripe token is required for invoice charge"
                    };
                }

                // Use SaaS admin Stripe account
                var requestOptions = new RequestOptions
                {
                    ApiKey = _saasStripeSecretKey
                };

                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = request.Currency?.ToLower() ?? "usd",
                    Source = request.StripeToken,
                    Description = request.Description,
                    ReceiptEmail = request.CustomerEmail,
                    Metadata = request.Metadata ?? new Dictionary<string, string>
                    {
                        { "charge_type", "package_invoice" },
                        { "saas_admin", "true" }
                    }
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions, requestOptions);

                _logger.LogInformation($"SaaS Admin charge created: {charge.Id}, Amount: ${request.Amount}, Status: {charge.Status}");

                return new PaymentResult
                {
                    ReferenceId = charge.Id,
                    Success = charge.Status == "succeeded",
                    ErrorMessage = charge.Status
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "SaaS Admin invoice charge failed");
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Invoice charge failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SaaS Admin invoice charge");
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating invoice charge: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResult> RefundPackageInvoiceChargeAsync(string chargeId, decimal? amount = null)
        {
            try
            {
                if (string.IsNullOrEmpty(chargeId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Charge ID is required"
                    };
                }

                // Use SaaS admin Stripe account
                var requestOptions = new RequestOptions
                {
                    ApiKey = _saasStripeSecretKey
                };

                var refundOptions = new RefundCreateOptions
                {
                    Charge = chargeId
                };

                if (amount.HasValue)
                {
                    refundOptions.Amount = (long)(amount.Value * 100); // Convert to cents
                }

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions, requestOptions);

                _logger.LogInformation($"SaaS Admin refund created: {refund.Id}, Charge: {chargeId}, Status: {refund.Status}");

                return new PaymentResult
                {
                    ReferenceId = refund.Id,
                    Success = refund.Status == "succeeded",
                    ErrorMessage = refund.Status
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"SaaS Admin refund failed for charge {chargeId}");
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating refund: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating SaaS Admin refund for charge {chargeId}");
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating refund: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResult> GetChargeStatusAsync(string chargeId)
        {
            try
            {
                if (string.IsNullOrEmpty(chargeId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Charge ID is required"
                    };
                }

                // Use SaaS admin Stripe account
                var requestOptions = new RequestOptions
                {
                    ApiKey = _saasStripeSecretKey
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.GetAsync(chargeId, null, requestOptions);

                return new PaymentResult
                {
                    ReferenceId = charge.Id,
                    Success = charge.Status == "succeeded",
                    ErrorMessage = charge.Status
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrieving charge status: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrieving charge status: {ex.Message}"
                };
            }
        }
    }
}
