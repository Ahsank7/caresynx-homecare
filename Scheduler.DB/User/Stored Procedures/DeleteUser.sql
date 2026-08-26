-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================
CREATE PROCEDURE [User].[DeleteUser] --[User].[DeleteUser] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pUserId uniqueidentifier
   ,@pUserStatusAction INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @vLookupUpID INT = 0

	IF(@pUserStatusAction = 1)
	BEGIN

		SELECT top 1 @vLookupUpID  = l.Id
		FROM  tblLookupItems l where LookupType='UserStatus' and Name='InActive'
		--	 [dbo].tbUserFranchise uf
		--JOIN tblFranchise f ON f.Id = uf.FranchiseId
		--JOIN tblOrganization o ON o.Id = f.OrganizationId
		--JOIN tblLookupItems l ON l.OrganizationId = f.OrganizationId
		--WHERE
		--	 uf.UserId = @pUserId
		--AND  l.[Name] LIKE '%In-Active%'
	END

	IF(@pUserStatusAction = 2)
	BEGIN

		SELECT top 1 @vLookupUpID  = l.Id
		FROM  tblLookupItems l where LookupType='UserStatus' and Name='Deactivated'

		--SELECT @vLookupUpID  = l.Id
		--FROM 
		--	 [dbo].tbUserFranchise uf
		--JOIN tblFranchise f ON f.Id = uf.FranchiseId
		--JOIN tblOrganization o ON o.Id = f.OrganizationId
		--JOIN tblLookupItems l ON l.OrganizationId = f.OrganizationId
		--WHERE
		--	 uf.UserId = @pUserId
		--AND  l.[Name] LIKE '%Deactivated%'
	END

	IF(@pUserStatusAction = 3)
	BEGIN

		SELECT top 1 @vLookupUpID  = l.Id
		FROM  tblLookupItems l where LookupType='UserStatus' and Name='Active'

		--SELECT @vLookupUpID  = l.Id
		--FROM 
		--	 [dbo].tbUserFranchise uf
		--JOIN tblFranchise f ON f.Id = uf.FranchiseId
		--JOIN tblOrganization o ON o.Id = f.OrganizationId
		--JOIN tblLookupItems l ON l.OrganizationId = f.OrganizationId
		--WHERE
		--	 uf.UserId = @pUserId
		--AND  l.[Name] LIKE 'Active%'
	END

	Update  u
	SET  [Status] = @vLookupUpID
	   , UpdatedDate = GETUTCDATE()
	FROM 
		  [dbo].[tblUser] u 
	Where 
		[Id] = @pUserId

	SELECT CASE WHEN @@ROWCOUNT > 0 THEN @pUserId ELSE '00000000-0000-0000-0000-000000000000' END 

END