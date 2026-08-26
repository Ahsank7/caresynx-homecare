CREATE TABLE [dbo].[tblNotification] (
    [Id]                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Title]             NVARCHAR(200)    NOT NULL,
    [Message]           NVARCHAR(MAX)    NOT NULL,
    [Type]              NVARCHAR(50)     NOT NULL, -- 'Info', 'Success', 'Warning', 'Error', 'Activity'
    [Priority]          INT              NULL DEFAULT 0, -- 0=Normal, 1=High, 2=Urgent
    [OrganizationId]    UNIQUEIDENTIFIER NULL,
    [FranchiseId]       UNIQUEIDENTIFIER NULL,
    [TargetRoleId]      INT              NULL, -- NULL means all roles
    [TargetUserId]      UNIQUEIDENTIFIER NULL, -- NULL means all users (filtered by org/franchise)
    [ActivityType]      NVARCHAR(100)    NULL, -- 'Create', 'Update', 'Delete', 'Custom'
    [ActivityEntity]    NVARCHAR(100)    NULL, -- 'Client', 'Staff', 'Schedule', etc.
    [ActivityEntityId]  UNIQUEIDENTIFIER NULL, -- ID of the entity that was changed
    [ActionUrl]         NVARCHAR(500)    NULL, -- URL to navigate to when clicked
    [CreatedBy]         UNIQUEIDENTIFIER NULL,
    [CreatedDate]       DATETIME         NOT NULL DEFAULT GETDATE(),
    [IsActive]          BIT              NOT NULL DEFAULT 1,
    [ExpiresAt]         DATETIME         NULL, -- NULL means never expires
    CONSTRAINT [PK_tblNotification] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

-- Create indexes for better query performance
CREATE INDEX [IX_tblNotification_OrganizationId] ON [dbo].[tblNotification] ([OrganizationId]);
CREATE INDEX [IX_tblNotification_FranchiseId] ON [dbo].[tblNotification] ([FranchiseId]);
CREATE INDEX [IX_tblNotification_TargetUserId] ON [dbo].[tblNotification] ([TargetUserId]);
CREATE INDEX [IX_tblNotification_CreatedDate] ON [dbo].[tblNotification] ([CreatedDate] DESC);
CREATE INDEX [IX_tblNotification_Type_IsActive] ON [dbo].[tblNotification] ([Type], [IsActive]);

GO

