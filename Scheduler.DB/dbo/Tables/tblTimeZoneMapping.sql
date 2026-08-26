CREATE TABLE [dbo].[tblTimeZoneMapping] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [DisplayName]         NVARCHAR (100) NOT NULL,
    [SqlServerIdentifier] NVARCHAR (100) NOT NULL,
    [IsActive]            BIT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_tblTimeZoneMapping] PRIMARY KEY CLUSTERED ([Id] ASC)
);

