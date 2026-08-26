CREATE TABLE [dbo].[tblNotification] (
    [Id]               UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Title]            NVARCHAR (200)   NOT NULL,
    [Message]          NVARCHAR (MAX)   NOT NULL,
    [Type]             NVARCHAR (50)    NOT NULL,
    [Priority]         INT              DEFAULT ((0)) NULL,
    [OrganizationId]   UNIQUEIDENTIFIER NULL,
    [FranchiseId]      UNIQUEIDENTIFIER NULL,
    [TargetRoleId]     INT              NULL,
    [TargetUserId]     UNIQUEIDENTIFIER NULL,
    [ActivityType]     NVARCHAR (100)   NULL,
    [ActivityEntity]   NVARCHAR (100)   NULL,
    [ActivityEntityId] UNIQUEIDENTIFIER NULL,
    [ActionUrl]        NVARCHAR (500)   NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NULL,
    [CreatedDate]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [IsActive]         BIT              DEFAULT ((1)) NOT NULL,
    [ExpiresAt]        DATETIME         NULL,
    CONSTRAINT [PK_tblNotification] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotification_Type_IsActive]
    ON [dbo].[tblNotification]([Type] ASC, [IsActive] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotification_CreatedDate]
    ON [dbo].[tblNotification]([CreatedDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotification_TargetUserId]
    ON [dbo].[tblNotification]([TargetUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotification_FranchiseId]
    ON [dbo].[tblNotification]([FranchiseId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotification_OrganizationId]
    ON [dbo].[tblNotification]([OrganizationId] ASC);

