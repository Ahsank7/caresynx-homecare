-- Migration Script: Add Bank Account Constraints
-- This script ensures users can only have one bank account and IBANs are unique
-- Run this script on existing databases to add the constraints

USE [YourDatabaseName] -- Replace with your actual database name
GO

-- Check if constraints already exist to avoid errors
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UQ_tblBankAccount_UserId]') AND type in (N'UQ'))
BEGIN
    -- Add unique constraint to ensure one bank account per user
    ALTER TABLE [dbo].[tblBankAccount] 
    ADD CONSTRAINT [UQ_tblBankAccount_UserId] UNIQUE ([UserId]);
    
    PRINT 'Added unique constraint on UserId - one bank account per user enforced';
END
ELSE
BEGIN
    PRINT 'Unique constraint on UserId already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UQ_tblBankAccount_IBAN]') AND type in (N'UQ'))
BEGIN
    -- Add unique constraint to ensure IBAN is unique across all users
    ALTER TABLE [dbo].[tblBankAccount] 
    ADD CONSTRAINT [UQ_tblBankAccount_IBAN] UNIQUE ([IBAN]);
    
    PRINT 'Added unique constraint on IBAN - unique IBAN across all users enforced';
END
ELSE
BEGIN
    PRINT 'Unique constraint on IBAN already exists';
END
GO

-- Add ModifiedDate column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[tblBankAccount]') AND name = 'ModifiedDate')
BEGIN
    ALTER TABLE [dbo].[tblBankAccount] 
    ADD [ModifiedDate] DATETIME NULL;
    
    PRINT 'Added ModifiedDate column to tblBankAccount';
END
ELSE
BEGIN
    PRINT 'ModifiedDate column already exists';
END
GO

-- Check if indexes already exist to avoid errors
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tblBankAccount]') AND name = 'IX_tblBankAccount_UserId')
BEGIN
    -- Add index on UserId for better performance
    CREATE NONCLUSTERED INDEX [IX_tblBankAccount_UserId] ON [dbo].[tblBankAccount] ([UserId]);
    
    PRINT 'Added index on UserId for better performance';
END
ELSE
BEGIN
    PRINT 'Index on UserId already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[tblBankAccount]') AND name = 'IX_tblBankAccount_IBAN')
BEGIN
    -- Add index on IBAN for better performance
    CREATE NONCLUSTERED INDEX [IX_tblBankAccount_IBAN] ON [dbo].[tblBankAccount] ([IBAN]);
    
    PRINT 'Added index on IBAN for better performance';
END
ELSE
BEGIN
    PRINT 'Index on IBAN already exists';
END
GO

-- Check for duplicate users (users with multiple bank accounts) before adding constraints
PRINT 'Checking for users with multiple bank accounts...';
SELECT 
    [UserId],
    COUNT(*) as BankAccountCount
FROM [dbo].[tblBankAccount]
GROUP BY [UserId]
HAVING COUNT(*) > 1;
GO

-- Check for duplicate IBANs before adding constraints
PRINT 'Checking for duplicate IBANs...';
SELECT 
    [IBAN],
    COUNT(*) as IBANCount
FROM [dbo].[tblBankAccount]
GROUP BY [IBAN]
HAVING COUNT(*) > 1;
GO

PRINT 'Migration completed. Please review the duplicate checks above and resolve any conflicts before the constraints will work properly.';
GO
