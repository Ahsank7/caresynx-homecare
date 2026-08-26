CREATE TABLE [dbo].[tblOrganizationPackage] (
    [Id]                      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [OrganizationId]          UNIQUEIDENTIFIER NOT NULL,
    [PackageId]               UNIQUEIDENTIFIER NOT NULL,
    [PerClientCharge]         DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [InitialOneTimeCost]      DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [InfrastructureCost]      DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [SupportCharges]          DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [NewFeatureReportCharges] DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [StartDate]               DATETIME2 (7)    DEFAULT (sysutcdatetime()) NOT NULL,
    [EndDate]                 DATETIME2 (7)    NULL,
    [IsActive]                BIT              DEFAULT ((1)) NOT NULL,
    [CreatedDate]             DATETIME2 (7)    DEFAULT (sysutcdatetime()) NOT NULL,
    [CreatedBy]               UNIQUEIDENTIFIER NULL,
    [Notes]                   NVARCHAR (500)   NULL,
    CONSTRAINT [PK_tblOrganizationPackage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblOrganizationPackage_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblOrganizationPackage_Package] FOREIGN KEY ([PackageId]) REFERENCES [dbo].[tblPackage] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationPackage_PackageId]
    ON [dbo].[tblOrganizationPackage]([PackageId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationPackage_OrganizationId]
    ON [dbo].[tblOrganizationPackage]([OrganizationId] ASC)
    INCLUDE([PackageId], [StartDate], [EndDate], [IsActive]);

