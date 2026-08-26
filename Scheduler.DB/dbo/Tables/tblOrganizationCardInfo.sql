CREATE TABLE [dbo].[tblOrganizationCardInfo] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [CardHolderName] VARCHAR (100)    NOT NULL,
    [CardNumber]     NVARCHAR (500)   NOT NULL,
    [ExpiryMonth]    TINYINT          NOT NULL,
    [ExpiryYear]     SMALLINT         NOT NULL,
    [CVV]            NVARCHAR (100)   NOT NULL,
    [TypeId]         INT              NOT NULL,
    [IsActive]       BIT              DEFAULT ((1)) NOT NULL,
    [CreatedAt]      DATETIME2 (7)    CONSTRAINT [DF_tblOrganizationCardInfo_CreatedAt] DEFAULT (sysutcdatetime()) NULL,
    [UpdatedAt]      DATETIME2 (7)    NULL,
    CONSTRAINT [PK_tblOrganizationCardInfo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblOrganizationCardInfo_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationCardInfo_OrganizationId]
    ON [dbo].[tblOrganizationCardInfo]([OrganizationId] ASC)
    INCLUDE([IsActive]);

