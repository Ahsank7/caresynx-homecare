-- Deploy Updated Time-Based Rates Implementation
-- This script includes table updates, stored procedure updates, and data migration

-- Step 1: Add ServiceTypeId column to existing table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrganizationTimeBasedRates') AND name = 'ServiceTypeId')
BEGIN
    ALTER TABLE [dbo].[OrganizationTimeBasedRates]
    ADD ServiceTypeId INT NULL;
    
    PRINT 'Added ServiceTypeId column to OrganizationTimeBasedRates table';
END

-- Step 2: Add foreign key constraint for ServiceTypeId
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrganizationTimeBasedRates_ServiceType')
BEGIN
    ALTER TABLE [dbo].[OrganizationTimeBasedRates]
    ADD CONSTRAINT FK_OrganizationTimeBasedRates_ServiceType 
    FOREIGN KEY (ServiceTypeId) REFERENCES [dbo].[tblServicesType](Id);
    
    PRINT 'Added foreign key constraint for ServiceTypeId';
END

-- Step 3: Update ServiceId foreign key constraint to reference tblServices instead of tblService
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrganizationTimeBasedRates_Service')
BEGIN
    ALTER TABLE [dbo].[OrganizationTimeBasedRates]
    DROP CONSTRAINT FK_OrganizationTimeBasedRates_Service;
    
    ALTER TABLE [dbo].[OrganizationTimeBasedRates]
    ADD CONSTRAINT FK_OrganizationTimeBasedRates_Service 
    FOREIGN KEY (ServiceId) REFERENCES [dbo].[tblServices](Id);
    
    PRINT 'Updated ServiceId foreign key constraint to reference tblServices';
END

-- Step 4: Rename table from OrganizationTimeBasedRates to tblOrganizationTimeBasedRates
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'OrganizationTimeBasedRates')
BEGIN
    EXEC sp_rename 'OrganizationTimeBasedRates', 'tblOrganizationTimeBasedRates';
    PRINT 'Renamed OrganizationTimeBasedRates to tblOrganizationTimeBasedRates';
END

-- Step 5: Update unique constraint to include ServiceTypeId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_OrganizationTimeBasedRates_Overlap')
BEGIN
    ALTER TABLE [dbo].[tblOrganizationTimeBasedRates]
    DROP CONSTRAINT UQ_OrganizationTimeBasedRates_Overlap;
    
    ALTER TABLE [dbo].[tblOrganizationTimeBasedRates]
    ADD CONSTRAINT UQ_tblOrganizationTimeBasedRates_Overlap 
    UNIQUE (OrganizationId, ServiceTypeId, ServiceId, DayOfWeek, StartTime, EndTime);
    
    PRINT 'Updated unique constraint to include ServiceTypeId';
END

-- Step 6: Create/Update stored procedures
PRINT 'Creating/Updating stored procedures...';

-- Create uspGetOrganizationBillingSettings
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'uspGetOrganizationBillingSettings')
    DROP PROCEDURE [dbo].[uspGetOrganizationBillingSettings];
GO

CREATE PROCEDURE [dbo].[uspGetOrganizationBillingSettings]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id AS OrganizationId,
        Name AS OrganizationName,
        ServiceRateForBilling,
        DefaultBillingRate,
        DefaultWageRate
    FROM [dbo].[tblOrganization]
    WHERE Id = @pOrganizationId;
END
GO

-- Create uspUpdateOrganizationBillingSettings
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'uspUpdateOrganizationBillingSettings')
    DROP PROCEDURE [dbo].[uspUpdateOrganizationBillingSettings];
GO

CREATE PROCEDURE [dbo].[uspUpdateOrganizationBillingSettings]
    @pOrganizationId UNIQUEIDENTIFIER,
    @pServiceRateForBilling INT,
    @pDefaultBillingRate DECIMAL(10,2),
    @pDefaultWageRate DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[tblOrganization]
    SET 
        ServiceRateForBilling = @pServiceRateForBilling,
        DefaultBillingRate = @pDefaultBillingRate,
        DefaultWageRate = @pDefaultWageRate,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @pOrganizationId;
END
GO

-- Create uspDeleteOrganizationTimeBasedRatesByOrganization
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'uspDeleteOrganizationTimeBasedRatesByOrganization')
    DROP PROCEDURE [dbo].[uspDeleteOrganizationTimeBasedRatesByOrganization];
GO

CREATE PROCEDURE [dbo].[uspDeleteOrganizationTimeBasedRatesByOrganization]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[tblOrganizationTimeBasedRates]
    WHERE OrganizationId = @pOrganizationId;
END
GO

PRINT 'Deployment completed successfully!';
PRINT 'Table renamed to tblOrganizationTimeBasedRates';
PRINT 'ServiceTypeId column added';
PRINT 'All stored procedures updated';
