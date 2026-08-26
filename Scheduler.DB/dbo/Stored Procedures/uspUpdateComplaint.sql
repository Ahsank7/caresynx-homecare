-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Update an existing complaint
-- =============================================
CREATE PROCEDURE [dbo].[uspUpdateComplaint]
    @ComplaintId UNIQUEIDENTIFIER,
    @Title NVARCHAR(200) = NULL,
    @Description NVARCHAR(2000) = NULL,
    @Category INT = NULL,
    @Severity INT = NULL,
    @Status INT = NULL,
    @Resolution NVARCHAR(2000) = NULL,
    @ResolvedBy UNIQUEIDENTIFIER = NULL,
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

    -- Update complaint
    UPDATE [dbo].[tblComplaint]
    SET
        [Title] = ISNULL(@Title, [Title]),
        [Description] = ISNULL(@Description, [Description]),
        [Category] = ISNULL(@Category, [Category]),
        [Severity] = ISNULL(@Severity, [Severity]),
        [Status] = ISNULL(@Status, [Status]),
        [Resolution] = CASE WHEN @Resolution IS NOT NULL THEN @Resolution ELSE [Resolution] END,
        [ResolvedBy] = CASE WHEN @ResolvedBy IS NOT NULL THEN @ResolvedBy ELSE [ResolvedBy] END,
        [ResolutionDate] = CASE WHEN @Resolution IS NOT NULL THEN GETDATE() ELSE [ResolutionDate] END,
        [UpdatedDate] = GETDATE(),
        [UpdatedBy] = @UpdatedBy
    WHERE [Id] = @ComplaintId;

    -- Return the updated complaint
    EXEC [dbo].[uspGetComplaintById] @ComplaintId;
END

