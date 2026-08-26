CREATE TABLE [dbo].[tblPackage] (
    [Id]                      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Name]                    NVARCHAR (100)   NOT NULL,
    [Description]             NVARCHAR (500)   NULL,
    [PerClientCharge]         DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [InitialOneTimeCost]      DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [InfrastructureCost]      DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [SupportCharges]          DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [NewFeatureReportCharges] DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [IsActive]                BIT              DEFAULT ((1)) NOT NULL,
    [CreatedDate]             DATETIME2 (7)    DEFAULT (sysutcdatetime()) NOT NULL,
    [UpdatedDate]             DATETIME2 (7)    NULL,
    [CreatedBy]               UNIQUEIDENTIFIER NULL,
    [UpdatedBy]               UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblPackage] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblPackage_IsActive]
    ON [dbo].[tblPackage]([IsActive] ASC);

