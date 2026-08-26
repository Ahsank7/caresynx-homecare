-- Deployment script for Time-Based Rates feature
-- This script should be run in the following order:

-- 1. First, update the organization table
PRINT 'Step 1: Updating tblOrganization table...'
GO

-- Add a new column with the new data type
ALTER TABLE [dbo].[tblOrganization]
ADD [ServiceRateForBillingNew] INT NULL;
GO

-- Update the new column with existing data (convert BIT to INT)
UPDATE [dbo].[tblOrganization]
SET [ServiceRateForBillingNew] = CASE 
    WHEN [UseServiceRateForBilling] = 1 THEN 2  -- Convert existing BIT=1 to INT=2 (Service-Specific)
    ELSE 1  -- Convert existing BIT=0 to INT=1 (Default Rate)
END;
GO

-- Drop the old column
ALTER TABLE [dbo].[tblOrganization]
DROP COLUMN [UseServiceRateForBilling];
GO

-- Rename the new column to the original name
EXEC sp_rename '[dbo].[tblOrganization].[ServiceRateForBillingNew]', 'ServiceRateForBilling', 'COLUMN';
GO

-- Add default constraint
ALTER TABLE [dbo].[tblOrganization]
ADD CONSTRAINT [DF_tblOrganization_ServiceRateForBilling] DEFAULT (1) FOR [ServiceRateForBilling];
GO

-- Add check constraint to ensure valid values
ALTER TABLE [dbo].[tblOrganization]
ADD CONSTRAINT [CK_tblOrganization_ServiceRateForBilling] 
    CHECK ([ServiceRateForBilling] IN (1, 2, 3));
GO

PRINT 'Step 1 completed: tblOrganization table updated successfully'
GO

-- 2. Create the OrganizationTimeBasedRates table
PRINT 'Step 2: Creating OrganizationTimeBasedRates table...'
GO

CREATE TABLE [dbo].[OrganizationTimeBasedRates] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceId] UNIQUEIDENTIFIER NULL,              -- NULL = applies to all services
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
    CONSTRAINT [FK_OrganizationTimeBasedRates_Service] 
        FOREIGN KEY ([ServiceId]) REFERENCES [dbo].[tblService]([Id]) ON DELETE CASCADE,
    
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
GO

-- Create indexes for better performance
CREATE INDEX [IX_OrganizationTimeBasedRates_OrganizationId] 
    ON [dbo].[OrganizationTimeBasedRates] ([OrganizationId]);
GO

CREATE INDEX [IX_OrganizationTimeBasedRates_ServiceId] 
    ON [dbo].[OrganizationTimeBasedRates] ([ServiceId]);
GO

CREATE INDEX [IX_OrganizationTimeBasedRates_DayOfWeek] 
    ON [dbo].[OrganizationTimeBasedRates] ([DayOfWeek]);
GO

CREATE INDEX [IX_OrganizationTimeBasedRates_TimeRange] 
    ON [dbo].[OrganizationTimeBasedRates] ([StartTime], [EndTime]);
GO

-- Create unique constraint to prevent overlapping time ranges for the same day/service
CREATE UNIQUE INDEX [IX_OrganizationTimeBasedRates_NoOverlap] 
    ON [dbo].[OrganizationTimeBasedRates] ([OrganizationId], [ServiceId], [DayOfWeek], [StartTime], [EndTime])
    WHERE [IsActive] = 1;
GO

PRINT 'Step 2 completed: OrganizationTimeBasedRates table created successfully'
GO

-- 3. Create stored procedures
PRINT 'Step 3: Creating stored procedures...'
GO

-- Get all time-based rates for an organization
CREATE OR ALTER PROCEDURE [dbo].[uspGetOrganizationTimeBasedRates]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        otbr.Id,
        otbr.OrganizationId,
        otbr.ServiceId,
        s.Name AS ServiceName,
        otbr.DayOfWeek,
        CASE otbr.DayOfWeek
            WHEN 0 THEN 'Sunday'
            WHEN 1 THEN 'Monday'
            WHEN 2 THEN 'Tuesday'
            WHEN 3 THEN 'Wednesday'
            WHEN 4 THEN 'Thursday'
            WHEN 5 THEN 'Friday'
            WHEN 6 THEN 'Saturday'
        END AS DayName,
        otbr.StartTime,
        otbr.EndTime,
        otbr.ClientRate,
        otbr.WageRate,
        otbr.IsActive,
        otbr.CreatedAt,
        otbr.UpdatedAt
    FROM [dbo].[OrganizationTimeBasedRates] otbr
    LEFT JOIN [dbo].[tblService] s ON otbr.ServiceId = s.Id
    WHERE otbr.OrganizationId = @OrganizationId
    ORDER BY otbr.DayOfWeek, otbr.StartTime;
