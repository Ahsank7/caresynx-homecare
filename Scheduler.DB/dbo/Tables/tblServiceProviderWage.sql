CREATE TABLE [dbo].[tblServiceProviderWage] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [Row_Guid]            UNIQUEIDENTIFIER NULL,
    [Description]         NVARCHAR (500)   NULL,
    [TotalAmount]         DECIMAL (18, 2)  NULL,
    [Date]                DATE             NULL,
    [StartDate]           DATE             NULL,
    [EndDate]             DATE             NULL,
    [DueDate]             DATE             NULL,
    [IsPaid]              BIT              NULL,
    [TransactionId]       NVARCHAR (500)   NULL,
    [ServiceProviderId]   UNIQUEIDENTIFIER NULL,
    [IsManualPayment]     BIT              DEFAULT ((0)) NULL,
    [ManualPaymentReason] NVARCHAR (500)   NULL,
    [PaymentDate]         DATETIME2 (7)    NULL,
    CONSTRAINT [PK_tblServiceProviderWage] PRIMARY KEY CLUSTERED ([Id] ASC)
);

