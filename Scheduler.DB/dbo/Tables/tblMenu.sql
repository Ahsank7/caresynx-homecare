CREATE TABLE [dbo].[tblMenu] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [MenuId]         NVARCHAR (100)   NOT NULL,
    [MenuName]       NVARCHAR (200)   NOT NULL,
    [ParentMenuId]   NVARCHAR (100)   NULL,
    [MenuPath]       NVARCHAR (500)   NULL,
    [MenuIcon]       NVARCHAR (100)   NULL,
    [MenuOrder]      INT              DEFAULT ((0)) NOT NULL,
    [IsActive]       BIT              DEFAULT ((1)) NOT NULL,
    [CreatedDate]    DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblMenu] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblMenu_MenuId]
    ON [dbo].[tblMenu]([MenuId] ASC);

