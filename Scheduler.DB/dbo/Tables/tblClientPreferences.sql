CREATE TABLE [dbo].[tblClientPreferences] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ClientId]            UNIQUEIDENTIFIER NOT NULL,
    [PreferenceType]      NVARCHAR(100)    NOT NULL, -- e.g., 'Gender', 'SmokingStatus', 'Language', 'AgeRange'
    [PreferenceValue]     NVARCHAR(200)    NULL,     -- The value for the preference
    [PreferenceItemId]    INT              NULL,     -- Reference to tblLookupItems if using lookup
    [IsRequired]          BIT              DEFAULT 0, -- Whether this preference is mandatory
    [CreatedDate]         DATETIME         NOT NULL DEFAULT GETDATE(),
    [UpdatedDate]         DATETIME         NULL,
    [CreatedBy]           UNIQUEIDENTIFIER NULL,
    [UpdatedBy]           UNIQUEIDENTIFIER NULL,
    [IsActive]            BIT              DEFAULT 1,
    CONSTRAINT [PK_tblClientPreferences] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientPreferences_User] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[tblUser]([Id])
);

GO

CREATE NONCLUSTERED INDEX [IX_ClientPreferences_ClientId] ON [dbo].[tblClientPreferences] ([ClientId] ASC);

