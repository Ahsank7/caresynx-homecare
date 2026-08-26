-- Create OrganizationTimeBasedRates table
CREATE TABLE [dbo].[OrganizationTimeBasedRates] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceTypeId] INT NULL,                       -- NULL = applies to all service types
    [ServiceId] INT NULL,                           -- NULL = applies to all services
    [DayOfWeek] TINYINT NOT NULL,                   -- 0 = Sunday, 1 = Monday, ..., 6 = Saturday
    [StartTime] TIME NOT NULL,
    [EndTime] TIME NOT NULL,
    [ClientRate] DECIMAL(10,2) NOT NULL,
    [WageRate] DECIMAL(10,2) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2 DEFAULT SYSUTCDATETIME(),
    
    -- Foreign key constraints
    CONSTRAINT [FK_OrganizationTimeBasedRates_Organization] 
        FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[tblOrganization]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrganizationTimeBasedRates_ServiceType] 
        FOREIGN KEY ([ServiceTypeId]) REFERENCES [dbo].[tblServicesType]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrganizationTimeBasedRates_Service] 
        FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[tblServices]([Id]) ON DELETE CASCADE,
    
    -- Check constraints
    CONSTRAINT [CK_OrganizationTimeBasedRates_DayOfWeek] 
        CHECK ([DayOfWeek] >= 0 AND [DayOfWeek] <= 6),
    CONSTRAINT [CK_OrganizationTimeBasedRates_StartTime] 
        CHECK ([StartTime] < [EndTime]),
    CONSTRAINT [CK_OrganizationTimeBasedRates_ClientRate] 
        CHECK ([ClientRate] >= 0),
    CONSTRAINT [CK_OrganizationTimeBasedRates_WageRate] 
        CHECK ([WageRate] >= 0)
);

-- Create indexes for better performance
CREATE INDEX [IX_OrganizationTimeBasedRates_OrganizationId] 
    ON [dbo].[OrganizationTimeBasedRates] ([OrganizationId]);

CREATE INDEX [IX_OrganizationTimeBasedRates_ServiceId] 
    ON [dbo].[OrganizationTimeBasedRates] ([ServiceId]);

CREATE INDEX [IX_OrganizationTimeBasedRates_DayOfWeek] 
    ON [dbo].[OrganizationTimeBasedRates] ([DayOfWeek]);

CREATE INDEX [IX_OrganizationTimeBasedRates_TimeRange] 
    ON [dbo].[OrganizationTimeBasedRates] ([StartTime], [EndTime]);

-- Create unique constraint to prevent overlapping time ranges for the same day/service
CREATE UNIQUE INDEX [IX_OrganizationTimeBasedRates_NoOverlap] 
    ON [dbo].[OrganizationTimeBasedRates] ([OrganizationId], [ServiceTypeId], [ServiceId], [DayOfWeek], [StartTime], [EndTime])
    WHERE [IsActive] = 1;
