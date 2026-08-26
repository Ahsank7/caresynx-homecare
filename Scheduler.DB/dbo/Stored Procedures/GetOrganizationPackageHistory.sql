CREATE PROCEDURE [dbo].[GetOrganizationPackageHistory]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        op.Id,
        op.OrganizationId,
        op.PackageId,
        p.Name AS PackageName,
        p.Description AS PackageDescription,
        -- Use snapshot values from organization package (allows customization)
        op.PerClientCharge,
        op.InitialOneTimeCost,
        op.InfrastructureCost,
        op.SupportCharges,
        op.NewFeatureReportCharges,
        op.StartDate,
        op.EndDate,
        op.IsActive,
        op.CreatedDate,
        op.Notes
    FROM [dbo].[tblOrganizationPackage] op
    INNER JOIN [dbo].[tblPackage] p ON p.Id = op.PackageId
    WHERE op.OrganizationId = @pOrganizationId
    ORDER BY op.StartDate DESC;
END