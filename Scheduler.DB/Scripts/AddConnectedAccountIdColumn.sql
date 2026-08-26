-- =============================================
-- Script to add ConnectedAccountId column to tblBankAccount
-- Run this script on existing databases to add the new column
-- =============================================

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'tblBankAccount' 
               AND COLUMN_NAME = 'ConnectedAccountId')
BEGIN
    -- Add the new column
    ALTER TABLE [dbo].[tblBankAccount] 
    ADD [ConnectedAccountId] NVARCHAR(255) NULL;
    
    PRINT 'ConnectedAccountId column added successfully to tblBankAccount table.';
END
ELSE
BEGIN
    PRINT 'ConnectedAccountId column already exists in tblBankAccount table.';
END

-- Update existing stored procedures if they don't have the new parameter
-- Note: This is a safety check. The stored procedures should be updated separately.

PRINT 'Script completed successfully.';
PRINT 'Please ensure that the stored procedures are also updated:';
PRINT '1. InserUpdateUserBankAccountInfo - Add @pConnectedAccountId parameter';
PRINT '2. GetUserBankAccountInfo - Include ConnectedAccountId in SELECT';
PRINT '3. uspGetPaymentData - Include ba.ConnectedAccountId in WAGE section';
