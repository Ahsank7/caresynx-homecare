CREATE TABLE [dbo].[tblUser] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [FirstName]        NVARCHAR (50)    NULL,
    [SurName]          NVARCHAR (50)    NULL,
    [LastName]         NVARCHAR (50)    NULL,
    [Alias]            NVARCHAR (50)    NULL,
    [Age]              INT              NULL,
    [Gender]           INT              NULL,
    [MaritalStatus]    INT              NULL,
    [Title]            INT              NULL,
    [Ethnicity]        INT              NULL,
    [BirthDate]        DATE             NULL,
    [JoiningDate]      DATE             NULL,
    [PassportNo]       NVARCHAR (50)    NULL,
    [IdentityNo]       NVARCHAR (50)    NULL,
    [MobileNo]         NVARCHAR (50)    NULL,
    [PhoneNo]          NVARCHAR (50)    NULL,
    [Email]            NVARCHAR (50)    NULL,
    [Status]           INT              NULL,
    [CreatedDate]      DATE             NOT NULL,
    [UpdatedDate]      DATE             NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NULL,
    [UpdatedBy]        UNIQUEIDENTIFIER NULL,
    [IsActive]         BIT              NULL,
    [UserType]         INT              NULL,
    [NationalityId]    INT              NULL,
    [FranchiseId]      UNIQUEIDENTIFIER NULL,
    [UserNo]           NVARCHAR (50)    NULL,
    [Notes]            NVARCHAR (500)   NULL,
    [UserName]         NVARCHAR (50)    NULL,
    [Password]         NVARCHAR (100)   NULL,
    [RoleId]           INT              NULL,
    [ProfileImagePath] NVARCHAR (500)   NULL,
    CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED ([Id] ASC)
);











