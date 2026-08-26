-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


CREATE PROCEDURE [dbo].[DeleteDocument] --[dbo].[DeleteExpenseInfo] '5DC92804-1ACE-4D6A-A2E6-D642220C2ED9'
	-- Add the parameters for the stored procedure here
	@pId int


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	Delete From dbo.tblDocument
	Where [Id]=@pId


END