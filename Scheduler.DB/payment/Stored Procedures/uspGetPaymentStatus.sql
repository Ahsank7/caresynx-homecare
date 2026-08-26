CREATE PROCEDURE [payment].[uspGetPaymentStatus]
    @pPaymentType VARCHAR(10),
    @pId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @pPaymentType = 'WAGE'
    BEGIN
        SELECT 
            Id,
            @pPaymentType AS PaymentType,
            CASE WHEN IsPaid = 1 THEN 'Paid' ELSE 'Unpaid' END AS Status,
            TransactionId,
            PaymentDate AS ProcessedDate,
            NULL AS ErrorMessage
        FROM tblServiceProviderWage
        WHERE Id = @pId;
    END
    ELSE IF @pPaymentType = 'INVOICE'
    BEGIN
        SELECT 
            Id,
            @pPaymentType AS PaymentType,
            CASE WHEN IsPaid = 1 THEN 'Paid' ELSE 'Unpaid' END AS Status,
            TransactionId,
            PaymentDate AS ProcessedDate,
            NULL AS ErrorMessage
        FROM tblBillingInvoice
        WHERE Id = @pId;
    END
    ELSE
    BEGIN
        -- Return empty result for invalid payment type
        SELECT 
            NULL AS Id,
            NULL AS PaymentType,
            NULL AS Status,
            NULL AS TransactionId,
            NULL AS ProcessedDate,
            'Invalid payment type' AS ErrorMessage
        WHERE 1 = 0;
    END
END