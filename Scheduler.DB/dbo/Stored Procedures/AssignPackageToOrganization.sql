CREATE PROCEDURE [dbo].[AssignPackageToOrganization]
    @pOrganizationId UNIQUEIDENTIFIER,
    @pPackageId UNIQUEIDENTIFIER,
    @pStartDate DATETIME2 = NULL,
    @pCreatedBy UNIQUEIDENTIFIER = NULL,
    @pNotes NVARCHAR(500) = NULL,
    @pOutId UNIQUEIDENTIFIER = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @vStartDate DATETIME2 = ISNULL(@pStartDate, SYSUTCDATETIME());
    DECLARE @vPerClientCharge DECIMAL(18, 2)
    DECLARE @vInitialOneTimeCost DECIMAL(18, 2)
    DECLARE @vInfrastructureCost DECIMAL(18, 2)
    DECLARE @vSupportCharges DECIMAL(18, 2)
    DECLARE @vNewFeatureReportCharges DECIMAL(18, 2)
    
    -- Get package pricing values (snapshot at assignment time)
    SELECT 
        @vPerClientCharge = PerClientCharge,
        @vInitialOneTimeCost = InitialOneTimeCost,
        @vInfrastructureCost = InfrastructureCost,
        @vSupportCharges = SupportCharges,
        @vNewFeatureReportCharges = NewFeatureReportCharges
    FROM [dbo].[tblPackage]
    WHERE Id = @pPackageId;
    
    -- End the current active package assignment if exists
    UPDATE [dbo].[tblOrganizationPackage]
    SET EndDate = @vStartDate,
        IsActive = 0
    WHERE OrganizationId = @pOrganizationId
      AND IsActive = 1
      AND EndDate IS NULL;
    
    -- Create new package assignment with pricing snapshot
    SET @pOutId = NEWID();
    
    INSERT INTO [dbo].[tblOrganizationPackage]
        (Id, OrganizationId, PackageId, PerClientCharge, InitialOneTimeCost, 
         InfrastructureCost, SupportCharges, NewFeatureReportCharges,
         StartDate, EndDate, IsActive, CreatedDate, CreatedBy, Notes)
    VALUES
        (@pOutId, @pOrganizationId, @pPackageId, 
         @vPerClientCharge, @vInitialOneTimeCost, @vInfrastructureCost, 
         @vSupportCharges, @vNewFeatureReportCharges,
         @vStartDate, NULL, 1, SYSUTCDATETIME(), @pCreatedBy, @pNotes);
END