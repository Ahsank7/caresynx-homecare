namespace Scheduler.API.Models.Package
{
    public class OrganizationCardInfoViewModel
    {
        public int? Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CVV { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public bool IsActive { get; set; }
    }

    public class PackageInvoiceViewModel
    {
        public int Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid OrganizationPackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime BillingPeriodStart { get; set; }
        public DateTime BillingPeriodEnd { get; set; }
        public decimal PerClientCharge { get; set; }
        public int ClientCount { get; set; }
        public decimal InitialOneTimeCost { get; set; }
        public decimal InfrastructureCost { get; set; }
        public decimal SupportCharges { get; set; }
        public decimal NewFeatureReportCharges { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsInitialCharge { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public string? PaymentTransactionId { get; set; }
        public string? PaymentFailureReason { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? InvoiceDocumentUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class GenerateMonthlyInvoicesRequest
    {
        public int BillingMonth { get; set; } // 1-12
        public int BillingYear { get; set; }
        public Guid? OrganizationId { get; set; } // Optional: null = all organizations
    }

    public class ProcessPackageInvoicePaymentRequest
    {
        public int InvoiceId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}

