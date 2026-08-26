CREATE PROCEDURE [dbo].[GetOrganizationCardInfo]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1
        Id,
        OrganizationId,
        CardHolderName,
        CardNumber,
        ExpiryMonth,
        ExpiryYear,
        CVV,
        TypeId,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[tblOrganizationCardInfo]
    WHERE OrganizationId = @pOrganizationId
      AND IsActive = 1
    ORDER BY CreatedAt DESC;
END