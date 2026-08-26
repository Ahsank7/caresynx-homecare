CREATE TABLE [dbo].[tblServiceProviderAttributes] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ServiceProviderId]   UNIQUEIDENTIFIER NOT NULL,
    [AttributeType]       NVARCHAR(100)    NOT NULL, -- e.g., 'Gender', 'SmokingStatus', 'Language', 'Age'
    [AttributeValue]      NVARCHAR(200)    NULL,     -- The value for the attribute
    [AttributeItemId]     INT              NULL,     -- Reference to tblLookupItems if using lookup
    [CreatedDate]         DATETIME         NOT NULL DEFAULT GETDATE(),
    [UpdatedDate]         DATETIME         NULL,
    [CreatedBy]           UNIQUEIDENTIFIER NULL,
    [UpdatedBy]           UNIQUEIDENTIFIER NULL,
    [IsActive]            BIT              DEFAULT 1,
    CONSTRAINT [PK_tblServiceProviderAttributes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ServiceProviderAttributes_User] FOREIGN KEY ([ServiceProviderId]) REFERENCES [dbo].[tblUser]([Id])
);

GO

CREATE NONCLUSTERED INDEX [IX_ServiceProviderAttributes_ServiceProviderId] ON [dbo].[tblServiceProviderAttributes] ([ServiceProviderId] ASC);
CREATE NONCLUSTERED INDEX [IX_ServiceProviderAttributes_AttributeType] ON [dbo].[tblServiceProviderAttributes] ([AttributeType] ASC);

