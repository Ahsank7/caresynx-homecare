CREATE TABLE [dbo].[tblOrganizationTimeBasedRates] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceTypeId]  INT              NULL,
    [ServiceId]      INT              NULL,
    [DayOfWeek]      TINYINT          NOT NULL,
    [StartTime]      TIME (7)         NOT NULL,
    [EndTime]        TIME (7)         NOT NULL,
    [ClientRate]     DECIMAL (10, 2)  NOT NULL,
    [WageRate]       DECIMAL (10, 2)  NOT NULL,
    [IsActive]       BIT              DEFAULT ((1)) NOT NULL,
    [CreatedAt]      DATETIME2 (7)    DEFAULT (sysutcdatetime()) NULL,
    [UpdatedAt]      DATETIME2 (7)    DEFAULT (sysutcdatetime()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_tblOrganizationTimeBasedRates_ClientRate] CHECK ([ClientRate]>=(0)),
    CONSTRAINT [CK_tblOrganizationTimeBasedRates_DayOfWeek] CHECK ([DayOfWeek]>=(0) AND [DayOfWeek]<=(6)),
    CONSTRAINT [CK_tblOrganizationTimeBasedRates_StartTime] CHECK ([StartTime]<[EndTime]),
    CONSTRAINT [CK_tblOrganizationTimeBasedRates_WageRate] CHECK ([WageRate]>=(0))
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblOrganizationTimeBasedRates_NoOverlap]
    ON [dbo].[tblOrganizationTimeBasedRates]([OrganizationId] ASC, [ServiceTypeId] ASC, [ServiceId] ASC, [DayOfWeek] ASC, [StartTime] ASC, [EndTime] ASC) WHERE ([IsActive]=(1));


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationTimeBasedRates_TimeRange]
    ON [dbo].[tblOrganizationTimeBasedRates]([StartTime] ASC, [EndTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationTimeBasedRates_DayOfWeek]
    ON [dbo].[tblOrganizationTimeBasedRates]([DayOfWeek] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationTimeBasedRates_ServiceId]
    ON [dbo].[tblOrganizationTimeBasedRates]([ServiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblOrganizationTimeBasedRates_OrganizationId]
    ON [dbo].[tblOrganizationTimeBasedRates]([OrganizationId] ASC);

