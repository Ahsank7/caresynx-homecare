

CREATE   PROCEDURE [dbo].[InsertUpdateUserAddress] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pUserId uniqueidentifier=null,
	@pAddressTypeId  int =null, 
	@pCountyId int=null,         
	@pAddressLine1 nvarchar(500)=null, 
	@pAddressLine2 nvarchar(500)=null,
	@pAddressLine3 nvarchar(500)=null,
	@pLatitude decimal(18,2)=null,     
	@pLongitude decimal(18,2)=null,    
	@pStateId int=null,     
	@pCountryId int=null,
	@pIsPrimaryAddress bit =0,
	@pIsActive bit =1

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	print @pid

	if @pId is null
	begin
	print 'i am in if'

SET @pOutId =  NEWID()	
	

-----------------------------------------------------------Address-----------------------------------------------


  INSERT INTO dbo.tblUserAddress
           (Id
		   ,UserId
		   ,[AddressType]
           ,[AddressLine1]
           ,[AddressLine2]
           ,[AddressLine3]
           ,[StateID]
		   ,[CountyId]
           ,[CountryID]
           ,[IsPrimaryAddress]
           ,[Latitude]
		   ,[Longitude]
		   ,[IsActive])
     VALUES
           (@pOutId,
		    @pUserId,
		    @pAddressTypeId,
            @pAddressLine1,
            @pAddressLine2,
            @pAddressLine3,
            @pStateID, 
			@pCountyId,
            @pCountryID, 
            @pIsPrimaryAddress, 
            @pLatitude,
			@pLongitude,
			@pIsActive)

	end

	else
	begin
	print 'i am in else'
		UPDATE dbo.tblUserAddress
		  SET   [AddressType]      = @pAddressTypeId 
		       ,[AddressLine1]      = @pAddressLine1     
			   ,[AddressLine2]		= @pAddressLine2
			   ,[AddressLine3]		= @pAddressLine3
			   ,[StateID]			= @pStateID
			   ,[CountyId]			= @pCountyId
			   ,[CountryID]			= @pCountryID
			   ,[IsPrimaryAddress]	= @pIsPrimaryAddress
			   ,[Latitude]			= @pLatitude
			   ,[Longitude]			= @pLongitude
			   ,[IsActive] =@pIsActive

		WHERE [Id]=@pID


		SET @pOutId = @pID	

	end

END