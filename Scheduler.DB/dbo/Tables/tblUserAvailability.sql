CREATE TABLE [dbo].[tblUserAvailability] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [UserId]    UNIQUEIDENTIFIER NULL,
    [StartTime] TIME (7)         NULL,
    [EndTime]   TIME (7)         NULL,
    [Day]       NVARCHAR (12)    NULL,
    [IsActive]  BIT              NULL,
    CONSTRAINT [PK_tblUserAvailability] PRIMARY KEY CLUSTERED ([Id] ASC)
);

