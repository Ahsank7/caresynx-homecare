using Scheduler.API.Models.Payment;

namespace Scheduler.API.Services.Payment
{
    /// <summary>
    /// Interface for SaaS Admin Stripe operations (charging organizations for packages)
    /// Separate from organization Stripe accounts which charge clients/service providers
    /// </summary>
    public interface ISaaSStripeService
    {
        /// <summary>
        /// Create a charge for a package invoice using SaaS admin Stripe account
        /// </summary>
        Task<PaymentResult> CreateChargeForPackageInvoiceAsync(InvoiceChargeRequest request);
        
        /// <summary>
        /// Refund a package invoice charge using SaaS admin Stripe account
        /// </summary>
        Task<PaymentResult> RefundPackageInvoiceChargeAsync(string chargeId, decimal? amount = null);
        
        /// <summary>
        /// Get charge status from SaaS admin Stripe account
        /// </summary>
        Task<PaymentResult> GetChargeStatusAsync(string chargeId);
    }
}
