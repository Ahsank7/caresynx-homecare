-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================



CREATE   PROCEDURE [dbo].[InsertUpdateUserAvailability] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pUserId  uniqueidentifier=null,
	@pStartTime  time(7)=null,
	@pEndTime  time(7)=null,
	@pDay  nvarchar(12)=null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	if @pId is null
	begin


         SET @pOutId =  NEWID()	

		 INSERT INTO [dbo].[tblUserAvailability]
           ([Id]
           ,[UserId]
           ,[StartTime]
           ,[EndTime]
           ,[Day]
           ,[IsActive])
     VALUES
           (@pOutId
           ,@pUserId
           ,@pStartTime
           ,@pEndTime
           ,@pDay
           ,1)
	

	end

	else
	begin

   UPDATE [dbo].[tblUserAvailability]
   SET [StartTime] = @pStartTime,
       [EndTime] = @pEndTime,
       [Day] = @pDay
 WHERE id=@pId


		set @pOutId=@pId
	end

END