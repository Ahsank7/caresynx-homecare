
  

CREATE PROCEDURE [Franchise].[InsertUpdateFranchise] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pName nvarchar(50)=null,   
	@pDescription nvarchar(50)=null,
	@pLogo nvarchar(30)=null,
	@pOrganizationId uniqueidentifier=null,
	@pUserId uniqueidentifier=null,
	@pIsActive bit = 1


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	if @pId is null
	begin


SET @pOutId =  NEWID()	
	
	-----------------------------------------------------Organization Basic Info--------------------------------------------


INSERT INTO [dbo].[tblFranchise]
           (Id
		   ,[Name]
		   ,[IsActive]
           ,[Description]
		   ,[OrganizationId])
     VALUES(@pOutId,@pName,@pIsActive,@pDescription,@pOrganizationId)

	 if(@pUserId is not null)
	 begin
	  
	    INSERT INTO [dbo].[tbUserFranchise]
	    values(@pUserId,@pOutId,1)

	 end


	end

	else
	begin

		UPDATE [dbo].[tblFranchise]
		  SET  [Name]     =@pName
				,[IsActive]	   =@pIsActive
			  ,[Description]	   =@pDescription
			  ,[OrganizationId]=@pOrganizationId
			  
		WHERE [Id]=@pId

		SET @pOutId = @pId	


	end

END