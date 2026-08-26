-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


Create PROCEDURE [ServiceProvider].[DeleteServiceProvider] --[CLIENT].[DeleteServiceProvider] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pUserId uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	
	Update [dbo].[tblUser]
	Set IsActive=0
	Where [Id]=@pUserId


END