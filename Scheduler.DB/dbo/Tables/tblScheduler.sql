CREATE TABLE [dbo].[tblScheduler] (
    [Id]                    INT              IDENTITY (1, 1) NOT NULL,
    [StartTime]             DATETIME         NULL,
    [EndTime]               DATETIME         NULL,
    [RecurrencePattern]     INT              NULL,
    [RecurrenceInterval]    INT              NULL,
    [RecurrenceDaysOfWeek]  NVARCHAR (50)    NULL,
    [RecurrenceDayOfMonth]  INT              NULL,
    [RecurrenceMonthOfYear] INT              NULL,
    [ServiceType]           INT              NULL,
    [CSVServiceIds]         NVARCHAR (50)    NULL,
    [ClientId]              UNIQUEIDENTIFIER NULL,
    [CSVServiceProviderIds] NVARCHAR (MAX)   NULL,
    [TimeZone]              NVARCHAR (100)   NULL,
    [Description]           NVARCHAR (500)   NULL,
    [CreatedDate]           DATETIME         NULL,
    [CreatedBy]             UNIQUEIDENTIFIER NULL,
    [UpdatedDate]           DATETIME         NULL,
    [UpdatedBy]             UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblScheduler] PRIMARY KEY CLUSTERED ([Id] ASC)
);










GO
CREATE NONCLUSTERED INDEX [IX_tblScheduler_StartTime]
    ON [dbo].[tblScheduler]([StartTime] ASC);

