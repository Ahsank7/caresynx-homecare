CREATE PROCEDURE [payment].[uspUpdatePaymentStatus]
    @pPaymentType VARCHAR(10),
    @pId INT,
    @pTransactionId VARCHAR(100)
AS
BEGIN
    IF @pPaymentType = 'WAGE'
    BEGIN
        UPDATE tblServiceProviderWage
        SET IsPaid = 1,
            TransactionId = @pTransactionId
        WHERE Id = @pId
    END
    ELSE IF @pPaymentType = 'INVOICE'
    BEGIN
        UPDATE tblBillingInvoice
        SET IsPaid = 1,
            TransactionId = @pTransactionId,
            PaymentDate = SYSUTCDATETIME()
        WHERE Id = @pId
    END
END