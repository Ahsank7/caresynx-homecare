CREATE TABLE [dbo].[tblServiceProviderAttributes] (
    [Id]                UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ServiceProviderId] UNIQUEIDENTIFIER NOT NULL,
    [AttributeType]     NVARCHAR (100)   NOT NULL,
    [AttributeValue]    NVARCHAR (200)   NULL,
    [AttributeItemId]   INT              NULL,
    [CreatedDate]       DATETIME         DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]       DATETIME         NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NULL,
    [UpdatedBy]         UNIQUEIDENTIFIER NULL,
    [IsActive]          BIT              DEFAULT ((1)) NULL,
    CONSTRAINT [PK_tblServiceProviderAttributes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ServiceProviderAttributes_User] FOREIGN KEY ([ServiceProviderId]) REFERENCES [dbo].[tblUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceProviderAttributes_AttributeType]
    ON [dbo].[tblServiceProviderAttributes]([AttributeType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceProviderAttributes_ServiceProviderId]
    ON [dbo].[tblServiceProviderAttributes]([ServiceProviderId] ASC);

