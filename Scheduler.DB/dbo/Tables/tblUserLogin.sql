CREATE TABLE [dbo].[tblUserLogin] (
    [Id]       INT              IDENTITY (1, 1) NOT NULL,
    [UserId]   UNIQUEIDENTIFIER NULL,
    [UserName] NVARCHAR (50)    NULL,
    [Password] NVARCHAR (50)    NULL,
    [IsActive] BIT              NULL,
    CONSTRAINT [PK_tblUserLogin] PRIMARY KEY CLUSTERED ([Id] ASC)
);

