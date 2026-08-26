-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


CREATE PROCEDURE [Contact].[DeleteUserContact] --[CLIENT].[DeleteServiceProvider] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	declare @vUserId uniqueidentifier=null
	select @vUserId=UserId from tblUserContact where id=@pId
	
	Update [dbo].[tblUser]
	Set IsActive=0
	Where [Id]=@vUserId

	UPDATE [dbo].[tblUserContact]
		  SET  IsActive=0
		WHERE Id=@pId


END