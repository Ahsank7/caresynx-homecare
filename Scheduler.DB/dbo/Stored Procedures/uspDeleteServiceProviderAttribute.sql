-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Soft delete a service provider attribute
-- =============================================
CREATE PROCEDURE [dbo].[uspDeleteServiceProviderAttribute]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[tblServiceProviderAttributes]
    SET 
        [IsActive] = 0,
        [UpdatedDate] = GETDATE(),
        [UpdatedBy] = @UserId
    WHERE [Id] = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END

