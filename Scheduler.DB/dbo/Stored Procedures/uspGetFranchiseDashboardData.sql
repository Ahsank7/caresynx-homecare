CREATE PROCEDURE [dbo].[uspGetFranchiseDashboardData]
    @pFranchiseId UNIQUEIDENTIFIER,
    @pStartDate DATE,
    @pEndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Get basic stats
    SELECT 
        (SELECT COUNT(DISTINCT uf.UserId) 
         FROM tbUserFranchise uf 
         JOIN tblUser u ON u.Id = uf.UserId 
         WHERE uf.FranchiseId = @pFranchiseId
         AND ISNULL(uf.IsActive, 0) = 1
         AND ISNULL(u.IsActive, 0) = 1
         AND u.UserType = 1
         AND u.CreatedDate BETWEEN @pStartDate AND @pEndDate) AS TotalClients,
        
        (SELECT COUNT(DISTINCT uf.UserId) 
         FROM tbUserFranchise uf 
         JOIN tblUser u ON u.Id = uf.UserId 
         WHERE uf.FranchiseId = @pFranchiseId
         AND ISNULL(uf.IsActive, 0) = 1
         AND ISNULL(u.IsActive, 0) = 1
         AND u.UserType = 2
         AND u.CreatedDate BETWEEN @pStartDate AND @pEndDate) AS TotalServiceProviders,
        
        (SELECT COUNT(DISTINCT uf.UserId) 
         FROM tblUserRole uf 
         JOIN tblUser u ON u.Id = uf.UserId 
         WHERE u.FranchiseId = @pFranchiseId
         AND ISNULL(uf.IsActive, 0) = 1
         AND ISNULL(u.IsActive, 0) = 1
         AND u.UserType = 3
         AND u.CreatedDate BETWEEN @pStartDate AND @pEndDate) AS TotalStaff,
        
        (SELECT COUNT(*) 
         FROM tblServicesTask st 
         JOIN tbUserFranchise uf ON st.ClientId = uf.UserId 
         WHERE uf.FranchiseId = @pFranchiseId 
         AND st.Date BETWEEN @pStartDate AND @pEndDate) AS TotalTasks,
        
        (SELECT COUNT(*) 
         FROM tblBillingInvoice bi 
         JOIN tbUserFranchise uf ON bi.ClientId = uf.UserId 
         WHERE uf.FranchiseId = @pFranchiseId 
         AND bi.Date BETWEEN @pStartDate AND @pEndDate) AS TotalBillingInvoices,
        
        (SELECT COUNT(*) 
         FROM tblServiceProviderWage spw 
         JOIN tbUserFranchise uf ON spw.ServiceProviderId = uf.UserId 
         WHERE uf.FranchiseId = @pFranchiseId 
         AND spw.Date BETWEEN @pStartDate AND @pEndDate) AS TotalWages;

    -- Get popular services
    SELECT TOP 10
        ISNULL(stype.Name, 'Unknown') AS ServiceType,
        COUNT(*) AS Count,
        SUM(ISNULL(st.BillingAmount, 0)) AS TotalAmount
    FROM tblServicesTask st
    JOIN tblScheduler sch ON st.ScheduleId = sch.Id
    JOIN tbUserFranchise uf ON st.ClientId = uf.UserId
    LEFT JOIN tblServicesType stype ON sch.ServiceType = stype.Id
    WHERE uf.FranchiseId = @pFranchiseId 
    AND st.Date BETWEEN @pStartDate AND @pEndDate
    GROUP BY stype.Name
    ORDER BY Count DESC;

    -- Get service task statuses distribution
    SELECT 
        ISNULL(li.Name, 'Unknown') AS TaskStatus,
        COUNT(*) AS Count,
        SUM(ISNULL(st.BillingAmount, 0)) AS TotalAmount,
        CASE 
            WHEN li.Name = 'Scheduled' THEN '#5933f0c9'
            WHEN li.Name = 'In-Progress' THEN '#40c057'
            WHEN li.Name = 'Cancelled' THEN '#fab005'
            WHEN li.Name = 'Completed' THEN '#228be6'
            WHEN li.Name = 'Delayed' THEN '#fa5252'
            ELSE '#868e96'
        END AS Color
    FROM tblServicesTask st
    JOIN tblScheduler sch ON st.ScheduleId = sch.Id
    JOIN tbUserFranchise uf ON st.ClientId = uf.UserId
    LEFT JOIN tblLookupItems li ON li.LookupType = 'TaskStatus' AND li.Id = st.Status
    WHERE uf.FranchiseId = @pFranchiseId 
    AND st.Date BETWEEN @pStartDate AND @pEndDate
    GROUP BY li.Name, li.Id
    ORDER BY li.Id;

    -- Get task status distribution
    SELECT 
        CASE 
            WHEN st.IsConfirmed = 1 THEN 'Confirmed'
            WHEN st.IsConfirmed = 0 THEN 'Pending'
            ELSE 'Unknown'
        END AS Status,
        COUNT(*) AS Count,
        CASE 
            WHEN st.IsConfirmed = 1 THEN '#40c057'
            WHEN st.IsConfirmed = 0 THEN '#fab005'
            ELSE '#868e96'
        END AS Color
    FROM tblServicesTask st
    JOIN tbUserFranchise uf ON st.ClientId = uf.UserId
    WHERE uf.FranchiseId = @pFranchiseId 
    AND st.Date BETWEEN @pStartDate AND @pEndDate
    GROUP BY st.IsConfirmed;

    -- Get billing trend (last 6 months)
    WITH Months AS (
        SELECT DATEADD(MONTH, -5, @pStartDate) AS MonthDate
        UNION ALL SELECT DATEADD(MONTH, -4, @pStartDate)
        UNION ALL SELECT DATEADD(MONTH, -3, @pStartDate)
        UNION ALL SELECT DATEADD(MONTH, -2, @pStartDate)
        UNION ALL SELECT DATEADD(MONTH, -1, @pStartDate)
        UNION ALL SELECT @pStartDate
    )
    SELECT 
        FORMAT(m.MonthDate, 'MMM yyyy') AS Month,
        ISNULL(SUM(bi.TotalAmount), 0) AS Amount,
        ISNULL(COUNT(bi.Id), 0) AS Count
    FROM Months m
    LEFT JOIN tblBillingInvoice bi ON bi.Date >= m.MonthDate 
        AND bi.Date < DATEADD(MONTH, 1, m.MonthDate)
    LEFT JOIN tbUserFranchise uf ON bi.ClientId = uf.UserId
    WHERE uf.FranchiseId = @pFranchiseId OR uf.FranchiseId IS NULL
    GROUP BY m.MonthDate
    ORDER BY m.MonthDate;

    -- Get wage trend (last 6 months)
    WITH WageMonths AS (
        SELECT DATEADD(MONTH, -5, @pStartDate) AS MonthDate
        UNION ALL SELECT DATEADD(MONTH, -4, @pStartDate)
        UNION ALL SELECT DATEADD(MONTH, -3, @pStartDate)
        UNION ALL SELECT DATEADD(MONTH, -2, @pStartDate)
        UNION ALL SELECT DATEADD(MONTH, -1, @pStartDate)
        UNION ALL SELECT @pStartDate
    )
    SELECT 
        FORMAT(wm.MonthDate, 'MMM yyyy') AS Month,
        ISNULL(SUM(spw.TotalAmount), 0) AS Amount,
        ISNULL(COUNT(spw.Id), 0) AS Count
    FROM WageMonths wm
    LEFT JOIN tblServiceProviderWage spw ON spw.Date >= wm.MonthDate 
        AND spw.Date < DATEADD(MONTH, 1, wm.MonthDate)
    LEFT JOIN tbUserFranchise uf ON spw.ServiceProviderId = uf.UserId
    WHERE uf.FranchiseId = @pFranchiseId OR uf.FranchiseId IS NULL
    GROUP BY wm.MonthDate
    ORDER BY wm.MonthDate;

    -- Get billing summary
    SELECT 
        COUNT(*) AS TotalCount,
        SUM(CASE WHEN bi.IsPaid = 1 THEN 1 ELSE 0 END) AS PaidCount,
        SUM(CASE WHEN bi.IsPaid = 0 THEN 1 ELSE 0 END) AS UnpaidCount,
        SUM(ISNULL(bi.TotalAmount, 0)) AS TotalAmount,
        SUM(CASE WHEN bi.IsPaid = 1 THEN ISNULL(bi.TotalAmount, 0) ELSE 0 END) AS PaidAmount,
        SUM(CASE WHEN bi.IsPaid = 0 THEN ISNULL(bi.TotalAmount, 0) ELSE 0 END) AS UnpaidAmount
    FROM tblBillingInvoice bi
    JOIN tbUserFranchise uf ON bi.ClientId = uf.UserId
    WHERE uf.FranchiseId = @pFranchiseId 
    AND bi.Date BETWEEN @pStartDate AND @pEndDate;

    -- Get wage summary
    SELECT 
        COUNT(*) AS TotalCount,
        SUM(CASE WHEN spw.IsPaid = 1 THEN 1 ELSE 0 END) AS PaidCount,
        SUM(CASE WHEN spw.IsPaid = 0 THEN 1 ELSE 0 END) AS UnpaidCount,
        SUM(ISNULL(spw.TotalAmount, 0)) AS TotalAmount,
        SUM(CASE WHEN spw.IsPaid = 1 THEN ISNULL(spw.TotalAmount, 0) ELSE 0 END) AS PaidAmount,
        SUM(CASE WHEN spw.IsPaid = 0 THEN ISNULL(spw.TotalAmount, 0) ELSE 0 END) AS UnpaidAmount
    FROM tblServiceProviderWage spw
    JOIN tbUserFranchise uf ON spw.ServiceProviderId = uf.UserId
    WHERE uf.FranchiseId = @pFranchiseId 
    AND spw.Date BETWEEN @pStartDate AND @pEndDate;

END
