-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

--select * from [dbo].[tblUserLeave]
--exec [dbo].[uspGetUserLeave]    @pUserId='01EE07D7-4C6B-4220-BB3D-46F379D00A94',	@pSortColumn  ='FirstName',	@pSortType ='asc' ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [dbo].[uspGetUserLeave] 	
-- Add the parameters for the stored procedure here
	@pUserId uniqueidentifier,
	@pDate date=null,
	@pTypeId int=null,
	@pStatusId int=null,
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
		
		 if isnull(@pStatusId,0) =0
		 set @pStatusId=null

		print @pUserId
		
		Select 
		    UL.[Id]
           ,UL.[UserId]
           ,UL.[Date]
           ,UL.[StartTime]
           ,UL.[EndTime]
           ,UL.[CreatedDate]
           ,UL.[CreatedBy]
           ,UL.[IsActive]
           ,UL.[Notes]
			,(select Top 1 N.Name from tblLookupItems N where  N.LookupType='LeaveStatus' And  N.Id=UL.[Status]) [Status]
			,(select Top 1 N.Name from tblLookupItems N where  N.LookupType='LeaveType' And  N.Id=UL.[Type]) [Type]
	Into  #Results
	From  [dbo].[tblUserLeave] UL
    WHERE 1=1
	 and ISNULL(UL.IsActive,0)=1
	 and (@pUserId is Null OR UL.UserId=@pUserId)
	 and (@pStatusId is Null OR UL.Status=@pStatusId)
	 and (@pTypeId is Null OR UL.Type =@pTypeId)
	 and (@pDate is Null OR UL.Date =@pDate) 


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='ASC' THEN [Date] END ,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='DESC' THEN [Date] END DESC,
CASE WHEN @pSortColumn = 'StartTime' AND @pSortType ='ASC' THEN StartTime END ,
CASE WHEN @pSortColumn = 'StartTime' AND @pSortType ='DESC' THEN StartTime END DESC,
CASE WHEN @pSortColumn = 'EndTime' AND @pSortType ='ASC' THEN EndTime END ,
CASE WHEN @pSortColumn = 'EndTime' AND @pSortType ='DESC' THEN EndTime END DESC,
CASE WHEN @pSortColumn = 'Type' AND @pSortType ='ASC' THEN [Type] END ,
CASE WHEN @pSortColumn = 'Type' AND @pSortType ='DESC' THEN [Type] END DESC,
CASE WHEN @pSortColumn = 'Status' AND @pSortType ='ASC' THEN [Type] END ,
CASE WHEN @pSortColumn = 'Status' AND @pSortType ='DESC' THEN [Type] END DESC,

CASE WHEN @pSortColumn = 'Notes' AND @pSortType ='ASC' THEN [Notes] END ,
CASE WHEN @pSortColumn = 'Notes' AND @pSortType ='DESC' THEN [Notes] END DESC
OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END