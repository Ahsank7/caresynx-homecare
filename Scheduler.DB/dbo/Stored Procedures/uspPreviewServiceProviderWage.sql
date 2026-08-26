CREATE PROCEDURE [dbo].[uspPreviewServiceProviderWage]
@pStartDate DATE,
@pEndDate DATE,
@pOrganizationId uniqueidentifier = NULL,
@pPageNumber INT = 1,
@pPageSize INT = 15,
@pSortColumn NVARCHAR(50) = 'TaskDate',
@pSortType NVARCHAR(10) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@pPageNumber - 1) * @pPageSize;
    DECLARE @TotalRecords INT;

    -- Get total count for both tasks and expenses
    SELECT @TotalRecords = (
        -- Count tasks
        SELECT COUNT(*)
        FROM [dbo].[tblServicesTask] st
        JOIN dbo.tbUserFranchise sf ON sf.UserId = st.ClientId
        JOIN tblFranchise f ON f.Id = sf.FranchiseId
        JOIN tblOrganization org ON org.Id = f.OrganizationId
        WHERE st.IsConfirmed = 1
          AND st.Date BETWEEN @pStartDate AND @pEndDate
          AND org.Id = @pOrganizationId
          AND st.Id NOT IN (SELECT TaskId FROM [dbo].[tblServiceProviderWageDetail])
    ) + (
        -- Count expenses
        SELECT COUNT(*)
        FROM [dbo].[tblUserExpense] ue
        JOIN [dbo].[tblServicesTask] st ON st.Id = ue.TaskId
        JOIN dbo.tbUserFranchise sf ON sf.UserId = st.ClientId
        JOIN tblFranchise f ON f.Id = sf.FranchiseId
        JOIN tblOrganization org ON org.Id = f.OrganizationId
        WHERE ue.IsActive = 1 
          AND ue.IsConfirmed = 1
          AND st.Date BETWEEN @pStartDate AND @pEndDate
          AND org.Id = @pOrganizationId
          AND ue.Id NOT IN (SELECT ExpenseId FROM [dbo].[tblServiceProviderWageDetail] WHERE ExpenseId IS NOT NULL)
    );

    -- Get tasks and expenses as independent records with proper ordering
    SELECT * FROM (
        -- Get tasks as independent records
        SELECT 
            st.Id AS TaskId,
            st.ServiceProviderId,
            sp.FirstName + ' ' + sp.LastName AS ServiceProviderName,
            st.ClientId,
            c.FirstName + ' ' + c.LastName AS ClientName,
            st.Date AS TaskDate,
            st.WageAmount,
            st.IsConfirmed,
            'Task' AS RecordType,
            NULL AS ExpenseId,
            NULL AS ExpenseAmount,
            NULL AS ExpenseType
        FROM [dbo].[tblServicesTask] st
        JOIN dbo.tbUserFranchise sf ON sf.UserId = st.ClientId
        JOIN tblFranchise f ON f.Id = sf.FranchiseId
        JOIN tblOrganization org ON org.Id = f.OrganizationId
        JOIN tblUser sp ON sp.Id = st.ServiceProviderId
        JOIN tblUser c ON c.Id = st.ClientId
        WHERE st.IsConfirmed = 1
          AND st.Date BETWEEN @pStartDate AND @pEndDate
          AND org.Id = @pOrganizationId
          AND st.Id NOT IN (SELECT TaskId FROM [dbo].[tblServiceProviderWageDetail])

        UNION ALL

        -- Get expenses as independent records
        SELECT 
            ue.TaskId AS TaskId,
            st.ServiceProviderId,
            sp.FirstName + ' ' + sp.LastName AS ServiceProviderName,
            st.ClientId,
            c.FirstName + ' ' + c.LastName AS ClientName,
            st.Date AS TaskDate,
            0 AS WageAmount, -- No wage amount for expense records
            st.IsConfirmed,
            'Expense' AS RecordType,
            ue.Id AS ExpenseId,
            ue.Amount AS ExpenseAmount,
            li.Name AS ExpenseType
        FROM [dbo].[tblUserExpense] ue
        JOIN [dbo].[tblServicesTask] st ON st.Id = ue.TaskId
        JOIN dbo.tbUserFranchise sf ON sf.UserId = st.ClientId
        JOIN tblFranchise f ON f.Id = sf.FranchiseId
        JOIN tblOrganization org ON org.Id = f.OrganizationId
        JOIN tblUser sp ON sp.Id = st.ServiceProviderId
        JOIN tblUser c ON c.Id = st.ClientId
        JOIN [dbo].[tblLookupItems] li ON li.Id = ue.Type AND li.LookupType = 'ExpenseType'
        WHERE ue.IsActive = 1 
          AND ue.IsConfirmed = 1
          AND st.Date BETWEEN @pStartDate AND @pEndDate
          AND org.Id = @pOrganizationId
          AND ue.Id NOT IN (SELECT ExpenseId FROM [dbo].[tblServiceProviderWageDetail] WHERE ExpenseId IS NOT NULL)
    ) AS CombinedResults
    ORDER BY 
        CASE WHEN @pSortColumn = 'TaskDate' AND @pSortType = 'ASC' THEN TaskDate END ASC,
        CASE WHEN @pSortColumn = 'TaskDate' AND @pSortType = 'DESC' THEN TaskDate END DESC,
        CASE WHEN @pSortColumn = 'ServiceProviderName' AND @pSortType = 'ASC' THEN ServiceProviderName END ASC,
        CASE WHEN @pSortColumn = 'ServiceProviderName' AND @pSortType = 'DESC' THEN ServiceProviderName END DESC,
        CASE WHEN @pSortColumn = 'WageAmount' AND @pSortType = 'ASC' THEN WageAmount END ASC,
        CASE WHEN @pSortColumn = 'WageAmount' AND @pSortType = 'DESC' THEN WageAmount END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @pPageSize ROWS ONLY;

    -- Return total count
    SELECT @TotalRecords AS TotalRecords;
END; 