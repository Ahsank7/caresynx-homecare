CREATE PROCEDURE [dbo].[UpdatePackageInvoicePaymentStatus]
    @pInvoiceId INT,
    @pPaymentStatus NVARCHAR(50),
    @pPaymentDate DATETIME2 = NULL,
    @pPaymentTransactionId NVARCHAR(200) = NULL,
    @pPaymentFailureReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[tblPackageInvoice]
    SET PaymentStatus = @pPaymentStatus,
        PaymentDate = ISNULL(@pPaymentDate, CASE WHEN @pPaymentStatus = 'Paid' THEN SYSUTCDATETIME() ELSE NULL END),
        PaymentTransactionId = @pPaymentTransactionId,
        PaymentFailureReason = @pPaymentFailureReason
    WHERE Id = @pInvoiceId;
    
    SELECT @@ROWCOUNT AS AffectedRows;
END