-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Soft delete a client preference
-- =============================================
CREATE PROCEDURE [dbo].[uspDeleteClientPreference]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[tblClientPreferences]
    SET 
        [IsActive] = 0,
        [UpdatedDate] = GETDATE(),
        [UpdatedBy] = @UserId
    WHERE [Id] = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END

