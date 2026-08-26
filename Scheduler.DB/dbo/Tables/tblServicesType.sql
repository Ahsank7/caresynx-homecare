CREATE TABLE [dbo].[tblServicesType] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (50)    NULL,
    [Description]    NVARCHAR (500)   NULL,
    [IsActive]       BIT              NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblServicesType] PRIMARY KEY CLUSTERED ([Id] ASC)
);



