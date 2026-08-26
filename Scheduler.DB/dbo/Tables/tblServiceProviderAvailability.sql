CREATE TABLE [dbo].[tblServiceProviderAvailability] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [AvailableDays]         NVARCHAR (500)   NULL,
    [StartTime]             DATETIME         NULL,
    [EndTime]               DATETIME         NULL,
    [ServiceProviderUserId] UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



