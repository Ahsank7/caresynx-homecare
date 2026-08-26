-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

--select * from tblUserTransaction

--exec [dbo].[uspGetAllUserTransaction]   	@pSortColumn  ='TransactionLine1',	@pSortType ='asc' ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [dbo].[uspGetAllUserTransaction] 	
-- Add the parameters for the stored procedure here
    @pFranchiseId uniqueidentifier,
	@pUserId Nvarchar(50)=null,
	@pTransactionTypeId int=null,
	@pTransactionDate Date =null,
	@pReferenceId nvarchar(50)=null,
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

		if isnull(@pTransactionTypeId,0) =0
		 set @pTransactionTypeId=null

		 if isnull(@pUserId,'') =''
		 set @pUserId=null

		 if isnull(@pTransactionDate,'') =''
		 set @pTransactionDate=null

		 
		
		
		Select 
		   
			 UA.TransactionId
			,UA.UserId
			,UA.Remarks
			,UA.ReferenceId
				,(
			    CASE when UA.StatusId=1 Then 'Succeeded'
			         when UA.StatusId=2 Then 'Failed'
				ELSE 'Undefined' 
				END
			
			
			) [Status] 
			,UA.TransactionDate
			,CI.CardNumber
			,BA.AccountNumber
			,UA.CardId
			,UA.BankAccountId
			,UA.TypeId
			,UA.StatusId  --1 for Succeeded 2 for Failed 
	Into  #Results
	From tblTransaction UA
	JOIN tblUser U on U.Id=UA.UserId
	LEFT JOIN tblCardInfo CI on CI.CardId = UA.CardId
	LEFT JOIN tblBankAccount BA on BA.BankAccountId = UA.BankAccountId
    WHERE 1=1
	 and U.FranchiseId=@pFranchiseId
	 and (@pUserId is Null OR UA.[UserId]=@pUserId)
	 and (@pTransactionTypeId is Null OR UA.TypeId =@pTransactionTypeId)
	 and (@pTransactionDate is Null OR  (UA.TransactionDate = @pTransactionDate)) 

 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'TransactionId' AND @pSortType ='ASC' THEN TransactionId END ,
CASE WHEN @pSortColumn = 'TransactionId' AND @pSortType ='DESC' THEN TransactionId END DESC,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='ASC' THEN UserId END ,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='DESC' THEN UserId END DESC,
CASE WHEN @pSortColumn = 'Status' AND @pSortType ='ASC' THEN [Status] END ,
CASE WHEN @pSortColumn = 'Status' AND @pSortType ='DESC' THEN [Status] END DESC,
CASE WHEN @pSortColumn = 'TransactionDate' AND @pSortType ='ASC' THEN TransactionDate END ,
CASE WHEN @pSortColumn = 'TransactionDate' AND @pSortType ='DESC' THEN TransactionDate END DESC,
CASE WHEN @pSortColumn = 'CardNumber' AND @pSortType ='ASC' THEN CardNumber END ,
CASE WHEN @pSortColumn = 'CardNumber' AND @pSortType ='DESC' THEN CardNumber END DESC,
CASE WHEN @pSortColumn = 'AccountNumber' AND @pSortType ='ASC' THEN AccountNumber END ,
CASE WHEN @pSortColumn = 'AccountNumber' AND @pSortType ='DESC' THEN AccountNumber END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END