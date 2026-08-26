-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [CLIENT].[GetClientInfo] @pID='6A1E3D10-C58F-4653-830F-A2E3642880C8'
CREATE PROCEDURE [CLIENT].[GetClientInfo] --[CLIENT].[GetClientInfo] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


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
			,1 as UserType
			,UA.Id as AddressId 
			,UA.AddressType as AddressTypeId
			,UA.AddressLine1
			,UA.AddressLine2
			,UA.AddressLIne3
			,UA.StateId
			,UA.CountyId
			,UA.CountryId
			,UA.Latitude
			,UA.Longitude
			,U.IsActive
			,U.Notes
			,U.UserNo
	From  [dbo].[tblUser] U
	JOIN [dbo].tblClient C on C.UserId=U.Id
	Left JOIN [dbo].tblUserAddress UA on UA.UserId=U.Id and IsPrimaryAddress=1 
    WHERE U.[Id]=@pID




END