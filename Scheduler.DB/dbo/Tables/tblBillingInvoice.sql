CREATE TABLE [dbo].[tblBillingInvoice] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [Row_Guid]            UNIQUEIDENTIFIER NULL,
    [Details]             NVARCHAR (500)   NULL,
    [TotalAmount]         DECIMAL (18, 2)  CONSTRAINT [DF_tblBillingInvoice_Amount] DEFAULT ((0)) NULL,
    [Date]                DATE             NULL,
    [StartDate]           DATE             NULL,
    [EndDate]             DATE             NULL,
    [DueDate]             DATE             NULL,
    [IsPaid]              BIT              CONSTRAINT [DF_tblBillingInvoice_IsPaid] DEFAULT ((0)) NULL,
    [ClientId]            UNIQUEIDENTIFIER NULL,
    [DiscountPercentage]  DECIMAL (18, 2)  NULL,
    [AmountAfterDiscount] DECIMAL (18, 2)  NULL,
    [TaxPercentage]       DECIMAL (18, 2)  NULL,
    [AmountAfterTax]      DECIMAL (18, 2)  NULL,
    [TransactionId]       NVARCHAR (500)   NULL,
    [IsManualPayment]     BIT              DEFAULT ((0)) NULL,
    [ManualPaymentReason] NVARCHAR (500)   NULL,
    [PaymentDate]         DATETIME2 (7)    NULL,
    [BillToType]         TINYINT         NULL,
    [BillToPayerId]      UNIQUEIDENTIFIER NULL,
    [BillToUserContactId] UNIQUEIDENTIFIER NULL,
    [BillToDisplayName]  NVARCHAR (200)  NULL,
    [DebtorEmail]         NVARCHAR (200)  NULL,
    CONSTRAINT [PK_tblBillingInvoice] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
ALTER TABLE [dbo].[tblBillingInvoice] WITH NOCHECK
    ADD CONSTRAINT [FK_tblBillingInvoice_tblPayer] FOREIGN KEY ([BillToPayerId]) REFERENCES [dbo].[tblPayer] ([Id]);

