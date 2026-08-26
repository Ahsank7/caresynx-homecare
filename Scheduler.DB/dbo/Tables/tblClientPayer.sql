CREATE TABLE [dbo].[tblClientPayer] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [ClientId]       UNIQUEIDENTIFIER NOT NULL,
    [PayerId]        UNIQUEIDENTIFIER NOT NULL,
    [EffectiveFrom]  DATE             NOT NULL,
    [EffectiveTo]    DATE             NULL,
    [IsDefaultBillTo] BIT              NOT NULL CONSTRAINT [DF_tblClientPayer_IsDefaultBillTo] DEFAULT ((0)),
    [MemberNumber]   NVARCHAR (100)   NULL,
    [PolicyNumber]   NVARCHAR (100)   NULL,
    [Notes]          NVARCHAR (500)   NULL,
    [IsActive]       BIT              NOT NULL CONSTRAINT [DF_tblClientPayer_IsActive] DEFAULT ((1)),
    CONSTRAINT [PK_tblClientPayer] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

ALTER TABLE [dbo].[tblClientPayer] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientPayer_tblUser] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[tblUser] ([Id]);

GO

ALTER TABLE [dbo].[tblClientPayer] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientPayer_tblPayer] FOREIGN KEY ([PayerId]) REFERENCES [dbo].[tblPayer] ([Id]);

GO

CREATE NONCLUSTERED INDEX [IX_tblClientPayer_ClientId]
    ON [dbo].[tblClientPayer]([ClientId] ASC) INCLUDE ([PayerId], [IsDefaultBillTo], [IsActive], [EffectiveFrom], [EffectiveTo]);
