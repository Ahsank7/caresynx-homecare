
  

CREATE   PROCEDURE [Lookup].[InsertUpdateLookup] 
	-- Add the parameters for the stored procedure here
	@pId int=0,
	@pOutId int=0 output,
	@pName nvarchar(100)=null,   
	@pDescription nvarchar(500)=null,
	@pLookupType nvarchar(100)=null,
	@pIsActive bit = 1,
	@pLoggedInUserId uniqueidentifier =null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	IF @pId <= 0
	BEGIN

	INSERT INTO [dbo].[tblLookupItems]
           ([LookupType]
           ,[Name]
           ,[Description]
           ,[IsActive])
     VALUES
           (
		    @pLookupType,
			@pName,
			@pDescription,
			1
		   )
   
     SET @pOutId = SCOPE_IDENTITY()

	END

	ELSE
	BEGIN

	   UPDATE [dbo].[tblLookupItems]
	   SET [Name] = @pName
		  ,[Description] = @pDescription
		  ,[IsActive] =@pIsActive
		WHERE [Id]=@pId

		SET @pOutId = @pId	


	END

END