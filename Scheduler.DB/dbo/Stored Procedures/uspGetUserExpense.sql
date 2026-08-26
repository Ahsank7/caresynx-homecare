-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

--select * from [dbo].[tblUserLeave]
--exec [dbo].[uspGetUserExpense]    @pUserId='B821366E-FE29-4884-8859-1ED6316F7558',	@pSortColumn  ='FirstName',	@pSortType ='asc' ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [dbo].[uspGetUserExpense] 	
-- Add the parameters for the stored procedure here
	@pUserId uniqueidentifier,
	@pDate date=null,
	@pTypeId int=null,
	@pTaskId int=null,
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

	

		 if isnull(@pTypeId,0) =0
		 set @pTypeId=null

		 if isnull(@pTaskId,0) =0
		 set @pTaskId=null

		print @pUserId
		
		Select 
		       UL.[Id]
			  ,UL.[UserId]
			  ,UL.[Date]
			  ,UL.[TaskId]
			  ,UL.[Amount]
			  ,UL.[IsPaid]
			  ,UL.[IsActive]
			  ,UL.[IsConfirmed]
			  ,UL.Notes
			,(select Top 1 N.Name from tblLookupItems N where LookupType='ExpenseType' and N.Id=UL.[Type]) [Type]
	Into  #Results
	From  [dbo].[tblUserExpense] UL
    WHERE 1=1
	 and IsNULL(UL.IsActive,0)=1
	 and UL.UserId=@pUserId
	 and (@pDate is Null OR UL.Date =@pDate) 
	 and (@pTypeId is Null OR UL.Type =@pTypeId)
	 and (@pTaskId is Null OR UL.TaskId =@pTaskId)


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='ASC' THEN [Date] END ,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='DESC' THEN [Date] END DESC,
CASE WHEN @pSortColumn = 'TaskId' AND @pSortType ='ASC' THEN TaskId END ,
CASE WHEN @pSortColumn = 'TaskId' AND @pSortType ='DESC' THEN TaskId END DESC,
CASE WHEN @pSortColumn = 'Amount' AND @pSortType ='ASC' THEN Amount END ,
CASE WHEN @pSortColumn = 'Amount' AND @pSortType ='DESC' THEN Amount END DESC,
CASE WHEN @pSortColumn = 'Type' AND @pSortType ='ASC' THEN [Type] END ,
CASE WHEN @pSortColumn = 'Type' AND @pSortType ='DESC' THEN [Type] END DESC,
CASE WHEN @pSortColumn = 'Notes' AND @pSortType ='ASC' THEN [Notes] END ,
CASE WHEN @pSortColumn = 'Notes' AND @pSortType ='DESC' THEN [Notes] END DESC
OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END