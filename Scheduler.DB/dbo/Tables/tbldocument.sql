CREATE TABLE [dbo].[tblDocument] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [DocumentTypeId] INT              NULL,
    [Name]           NVARCHAR (50)    NULL,
    [Description]    NVARCHAR (500)   NULL,
    [AccessRoles]    NVARCHAR (50)    NULL,
    [DocumentPath]   NVARCHAR (500)   NULL,
    [UserId]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblDocument] PRIMARY KEY CLUSTERED ([Id] ASC)
);





