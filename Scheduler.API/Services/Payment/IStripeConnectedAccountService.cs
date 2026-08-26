using Scheduler.API.Models.Payment;

namespace Scheduler.API.Services.Payment
{
    public interface IStripeConnectedAccountService
    {
        Task<ConnectedAccountResult> CreateConnectedAccountAsync(ConnectedAccountRequest request);
        Task<ConnectedAccountResult> GetConnectedAccountAsync(string connectedAccountId);
        Task<bool> UpdateConnectedAccountAsync(string connectedAccountId, ConnectedAccountUpdateRequest request);
        Task<bool> DeleteConnectedAccountAsync(string connectedAccountId);
        Task<AccountLinkResult> CreateAccountLinkAsync(string connectedAccountId);
        
        // Direct payout methods
        Task<PaymentResult> CreateDirectPayoutAsync(DirectPayoutRequest request);
        
        // Payout methods
        Task<PaymentResult> CreatePayoutToConnectedAccountAsync(PayoutRequest request);
        Task<PaymentResult> GetPayoutStatusAsync(string payoutId);
        Task<PaymentResult> RetryFailedPayoutAsync(string payoutId);
        
        // Invoice charging methods
        Task<PaymentResult> CreateChargeForInvoiceAsync(InvoiceChargeRequest request);
        Task<PaymentResult> GetChargeStatusAsync(string chargeId);
        Task<PaymentResult> RefundChargeAsync(string chargeId, decimal? amount = null);
        
        // Token creation methods
        Task<string> CreateStripeTokenAsync(CardTokenRequest request);
        
        // Card validation methods
        Task<PaymentResult> ValidateCardWithTestChargeAsync(CardTokenRequest request);
    }

    public class ConnectedAccountRequest
    {
        public string Email { get; set; }
        public string Country { get; set; }
        public string BusinessType { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    public class ConnectedAccountUpdateRequest
    {
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    public class ConnectedAccountResult
    {
        public bool Success { get; set; }
        public string ConnectedAccountId { get; set; }
        public string ErrorMessage { get; set; }
        public string AccountLink { get; set; }
        public string Status { get; set; }
    }

    public class DirectPayoutRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountHolderType { get; set; } = "individual";
        public string Description { get; set; }
        public string StatementDescriptor { get; set; }
        public string Method { get; set; } = "standard"; // "standard" or "instant"
    }

    public class PayoutRequest
    {
        public string ConnectedAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string StatementDescriptor { get; set; }
    }

    public class InvoiceChargeRequest
    {
        public string StripeToken { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class CardTokenRequest
    {
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvc { get; set; }
        public string CardHolderName { get; set; }
    }

    public class AccountLinkResult
    {
        public bool Success { get; set; }
        public string AccountLink { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string ErrorMessage { get; set; }
    }
}
