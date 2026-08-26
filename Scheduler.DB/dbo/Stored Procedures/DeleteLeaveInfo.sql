-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


create PROCEDURE [dbo].[DeleteLeaveInfo] --[dbo].[DeleteLeaveInfo] '5DC92804-1ACE-4D6A-A2E6-D642220C2ED9'
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	
	Update [dbo].[tblUserLeave]
	Set IsActive=0
	Where [Id]=@pId


END