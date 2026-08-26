-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [ServiceProvider].[GetServiceProviderInfo] @pID='B5BAA3E6-C962-4A33-B67A-ADC8CA6AE41D'
CREATE PROCEDURE [ServiceProvider].[GetServiceProviderInfo] --[CLIENT].[GetClientInfo] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select 
		     U.[Id] as UserId
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
			,U.UserType
			,U.IsActive
			,'Notes' as Notes
	From  [dbo].[tblUser] U
	JOIN [dbo].tblServiceProvider C on C.UserId=U.Id
    WHERE U.[Id]=@pID




END