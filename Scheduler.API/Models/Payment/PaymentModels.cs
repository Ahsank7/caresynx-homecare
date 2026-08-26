
namespace Scheduler.API.Models.Payment
{
    public class PaymentRequest
    {
        public int Id { get; set; }
    }

    public class PaymentData
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }
        public Guid Row_Guid { get; set; }
        public string BankAccountId { get; set; }
        
        // User Information
        public string Email { get; set; }
        
        // Bank Account Information for WAGE payments
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string IBAN { get; set; }
        public string ConnectedAccountId { get; set; }
        
        // Card Information for INVOICE payments (keeping for backward compatibility)
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        
        // Currency Information from Organization
        public int CurrencyId { get; set; }
        public string CurrencySign { get; set; }
        public string CurrencyCode { get; set; } // ISO currency code like USD, EUR, etc.

        /// <summary>Invoice debtor: 1 = client, 2 = organization payer, 3 = guarantor (INVOICE only).</summary>
        public byte? BillToType { get; set; }
        public Guid? BillToPayerId { get; set; }
        /// <summary>ClientCard | PayerCard | PayerManual (INVOICE only).</summary>
        public string ChargeSource { get; set; }
    }

    public class PaymentResult
    {
        public string ReferenceId { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PaymentResponse
    {
        public bool Success { get; set; }
        public int ProcessedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; }
        public string Message { get; set; }
    }

    public class PaymentValidationRequest
    {
        public string PaymentType { get; set; } // "INVOICE" or "WAGE"
        public int PaymentId { get; set; }
    }

    public class PaymentValidationResponse
    {
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; }
    }

    public class PaymentStatus
    {
        public int Id { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TestChargeRequest
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
    }

    public class ManualMarkAsPaidRequest
    {
        public string PaymentType { get; set; } // "INVOICE" or "WAGE"
        public int PaymentId { get; set; }
        public string Reason { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class ManualMarkAsPaidResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int AffectedRows { get; set; }
    }
}