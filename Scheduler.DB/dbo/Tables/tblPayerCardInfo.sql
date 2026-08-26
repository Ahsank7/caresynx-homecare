-- Encrypted card on file for organization payers (auto-charge when BillToType = organization payer).
CREATE TABLE [dbo].[tblPayerCardInfo] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [CardId]         UNIQUEIDENTIFIER NOT NULL,
    [PayerId]        UNIQUEIDENTIFIER NOT NULL,
    [CardHolderName] VARCHAR (100)    NOT NULL,
    [CardNumber]     NVARCHAR (500)   NOT NULL,
    [ExpiryMonth]    TINYINT          NOT NULL,
    [ExpiryYear]     SMALLINT         NOT NULL,
    [CVV]            NVARCHAR (100)   NOT NULL,
    [TypeId]         INT              NOT NULL,
    [CreatedAt]      DATETIME         CONSTRAINT [DF_tblPayerCardInfo_CreatedAt] DEFAULT (GETUTCDATE()) NULL,
    CONSTRAINT [PK_tblPayerCardInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[tblPayerCardInfo] WITH NOCHECK
    ADD CONSTRAINT [FK_tblPayerCardInfo_tblPayer] FOREIGN KEY ([PayerId]) REFERENCES [dbo].[tblPayer] ([Id]) ON DELETE CASCADE;
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_tblPayerCardInfo_PayerId]
    ON [dbo].[tblPayerCardInfo]([PayerId] ASC);
