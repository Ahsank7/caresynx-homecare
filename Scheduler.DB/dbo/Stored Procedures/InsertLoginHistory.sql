-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Insert login history record with system and browser information
-- =============================================

CREATE PROCEDURE [dbo].[InsertLoginHistory]
    @pUserId UNIQUEIDENTIFIER,
    @pUserName NVARCHAR(100),
    @pUserEmail NVARCHAR(255) = NULL,
    @pUserType INT,
    @pOrganizationId UNIQUEIDENTIFIER,
    @pFranchiseId UNIQUEIDENTIFIER = NULL,
    @pIPAddress NVARCHAR(45) = NULL,
    @pUserAgent NVARCHAR(500) = NULL,
    @pBrowserName NVARCHAR(100) = NULL,
    @pBrowserVersion NVARCHAR(50) = NULL,
    @pOperatingSystem NVARCHAR(100) = NULL,
    @pDeviceType NVARCHAR(50) = NULL,
    @pScreenResolution NVARCHAR(50) = NULL,
    @pTimezone NVARCHAR(100) = NULL,
    @pLanguage NVARCHAR(20) = NULL,
    @pCountry NVARCHAR(100) = NULL,
    @pCity NVARCHAR(100) = NULL,
    @pLoginStatus NVARCHAR(20) = 'Success',
    @pFailureReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[tblLoginHistory] (
        [UserId], [UserName], [UserEmail], [UserType], [OrganizationId], [FranchiseId],
        [IPAddress], [UserAgent], [BrowserName], [BrowserVersion], [OperatingSystem],
        [DeviceType], [ScreenResolution], [Timezone], [Language], [Country], [City],
        [LoginStatus], [FailureReason]
    )
    VALUES (
        @pUserId, @pUserName, @pUserEmail, @pUserType, @pOrganizationId, @pFranchiseId,
        @pIPAddress, @pUserAgent, @pBrowserName, @pBrowserVersion, @pOperatingSystem,
        @pDeviceType, @pScreenResolution, @pTimezone, @pLanguage, @pCountry, @pCity,
        @pLoginStatus, @pFailureReason
    );
    
    SELECT SCOPE_IDENTITY() AS LoginHistoryId;
END