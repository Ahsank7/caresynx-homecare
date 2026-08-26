
CREATE PROCEDURE [dbo].[uspUpsertServiceProviderAttribute]
    @Id UNIQUEIDENTIFIER = NULL,
    @ServiceProviderId UNIQUEIDENTIFIER,
    @AttributeType NVARCHAR(100),
    @AttributeValue NVARCHAR(200) = NULL,
    @AttributeItemId INT = NULL,
    @UserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Now DATETIME = GETDATE();

    IF @Id IS NOT NULL AND EXISTS(SELECT 1 FROM [dbo].[tblServiceProviderAttributes] WHERE [Id] = @Id)
    BEGIN
        UPDATE [dbo].[tblServiceProviderAttributes]
        SET 
            [AttributeType] = @AttributeType,
            [AttributeValue] = @AttributeValue,
            [AttributeItemId] = @AttributeItemId,
            [UpdatedDate] = @Now,
            [UpdatedBy] = @UserId
        WHERE [Id] = @Id;
        
        SELECT @Id AS Id;
    END
    ELSE
    BEGIN
        IF EXISTS(SELECT 1 FROM [dbo].[tblServiceProviderAttributes] 
                  WHERE [ServiceProviderId] = @ServiceProviderId 
                  AND [AttributeType] = @AttributeType 
                  AND [IsActive] = 1)
        BEGIN
            UPDATE [dbo].[tblServiceProviderAttributes]
            SET 
                [AttributeValue] = @AttributeValue,
                [AttributeItemId] = @AttributeItemId,
                [UpdatedDate] = @Now,
                [UpdatedBy] = @UserId
            WHERE [ServiceProviderId] = @ServiceProviderId 
                AND [AttributeType] = @AttributeType 
                AND [IsActive] = 1;
                
            SELECT [Id] FROM [dbo].[tblServiceProviderAttributes] 
            WHERE [ServiceProviderId] = @ServiceProviderId 
                AND [AttributeType] = @AttributeType 
                AND [IsActive] = 1;
        END
        ELSE
        BEGIN
            SET @Id = NEWID();
            
            INSERT INTO [dbo].[tblServiceProviderAttributes] (
                [Id], [ServiceProviderId], [AttributeType], [AttributeValue], 
                [AttributeItemId], [CreatedDate], [CreatedBy], [IsActive]
            )
            VALUES (
                @Id, @ServiceProviderId, @AttributeType, @AttributeValue,
                @AttributeItemId, @Now, @UserId, 1
            );
            
            SELECT @Id AS Id;
        END
    END
END