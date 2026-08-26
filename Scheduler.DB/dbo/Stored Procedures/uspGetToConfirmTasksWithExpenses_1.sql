-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspGetToConfirmTasksWithExpenses] 
	-- Add the parameters for the stored procedure here
	@pFranchiseId uniqueidentifier,
	@pTaskId nvarchar(50)=null,
	@pClientEmail nvarchar(50)=null,
	@pClientId nvarchar(50)=null,
	@pStartDate date=null,
	@pEndDate date=null,
	@pTaskStatusIds nvarchar(50)=null,
	@pClientName nvarchar(50)=null,
	@pClientPhoneNumber nvarchar(50)=null,
	@pClientUserNo  nvarchar(50)=null,
	@pServiceProviderId nvarchar(50)=null,
	@pServiceProviderName nvarchar(50)=null,
	@pServiceProviderUserNo nvarchar(50)=null,
	@pOrganizationId UNIQUEIDENTIFIER=null,
	@pSortColumn nvarchar(50) = N'Id',
	@pSortType nvarchar(10) = N'asc' ,
	@PageNumber int =1,
	@pPageSize int =10
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults
		IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results

		if isnull(@pClientId,'') =''
		 set @pClientId=null

		 if isnull(@pServiceProviderId,'') =''
		 set @pServiceProviderId=null

		 if isnull(@pClientUserNo,'') =''
		 set @pClientUserNo=null

		 if isnull(@pServiceProviderUserNo,'') =''
		 set @pServiceProviderUserNo=null

		 if isnull(@pServiceProviderName,'') =''
		 set @pServiceProviderName=null
		

		 if isnull(@pClientName,'') =''
		 set @pClientName=null
		

		 if isnull(@pClientPhoneNumber,'') =''
		 set @pClientPhoneNumber=null
		

		 if isnull(@pTaskStatusIds,'') =''
		 set @pTaskStatusIds=null

		  if isnull(@pTaskId,'') =''
		 set @pTaskId=null

		 if isnull(@pClientEmail,'') =''
		 set @pClientEmail=null

		   if isnull(@pStartDate,'') =''  
          set @pStartDate=null  

		   if isnull(@pEndDate,'') =''  
          set @pEndDate=null  

	 -- Get timezone from organization settings
	 DECLARE @pTimeZone NVARCHAR(50)
	 SELECT @pTimeZone = [TimeZone] FROM [dbo].[tblOrganization] WHERE [Id] = @pOrganizationId AND [IsActive] = 1
	 
	 -- If no organization timezone found, use default
	 If IsNull(@pTimeZone,'') = ''
	   Set @pTimeZone='Pakistan Standard Time'
	 
	 -- Convert timezone name to SQL Server timezone identifier
	 DECLARE @pTimeZoneId NVARCHAR(50)
	 SET @pTimeZoneId = [dbo].[GetTimeZoneId](@pTimeZone)

    -- Insert statements for procedure here
    -- Get Tasks
    select st.Id as TaskId 
      ,st.ScheduleId
	      -- Convert StartTime only as TIME, then add back to original Date
    ,DATEADD(
        DAY, 0,
        CAST(St.[Date] AS datetime) +
        CAST(
            CAST(St.[StartTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS time)
            AS datetime
        )
    ) AS StartTime

    ,DATEADD(
        DAY, 0,
        CAST(St.[Date] AS datetime) +
        CAST(
            CAST(St.[EndTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS time)
            AS datetime
        )
    ) AS EndTime
	  ,st.Date
      ,st.ClientId
	  ,u.UserNo as ClientUserNo
	  ,su.UserNo as ServiceProviderUserNo
	   ,u.FirstName+' '+u.LastName as ClientName
	  ,IsNull(u.Email,'N/A') as ClientEmail
	  ,IsNull(u.PhoneNo,'N/A') as ClientPhone
	  ,IsNull(u.MobileNo,'N/A') as ClientMobile
	  ,ST.ServiceProviderId
	  ,su.FirstName+' '+su.LastName as ServiceProviderName
	  ,IsNull(su.Email,'N/A') as ServiceProviderEmail
	  ,IsNull(su.PhoneNo,'N/A') as ServiceProviderPhone
	  ,IsNull(su.MobileNo,'N/A') as ServiceProviderMobile
	  ,st.IsConfirmed
	  ,ISNULL(sty.[Name], 'N/A') as ServiceType
	  --,ISNULL(s.[Name], 'N/A') as ServiceName
	  ,'Task' as RecordType
	  ,NULL as ExpenseId
	  ,NULL as ExpenseType
	  ,NULL as ExpenseAmount
	  ,NULL as ExpenseDate
	  ,NULL as ExpenseNotes
	  ,NULL as ExpenseIsConfirmed
 	  ,CAST(ST.CheckIn AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) as CheckInTime
	  ,CAST(ST.CheckOut AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) as CheckOutTime
 Into  #Results
 from [dbo].[tblServicesTask] (Nolock) st
	JOIN [dbo].[tblUser] (Nolock) u on u.Id=st.ClientId   
   JOIN [dbo].[tblUser] (Nolock) su on su.Id=st.ServiceProviderId
   JOIN [dbo].[tblFranchise]  (Nolock) f on f.Id=u.FranchiseId 
   JOIN [tblLookupItems] T on T.LookupType ='TaskStatus' and T.Id=st.[Status]
   LEFT JOIN [dbo].[tblScheduler] (Nolock) sch on sch.Id = st.ScheduleId
   LEFT JOIN [dbo].[tblServicesType] (Nolock) sty on sty.Id = sch.ServiceType
   --LEFT JOIN [dbo].[tblServices] (Nolock) s on s.Id = sch.CSVServiceIds
 Where 1=1
 and ISNULL(st.IsConfirmed,0)=0
 and u.FranchiseId=@pFranchiseId
 and ((@pTaskId is Null) OR (st.Id=@pTaskId))
 and ((@pClientId is Null) OR (st.ClientId=@pClientId))
 and ((@pClientEmail is Null) OR (u.Email like '%'+@pClientEmail+'%'))
 and ((@pClientName is Null) OR (u.FirstName like '%'+@pClientName+'%'))
 and ((@pClientPhoneNumber is Null) OR (u.PhoneNo like '%'+@pClientPhoneNumber+'%'))
 and ((@pClientUserNo is Null) OR (u.UserNo=@pClientUserNo))
 and ((@pServiceProviderId is Null) OR (st.ServiceProviderId=@pServiceProviderId))
 and ((@pServiceProviderName is Null) OR (su.FirstName like '%'+@pServiceProviderName+'%'))
 and ((@pServiceProviderUserNo is Null) OR (su.UserNo=@pServiceProviderUserNo))
 and ((@pStartDate is Null) OR (st.Date >= @pStartDate))
 and ((@pEndDate is Null) OR (st.Date <= @pEndDate))
 and T.[Name] ='Completed' ---Completed Status

 ---- Get Expenses for the same tasks
 --UNION ALL

 --select st.Id as TaskId 
 --     ,st.ScheduleId
	--      -- Convert StartTime only as TIME, then add back to original Date
 --   ,DATEADD(
 --       DAY, 0,
 --       CAST(St.[Date] AS datetime) +
 --       CAST(
 --           CAST(St.[StartTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS time)
 --           AS datetime
 --       )
 --   ) AS StartTime

 --   ,DATEADD(
 --       DAY, 0,
 --       CAST(St.[Date] AS datetime) +
 --       CAST(
 --           CAST(St.[EndTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS time)
 --           AS datetime
 --       )
 --   ) AS EndTime
	--  ,st.Date
 --     ,st.ClientId
	--  ,u.UserNo as ClientUserNo
	--  ,su.UserNo as ServiceProviderUserNo
	--   ,u.FirstName+' '+u.LastName as ClientName
	--  ,IsNull(u.Email,'N/A') as ClientEmail
	--  ,IsNull(u.PhoneNo,'N/A') as ClientPhone
	--  ,IsNull(u.MobileNo,'N/A') as ClientMobile
	--  ,ST.ServiceProviderId
	--  ,su.FirstName+' '+su.LastName as ServiceProviderName
	--  ,IsNull(su.Email,'N/A') as ServiceProviderEmail
	--  ,IsNull(su.PhoneNo,'N/A') as ServiceProviderPhone
	--  ,IsNull(su.MobileNo,'N/A') as ServiceProviderMobile
	--  ,st.IsConfirmed
	--  ,ISNULL(sty.[Name], 'N/A') as ServiceType
	--  ,ISNULL(s.[Name], 'N/A') as ServiceName
	--  ,'Expense' as RecordType
	--  ,ue.Id as ExpenseId
	--  ,li.Name as ExpenseType
	--  ,ue.Amount as ExpenseAmount
	--  ,ue.Date as ExpenseDate
	--  ,ue.Notes as ExpenseNotes
	--  ,ue.IsConfirmed as ExpenseIsConfirmed
 --from [dbo].[tblServicesTask] (Nolock) st
	--JOIN [dbo].[tblUser] (Nolock) u on u.Id=st.ClientId   
 --  JOIN [dbo].[tblUser] (Nolock) su on su.Id=st.ServiceProviderId
 --  JOIN [dbo].[tblFranchise]  (Nolock) f on f.Id=u.FranchiseId 
 --  JOIN [tblLookupItems] T on T.LookupType ='TaskStatus' and T.Id=st.[Status]
 --  LEFT JOIN [dbo].[tblScheduler] (Nolock) sch on sch.Id = st.ScheduleId
 --  LEFT JOIN [dbo].[tblServicesType] (Nolock) sty on sty.Id = sch.ServiceType
 --  LEFT JOIN [dbo].[tblServices] (Nolock) s on s.Id = sch.CSVServiceIds
 --  INNER JOIN [dbo].[tblUserExpense] (Nolock) ue on ue.TaskId = st.Id
 --  LEFT JOIN [dbo].[tblLookupItems] (Nolock) li on li.LookupType = 'ExpenseType' and li.Id = ue.Type
 --Where 1=1
 --and ISNULL(ue.IsActive,0) = 1
 --and ISNULL(st.IsConfirmed,0)=0
 --and ISNULL(ue.IsConfirmed,0)=0
 --and u.FranchiseId=@pFranchiseId
 --and ((@pTaskId is Null) OR (st.Id=@pTaskId))
 --and ((@pClientId is Null) OR (st.ClientId=@pClientId))
 --and ((@pClientEmail is Null) OR (u.Email like '%'+@pClientEmail+'%'))
 --and ((@pClientName is Null) OR (u.FirstName like '%'+@pClientName+'%'))
 --and ((@pClientPhoneNumber is Null) OR (u.PhoneNo like '%'+@pClientPhoneNumber+'%'))
 --and ((@pClientUserNo is Null) OR (u.UserNo=@pClientUserNo))
 --and ((@pServiceProviderId is Null) OR (st.ServiceProviderId=@pServiceProviderId))
 --and ((@pServiceProviderName is Null) OR (su.FirstName like '%'+@pServiceProviderName+'%'))
 --and ((@pServiceProviderUserNo is Null) OR (su.UserNo=@pServiceProviderUserNo))
 --and ((@pStartDate is Null) OR (st.Date >= @pStartDate))
 --and ((@pEndDate is Null) OR (st.Date <= @pEndDate))
 --and T.[Name] ='Completed' ---Completed Status


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'TaskId' AND @pSortType ='ASC' THEN TaskId END ,
CASE WHEN @pSortColumn = 'TaskId' AND @pSortType ='DESC' THEN TaskId END DESC,
CASE WHEN @pSortColumn = 'ScheduleId' AND @pSortType ='ASC' THEN ScheduleId END ,
CASE WHEN @pSortColumn = 'ScheduleId' AND @pSortType ='DESC' THEN ScheduleId END DESC,
CASE WHEN @pSortColumn = 'StartTime' AND @pSortType ='ASC' THEN StartTime END ,
CASE WHEN @pSortColumn = 'StartTime' AND @pSortType ='DESC' THEN StartTime END DESC,
CASE WHEN @pSortColumn = 'EndTime' AND @pSortType ='ASC' THEN EndTime END ,
CASE WHEN @pSortColumn = 'EndTime' AND @pSortType ='DESC' THEN EndTime END DESC,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='ASC' THEN [Date] END ,
CASE WHEN @pSortColumn = 'Date' AND @pSortType ='DESC' THEN [Date] END DESC,
CASE WHEN @pSortColumn = 'ClientId' AND @pSortType ='ASC' THEN ClientId END ,
CASE WHEN @pSortColumn = 'ClientId' AND @pSortType ='DESC' THEN ClientId END DESC,

CASE WHEN @pSortColumn = 'ClientName' AND @pSortType ='ASC' THEN ClientName END ,
CASE WHEN @pSortColumn = 'ClientName' AND @pSortType ='DESC' THEN ClientName END DESC,
CASE WHEN @pSortColumn = 'ClientEmail' AND @pSortType ='ASC' THEN ClientEmail END ,
CASE WHEN @pSortColumn = 'ClientEmail' AND @pSortType ='DESC' THEN ClientEmail END DESC,
CASE WHEN @pSortColumn = 'ClientPhone' AND @pSortType ='ASC' THEN ClientPhone END ,
CASE WHEN @pSortColumn = 'ClientPhone' AND @pSortType ='DESC' THEN ClientPhone END DESC,

CASE WHEN @pSortColumn = 'ClientMobile' AND @pSortType ='ASC' THEN ClientMobile END ,
CASE WHEN @pSortColumn = 'ClientMobile' AND @pSortType ='DESC' THEN ClientMobile END DESC,
CASE WHEN @pSortColumn = 'ServiceProviderName' AND @pSortType ='ASC' THEN ServiceProviderName END ,
CASE WHEN @pSortColumn = 'ServiceProviderName' AND @pSortType ='DESC' THEN ServiceProviderName END DESC,
CASE WHEN @pSortColumn = 'ServiceProviderEmail' AND @pSortType ='ASC' THEN ServiceProviderEmail END ,
CASE WHEN @pSortColumn = 'ServiceProviderEmail' AND @pSortType ='DESC' THEN ServiceProviderEmail END DESC,

CASE WHEN @pSortColumn = 'ServiceProviderId' AND @pSortType ='ASC' THEN ServiceProviderId END ,
CASE WHEN @pSortColumn = 'ServiceProviderId' AND @pSortType ='DESC' THEN ServiceProviderId END DESC,
CASE WHEN @pSortColumn = 'ServiceProviderPhone' AND @pSortType ='ASC' THEN ServiceProviderPhone END ,
CASE WHEN @pSortColumn = 'ServiceProviderPhone' AND @pSortType ='DESC' THEN ServiceProviderPhone END DESC,
CASE WHEN @pSortColumn = 'ServiceProviderMobile' AND @pSortType ='ASC' THEN ServiceProviderMobile END ,
CASE WHEN @pSortColumn = 'ServiceProviderMobile' AND @pSortType ='DESC' THEN ServiceProviderMobile END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END