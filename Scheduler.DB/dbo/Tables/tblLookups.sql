CREATE TABLE [dbo].[tblLookups] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NULL,
    [Description] NVARCHAR (500) NULL,
    [DisplayName] NVARCHAR (50)  NULL,
    [IsActive]    BIT            NULL,
    [IsVisible]   BIT            CONSTRAINT [DF_tblLookups_IsVisible] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_tblLookups] PRIMARY KEY CLUSTERED ([Id] ASC)
);



