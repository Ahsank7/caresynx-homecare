CREATE TABLE [dbo].[tblUserExpense] (
    [Id]          UNIQUEIDENTIFIER NULL,
    [UserId]      UNIQUEIDENTIFIER NULL,
    [Date]        DATE             NULL,
    [TaskId]      INT              NULL,
    [Type]        INT              NULL,
    [Amount]      DECIMAL (18, 2)  NULL,
    [IsPaid]      BIT              NULL,
    [CreatedAt]   DATETIME         NULL,
    [UpdatedAt]   DATETIME         NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NULL,
    [UpdatedBy]   UNIQUEIDENTIFIER NULL,
    [IsActive]    BIT              NULL,
    [IsConfirmed] BIT              NULL,
    [Notes]       NVARCHAR (500)   NULL
);



