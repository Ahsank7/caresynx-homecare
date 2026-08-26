CREATE TABLE [dbo].[tblTaskLog] (
    [Id]            INT              IDENTITY (1, 1) NOT NULL,
    [TaskId]        INT              NOT NULL,
    [ActionType]    NVARCHAR (50)    NOT NULL,
    [PreviousValue] NVARCHAR (MAX)   NULL,
    [NewValue]      NVARCHAR (MAX)   NULL,
    [FieldName]     NVARCHAR (100)   NULL,
    [Description]   NVARCHAR (500)   NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate]   DATETIME         DEFAULT (getdate()) NOT NULL,
    [IPAddress]     NVARCHAR (45)    NULL,
    [UserAgent]     NVARCHAR (500)   NULL,
    CONSTRAINT [PK_tblTaskLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblTaskLog_tblServicesTask] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[tblServicesTask] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblTaskLog_tblUser] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[tblUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTaskLog_ActionType]
    ON [dbo].[tblTaskLog]([ActionType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTaskLog_CreatedDate]
    ON [dbo].[tblTaskLog]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTaskLog_TaskId]
    ON [dbo].[tblTaskLog]([TaskId] ASC);

