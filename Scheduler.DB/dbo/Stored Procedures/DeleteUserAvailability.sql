-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


Create PROCEDURE [dbo].[DeleteUserAvailability] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	
	UPDATE [dbo].[tblUserAvailability]
		  SET  IsActive=0
		WHERE Id=@pId


END