-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================



CREATE  PROCEDURE [dbo].[InsertUpdateUserExpense] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pType int=null,   
	@pTaskId int=null,
	@pAmount decimal(18,2)=null,
	@pDate  date=null,  
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
	
	-----------------------------------------------------Expense Info--------------------------------------------
	INSERT INTO [dbo].[tblUserExpense]
           ([Id]
           ,[UserId]
           ,[Date]
           ,[TaskId]
           ,[Type]
           ,[Amount]
           ,[IsPaid]
           ,[CreatedAt]
           ,[CreatedBy]
           ,[IsActive]
           ,[IsConfirmed]
           ,[Notes])
     VALUES
           (@pOutId
           ,@pUserId
           ,@pDate
		   ,@pTaskId
           ,@pType
           ,@pAmount
           ,0
           ,GETUTCDATE()
           ,@pCreatedBy
           ,1
		   ,0
           ,@pNotes)


	end
	else
	begin

		UPDATE [dbo].[tblUserExpense]
		SET [UserId]=@pUserId
           ,[Date]=@pDate
           ,[TaskId]=@pTaskId
           ,[Type]=@pType
           ,[Amount]=@pAmount
           ,[IsPaid]=0
           ,[CreatedAt]=GETUTCDATE()
           ,[CreatedBy]=@pCreatedBy
           ,[IsActive]=1
           ,[IsConfirmed]=0
           ,[Notes]=@pNotes
		   WHERE [ID]=@pId
		   SET @pOutId = @pID	
		   end
END