-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


--exec [Contact].[uspGetAllAvailability]    @pUserId='B821366E-FE29-4884-8859-1ED6316F7558',	@pSortColumn  ='FirstName',	@pSortType ='asc' ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [dbo].[uspGetAllAvailability] 	
-- Add the parameters for the stored procedure here
	@pUserId Nvarchar(50)=null,
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

		
		SELECT [Id]
			  ,[UserId]
			  ,Cast([StartTime] as nvarchar(8)) [StartTime]
			  ,Cast([EndTime] as nvarchar(8)) [EndTime]
			  ,[Day]
			  ,[IsActive]
	  Into  #Results
	  FROM [dbo].[tblUserAvailability]
		WHERE 1=1
		 and  IsNULL(IsActive,0)=1
		 and UserId=@pUserId
		 


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='ASC' THEN UserId END ,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='DESC' THEN UserId END DESC,
CASE WHEN @pSortColumn = 'StartTime' AND @pSortType ='ASC' THEN StartTime END ,
CASE WHEN @pSortColumn = 'StartTime' AND @pSortType ='DESC' THEN StartTime END DESC,
CASE WHEN @pSortColumn = 'EndTime' AND @pSortType ='ASC' THEN EndTime END ,
CASE WHEN @pSortColumn = 'EndTime' AND @pSortType ='DESC' THEN EndTime END DESC,
CASE WHEN @pSortColumn = 'Day' AND @pSortType ='ASC' THEN [Day] END ,
CASE WHEN @pSortColumn = 'Day' AND @pSortType ='DESC' THEN [Day] END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END