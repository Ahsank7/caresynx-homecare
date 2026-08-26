CREATE PROCEDURE [dbo].[InsertUpdateOrganizationCardInfo]
    @pId INT = NULL,
    @pOrganizationId UNIQUEIDENTIFIER,
    @pCardHolderName NVARCHAR(100),
    @pCardNumber NVARCHAR(500),
    @pExpiryMonth TINYINT,
    @pExpiryYear SMALLINT,
    @pCVV NVARCHAR(100),
    @pTypeId INT,
    @pIsActive BIT = 1,
    @pOutId INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Deactivate other cards for this organization
    UPDATE [dbo].[tblOrganizationCardInfo]
    SET IsActive = 0, UpdatedAt = SYSUTCDATETIME()
    WHERE OrganizationId = @pOrganizationId
      AND IsActive = 1;
    
    IF @pId IS NULL
    BEGIN
        INSERT INTO [dbo].[tblOrganizationCardInfo]
            (OrganizationId, CardHolderName, CardNumber, ExpiryMonth, ExpiryYear, 
             CVV, TypeId, IsActive, CreatedAt)
        VALUES
            (@pOrganizationId, @pCardHolderName, @pCardNumber, @pExpiryMonth, 
             @pExpiryYear, @pCVV, @pTypeId, @pIsActive, SYSUTCDATETIME());
        
        SET @pOutId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE [dbo].[tblOrganizationCardInfo]
        SET CardHolderName = @pCardHolderName,
            CardNumber = @pCardNumber,
            ExpiryMonth = @pExpiryMonth,
            ExpiryYear = @pExpiryYear,
            CVV = @pCVV,
            TypeId = @pTypeId,
            IsActive = @pIsActive,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @pId;
        
        SET @pOutId = @pId;
    END
END