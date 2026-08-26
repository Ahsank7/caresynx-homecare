CREATE PROCEDURE [dbo].[ChangeUserPassword]
    @pUserId uniqueidentifier,
    @pOldPassword nvarchar(500),
    @pNewPassword nvarchar(500),
    @pIsValid bit OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @vCurrentPassword nvarchar(500)
    DECLARE @vUserExists bit = 0
    
    -- Check if user exists and get current password
    SELECT @vCurrentPassword = [Password], @vUserExists = 1
    FROM [dbo].[tblUser]
    WHERE [Id] = @pUserId AND [IsActive] = 1
    
    -- Validate old password
    IF @vUserExists = 0
    BEGIN
        SET @pIsValid = 0
        RETURN
    END
    
    IF @vCurrentPassword != @pOldPassword
    BEGIN
        SET @pIsValid = 0
        RETURN
    END
    
    -- Check if new password is different from old password
    IF @pOldPassword = @pNewPassword
    BEGIN
        SET @pIsValid = 0
        RETURN
    END
    
    -- Update password
    UPDATE [dbo].[tblUser]
    SET [Password] = @pNewPassword,
        [UpdatedDate] = GETUTCDATE()
    WHERE [Id] = @pUserId
    
    SET @pIsValid = 1
END 