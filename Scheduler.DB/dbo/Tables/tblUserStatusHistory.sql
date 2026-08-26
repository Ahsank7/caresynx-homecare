CREATE TABLE [dbo].[tblUserStatusHistory] (
    [Id]               UNIQUEIDENTIFIER NULL,
    [UserId]           UNIQUEIDENTIFIER NULL,
    [CurrentStatusId]  INT              NOT NULL,
    [PreviousStatusId] INT              NOT NULL,
    [CreatedAt]        DATETIME         NULL,
    [UpdatedAt]        DATETIME         NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NULL,
    [UpdatedBy]        UNIQUEIDENTIFIER NULL
);

