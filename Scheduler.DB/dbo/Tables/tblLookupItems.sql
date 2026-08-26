CREATE TABLE [dbo].[tblLookupItems] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [LookupType]   NVARCHAR (100)   NULL,
    [Name]         NVARCHAR (100)   NULL,
    [Description]  NVARCHAR (500)   NULL,
    [IsActive]     BIT              NULL,
    [InsertedById] UNIQUEIDENTIFIER NULL,
    [InsertedAt]   DATETIME         NULL,
    [UpdatedById]  UNIQUEIDENTIFIER NULL,
    [UpdatedAt]    DATETIME         NULL,
    CONSTRAINT [PK_tblLookupItems] PRIMARY KEY CLUSTERED ([Id] ASC)
);





