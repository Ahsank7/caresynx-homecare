-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================



CREATE  PROCEDURE [dbo].[InsertUpdateUserLeave] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pType int=null,   
	@pStatus int=null,
	@pDate  date=null,  
	@pStartTime  datetime=null,  
	@pEndTime  datetime=null,  
	@pCreatedBy uniqueidentifier=null,
	@pUserId uniqueidentifier,
	@pNotes nvarchar(500) =null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	if @pId is null
	begin


     SET @pOutId =  NEWID()	
	
	print 'if'

	-----------------------------------------------------Leave Info--------------------------------------------
	INSERT INTO [dbo].[tblUserLeave]
           ([Id]
           ,[UserId]
           ,[Type]
           ,[Status]
           ,[Date]
           ,[StartTime]
           ,[EndTime]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[IsActive]
           ,[Notes])
     VALUES
           (@pOutId
           ,@pUserId
           ,@pType
           ,@pStatus
           ,@pDate
           ,@pStartTime
           ,@pEndTime
           ,GETUTCDATE()
           ,@pCreatedBy
           ,1
           ,@pNotes)


	end

	Else
	Begin

	print 'else'

	      Update [dbo].[tblUserLeave]
		  Set [Type] = @pType
           ,[Status] = @pStatus
           ,[Date] = @pDate
           ,[StartTime] = @pStartTime
           ,[EndTime] = @pEndTime
           ,[Notes] = @pNotes
	      where Id=@pId

		 SET @pOutId = @pID	

	end


END