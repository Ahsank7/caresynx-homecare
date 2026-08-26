
CREATE PROCEDURE [User].[CheckUserNameExists] 
	@pUserName NVARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Check if username exists in any franchise
	SELECT 
		COUNT(*) AS UserNameExists,
		CASE 
			WHEN COUNT(*) > 0 THEN 1 
			ELSE 0 
		END AS IsUserNameExists,
		U.Id AS UserId 
	FROM [dbo].[tblUser] U
	INNER JOIN [dbo].[tblFranchise] F ON U.FranchiseId = F.Id
	WHERE U.UserName = @pUserName
		AND U.IsActive = 1
		AND F.IsActive = 1
	Group By U.Id

	-- Optional: Return detailed information about existing usernames
	SELECT 
		U.Id AS UserId,
		U.UserName,
		U.FirstName,
		U.LastName,
		U.Email,
		F.Id AS FranchiseId,
		F.Name AS FranchiseName,
		O.Id AS OrganizationId,
		O.Name AS OrganizationName
	FROM [dbo].[tblUser] U
	INNER JOIN [dbo].[tblFranchise] F ON U.FranchiseId = F.Id
	INNER JOIN [dbo].[tblOrganization] O ON F.OrganizationId = O.Id
	WHERE U.UserName = @pUserName
		AND U.IsActive = 1
		AND F.IsActive = 1;

END