CREATE PROCEDURE [payment].[uspManualMarkPaymentAsPaid]
    @pPaymentType VARCHAR(10),
    @pId INT,
    @pManualPaymentReason NVARCHAR(500),
    @pPaymentDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Default payment date to current UTC time if not provided
    IF @pPaymentDate IS NULL
        SET @pPaymentDate = SYSUTCDATETIME();
    
    IF @pPaymentType = 'WAGE'
    BEGIN
        UPDATE tblServiceProviderWage
        SET IsPaid = 1,
            IsManualPayment = 1,
            ManualPaymentReason = @pManualPaymentReason,
            PaymentDate = @pPaymentDate,
            TransactionId = CONCAT('MANUAL_', @pId, '_', FORMAT(@pPaymentDate, 'yyyyMMddHHmmss'))
        WHERE Id = @pId AND IsPaid = 0;
        
        SELECT @@ROWCOUNT AS AffectedRows;
    END
    ELSE IF @pPaymentType = 'INVOICE'
    BEGIN
        UPDATE tblBillingInvoice
        SET IsPaid = 1,
            IsManualPayment = 1,
            ManualPaymentReason = @pManualPaymentReason,
            PaymentDate = @pPaymentDate,
            TransactionId = CONCAT('MANUAL_', @pId, '_', FORMAT(@pPaymentDate, 'yyyyMMddHHmmss'))
        WHERE Id = @pId AND IsPaid = 0;
        
        SELECT @@ROWCOUNT AS AffectedRows;
    END
    ELSE
    BEGIN
        SELECT 0 AS AffectedRows;
    END
END