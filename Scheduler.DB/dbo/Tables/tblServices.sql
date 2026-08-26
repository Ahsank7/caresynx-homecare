CREATE TABLE [dbo].[tblServices] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [ServiceTypeId] INT             NOT NULL,
    [Name]          NVARCHAR (50)   NULL,
    [Description]   NVARCHAR (500)  NULL,
    [Rate]          DECIMAL (18, 2) NULL,
    [IsActive]      BIT             NULL,
    CONSTRAINT [PK_tblServices] PRIMARY KEY CLUSTERED ([Id] ASC)
);





