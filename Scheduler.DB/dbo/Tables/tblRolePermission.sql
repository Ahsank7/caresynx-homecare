CREATE TABLE [dbo].[tblRolePermission] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RoleId]         INT              NOT NULL,
    [MenuId]         NVARCHAR (100)   NOT NULL,
    [CanView]        BIT              DEFAULT ((1)) NOT NULL,
    [CanCreate]      BIT              DEFAULT ((0)) NOT NULL,
    [CanEdit]        BIT              DEFAULT ((0)) NOT NULL,
    [CanDelete]      BIT              DEFAULT ((0)) NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    [CreatedDate]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NULL,
    [IsActive]       BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_tblRolePermission] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblRolePermission_OrganizationId]
    ON [dbo].[tblRolePermission]([OrganizationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblRolePermission_MenuId]
    ON [dbo].[tblRolePermission]([MenuId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblRolePermission_RoleId]
    ON [dbo].[tblRolePermission]([RoleId] ASC);

