CREATE PROCEDURE [dbo].[GetCurrentOrganizationPackage]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1
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
      AND op.IsActive = 1
      AND op.EndDate IS NULL
    ORDER BY op.StartDate DESC;
END