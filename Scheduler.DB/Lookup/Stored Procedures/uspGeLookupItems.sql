-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================



--exec [ServiceProvider].[uspGetAllServiceProviders]    @pServiceProviderId =null,	@pUserId =null,	@pEmail =null,	@pPhoneNumber =null,	@pJoiningDate =null,	@pLastName =null,	@pFirstName =null,	@pGenderId =null,	@pEthnicityId =null,	@pStatusId =null,	@pSortColumn  =null,	@pSortType =null ,	@PageNumber  =1,	@pPageSize  =10

CREATE   PROCEDURE [Lookup].[uspGeLookupItems] 	
-- Add the parameters for the stored procedure here
	@pIsActive bit=null,
	@pName nvarchar(100)=null,
	@pLookupType nvarchar(100)=null,
	@pSortColumn nvarchar(50) = N'id',
	@pSortType nvarchar(10) = N'asc' ,
	@pPageNumber int =1,
	@pPageSize int =10
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults
		IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results

	
		 if isnull(@pName,'') =''
		 set @pName=null

		
		
SELECT [Id]
      ,[LookupType]
      ,[Name]
      ,[Description]
      ,[IsActive]
  Into  #Results
  FROM [dbo].[tblLookupItems] U

  WHERE 1=1
 and U.LookupType = @pLookupType
 and @pIsActive is Null OR ISNULL(U.IsActive,0) = @pIsActive
 and (@pName is Null OR U.[Name] like '%'+@pName+'%')


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'LookupType' AND @pSortType ='ASC' THEN LookupType END ,
CASE WHEN @pSortColumn = 'LookupType' AND @pSortType ='DESC' THEN LookupType END DESC,
CASE WHEN @pSortColumn = 'Name' AND @pSortType ='ASC' THEN [Name] END ,
CASE WHEN @pSortColumn = 'Name' AND @pSortType ='DESC' THEN [Name] END DESC,
CASE WHEN @pSortColumn = 'IsActive' AND @pSortType ='ASC' THEN IsActive END ,
CASE WHEN @pSortColumn = 'IsActive' AND @pSortType ='DESC' THEN IsActive END DESC

OFFSET (@pPageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END