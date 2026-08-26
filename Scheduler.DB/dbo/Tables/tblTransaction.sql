CREATE TABLE [dbo].[tblTransaction] (
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [TransactionId]   UNIQUEIDENTIFIER NULL,
    [UserId]          UNIQUEIDENTIFIER NOT NULL,
    [CardId]          UNIQUEIDENTIFIER NULL,
    [BankAccountId]   UNIQUEIDENTIFIER NULL,
    [TypeId]          INT              NOT NULL,
    [TransactionDate] DATETIME         CONSTRAINT [DF__tblTransa__Trans__7EF6D905] DEFAULT (getdate()) NULL,
    [StatusId]        INT              NOT NULL,
    [ReferenceId]     VARCHAR (50)     NULL,
    [Remarks]         VARCHAR (255)    NULL,
    [CreatedAt]       DATETIME         CONSTRAINT [DF__tblTransa__Creat__7FEAFD3E] DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK__tblTrans__3214EC07966D8597] PRIMARY KEY CLUSTERED ([Id] ASC)
);

