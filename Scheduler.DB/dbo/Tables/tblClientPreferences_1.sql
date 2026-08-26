CREATE TABLE [dbo].[tblClientPreferences] (
    [Id]               UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ClientId]         UNIQUEIDENTIFIER NOT NULL,
    [PreferenceType]   NVARCHAR (100)   NOT NULL,
    [PreferenceValue]  NVARCHAR (200)   NULL,
    [PreferenceItemId] INT              NULL,
    [IsRequired]       BIT              DEFAULT ((0)) NULL,
    [CreatedDate]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]      DATETIME         NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NULL,
    [UpdatedBy]        UNIQUEIDENTIFIER NULL,
    [IsActive]         BIT              DEFAULT ((1)) NULL,
    CONSTRAINT [PK_tblClientPreferences] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientPreferences_User] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[tblUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ClientPreferences_ClientId]
    ON [dbo].[tblClientPreferences]([ClientId] ASC);

