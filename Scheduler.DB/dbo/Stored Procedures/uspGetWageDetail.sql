-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspGetWageDetail] 
	-- Add the parameters for the stored procedure here
	@pWageId int=null,
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

	

    -- Insert statements for procedure here
-- Get task records
select BI.Id,
       TaskId,
	   (Select S.[Name] from tblServicesType S WHERE S.Id=SCH.ServiceType) AS ServiceType,
	   ST.[Date],
	   ( CASE 
                WHEN st.CalculationTypeId = 1 THEN DATEDIFF(HOUR, st.StartTime, st.EndTime)
                WHEN st.CalculationTypeId = 2 THEN DATEDIFF(HOUR, st.CheckIn, st.CheckOut)
            END) AS Qty,
	   ST.BillingRate as Rate,
	   Amount,
	   ServiceProviderWageId,
	   'Task' AS RecordType,
	   NULL AS ExpenseId,
	   NULL AS ExpenseAmount,
	   NULL AS ExpenseType
 Into  #Results
 from [dbo].[tblServiceProviderWageDetail] (Nolock) BI
 JOIN dbo.tblServicesTask ST on ST.Id = BI.TaskId
 JOIN tblScheduler SCH on SCH.Id = ST.ScheduleId
 Where 1=1
 and BI.ServiceProviderWageId=@pWageId
 and BI.TaskId IS NOT NULL
 and BI.ExpenseId IS NULL

UNION ALL

-- Get expense records
select BI.Id,
       BI.TaskId,
	   li.Name AS ServiceType,
	   ST.[Date],
	   1 AS Qty,
	   0 AS Rate,
	   BI.ExpenseAmount AS Amount,
	   ServiceProviderWageId,
	   'Expense' AS RecordType,
	   BI.ExpenseId,
	   BI.ExpenseAmount,
	   li.Name AS ExpenseType
 from [dbo].[tblServiceProviderWageDetail] (Nolock) BI
 JOIN dbo.tblServicesTask ST on ST.Id = BI.TaskId
 JOIN [dbo].[tblUserExpense] UE on UE.Id = BI.ExpenseId
 JOIN [dbo].[tblLookupItems] li ON li.Id = UE.Type AND li.LookupType = 'ExpenseType'
 Where 1=1
 and BI.ServiceProviderWageId=@pWageId
 and BI.ExpenseId IS NOT NULL


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'Amount' AND @pSortType ='ASC' THEN Amount END ,
CASE WHEN @pSortColumn = 'Amount' AND @pSortType ='DESC' THEN Amount END DESC,
CASE WHEN @pSortColumn = 'TaskId' AND @pSortType ='ASC' THEN TaskId END ,
CASE WHEN @pSortColumn = 'TaskId' AND @pSortType ='DESC' THEN TaskId END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END