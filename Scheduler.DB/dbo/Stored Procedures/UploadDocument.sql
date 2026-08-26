-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


CREATE  PROCEDURE [dbo].[UploadDocument] 
	-- Add the parameters for the stored procedure here
	@pOutId int=null output,
	@pDocumentTypeId int=null,   
	@pName nvarchar(50)=null,
	@pDescription nvarchar(500)=null,
	@pAccessRoles  nvarchar(50)=null,  
	@pUserId uniqueidentifier,
	@pDocumentPath nvarchar(500) =null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here


INSERT INTO [dbo].[tblDocument]
           ([DocumentTypeId]
           ,[Name]
           ,[Description]
           ,[AccessRoles]
           ,[UserId]
           ,[DocumentPath])
     VALUES
           (@pDocumentTypeId
		   ,@pName
		   ,@pDescription
           ,@pAccessRoles
           ,@pUserId
           ,@pDocumentPath)

SET @pOutId =  SCOPE_IDENTITY()	

END