END
GO

-- Insert or update time-based rate for an organization
CREATE OR ALTER PROCEDURE [dbo].[uspInsertUpdateOrganizationTimeBasedRate]
    @Id INT = NULL,
    @OrganizationId UNIQUEIDENTIFIER,
    @ServiceId UNIQUEIDENTIFIER = NULL,
    @DayOfWeek TINYINT,
    @StartTime TIME,
    @EndTime TIME,
    @ClientRate DECIMAL(10,2),
    @WageRate DECIMAL(10,2),
    @IsActive BIT = 1,
    @OutId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate input parameters
        IF @OrganizationId IS NULL
        BEGIN
            RAISERROR('OrganizationId is required', 16, 1);
            RETURN;
        END
        
        IF @DayOfWeek < 0 OR @DayOfWeek > 6
        BEGIN
            RAISERROR('DayOfWeek must be between 0 and 6', 16, 1);
            RETURN;
        END
        
        IF @StartTime >= @EndTime
        BEGIN
            RAISERROR('StartTime must be less than EndTime', 16, 1);
            RETURN;
        END
        
        IF @ClientRate < 0 OR @WageRate < 0
        BEGIN
            RAISERROR('Rates must be non-negative', 16, 1);
            RETURN;
        END
        
        -- Check for overlapping time ranges (only for active rates)
        IF @IsActive = 1
        BEGIN
            IF EXISTS (
                SELECT 1 FROM [dbo].[OrganizationTimeBasedRates]
                WHERE OrganizationId = @OrganizationId
                AND (@ServiceId IS NULL OR ServiceId = @ServiceId OR ServiceId IS NULL)
                AND DayOfWeek = @DayOfWeek
                AND IsActive = 1
                AND Id != ISNULL(@Id, -1)
                AND (
                    (@StartTime < EndTime AND @EndTime > StartTime)
                )
            )
            BEGIN
                RAISERROR('Time range overlaps with existing active rate for the same day and service', 16, 1);
                RETURN;
            END
        END
        
        -- Insert or update
        IF @Id IS NULL OR @Id = 0
        BEGIN
            -- Insert new record
            INSERT INTO [dbo].[OrganizationTimeBasedRates] (
                OrganizationId, ServiceId, DayOfWeek, StartTime, EndTime, 
                ClientRate, WageRate, IsActive, CreatedAt, UpdatedAt
            )
            VALUES (
                @OrganizationId, @ServiceId, @DayOfWeek, @StartTime, @EndTime,
                @ClientRate, @WageRate, @IsActive, SYSUTCDATETIME(), SYSUTCDATETIME()
            );
            
            SET @OutId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Update existing record
            UPDATE [dbo].[OrganizationTimeBasedRates]
            SET 
                ServiceId = @ServiceId,
                DayOfWeek = @DayOfWeek,
                StartTime = @StartTime,
                EndTime = @EndTime,
                ClientRate = @ClientRate,
                WageRate = @WageRate,
                IsActive = @IsActive,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id AND OrganizationId = @OrganizationId;
            
            IF @@ROWCOUNT = 0
            BEGIN
                RAISERROR('Time-based rate not found or access denied', 16, 1);
                RETURN;
            END
            
            SET @OutId = @Id;
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

-- Delete time-based rate for an organization
CREATE OR ALTER PROCEDURE [dbo].[uspDeleteOrganizationTimeBasedRate]
    @Id INT,
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate input parameters
        IF @Id IS NULL OR @Id <= 0
        BEGIN
            RAISERROR('Valid Id is required', 16, 1);
            RETURN;
        END
        
        IF @OrganizationId IS NULL
        BEGIN
            RAISERROR('OrganizationId is required', 16, 1);
            RETURN;
        END
        
        -- Delete the record
        DELETE FROM [dbo].[OrganizationTimeBasedRates]
        WHERE Id = @Id AND OrganizationId = @OrganizationId;
        
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Time-based rate not found or access denied', 16, 1);
            RETURN;
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

