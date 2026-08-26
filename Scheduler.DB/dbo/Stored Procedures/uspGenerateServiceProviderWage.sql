CREATE PROCEDURE [dbo].[uspGenerateServiceProviderWage] -- [dbo].[uspGenerateServiceProviderWage] "2024-01-01", "2024-12-30"
@pStartDate DATE,
@pEndDate DATE,
@pOrganizationId uniqueidentifier = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewServiceProviderWageId INT;
    DECLARE @ServiceProviderId uniqueidentifier;
    DECLARE @GeneratedWageCount INT = 0;

    -- Cursor to iterate over distinct service providers with confirmed tasks in the date range
    DECLARE provider_cursor CURSOR FOR
    SELECT DISTINCT st.ServiceProviderId
    FROM [dbo].[tblServicesTask] st
	   JOIN dbo.tbUserFranchise sf  on sf.UserId = st.ClientId
	   JOIN tblFranchise f on f.Id = sf.FranchiseId
	   JOIN tblOrganization org on org.Id = f.OrganizationId
    WHERE st.IsConfirmed = 1
      AND st.Date BETWEEN @pStartDate AND @pEndDate
	  AND org.Id = @pOrganizationId
      AND st.Id NOT IN (SELECT TaskId FROM [dbo].[tblServiceProviderWageDetail]);

    OPEN provider_cursor;
    FETCH NEXT FROM provider_cursor INTO @ServiceProviderId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Insert a new record into tblServiceProviderWage for the current service provider
        INSERT INTO [dbo].[tblServiceProviderWage] (
            [ServiceProviderId], -- Assuming this column exists in tblServiceProviderWage
            [Description],
            [Date],
            StartDate,
            EndDate,
            DueDate,
            [IsPaid],
			[Row_Guid]
        )
        VALUES (
            @ServiceProviderId,
            'Wage Details for Service Provider ' + CAST(@ServiceProviderId AS VARCHAR(50)), -- Customize description
            GETDATE(),
            @pStartDate,
            @pEndDate,
            GETDATE() + 7,
            0, -- Unpaid
			NEWID()
        );

        -- Get the ID of the newly inserted wage record
        SET @NewServiceProviderWageId = SCOPE_IDENTITY();
        
        -- Increment the counter for generated wages
        SET @GeneratedWageCount = @GeneratedWageCount + 1;

        -- Insert new records into tblServiceProviderWageDetail for the current service provider (tasks)
        INSERT INTO [dbo].[tblServiceProviderWageDetail] (
            [ServiceProviderWageId],
            [TaskId],
            [Amount],
            [ExpenseId],
            [ExpenseAmount]
        )
        SELECT
            @NewServiceProviderWageId AS ServiceProviderWageId,
            st.Id AS TaskId,
            st.WageAmount,
            NULL AS ExpenseId,
            NULL AS ExpenseAmount
        FROM
            [dbo].[tblServicesTask] st
        WHERE
            st.ServiceProviderId = @ServiceProviderId
            AND st.IsConfirmed = 1
            AND st.Date BETWEEN @pStartDate AND @pEndDate
            AND st.Id NOT IN (SELECT TaskId FROM [dbo].[tblServiceProviderWageDetail]);

        -- Insert expense records into tblServiceProviderWageDetail for the current service provider
        INSERT INTO [dbo].[tblServiceProviderWageDetail] (
            [ServiceProviderWageId],
            [TaskId],
            [Amount],
            [ExpenseId],
            [ExpenseAmount]
        )
        SELECT
            @NewServiceProviderWageId AS ServiceProviderWageId,
            ue.TaskId,
            0 AS Amount, -- No task amount for expense records
            ue.Id AS ExpenseId,
            ue.Amount AS ExpenseAmount
        FROM
            [dbo].[tblUserExpense] ue
        INNER JOIN [dbo].[tblServicesTask] st ON st.Id = ue.TaskId
        WHERE
            st.ServiceProviderId = @ServiceProviderId
            AND ue.IsActive = 1
            AND ue.IsConfirmed = 1
            AND st.Date BETWEEN @pStartDate AND @pEndDate
            AND ue.Id NOT IN (SELECT ExpenseId FROM [dbo].[tblServiceProviderWageDetail] WHERE ExpenseId IS NOT NULL);

        -- Update the total amount for the current wage record
        UPDATE [dbo].[tblServiceProviderWage]
        SET TotalAmount = ISNULL(
            (SELECT SUM(ISNULL(Amount, 0) + ISNULL(ExpenseAmount, 0)) 
             FROM [dbo].[tblServiceProviderWageDetail] 
             WHERE ServiceProviderWageId = @NewServiceProviderWageId), 0)
        WHERE Id = @NewServiceProviderWageId;

        -- Fetch the next service provider
        FETCH NEXT FROM provider_cursor INTO @ServiceProviderId;
    END

    CLOSE provider_cursor;
    DEALLOCATE provider_cursor;
    
    -- Return the count of generated wages
    SELECT @GeneratedWageCount AS GeneratedWageCount;
END;