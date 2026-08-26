CREATE TABLE [dbo].[tblUserRole] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NULL,
    [RoleId]      INT              NULL,
    [IsActive]    BIT              NULL,
    [UpdatedDate] DATETIME         NULL,
    [CreatedDate] DATETIME         NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblUserRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblUserRole_tblUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[tblUser] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_tblUserRole_UserId]
    ON [dbo].[tblUserRole]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblUserRole_RoleId]
    ON [dbo].[tblUserRole]([RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblUserRole_IsActive]
    ON [dbo].[tblUserRole]([IsActive] ASC);

