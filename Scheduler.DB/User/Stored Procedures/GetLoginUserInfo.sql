-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

CREATE PROCEDURE [User].[GetLoginUserInfo] 
	-- Add the parameters for the stored procedure here
	@pLogin nvarchar(50),
	@pPassword nvarchar(50)


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select U.[Id] as UserId
			,U.[FirstName]    
			,U.[SurName]	  
			,U.[LastName]	  
			,U.[Alias]		  
			,U.[Age]		  
			,U.[Gender]	as GenderId	  
			,U.[MaritalStatus] as MaritalStatusId	  
			,U.[Title] as TitleId		  
			,U.[Ethnicity] as EthnicityId	  
			,U.[BirthDate]	  
			,U.[JoiningDate]  
			,U.[PassportNo]	  
			,U.[IdentityNo]	  
			,U.[MobileNo]	  
			,U.[PhoneNo]	  
			,U.[Email]		  
			,U.[UpdatedDate]  
			,U.[NationalityId]
			,U.[FranchiseId] 
			,U.[Status] as statusId
			,UserType
			,U.IsActive
			,U.Notes
			,U.UserNo
			,UR.RoleId
			,f.OrganizationId
			,ProfileImagePath
	From  [dbo].[tblUser] U 
	LEFt JOIN tblFranchise F on F.Id=U.FranchiseId
	LEFT JOIN tblUserRole UR on UR.UserId=U.Id AND UR.IsActive=1
    WHERE U.[UserName]=@pLogin and U.Password= @pPassword




END