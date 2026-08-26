-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Get total count of login history records for pagination
-- =============================================

CREATE PROCEDURE [dbo].[GetLoginHistoryCount]
    @pOrganizationId UNIQUEIDENTIFIER,
    @pUserId UNIQUEIDENTIFIER = NULL,
    @pUserType INT = NULL,
    @pStartDate DATETIME = NULL,
    @pEndDate DATETIME = NULL,
    @pLoginStatus NVARCHAR(20) = NULL,
    @pIPAddress NVARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS TotalRecords 
    FROM [dbo].[tblLoginHistory] lh
    WHERE lh.OrganizationId = @pOrganizationId
        AND (@pUserId IS NULL OR lh.UserId = @pUserId)
        AND (@pUserType IS NULL OR lh.UserType = @pUserType)
        AND (@pStartDate IS NULL OR lh.LoginTime >= @pStartDate)
        AND (@pEndDate IS NULL OR lh.LoginTime <= @pEndDate)
        AND (@pLoginStatus IS NULL OR lh.LoginStatus = @pLoginStatus)
        AND (@pIPAddress IS NULL OR lh.IPAddress LIKE '%' + @pIPAddress + '%');
END