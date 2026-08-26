CREATE TABLE [dbo].[tblFranchise] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (50)    NULL,
    [IsActive]           BIT              NULL,
    [Description]        NVARCHAR (500)   NULL,
    [logo]               IMAGE            NULL,
    [OrganizationId]     UNIQUEIDENTIFIER NULL,
    [DefaultBillingRate] DECIMAL (18, 2)  CONSTRAINT [DF_tblFranchise_DefaultBillingRate] DEFAULT ((0)) NULL,
    [DefaultWageRate]    DECIMAL (18, 2)  CONSTRAINT [DF_tblFranchise_DefaultWageRate] DEFAULT ((0)) NULL,
    [Currency]           NVARCHAR (50)    NULL,
    [CalculationTypeId]  INT              NULL,
    CONSTRAINT [PK_tblFranchise_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblFranchise_OrganizationId]
    ON [dbo].[tblFranchise]([OrganizationId] ASC)
    INCLUDE([Id], [IsActive]);

