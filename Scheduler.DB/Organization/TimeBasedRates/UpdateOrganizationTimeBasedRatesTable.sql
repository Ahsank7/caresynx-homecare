-- Add ServiceTypeId column to OrganizationTimeBasedRates table
ALTER TABLE [dbo].[OrganizationTimeBasedRates]
ADD ServiceTypeId INT NULL;

-- Add foreign key constraint for ServiceTypeId
ALTER TABLE [dbo].[OrganizationTimeBasedRates]
ADD CONSTRAINT FK_OrganizationTimeBasedRates_ServiceType 
FOREIGN KEY (ServiceTypeId) REFERENCES [dbo].[tblServicesType](Id);

-- Change ServiceId from UNIQUEIDENTIFIER to INT
-- First, drop the existing foreign key constraint
ALTER TABLE [dbo].[OrganizationTimeBasedRates]
DROP CONSTRAINT FK_OrganizationTimeBasedRates_Service;

-- Add new foreign key constraint for ServiceId (INT)
ALTER TABLE [dbo].[OrganizationTimeBasedRates]
ADD CONSTRAINT FK_OrganizationTimeBasedRates_Service 
FOREIGN KEY (ServiceId) REFERENCES [dbo].[tblServices](Id);

-- Rename table from OrganizationTimeBasedRates to tblOrganizationTimeBasedRates
EXEC sp_rename 'OrganizationTimeBasedRates', 'tblOrganizationTimeBasedRates';

-- Update the unique constraint to include ServiceTypeId
ALTER TABLE [dbo].[tblOrganizationTimeBasedRates]
DROP CONSTRAINT UQ_OrganizationTimeBasedRates_Overlap;

ALTER TABLE [dbo].[tblOrganizationTimeBasedRates]
ADD CONSTRAINT UQ_tblOrganizationTimeBasedRates_Overlap 
UNIQUE (OrganizationId, ServiceTypeId, ServiceId, DayOfWeek, StartTime, EndTime);
