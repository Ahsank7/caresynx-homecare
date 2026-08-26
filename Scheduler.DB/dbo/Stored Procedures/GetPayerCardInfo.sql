CREATE PROCEDURE [dbo].[GetPayerCardInfo]
    @pPayerId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        BA.Id,
        BA.CardId,
        BA.PayerId,
        BA.CardHolderName,
        BA.CardNumber,
        BA.ExpiryMonth,
        BA.ExpiryYear,
        BA.CVV,
        BA.TypeId
    FROM [dbo].[tblPayerCardInfo] BA
    WHERE BA.[PayerId] = @pPayerId;
END
