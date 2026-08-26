CREATE TABLE [dbo].[tblBillingInvoiceDetail] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [BillingInvoiceId] INT             NULL,
    [TaskId]           INT             NULL,
    [Amount]           DECIMAL (18, 2) NULL,
    [ExpenseId]        UNIQUEIDENTIFIER NULL,
    [ExpenseAmount]    DECIMAL (18, 2) NULL,
    [LineRole]         TINYINT          NULL CONSTRAINT [DF_tblBillingInvoiceDetail_LineRole] DEFAULT ((1)),
    CONSTRAINT [PK_tblBillingInvoiceDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
-- LineRole: 1 = client/household responsibility, 2 = third-party payer responsibility (funded portion)

