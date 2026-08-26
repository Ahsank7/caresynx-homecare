CREATE TABLE [dbo].[tblStaff] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NULL,
    [IsActive]    BIT              NULL,
    [CreatedDate] DATE             NOT NULL,
    CONSTRAINT [PK_tblStaff] PRIMARY KEY CLUSTERED ([Id] ASC)
);

