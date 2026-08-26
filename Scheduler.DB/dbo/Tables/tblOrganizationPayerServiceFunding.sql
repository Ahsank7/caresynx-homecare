-- Organization-wide default funding: which org payer funds what % of a service (or all),
-- before any client-specific override in tblClientPayerServiceFunding.
CREATE TABLE [dbo].[tblOrganizationPayerServiceFunding] (
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationId]  UNIQUEIDENTIFIER NOT NULL,
    [PayerId]         UNIQUEIDENTIFIER NOT NULL,
    [ServiceId]       INT              NULL,
    [FundedPercent]  DECIMAL (5, 2)  NOT NULL,
    [EffectiveFrom]   DATE             NOT NULL,
    [EffectiveTo]     DATE             NULL,
    [IsActive]        BIT              NOT NULL CONSTRAINT [DF_tblOrganizationPayerServiceFunding_IsActive] DEFAULT ((1)),
    CONSTRAINT [PK_tblOrganizationPayerServiceFunding] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_tblOrganizationPayerServiceFunding_FundedPercent] CHECK ([FundedPercent] >= 0 AND [FundedPercent] <= 100)
);
GO

ALTER TABLE [dbo].[tblOrganizationPayerServiceFunding] WITH NOCHECK
    ADD CONSTRAINT [FK_tblOrgPayerServiceFunding_tblOrg] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]);
GO

ALTER TABLE [dbo].[tblOrganizationPayerServiceFunding] WITH NOCHECK
    ADD CONSTRAINT [FK_tblOrgPayerServiceFunding_tblPayer] FOREIGN KEY ([PayerId]) REFERENCES [dbo].[tblPayer] ([Id]);
GO

CREATE NONCLUSTERED INDEX [IX_tblOrgPayerServiceFunding_Lookup]
    ON [dbo].[tblOrganizationPayerServiceFunding]([OrganizationId] ASC) INCLUDE ([PayerId], [ServiceId], [FundedPercent], [EffectiveFrom], [EffectiveTo], [IsActive]);
