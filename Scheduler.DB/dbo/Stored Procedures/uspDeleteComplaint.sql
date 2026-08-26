-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Soft delete a complaint (set IsActive = 0)
-- =============================================
CREATE PROCEDURE [dbo].[uspDeleteComplaint]
    @ComplaintId UNIQUEIDENTIFIER,
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if complaint exists
    IF NOT EXISTS (SELECT 1 FROM [dbo].[tblComplaint] WHERE [Id] = @ComplaintId)
    BEGIN
        RAISERROR ('Complaint not found', 16, 1);
        RETURN;
    END

    -- Soft delete
    UPDATE [dbo].[tblComplaint]
    SET 
        [IsActive] = 0,
        [UpdatedDate] = GETDATE(),
        [UpdatedBy] = @UpdatedBy
    WHERE [Id] = @ComplaintId;

    SELECT 1 AS Success;
END

