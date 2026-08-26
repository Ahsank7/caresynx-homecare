CREATE TABLE [dbo].[tblLoginHistory] (
    [Id]               INT              IDENTITY (1, 1) NOT NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    [UserName]         NVARCHAR (100)   NOT NULL,
    [UserEmail]        NVARCHAR (255)   NULL,
    [UserType]         INT              NOT NULL,
    [OrganizationId]   UNIQUEIDENTIFIER NOT NULL,
    [FranchiseId]      UNIQUEIDENTIFIER NULL,
    [LoginTime]        DATETIME         DEFAULT (getdate()) NOT NULL,
    [LogoutTime]       DATETIME         NULL,
    [IPAddress]        NVARCHAR (45)    NULL,
    [UserAgent]        NVARCHAR (500)   NULL,
    [BrowserName]      NVARCHAR (100)   NULL,
    [BrowserVersion]   NVARCHAR (50)    NULL,
    [OperatingSystem]  NVARCHAR (100)   NULL,
    [DeviceType]       NVARCHAR (50)    NULL,
    [ScreenResolution] NVARCHAR (50)    NULL,
    [Timezone]         NVARCHAR (100)   NULL,
    [Language]         NVARCHAR (20)    NULL,
    [Country]          NVARCHAR (100)   NULL,
    [City]             NVARCHAR (100)   NULL,
    [LoginStatus]      NVARCHAR (20)    DEFAULT ('Success') NOT NULL,
    [FailureReason]    NVARCHAR (500)   NULL,
    [SessionDuration]  INT              NULL,
    [IsActive]         BIT              DEFAULT ((1)) NOT NULL,
    [CreatedDate]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [ModifiedDate]     DATETIME         NULL,
    CONSTRAINT [PK_tblLoginHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblLoginHistory_tblFranchise] FOREIGN KEY ([FranchiseId]) REFERENCES [dbo].[tblFranchise] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_tblLoginHistory_tblOrganization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblLoginHistory_tblUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[tblUser] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_tblLoginHistory_IPAddress]
    ON [dbo].[tblLoginHistory]([IPAddress] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblLoginHistory_UserType]
    ON [dbo].[tblLoginHistory]([UserType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblLoginHistory_LoginTime]
    ON [dbo].[tblLoginHistory]([LoginTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblLoginHistory_OrganizationId]
    ON [dbo].[tblLoginHistory]([OrganizationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblLoginHistory_UserId]
    ON [dbo].[tblLoginHistory]([UserId] ASC);

