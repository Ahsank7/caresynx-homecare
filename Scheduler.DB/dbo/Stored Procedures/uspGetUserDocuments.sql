-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[uspGetUserDocuments] 	
-- Add the parameters for the stored procedure here
	@pUserId uniqueidentifier,
	@pDocumentTypeId int=null,
	@pSortColumn nvarchar(50) = null,
	@pSortType nvarchar(10) = null ,
	@PageNumber int =1,
	@pPageSize int =10
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults
		IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results

	

		 if isnull(@pDocumentTypeId,0) =0
		 set @pDocumentTypeId=null
		

		print @pUserId
		
	Select Id,
	      (select Top 1 N.Name from tblLookupItems N where N.Id=UL.DocumentTypeId and LookupType='DocumentType') AS DocumentType,
	      UL.DocumentTypeId,
		  [Name],
		  [Description],
		  DocumentPath,
		  UserId,
		  AccessRoles
	Into  #Results
	From  [dbo].[tbldocument] UL
    WHERE 1=1
	 and UL.UserId=@pUserId
	 and (@pDocumentTypeId is Null) OR (UL.DocumentTypeId =@pDocumentTypeId)


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Description' AND @pSortType ='ASC' THEN [Description] END ,
CASE WHEN @pSortColumn = 'Description' AND @pSortType ='DESC' THEN [Description] END DESC,
CASE WHEN @pSortColumn = 'Name' AND @pSortType ='ASC' THEN [Name] END ,
CASE WHEN @pSortColumn = 'Name' AND @pSortType ='DESC' THEN [Name] END DESC
OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END