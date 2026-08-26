CREATE TABLE [dbo].[tblUserLeave] (
    [Id]          UNIQUEIDENTIFIER NULL,
    [UserId]      UNIQUEIDENTIFIER NULL,
    [Type]        INT              NULL,
    [Status]      INT              NULL,
    [Date]        DATE             NULL,
    [StartTime]   DATETIME         NULL,
    [EndTime]     DATETIME         NULL,
    [CreatedDate] DATETIME         NULL,
    [UpdatedDate] DATETIME         NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NULL,
    [UpdatedBy]   UNIQUEIDENTIFIER NULL,
    [IsActive]    BIT              NULL,
    [Notes]       NVARCHAR (500)   NULL
);



