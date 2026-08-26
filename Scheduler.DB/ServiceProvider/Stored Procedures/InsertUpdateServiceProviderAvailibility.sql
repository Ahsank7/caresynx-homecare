-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


--declare @vId uniqueidentifier,@vFranchiseId uniqueidentifier;
--select top 1 @vFranchiseId=id from tblfranchise

--exec [ServiceProvider.[InsertUpdateServiceProvider] 
--	-- Add the parameters for the stored procedure here
--	@pId =@vId output,
--	@pFirstName ='Imran',   
--	@pSurName ='khan',
--	@pLastName  ='niazi',   
--	@pAlias ='IK',     
--	@pUserName ='ikpti',    
--	@pPhoneNo ='+92515963549',     
--	@pMobileNo ='+923465415102',    
--	@pPassportNo ='88986786868',  
--	@pIdentityNo ='7775765645645',  
--	@pEthnicityId = 1,
--	@pMaritalStatusId =1,
--	@pAge =10,         
--	@pBirthDate  ='2023-04-02',   
--	@pJoiningDate ='2023-04-02', 
--	@pCountyId =1,    
--	@pEmail ='test@test.com',       
--	@pAddressLine1 ='address1', 
--	@pAddressLine2 ='address2',
--	@pAddressLine3 ='address3',
--	@pLatitude =92.99,     
--	@pLongitude =66.98,    
--	@pStateId =1,     
--	@pNationalityId =1,
--	@pCountryId =1,   
--	@pGenderId =1,    
--	@pTitleId =1,     
--	@pPasswordHash  ='ahsank7',
--	@pFranchiseId =@vFranchiseId
-- @pNotes ='gjg uyfyuf'

--	select @vId


CREATE   PROCEDURE [ServiceProvider].[InsertUpdateServiceProviderAvailibility] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pAvailableDays nvarchar(500)=null,   
	@pStateTime datetime,
	@pEndTime datetime,
	@pServiceProviderId uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	if @pId is null
	begin


SET @pOutId =  NEWID()	
	
	-----------------------------------------------------User Basic Info--------------------------------------------


INSERT INTO [dbo].[tblServiceProviderAvailability]
           (Id
		   ,[AvailableDays]
		   ,[StartTime]
		   ,[EndTime]
		   ,[ServiceProviderUserId])
     VALUES(@pOutId,@pAvailableDays,@pStateTime,@pEndTime,@pServiceProviderId)

end

	else
	begin

		UPDATE [dbo].[tblServiceProviderAvailability]
		  SET  [AvailableDays]     =@pAvailableDays
			  ,[StartTime]	   =@pStateTime
			  ,[EndTime]	   =@pEndTime
			  ,[ServiceProviderUserId]		   =@pServiceProviderId
		WHERE [Id]=@pID

		SET @pOutId = @pID	


	end

END