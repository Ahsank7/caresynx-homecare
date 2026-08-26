CREATE TABLE [dbo].[tblPackageInvoice] (
    [Id]                      INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationId]          UNIQUEIDENTIFIER NOT NULL,
    [OrganizationPackageId]   UNIQUEIDENTIFIER NOT NULL,
    [InvoiceDate]             DATETIME2 (7)    DEFAULT (sysutcdatetime()) NOT NULL,
    [BillingPeriodStart]      DATETIME2 (7)    NOT NULL,
    [BillingPeriodEnd]        DATETIME2 (7)    NOT NULL,
    [PerClientCharge]         DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [ClientCount]             INT              DEFAULT ((0)) NOT NULL,
    [InitialOneTimeCost]      DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [InfrastructureCost]      DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [SupportCharges]          DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [NewFeatureReportCharges] DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [SubTotal]                DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [TaxAmount]               DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [TotalAmount]             DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [IsInitialCharge]         BIT              DEFAULT ((0)) NOT NULL,
    [PaymentStatus]           NVARCHAR (50)    DEFAULT ('Pending') NOT NULL,
    [PaymentDate]             DATETIME2 (7)    NULL,
    [PaymentTransactionId]    NVARCHAR (200)   NULL,
    [PaymentFailureReason]    NVARCHAR (500)   NULL,
    [InvoiceNumber]           NVARCHAR (50)    NOT NULL,
    [InvoiceDocumentUrl]      NVARCHAR (1000)  NULL,
    [CreatedDate]             DATETIME2 (7)    DEFAULT (sysutcdatetime()) NOT NULL,
    CONSTRAINT [PK_tblPackageInvoice] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblPackageInvoice_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]),
    CONSTRAINT [FK_tblPackageInvoice_OrganizationPackage] FOREIGN KEY ([OrganizationPackageId]) REFERENCES [dbo].[tblOrganizationPackage] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblPackageInvoice_BillingPeriod]
    ON [dbo].[tblPackageInvoice]([BillingPeriodStart] ASC, [BillingPeriodEnd] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblPackageInvoice_OrganizationId]
    ON [dbo].[tblPackageInvoice]([OrganizationId] ASC)
    INCLUDE([InvoiceDate], [PaymentStatus]);

