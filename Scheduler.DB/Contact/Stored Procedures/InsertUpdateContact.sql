-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================



CREATE   PROCEDURE [Contact].[InsertUpdateContact] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pFirstName nvarchar(50)=null,   
	@pSurName nvarchar(50)=null,
	@pLastName  nvarchar(50)=null,   
	@pAlias nvarchar(50)=null,       
	@pPhoneNo nvarchar(20)=null,     
	@pMobileNo nvarchar(20)=null,     
	@pIdentityNo nvarchar(100)=null,  
	@pBirthDate  date=null,   
	@pCountyId int=null,    
	@pEmail nvarchar(100)=null,       
	@pAddressLine1 nvarchar(500)=null, 
	@pAddressLine2 nvarchar(500)=null,
	@pAddressLine3 nvarchar(500)=null,
	@pLatitude float=null,     
	@pLongitude float=null,    
	@pStateId int=null,     
	@pCountryId int=null,   
	@pGenderId int=null,    
	@pTitleId int=null,     
	@pCreatedBy uniqueidentifier=null,
	@pFranchiseId uniqueidentifier,
	@pNotes nvarchar(500) =null,
	@pContactTypeId int=null,
	@pContactUserId uniqueidentifier=null,
	@pIsBillingContact bit = 0


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
		DECLARE @vStatusId int
	    Select @vStatusId=Id from tblLookupItems where LookupType='UserStatus' and [Name]='Active'

INSERT INTO [dbo].[tblUser]
           (Id
		   ,[FirstName]
           ,[SurName]
           ,[LastName]
           ,[Alias]
           ,[Gender]
           ,[Title]
           ,[BirthDate]
           ,[IdentityNo]
           ,[MobileNo]
           ,[PhoneNo]
           ,[Email]
           ,[Status]
           ,[CreatedDate]
           ,[IsActive]
           ,[UserType]
		   ,[FranchiseId]   )
     VALUES(@pOutId,@pFirstName,@pSurName,@pLastName,@pAlias,@pGenderId,@pTitleId,@pBirthDate,@pIdentityNo,@pMobileNo,@pPhoneNo,@pEmail,@vStatusId,GETUTCDATE(),1,4,@pFranchiseId)   -- 4 contact 


 ------------------------------------------------------Client-----------------------------------------------

	Insert into [dbo].[tblUserContact] ( [Id], [UserId], [ContactUserId], [ContactTypeId], [IsActive], [Notes], [IsBillingContact] )
	values(NEWID(),@pOutId,@pContactUserId,@pContactTypeId,1,@pNotes, ISNULL(@pIsBillingContact,0))

 ------------------------------------------------------User Franchise-----------------------------------------------


	INSERT INTO [dbo].[tbUserFranchise]
	values(@pOutId,@pFranchiseId,1)



-----------------------------------------------------------Address-----------------------------------------------


  INSERT INTO dbo.tblUserAddress
           ([UserID]
		   ,[AddressType]
           ,[AddressLine1]
           ,[AddressLine2]
           ,[AddressLine3]
           ,[StateID]
		   ,[CountyId]
           ,[CountryID]
           ,[IsPrimaryAddress]
           ,[Latitude]
		   ,[Longitude])
     VALUES
           (@pOutID,
		    (select top 1 Id from tblLookupItems t where  t.Name='Primary' and t.LookupType='AddressType'),
            @pAddressLine1,
            @pAddressLine2,
            @pAddressLine3,
            @pStateID, 
			@pCountyId,
            @pCountryID, 
            1, 
            @pLatitude,
			@pLongitude)

	end

	else
	begin

	     declare @vUserId uniqueidentifier=null
	    select @vUserId=UserId from tblUserContact where id=@pId

		
		UPDATE [dbo].[tblUserContact]
		  SET  Notes=@pNotes
		  ,[IsBillingContact] = ISNULL(@pIsBillingContact,0)
		WHERE Id=@pId
		
		UPDATE [dbo].[tblUserAddress]
		  SET  [AddressLine1]     =@pAddressLine1
			  ,[AddressLine2]		=@pAddressLine2
			  ,[AddressLine3]		=@pAddressLine3
			  ,[StateID]			=@pStateID
			  ,[CountyId]			=@pCountyId
			  ,[CountryID]			=@pCountryID
			  ,[Latitude]			=@pLatitude
			  ,[Longitude]			=@pLongitude
		WHERE [UserId]=@vUserId


		UPDATE [dbo].[tblUser]
		  SET  [FirstName]     =@pFirstName
			  ,[SurName]	   =@pSurName
			  ,[LastName]	   =@pLastName
			  ,[Alias]		   =@pAlias
			  ,[Gender]		   =@pGenderId
			  ,[Title]		   =@pTitleId
			  ,[BirthDate]	   =@pBirthDate
			  ,[IdentityNo]	   =@pIdentityNo
			  ,[MobileNo]	   =@pMobileNo
			  ,[PhoneNo]	   =@pPhoneNo
			  ,[Email]		   =@pEmail
			  ,[UpdatedDate]   =GETUTCDATE()
			  ,[FranchiseId]   =@pFranchiseId  
		WHERE [Id]=@vUserId



		set @pOutId=@pId
	end

END