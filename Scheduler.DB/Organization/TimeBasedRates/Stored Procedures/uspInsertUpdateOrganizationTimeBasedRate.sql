-- Insert or update time-based rate for an organization
CREATE OR ALTER PROCEDURE [dbo].[uspInsertUpdateOrganizationTimeBasedRate]
    @pId INT = NULL,
    @pOrganizationId UNIQUEIDENTIFIER,
    @pServiceTypeId INT = NULL,
    @pServiceId INT = NULL,
    @pDayOfWeek TINYINT,
    @pStartTime TIME,
    @pEndTime TIME,
    @pClientRate DECIMAL(10,2),
    @pWageRate DECIMAL(10,2),
    @pIsActive BIT = 1,
    @pOutId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate input parameters
        IF @pOrganizationId IS NULL
        BEGIN
            RAISERROR('OrganizationId is required', 16, 1);
            RETURN;
        END
        
        IF @pDayOfWeek < 0 OR @pDayOfWeek > 6
        BEGIN
            RAISERROR('DayOfWeek must be between 0 and 6', 16, 1);
            RETURN;
        END
        
        IF @pStartTime >= @pEndTime
        BEGIN
            RAISERROR('StartTime must be less than EndTime', 16, 1);
            RETURN;
        END
        
        IF @pClientRate < 0 OR @pWageRate < 0
        BEGIN
            RAISERROR('Rates must be non-negative', 16, 1);
            RETURN;
        END
        
        -- Check for overlapping time ranges (only for active rates)
        IF @pIsActive = 1
        BEGIN
            IF EXISTS (
                SELECT 1 FROM [dbo].[tblOrganizationTimeBasedRates]
                WHERE OrganizationId = @pOrganizationId
                AND (@pServiceTypeId IS NULL OR ServiceTypeId = @pServiceTypeId OR ServiceTypeId IS NULL)
                AND (@pServiceId IS NULL OR ServiceId = @pServiceId OR ServiceId IS NULL)
                AND DayOfWeek = @pDayOfWeek
                AND IsActive = 1
                AND Id != ISNULL(@pId, -1)
                AND (
                    (@pStartTime < EndTime AND @pEndTime > StartTime)
                )
            )
            BEGIN
                RAISERROR('Time range overlaps with existing active rate for the same day and service', 16, 1);
                RETURN;
            END
        END
        
        -- Insert or update
        IF @pId IS NULL OR @pId = 0
        BEGIN
            -- Insert new record
            INSERT INTO [dbo].[tblOrganizationTimeBasedRates] (
                OrganizationId, ServiceTypeId, ServiceId, DayOfWeek, StartTime, EndTime, 
                ClientRate, WageRate, IsActive, CreatedAt, UpdatedAt
            )
            VALUES (
                @pOrganizationId, @pServiceTypeId, @pServiceId, @pDayOfWeek, @pStartTime, @pEndTime,
                @pClientRate, @pWageRate, @pIsActive, SYSUTCDATETIME(), SYSUTCDATETIME()
            );
            
            SET @pOutId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Update existing record
            UPDATE [dbo].[tblOrganizationTimeBasedRates]
            SET 
                ServiceTypeId = @pServiceTypeId,
                ServiceId = @pServiceId,
                DayOfWeek = @pDayOfWeek,
                StartTime = @pStartTime,
                EndTime = @pEndTime,
                ClientRate = @pClientRate,
                WageRate = @pWageRate,
                IsActive = @pIsActive,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @pId AND OrganizationId = @pOrganizationId;
            
            IF @@ROWCOUNT = 0
            BEGIN
                RAISERROR('Time-based rate not found or access denied', 16, 1);
                RETURN;
            END
            
            SET @pOutId = @pId;
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
