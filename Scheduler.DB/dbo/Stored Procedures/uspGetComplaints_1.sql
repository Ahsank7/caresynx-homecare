-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Get complaints with filtering options
-- =============================================
CREATE PROCEDURE [dbo].[uspGetComplaints]
    @UserId UNIQUEIDENTIFIER = NULL,           -- Filter by user (complainant or complained against)
    @ComplainantId UNIQUEIDENTIFIER = NULL,    -- Filter by who filed the complaint
    @ComplainedAgainstId UNIQUEIDENTIFIER = NULL, -- Filter by who the complaint is about
    @FranchiseId UNIQUEIDENTIFIER = NULL,      -- Filter by franchise
    @Status INT = NULL,                         -- Filter by status
    @Category INT = NULL,                       -- Filter by category
    @Severity INT = NULL,                       -- Filter by severity
    @IncludeInactive BIT = 0                    -- Include inactive complaints
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.[Id],
        C.[ComplainantId],
        C.[ComplainantType],
        C.[ComplainedAgainstId],
        C.[ComplainedAgainstType],
        C.[FranchiseId],
        C.[Title],
        C.[Description],
        C.[Category],
        C.[Severity],
        C.[Status],
        C.[Resolution],
        C.[ResolutionDate],
        C.[ResolvedBy],
        C.[CreatedDate],
        C.[UpdatedDate],
        C.[CreatedBy],
        C.[UpdatedBy],
        C.[IsActive],
        
        -- Complainant information
        U1.[FirstName] + ' ' + ISNULL(U1.[SurName], '') + ' ' + ISNULL(U1.[LastName], '') AS ComplainantName,
        U1.[Email] AS ComplainantEmail,
        
        -- Complained Against information
        U2.[FirstName] + ' ' + ISNULL(U2.[SurName], '') + ' ' + ISNULL(U2.[LastName], '') AS ComplainedAgainstName,
        U2.[Email] AS ComplainedAgainstEmail,
        
        -- Resolver information
        U3.[FirstName] + ' ' + ISNULL(U3.[SurName], '') + ' ' + ISNULL(U3.[LastName], '') AS ResolvedByName,
        
        -- Lookup values
        LI_Cat.[Name] AS CategoryName,
        LI_Sev.[Name] AS SeverityName,
        LI_Stat.[Name] AS StatusName
        
    FROM [dbo].[tblComplaint] C
    LEFT JOIN [dbo].[tblUser] U1 ON C.[ComplainantId] = U1.[Id]
    LEFT JOIN [dbo].[tblUser] U2 ON C.[ComplainedAgainstId] = U2.[Id]
    LEFT JOIN [dbo].[tblUser] U3 ON C.[ResolvedBy] = U3.[Id]
    LEFT JOIN [dbo].[tblLookupItems] LI_Cat ON C.[Category] = LI_Cat.[Id] AND LI_Cat.[LookupType] = 'ComplaintCategory'
    LEFT JOIN [dbo].[tblLookupItems] LI_Sev ON C.[Severity] = LI_Sev.[Id] AND LI_Sev.[LookupType] = 'ComplaintSeverity'
    LEFT JOIN [dbo].[tblLookupItems] LI_Stat ON C.[Status] = LI_Stat.[Id] AND LI_Stat.[LookupType] = 'ComplaintStatus'
    WHERE 
        (@UserId IS NULL OR C.[ComplainantId] = @UserId OR C.[ComplainedAgainstId] = @UserId)
        AND (@ComplainantId IS NULL OR C.[ComplainantId] = @ComplainantId)
        AND (@ComplainedAgainstId IS NULL OR C.[ComplainedAgainstId] = @ComplainedAgainstId)
        AND (@FranchiseId IS NULL OR C.[FranchiseId] = @FranchiseId)
        AND (@Status IS NULL OR C.[Status] = @Status)
        AND (@Category IS NULL OR C.[Category] = @Category)
        AND (@Severity IS NULL OR C.[Severity] = @Severity)
        AND (C.[IsActive] = 1 OR @IncludeInactive = 1)
    ORDER BY C.[CreatedDate] DESC;
END