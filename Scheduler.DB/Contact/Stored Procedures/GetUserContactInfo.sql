-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [Contact].[GetUserContactInfo] @pID='B5BAA3E6-C962-4A33-B67A-ADC8CA6AE41D'
CREATE   PROCEDURE [Contact].[GetUserContactInfo]
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select 
		     C.Id
		    ,U.[Id] as UserId
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
			,C.Notes
			,C.ContactTypeId
			,C.IsBillingContact
			,UA.Id as UserAddressId
			,UA.AddressType
			,UA.AddressLine1
			,UA.AddressLine2
			,UA.AddressLIne3
			,UA.StateId
			,UA.CountyId
			,UA.CountryId
			,UA.Latitude
			,UA.Longitude
	From  [dbo].[tblUser] U
	JOIN [dbo].tblUserContact C on C.UserId=U.Id
	JOIN [dbo].tblUserAddress UA on UA.UserId=U.Id and UA.IsPrimaryAddress=1
    WHERE C.[Id]=@pID




END