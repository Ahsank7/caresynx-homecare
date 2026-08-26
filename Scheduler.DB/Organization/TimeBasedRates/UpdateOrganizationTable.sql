-- Update tblOrganization table to change ServiceRateForBilling from BIT to INT
-- This allows for three billing modes: 1 = Default Rate, 2 = Service-Specific Rate, 3 = Time-Based Rate

-- First, add a new column with the new data type
ALTER TABLE [dbo].[tblOrganization]
ADD [ServiceRateForBillingNew] INT NULL;

-- Update the new column with existing data (convert BIT to INT)
UPDATE [dbo].[tblOrganization]
SET [ServiceRateForBillingNew] = CASE 
    WHEN [UseServiceRateForBilling] = 1 THEN 2  -- Convert existing BIT=1 to INT=2 (Service-Specific)
    ELSE 1  -- Convert existing BIT=0 to INT=1 (Default Rate)
END;

-- Drop the old column
ALTER TABLE [dbo].[tblOrganization]
DROP COLUMN [UseServiceRateForBilling];

-- Rename the new column to the original name
EXEC sp_rename '[dbo].[tblOrganization].[ServiceRateForBillingNew]', 'ServiceRateForBilling', 'COLUMN';

-- Add default constraint
ALTER TABLE [dbo].[tblOrganization]
ADD CONSTRAINT [DF_tblOrganization_ServiceRateForBilling] DEFAULT (1) FOR [ServiceRateForBilling];

-- Add check constraint to ensure valid values
ALTER TABLE [dbo].[tblOrganization]
ADD CONSTRAINT [CK_tblOrganization_ServiceRateForBilling] 
    CHECK ([ServiceRateForBilling] IN (1, 2, 3));
