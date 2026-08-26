-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================



CREATE   PROCEDURE [User].[InsertUpdateUser] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pFirstName nvarchar(50)=null,   
	@pSurName nvarchar(50)=null,
	@pLastName  nvarchar(50)=null,   
	@pAlias nvarchar(50)=null,     
	@pUserName nvarchar(50)=null,    
	@pPhoneNo nvarchar(20)=null,     
	@pMobileNo nvarchar(20)=null,    
	@pPassportNo nvarchar(100)=null,  
	@pIdentityNo nvarchar(100)=null,  
	@pEthnicityId int= null,
	@pMaritalStatusId int=null,
	@pAge int=0,         
	@pBirthDate  date=null,   
	@pJoiningDate date=null, 
	@pCountyId int=0,    
	@pEmail nvarchar(100)=null,       
	@pAddressLine1 nvarchar(500)=null, 
	@pAddressLine2 nvarchar(500)=null,
	@pAddressLine3 nvarchar(500)=null,
	@pLatitude decimal(18,2)=null,     
	@pLongitude decimal(18,2)=null,    
	@pStateId int=0,     
	@pNationalityId int=null,
	@pCountryId int=null,   
	@pGenderId int=null,    
	@pTitleId int=null,     
	@pPasswordHash nvarchar(MAX) =null,
	@pCreatedBy uniqueidentifier=null,
	@pFranchiseId uniqueidentifier,
	@pNotes nvarchar(500) =null,
	@pUserType int null, -- 1-Client 2-ServiceProvider 3-Staff
	@pAddressId uniqueidentifier = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Check for duplicate email
    IF EXISTS (SELECT 1 FROM [dbo].[tblUser] WHERE Email = @pEmail AND ISNULL(Email,'')<>'' AND IsActive = 1 AND (@pId IS NULL OR Id != @pId))
    BEGIN
        RAISERROR('Email already exists', 16, 1);
        RETURN;
    END
    
    -- Check for duplicate phone number
    IF EXISTS (SELECT 1 FROM [dbo].[tblUser] WHERE PhoneNo = @pPhoneNo AND ISNULL(PhoneNo,'')<>'' AND IsActive = 1 AND (@pId IS NULL OR Id != @pId))
    BEGIN
        RAISERROR('Phone number already exists', 16, 1);
        RETURN;
    END
    
    -- Check for duplicate mobile number
    IF EXISTS (SELECT 1 FROM [dbo].[tblUser] WHERE   MobileNo = @pMobileNo AND ISNULL(MobileNo,'')<>'' AND IsActive = 1 AND (@pId IS NULL OR Id != @pId))
    BEGIN
        RAISERROR('Mobile number already exists', 16, 1);
        RETURN;
    END

    -- Insert statements for procedure here

	if @pStateId is null
	begin 
	   set @pStateId = 0
	end

	if @pCountryId is null
	begin 
	   set @pCountryId = 0
	end

	if @pCountyId is null
	begin 
	   set @pCountyId = 0
	end

	if @pId is null
	begin


SET @pOutId =  NEWID()	

	-----------------------------------------------------User Basic Info--------------------------------------------

	DECLARE @vStatusId int,@vRoleId int =0

	DECLARE @Year NVARCHAR(4) = CONVERT(NVARCHAR(4), YEAR(GETDATE()));  -- Current year
    DECLARE @RandomNumber NVARCHAR(5) = RIGHT(CAST(ABS(CHECKSUM(NEWID())) AS NVARCHAR), 5);

   -- Combine the parts to form the UserNo
    DECLARE @pUserNo NVARCHAR(20) = @Year + '-' + Cast(@pUserType as varchar) + '-' + @RandomNumber;


	Select @vStatusId=Id from tblLookupItems where LookupType='UserStatus' and [Name]='Active'
	--Select @vRoleId=Id from tblLookupItems where LookupType='Role' and OtherFieldValue1=@pUserType -- [Name]='User'

