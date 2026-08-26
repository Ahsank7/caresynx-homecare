CREATE TABLE [dbo].[tblRole] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100)   NOT NULL,
    [Description]    NVARCHAR (500)   NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    [IsActive]       BIT              DEFAULT ((1)) NOT NULL,
    [CreatedDate]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NULL,
    [UpdatedDate]    DATETIME         NULL,
    [UpdatedBy]      UNIQUEIDENTIFIER NULL,
    [RoleLevel]      INT              NOT NULL,
    CONSTRAINT [PK_tblRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblRole_IsActive]
    ON [dbo].[tblRole]([IsActive] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblRole_OrganizationId]
    ON [dbo].[tblRole]([OrganizationId] ASC);

