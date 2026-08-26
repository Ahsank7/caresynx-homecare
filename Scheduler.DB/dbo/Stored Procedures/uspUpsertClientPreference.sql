-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Insert or Update a client preference
-- =============================================
CREATE PROCEDURE [dbo].[uspUpsertClientPreference]
    @Id UNIQUEIDENTIFIER = NULL,
    @ClientId UNIQUEIDENTIFIER,
    @PreferenceType NVARCHAR(100),
    @PreferenceValue NVARCHAR(200) = NULL,
    @PreferenceItemId INT = NULL,
    @IsRequired BIT = 0,
    @UserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Now DATETIME = GETDATE();

    -- Check if updating existing preference
    IF @Id IS NOT NULL AND EXISTS(SELECT 1 FROM [dbo].[tblClientPreferences] WHERE [Id] = @Id)
    BEGIN
        UPDATE [dbo].[tblClientPreferences]
        SET 
            [PreferenceType] = @PreferenceType,
            [PreferenceValue] = @PreferenceValue,
            [PreferenceItemId] = @PreferenceItemId,
            [IsRequired] = @IsRequired,
            [UpdatedDate] = @Now,
            [UpdatedBy] = @UserId
        WHERE [Id] = @Id;
        
        SELECT @Id AS Id;
    END
    ELSE
    BEGIN
        -- Check if preference of this type already exists for client
        IF EXISTS(SELECT 1 FROM [dbo].[tblClientPreferences] 
                  WHERE [ClientId] = @ClientId 
                  AND [PreferenceType] = @PreferenceType 
                  AND [IsActive] = 1)
        BEGIN
            -- Update existing preference
            UPDATE [dbo].[tblClientPreferences]
            SET 
                [PreferenceValue] = @PreferenceValue,
                [PreferenceItemId] = @PreferenceItemId,
                [IsRequired] = @IsRequired,
                [UpdatedDate] = @Now,
                [UpdatedBy] = @UserId
            WHERE [ClientId] = @ClientId 
                AND [PreferenceType] = @PreferenceType 
                AND [IsActive] = 1;
                
            SELECT [Id] FROM [dbo].[tblClientPreferences] 
            WHERE [ClientId] = @ClientId 
                AND [PreferenceType] = @PreferenceType 
                AND [IsActive] = 1;
        END
        ELSE
        BEGIN
            -- Insert new preference
            SET @Id = NEWID();
            
            INSERT INTO [dbo].[tblClientPreferences] (
                [Id],
                [ClientId],
                [PreferenceType],
                [PreferenceValue],
                [PreferenceItemId],
                [IsRequired],
                [CreatedDate],
                [CreatedBy],
                [IsActive]
            )
            VALUES (
                @Id,
                @ClientId,
                @PreferenceType,
                @PreferenceValue,
                @PreferenceItemId,
                @IsRequired,
                @Now,
                @UserId,
                1
            );
            
            SELECT @Id AS Id;
        END
    END
END

