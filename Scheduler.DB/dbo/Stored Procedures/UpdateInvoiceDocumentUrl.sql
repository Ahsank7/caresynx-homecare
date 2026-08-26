CREATE PROCEDURE [dbo].[UpdateInvoiceDocumentUrl]
    @pInvoiceId INT,
    @pDocumentUrl NVARCHAR(1000)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[tblPackageInvoice]
    SET InvoiceDocumentUrl = @pDocumentUrl
    WHERE Id = @pInvoiceId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
