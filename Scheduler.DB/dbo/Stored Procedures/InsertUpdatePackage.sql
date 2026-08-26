CREATE PROCEDURE [dbo].[InsertUpdatePackage]
    @pId UNIQUEIDENTIFIER = NULL,
    @pOutId UNIQUEIDENTIFIER = NULL OUTPUT,
    @pName NVARCHAR(100),
    @pDescription NVARCHAR(500) = NULL,
    @pPerClientCharge DECIMAL(18, 2) = 0,
    @pInitialOneTimeCost DECIMAL(18, 2) = 0,
    @pInfrastructureCost DECIMAL(18, 2) = 0,
    @pSupportCharges DECIMAL(18, 2) = 0,
    @pNewFeatureReportCharges DECIMAL(18, 2) = 0,
    @pIsActive BIT = 1,
    @pCreatedBy UNIQUEIDENTIFIER = NULL,
    @pUpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @pId IS NULL
    BEGIN
        SET @pOutId = NEWID();
        
        INSERT INTO [dbo].[tblPackage]
            (Id, Name, Description, PerClientCharge, InitialOneTimeCost, 
             InfrastructureCost, SupportCharges, NewFeatureReportCharges, 
             IsActive, CreatedDate, CreatedBy)
        VALUES
            (@pOutId, @pName, @pDescription, @pPerClientCharge, @pInitialOneTimeCost,
             @pInfrastructureCost, @pSupportCharges, @pNewFeatureReportCharges,
             @pIsActive, SYSUTCDATETIME(), @pCreatedBy);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[tblPackage]
        SET Name = @pName,
            Description = @pDescription,
            PerClientCharge = @pPerClientCharge,
            InitialOneTimeCost = @pInitialOneTimeCost,
            InfrastructureCost = @pInfrastructureCost,
            SupportCharges = @pSupportCharges,
            NewFeatureReportCharges = @pNewFeatureReportCharges,
            IsActive = @pIsActive,
            UpdatedDate = SYSUTCDATETIME(),
            UpdatedBy = @pUpdatedBy
        WHERE Id = @pId;
        
        SET @pOutId = @pId;
    END
END