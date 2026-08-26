CREATE TABLE [dbo].[tblPayer] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId]  UNIQUEIDENTIFIER NOT NULL,
    [LegalName]         NVARCHAR (200)   NOT NULL,
    [PayerType]         TINYINT          NOT NULL CONSTRAINT [DF_tblPayer_PayerType] DEFAULT ((0)),
    [BillingAddressLine1] NVARCHAR (200) NULL,
    [BillingAddressLine2] NVARCHAR (200) NULL,
    [BillingAddressLine3] NVARCHAR (200) NULL,
    [BillingEmail]      NVARCHAR (200)   NULL,
    [DefaultPaymentTermsDays] INT        NULL CONSTRAINT [DF_tblPayer_DefaultPaymentTermsDays] DEFAULT ((30)),
    [TaxId]             NVARCHAR (100)   NULL,
    [IsActive]          BIT              NOT NULL CONSTRAINT [DF_tblPayer_IsActive] DEFAULT ((1)),
    [CreatedDate]       DATETIME2 (7)    NOT NULL CONSTRAINT [DF_tblPayer_CreatedDate] DEFAULT (GETUTCDATE()),
    [UpdatedDate]       DATETIME2 (7)    NULL,
    CONSTRAINT [PK_tblPayer] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

ALTER TABLE [dbo].[tblPayer] WITH NOCHECK
    ADD CONSTRAINT [FK_tblPayer_tblOrganization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]);

GO

CREATE NONCLUSTERED INDEX [IX_tblPayer_OrganizationId]
    ON [dbo].[tblPayer]([OrganizationId] ASC) WHERE ([IsActive] = (1));
