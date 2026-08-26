-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Get a specific complaint by ID
-- =============================================
CREATE PROCEDURE [dbo].[uspGetComplaintById]
    @ComplaintId UNIQUEIDENTIFIER
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
        U1.[UserType] AS ComplainantUserType,
        
        -- Complained Against information
        U2.[FirstName] + ' ' + ISNULL(U2.[SurName], '') + ' ' + ISNULL(U2.[LastName], '') AS ComplainedAgainstName,
        U2.[Email] AS ComplainedAgainstEmail,
        U2.[UserType] AS ComplainedAgainstUserType,
        
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
    WHERE C.[Id] = @ComplaintId;
END