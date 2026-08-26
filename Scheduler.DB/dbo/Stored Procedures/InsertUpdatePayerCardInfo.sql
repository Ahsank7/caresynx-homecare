CREATE PROCEDURE [dbo].[InsertUpdatePayerCardInfo]
    @pOutId UNIQUEIDENTIFIER = NULL OUTPUT,
    @pPayerId UNIQUEIDENTIFIER = NULL,
    @pCardId UNIQUEIDENTIFIER = NULL,
    @pCVV NVARCHAR(100) = NULL,
    @pCardHolderName NVARCHAR(500) = NULL,
    @pCardNumber NVARCHAR(500) = NULL,
    @pTypeId INT = NULL,
    @pExpiryYear INT = NULL,
    @pExpiryMonth INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @pCardId IS NULL
    BEGIN
        SET @pOutId = NEWID();
        INSERT INTO [dbo].[tblPayerCardInfo] (
            [CardId], [PayerId], [CardHolderName], [CardNumber], [ExpiryMonth], [ExpiryYear], [CVV], [TypeId]
        )
        VALUES (
            @pOutId, @pPayerId, @pCardHolderName, @pCardNumber, @pExpiryMonth, @pExpiryYear, @pCVV, @pTypeId
        );
    END
    ELSE
    BEGIN
        UPDATE [dbo].[tblPayerCardInfo]
        SET [CardHolderName] = @pCardHolderName,
            [CardNumber] = CASE WHEN @pCardNumber IS NOT NULL THEN @pCardNumber ELSE [CardNumber] END,
            [ExpiryMonth] = @pExpiryMonth,
            [ExpiryYear] = @pExpiryYear,
            [CVV] = CASE WHEN @pCVV IS NOT NULL THEN @pCVV ELSE [CVV] END,
            [TypeId] = @pTypeId
        WHERE [CardId] = @pCardId AND [PayerId] = @pPayerId;

        SET @pOutId = @pCardId;
    END
END
