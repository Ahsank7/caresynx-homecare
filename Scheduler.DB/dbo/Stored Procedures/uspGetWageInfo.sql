-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspGetWageInfo] 
	-- Add the parameters for the stored procedure here
	@pFranchiseId uniqueidentifier,
	@pUserId nvarchar(100)=null,
    @pDate date=null,
	@pTransactionId nvarchar(50)=null,
	@pUserNo nvarchar(20)=null,
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

		if isnull(@pUserId,'') =''
		 set @pUserId=null

		if isnull(@pTransactionId,'') =''
		 set @pTransactionId=null

		if isnull(@pUserNo,'') =''
		 set @pUserNo=null

		if isnull(@pDate,'') =''
		 set @pDate=null


    -- Insert statements for procedure here
select BI.Id,
       Description,
	   TotalAmount,
	   Date,
	   StartDate,
	   EndDate,
	   DueDate,
	   IsPaid,
	   ServiceProviderId,
	   u.UserNo,
	   (u.FirstName +' '+ u.LastName) ServiceProviderName,
	   --TransactionId,
	   BI.Row_Guid as TransactionId
 Into  #Results
 from [dbo].[tblServiceProviderWage] (Nolock) BI
	JOIN [dbo].[tblUser] (Nolock) u on u.Id=BI.ServiceProviderId   
 Where 1=1
 and u.FranchiseId=@pFranchiseId
 and (@pUserNo is null OR u.UserNo=@pUserNo)
 and (@pTransactionId is null OR BI.TransactionId=@pTransactionId)
 and (@pDate is null OR BI.[Date]=@pDate)
 and (@pUserId is Null) OR (BI.ServiceProviderId =@pUserId)


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'TotalAmount' AND @pSortType ='ASC' THEN TotalAmount END ,
CASE WHEN @pSortColumn = 'TotalAmount' AND @pSortType ='DESC' THEN TotalAmount END DESC,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='ASC' THEN Date END ,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='DESC' THEN Date END DESC,
CASE WHEN @pSortColumn = 'IsPaid' AND @pSortType ='ASC' THEN IsPaid END ,
CASE WHEN @pSortColumn = 'IsPaid' AND @pSortType ='DESC' THEN IsPaid END DESC,
CASE WHEN @pSortColumn = 'UserNo' AND @pSortType ='ASC' THEN UserNo END ,
CASE WHEN @pSortColumn = 'UserNo' AND @pSortType ='DESC' THEN UserNo END DESC,
CASE WHEN @pSortColumn = 'ClientName' AND @pSortType ='ASC' THEN ServiceProviderName END ,
CASE WHEN @pSortColumn = 'ClientName' AND @pSortType ='DESC' THEN ServiceProviderName END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END