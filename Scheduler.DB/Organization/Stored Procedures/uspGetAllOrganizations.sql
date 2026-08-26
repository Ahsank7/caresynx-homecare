CREATE PROCEDURE [Organization].[uspGetAllOrganizations]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all organizations including inactive ones (for super admin)
    SELECT 
        Id,
        Name,
        Description,
        IsActive
    INTO #FinalResults
    FROM [dbo].[tblOrganization]
    
    -- First result set: the data
    SELECT * FROM #FinalResults
    ORDER BY Name;
    
    -- Second result set: the count (required by GetAll method)
    SELECT COUNT(*) TotalRecords FROM #FinalResults
END