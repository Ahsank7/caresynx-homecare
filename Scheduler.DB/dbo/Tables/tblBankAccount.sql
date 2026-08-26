CREATE TABLE [dbo].[tblBankAccount] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [BankAccountId]     UNIQUEIDENTIFIER NULL,
    [UserId]            UNIQUEIDENTIFIER NOT NULL,
    [AccountHolderName] VARCHAR (100)    NOT NULL,
    [AccountNumber]     NVARCHAR (500)   NOT NULL,
    [BankId]            VARCHAR (50)     NOT NULL,
    [BranchCode]        VARCHAR (50)     NOT NULL,
    [IBAN]              VARCHAR (50)     NOT NULL,
    [ConnectedAccountId] NVARCHAR (255)  NULL,
    [CreatedAt]         DATETIME         DEFAULT (getutcdate()) NULL,
    [ModifiedDate]      DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Add unique constraint to ensure one bank account per user
ALTER TABLE [dbo].[tblBankAccount] 
ADD CONSTRAINT [UQ_tblBankAccount_UserId] UNIQUE ([UserId]);
GO

-- Add unique constraint to ensure IBAN is unique across all users
ALTER TABLE [dbo].[tblBankAccount] 
ADD CONSTRAINT [UQ_tblBankAccount_IBAN] UNIQUE ([IBAN]);
GO

-- Add index on UserId for better performance
CREATE NONCLUSTERED INDEX [IX_tblBankAccount_UserId] ON [dbo].[tblBankAccount] ([UserId]);
GO

-- Add index on IBAN for better performance
CREATE NONCLUSTERED INDEX [IX_tblBankAccount_IBAN] ON [dbo].[tblBankAccount] ([IBAN]);
GO

