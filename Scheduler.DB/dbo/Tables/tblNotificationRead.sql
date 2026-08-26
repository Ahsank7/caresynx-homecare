CREATE TABLE [dbo].[tblNotificationRead] (
    [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [NotificationId]  UNIQUEIDENTIFIER NOT NULL,
    [UserId]          UNIQUEIDENTIFIER NOT NULL,
    [ReadDate]        DATETIME         NOT NULL DEFAULT GETDATE(),
    [IsRead]          BIT              NOT NULL DEFAULT 1,
    CONSTRAINT [PK_tblNotificationRead] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblNotificationRead_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [dbo].[tblNotification]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_NotificationUser] UNIQUE ([NotificationId], [UserId])
);

GO

-- Create indexes for better query performance
CREATE INDEX [IX_tblNotificationRead_UserId] ON [dbo].[tblNotificationRead] ([UserId]);
CREATE INDEX [IX_tblNotificationRead_NotificationId_UserId] ON [dbo].[tblNotificationRead] ([NotificationId], [UserId]);

GO

