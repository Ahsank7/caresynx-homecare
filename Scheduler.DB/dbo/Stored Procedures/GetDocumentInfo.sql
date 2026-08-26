-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [dbo].[GetUserExpenseInfo] @pID='5DC92804-1ACE-4D6A-A2E6-D642220C2ED9'
CREATE   PROCEDURE [dbo].[GetDocumentInfo]
	-- Add the parameters for the stored procedure here
	@pID int


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	Select	
	   d.[Id]
      ,d.[UserId]
      ,d.[DocumentTypeId]
	  ,(select Top 1 N.Name from tblLookupItems N where N.Id=d.DocumentTypeId and LookupType='DocumentType') [DocumentType]
      ,d.[Name]
      ,d.[Description]
      ,d.[AccessRoles]
	  ,d.[DocumentPath]
	From  dbo.tbldocument d
    WHERE 1=1
    and d.[Id]=@pID




END