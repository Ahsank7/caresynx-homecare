CREATE TABLE [dbo].[tblServiceProviderWageDetail] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [ServiceProviderWageId] INT             NULL,
    [TaskId]                INT             NULL,
    [Amount]                DECIMAL (18, 2) NULL,
    [ExpenseId]             UNIQUEIDENTIFIER NULL,
    [ExpenseAmount]         DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_tblServiceProviderWageDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

