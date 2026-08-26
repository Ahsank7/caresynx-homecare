CREATE TABLE [dbo].[tblServiceProvider] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NULL,
    [IsActive]    BIT              NULL,
    [CreatedDate] DATE             NULL,
    CONSTRAINT [PK_tblServiceProvider] PRIMARY KEY CLUSTERED ([Id] ASC)
);

