-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Create a new complaint
-- =============================================
CREATE PROCEDURE [dbo].[uspCreateComplaint]
    @ComplainantId UNIQUEIDENTIFIER,
    @ComplainantType INT,
    @ComplainedAgainstId UNIQUEIDENTIFIER,
    @ComplainedAgainstType INT,
    @FranchiseId UNIQUEIDENTIFIER = NULL,
    @Title NVARCHAR(200),
    @Description NVARCHAR(2000),
    @Category INT = NULL,
    @Severity INT = NULL,
    @Status INT = NULL,
    @CreatedBy UNIQUEIDENTIFIER = NULL,
    @ComplaintId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Generate new ID
    SET @ComplaintId = NEWID();

    -- Set default status if not provided (Submitted)
    IF @Status IS NULL
        SET @Status = (SELECT Id FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'Submitted');

    INSERT INTO [dbo].[tblComplaint] (
        [Id],
        [ComplainantId],
        [ComplainantType],
        [ComplainedAgainstId],
        [ComplainedAgainstType],
        [FranchiseId],
        [Title],
        [Description],
        [Category],
        [Severity],
        [Status],
        [CreatedDate],
        [CreatedBy],
        [IsActive]
    )
    VALUES (
        @ComplaintId,
        @ComplainantId,
        @ComplainantType,
        @ComplainedAgainstId,
        @ComplainedAgainstType,
        @FranchiseId,
        @Title,
        @Description,
        @Category,
        @Severity,
        @Status,
        GETDATE(),
        @CreatedBy,
        1
    );

    -- Return the created complaint
    EXEC [dbo].[uspGetComplaintById] @ComplaintId;
END