PRINT 'Step 3 completed: Stored procedures created successfully'
GO

-- 4. Update existing organization stored procedure
PRINT 'Step 4: Updating organization stored procedure...'
GO

-- Update the InsertUpdateOrganization stored procedure
-- Note: This assumes the procedure is in the Organization schema
-- If it's in dbo schema, adjust the schema reference accordingly

-- First, let's check if the procedure exists and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Organization].[InsertUpdateOrganization]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [Organization].[InsertUpdateOrganization]
END
GO

-- Create the updated procedure
CREATE PROCEDURE [Organization].[InsertUpdateOrganization] 
    @pId uniqueidentifier=null,
    @pOutId uniqueidentifier=null output,
    @pName nvarchar(50)=null,   
    @pDescription nvarchar(50)=null,
    @pDefaultBillingRate decimal(18,2)=0,
    @pDefaultWageRate decimal(18,2)=0,
    @pCompleteAddress  nvarchar(500)=null,
    @pContactNo  nvarchar(50)=null,
    @pEmail  nvarchar(50)=null,
    @pWebSite  nvarchar(50)=null,
    @pCurrencyId  int=0,
    @pcalculationTypeId int=0,
    @ptaxPercentage decimal(18,2)=0,
    @pdiscountPercentage decimal(18,2)=0,
    @pCurrencySignId   int=0,
    @pServiceRateForBilling int=1,
    @pTimeZone  nvarchar(100)=null
AS
BEGIN
    SET NOCOUNT ON;

    if @pId is null
    begin
        SET @pOutId =  NEWID()	

        INSERT INTO [dbo].[tblOrganization]
               (Id
               ,[Name]
               ,[Description]
               ,DefaultBillingRate
               ,DefaultWageRate
               ,CompleteAddress
               ,ContactNo
               ,CurrencyId
               ,Email
               ,WebSite
               ,CalculationTypeId
               ,TaxPercentage
               ,DiscountPercentage
               ,CurrencySignId
               ,ServiceRateForBilling
               ,TimeZone
               ,[IsActive])
         VALUES(@pOutId,@pName,@pDescription,@pDefaultBillingRate,@pDefaultWageRate,@pCompleteAddress,@pContactNo,@pCurrencyId,@pEmail,@pWebSite,@pcalculationTypeId,@ptaxPercentage,@pdiscountPercentage,@pCurrencySignId,@pServiceRateForBilling,@pTimeZone,1)
    end
    else
    begin
        UPDATE [dbo].[tblOrganization]
          SET  [Name]     =@pName
              ,[Description]      =@pDescription
              ,DefaultBillingRate = @pDefaultBillingRate
              ,DefaultWageRate    = @pDefaultWageRate
              ,CompleteAddress    = @pCompleteAddress
              ,ContactNo          = @pContactNo
              ,[CurrencyId]  =      @pCurrencyId
              ,Email  =  @pEmail
              ,WebSite  = @pWebSite
              ,CalculationTypeId = @pCalculationTypeId
              ,TaxPercentage =@pTaxPercentage
              ,DiscountPercentage = @pDiscountPercentage
              ,CurrencySignId = @pCurrencySignId
              ,ServiceRateForBilling = @pServiceRateForBilling
              ,TimeZone = @pTimeZone
              ,[IsActive]      =1
        WHERE [Id]=@pId

        SET @pOutId = @pId
    end
END
GO

PRINT 'Step 4 completed: Organization stored procedure updated successfully'
GO

PRINT 'Deployment completed successfully!'
PRINT 'Time-Based Rates feature is now ready to use.'
PRINT ''
PRINT 'Summary of changes:'
PRINT '1. Updated tblOrganization.ServiceRateForBilling from BIT to INT (1=Default, 2=Service-Specific, 3=Time-Based)'
PRINT '2. Created OrganizationTimeBasedRates table for storing time-based rate rules'
PRINT '3. Created stored procedures for managing time-based rates'
PRINT '4. Updated organization stored procedure to handle new ServiceRateForBilling field'
PRINT ''
PRINT 'Next steps:'
PRINT '1. Deploy the updated API with new OrganizationBillingSettingsController'
PRINT '2. Deploy the updated frontend with RatesAndBillingSettings component'
PRINT '3. Test the functionality in the organization settings'
