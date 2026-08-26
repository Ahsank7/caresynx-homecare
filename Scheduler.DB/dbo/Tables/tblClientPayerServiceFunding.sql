-- Rule-based funded portion of a visit; applied after base BillingAmount is calculated.
CREATE TABLE [dbo].[tblClientPayerServiceFunding] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [ClientId]       UNIQUEIDENTIFIER NOT NULL,
    [PayerId]         UNIQUEIDENTIFIER NOT NULL,
    [ServiceId]     INT              NULL,
    [FundedPercent] DECIMAL (5, 2)  NOT NULL,
    [EffectiveFrom]  DATE             NOT NULL,
    [EffectiveTo]    DATE             NULL,
    [IsActive]       BIT              NOT NULL CONSTRAINT [DF_tblClientPayerServiceFunding_IsActive] DEFAULT ((1)),
    CONSTRAINT [PK_tblClientPayerServiceFunding] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_tblClientPayerServiceFunding_FundedPercent] CHECK ([FundedPercent] >= 0 AND [FundedPercent] <= 100)
);

GO

ALTER TABLE [dbo].[tblClientPayerServiceFunding] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientPayerServiceFunding_tblOrg] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]);

GO

ALTER TABLE [dbo].[tblClientPayerServiceFunding] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientPayerServiceFunding_tblUser] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[tblUser] ([Id]);

GO

ALTER TABLE [dbo].[tblClientPayerServiceFunding] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientPayerServiceFunding_tblPayer] FOREIGN KEY ([PayerId]) REFERENCES [dbo].[tblPayer] ([Id]);

GO

CREATE NONCLUSTERED INDEX [IX_tblClientPayerServiceFunding_Lookup]
    ON [dbo].[tblClientPayerServiceFunding]([ClientId] ASC, [OrganizationId] ASC) INCLUDE ([PayerId], [ServiceId], [FundedPercent], [EffectiveFrom], [EffectiveTo], [IsActive]);
