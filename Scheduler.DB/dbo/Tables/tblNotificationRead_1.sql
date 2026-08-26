CREATE TABLE [dbo].[tblNotificationRead] (
    [Id]             UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [NotificationId] UNIQUEIDENTIFIER NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [ReadDate]       DATETIME         DEFAULT (getdate()) NOT NULL,
    [IsRead]         BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_tblNotificationRead] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblNotificationRead_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [dbo].[tblNotification] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_NotificationUser] UNIQUE NONCLUSTERED ([NotificationId] ASC, [UserId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotificationRead_NotificationId_UserId]
    ON [dbo].[tblNotificationRead]([NotificationId] ASC, [UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblNotificationRead_UserId]
    ON [dbo].[tblNotificationRead]([UserId] ASC);