INSERT INTO [dbo].[tblUser]
           (Id
		   ,[FirstName]
           ,[SurName]
           ,[LastName]
           ,[Alias]
           ,[Age]
           ,[Gender]
           ,[MaritalStatus]
           ,[Title]
           ,[Ethnicity]
           ,[BirthDate]
           ,[JoiningDate]
           ,[PassportNo]
           ,[IdentityNo]
           ,[MobileNo]
           ,[PhoneNo]
           ,[Email]
           ,[Status]
           ,[CreatedDate]
           ,[IsActive]
           ,[UserType]
           ,[NationalityId]
		   ,[FranchiseId] 
		   ,UserNo
		   ,Notes)
     VALUES(@pOutId,@pFirstName,@pSurName,@pLastName,@pAlias,@pAge,@pGenderId,@pMaritalStatusId,@pTitleId,@pEthnicityId,@pBirthDate,@pJoiningDate,@pPassportNo,@pIdentityNo,@pMobileNo,@pPhoneNo,@pEmail,@vStatusId,GETUTCDATE(),1,@pUserType,@pNationalityId,@pFranchiseId,@pUserNo,@pNotes)


 ------------------------------------------------------User-----------------------------------------------

 IF @pUserType=1
 BEGIN
	Insert into [dbo].[tblClient]
	values(NEWID(),@pOutId,1,GETDATE())
END

 IF @pUserType=2
 BEGIN
	Insert into [dbo].[tblServiceProvider]
	values(NEWID(),@pOutId,1,GETDATE())
END

 IF @pUserType=3
 BEGIN
	Insert into [dbo].[tblStaff]
	values(NEWID(),@pOutId,1,GETDATE())

	-- Get default Staff role from new tblRole table
	Select @vRoleId=Id from tblRole where [Name]='Staff' AND IsActive=1
END
 ------------------------------------------------------User Franchise-----------------------------------------------


	INSERT INTO [dbo].[tbUserFranchise]
	values(@pOutId,@pFranchiseId,1)


---------------------------------------------------------User Login--------------------------------
	INSERT INTO [dbo].[tblUserLogin]
values(@pOutId,@pUserName,@pPasswordHash,1)


---------------------------------------------------------Uaer Role------------------------------

IF @vRoleId >0
begin

INSERT INTO [dbo].[tblUserRole](
            [Id]
           ,[UserId]
           ,[RoleId]
           ,[IsActive]
           ,[CreatedDate])
values( NEWID(),@pOutId,@vRoleId,1,GETUTCDATE())

end



-----------------------------------------------------------Address-----------------------------------------------


  INSERT INTO dbo.tblUserAddress
           (ID
		   ,[UserID]
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
           (NEWID()	
		   ,@pOutId,
		    (select top 1 Id from tblLookupItems t where  t.Name='Primary' and LookupType='AddressType'),
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

		UPDATE [dbo].[tblUser]
		  SET  [FirstName]     =@pFirstName
			  ,[SurName]	   =@pSurName
			  ,[LastName]	   =@pLastName
			  ,[Alias]		   =@pAlias
			  ,[Age]		   =@pAge
			  ,[Gender]		   =@pGenderId
			  ,[MaritalStatus] =@pMaritalStatusId
			  ,[Title]		   =@pTitleId
			  ,[Ethnicity]	   =@pEthnicityId
			  ,[BirthDate]	   =@pBirthDate
			  ,[JoiningDate]   =@pJoiningDate
			  ,[PassportNo]	   =@pPassportNo
			  ,[IdentityNo]	   =@pIdentityNo
			  ,[MobileNo]	   =@pMobileNo
			  ,[PhoneNo]	   =@pPhoneNo
			  ,[Email]		   =@pEmail
			  ,[UpdatedDate]   =GETUTCDATE()
			  ,[NationalityId] =@pNationalityId
			 -- ,[FranchiseId]   =@pFranchiseId  
			  ,Notes = @pNotes
		WHERE [Id]=@pID

		UPDATE dbo.tblUserAddress
		SET  AddressLine1 = @pAddressLine1
		    ,AddressLine2 = @pAddressLine2
			,AddressLine3 = @pAddressLine3
			,CountryId = @pCountryId
			,CountyId = @pCountyId
			,StateId = @pStateId
			,Latitude = @pLatitude
			,Longitude = @pLongitude
		WHERE 
			Id = @pAddressId
			

		SET @pOutId = @pID	


	end

END