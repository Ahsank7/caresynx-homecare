CREATE PROCEDURE [dbo].[GetOrganizationPackageInvoices]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pi.Id,
        pi.OrganizationId,
        pi.OrganizationPackageId,
        p.Name AS PackageName,
        pi.InvoiceDate,
        pi.BillingPeriodStart,
        pi.BillingPeriodEnd,
        pi.PerClientCharge,
        pi.ClientCount,
        pi.InitialOneTimeCost,
        pi.InfrastructureCost,
        pi.SupportCharges,
        pi.NewFeatureReportCharges,
        pi.SubTotal,
        pi.TaxAmount,
        pi.TotalAmount,
        pi.IsInitialCharge,
        pi.PaymentStatus,
        pi.PaymentDate,
        pi.PaymentTransactionId,
        pi.PaymentFailureReason,
        pi.InvoiceNumber,
        pi.CreatedDate
    FROM [dbo].[tblPackageInvoice] pi
    INNER JOIN [dbo].[tblOrganizationPackage] op ON pi.OrganizationPackageId = op.Id
    INNER JOIN [dbo].[tblPackage] p ON op.PackageId = p.Id
    WHERE pi.OrganizationId = @pOrganizationId
    ORDER BY pi.InvoiceDate DESC, pi.BillingPeriodStart DESC;
END