-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


CREATE   PROCEDURE [Lookup].[DeleteLookupItem] 
	-- Add the parameters for the stored procedure here
	@pId int


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	
	Update [dbo].[tblLookupItems]
	Set IsActive=0
	Where [Id]=@pId


END