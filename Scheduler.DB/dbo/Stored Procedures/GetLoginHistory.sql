-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Get paginated login history with filtering options
-- =============================================

CREATE PROCEDURE [dbo].[GetLoginHistory]
    @pOrganizationId UNIQUEIDENTIFIER,
    @pUserId UNIQUEIDENTIFIER = NULL,
    @pUserType INT = NULL,
    @pStartDate DATETIME = NULL,
    @pEndDate DATETIME = NULL,
    @pLoginStatus NVARCHAR(20) = NULL,
    @pIPAddress NVARCHAR(45) = NULL,
    @pPageNumber INT = 1,
    @pPageSize INT = 50,
    @pSortColumn NVARCHAR(50) = 'LoginTime',
    @pSortDirection NVARCHAR(4) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@pPageNumber - 1) * @pPageSize;
    
    -- Get paginated login history entries
    SELECT 
        lh.[Id],
        lh.[UserId],
        lh.[UserName],
        lh.[UserEmail],
        lh.[UserType],
        lh.[OrganizationId],
        lh.[FranchiseId],
        lh.[LoginTime],
        lh.[LogoutTime],
        lh.[IPAddress],
        lh.[UserAgent],
        lh.[BrowserName],
        lh.[BrowserVersion],
        lh.[OperatingSystem],
        lh.[DeviceType],
        lh.[ScreenResolution],
        lh.[Timezone],
        lh.[Language],
        lh.[Country],
        lh.[City],
        lh.[LoginStatus],
        lh.[FailureReason],
        lh.[SessionDuration],
        lh.[IsActive],
        lh.[CreatedDate],
        lh.[ModifiedDate],
        -- Calculate session duration if logout time exists
        CASE 
            WHEN lh.LogoutTime IS NOT NULL 
            THEN DATEDIFF(MINUTE, lh.LoginTime, lh.LogoutTime)
            ELSE NULL 
        END AS CalculatedSessionDuration,
        -- Get user type name
        CASE lh.UserType
            WHEN 1 THEN 'Client'
            WHEN 2 THEN 'Service Provider'
            WHEN 3 THEN 'Staff'
            ELSE 'Unknown'
        END AS UserTypeName,
        -- Get franchise name
        ISNULL(f.[Name], 'N/A') AS FranchiseName
    FROM [dbo].[tblLoginHistory] lh
    LEFT JOIN [dbo].[tblFranchise] f ON f.Id = lh.FranchiseId
    WHERE lh.OrganizationId = @pOrganizationId
        AND (@pUserId IS NULL OR lh.UserId = @pUserId)
        AND (@pUserType IS NULL OR lh.UserType = @pUserType)
        AND (@pStartDate IS NULL OR lh.LoginTime >= @pStartDate)
        AND (@pEndDate IS NULL OR lh.LoginTime <= @pEndDate)
        AND (@pLoginStatus IS NULL OR lh.LoginStatus = @pLoginStatus)
        AND (@pIPAddress IS NULL OR lh.IPAddress LIKE '%' + @pIPAddress + '%')
    ORDER BY 
        CASE 
            WHEN @pSortColumn = 'LoginTime' AND @pSortDirection = 'ASC' THEN lh.LoginTime
        END ASC,
        CASE 
            WHEN @pSortColumn = 'LoginTime' AND @pSortDirection = 'DESC' THEN lh.LoginTime
        END DESC,
        CASE 
            WHEN @pSortColumn = 'UserName' AND @pSortDirection = 'ASC' THEN lh.UserName
        END ASC,
        CASE 
            WHEN @pSortColumn = 'UserName' AND @pSortDirection = 'DESC' THEN lh.UserName
        END DESC,
        CASE 
            WHEN @pSortColumn = 'IPAddress' AND @pSortDirection = 'ASC' THEN lh.IPAddress
        END ASC,
        CASE 
            WHEN @pSortColumn = 'IPAddress' AND @pSortDirection = 'DESC' THEN lh.IPAddress
        END DESC
    OFFSET @Offset ROWS FETCH NEXT @pPageSize ROWS ONLY;
END