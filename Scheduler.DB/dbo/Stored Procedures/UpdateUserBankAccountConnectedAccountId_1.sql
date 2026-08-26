CREATE PROCEDURE [dbo].[UpdateUserBankAccountConnectedAccountId]
    @pUserId UNIQUEIDENTIFIER,
    @pConnectedAccountId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE [dbo].[tblBankAccount]
        SET [ConnectedAccountId] = @pConnectedAccountId,
            [ModifiedDate] = GETDATE()
        WHERE [UserId] = @pUserId;
        
        -- Return the number of rows affected
        SELECT @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        -- Log the error and return 0
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        
        SELECT 0;
    END CATCH
END