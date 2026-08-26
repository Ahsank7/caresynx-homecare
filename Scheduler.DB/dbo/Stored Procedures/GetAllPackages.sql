CREATE PROCEDURE [dbo].[GetAllPackages]
    @pIncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        PerClientCharge,
        InitialOneTimeCost,
        InfrastructureCost,
        SupportCharges,
        NewFeatureReportCharges,
        IsActive,
        CreatedDate,
        UpdatedDate
    FROM [dbo].[tblPackage]
    WHERE (@pIncludeInactive = 1 OR IsActive = 1)
    ORDER BY Name;
    
    -- Return total count as a second result set
    SELECT COUNT(*) AS TotalRecords 
    FROM [dbo].[tblPackage]
    WHERE (@pIncludeInactive = 1 OR IsActive = 1);
END