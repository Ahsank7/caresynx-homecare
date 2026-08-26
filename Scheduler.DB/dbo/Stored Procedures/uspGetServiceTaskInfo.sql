-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- =============================================  
--exec [dbo].[uspGetPlanboardTasks] @pPageSize=10,@pSortColumn=N'TaskId',@pSortType=N'TaskId',@pClientEmail=NULL,@pClientId=N'',@pTaskDate=NULL,@pTaskId=NULL,@pTaskStatusIds=NULL,@pClientName=NULL,@pClientPhoneNumber=NULL,@pServiceProviderId=N'',@pServiceProviderName=NULL,@PageNumber=N'1'  
CREATE PROCEDURE [dbo].[uspGetServiceTaskInfo] 
 -- Add the parameters for the stored procedure here  

  @pTaskId nvarchar(50)=null, 
  @pFranchiseId nvarchar(50)=null 
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
 
    
  
select st.Id as TaskId   
      ,st.ScheduleId  
	  ,CAST(st.StartTime AT TIME ZONE 'UTC' AT TIME ZONE sc.TimeZone AS DATETIME) as StartTime
      ,CAST(st.EndTime AT TIME ZONE 'UTC' AT TIME ZONE sc.TimeZone AS DATETIME) as EndTime
      ,st.Date  
      ,st.ClientId  
      ,u.FirstName+' '+ISNULL(U.SurName,'')+' '+u.LastName as ClientName   
   ,IsNull(u.Email,'N/A') as ClientEmail  
   ,IsNull(u.PhoneNo,'N/A') as ClientPhone  
   ,IsNull(u.MobileNo,'N/A') as ClientMobile  
   --,IsNull(st.,'N/A') as ClientAddress  
   ,st.ServiceProviderId  
   ,CASE WHEN su.Id IS NULL THEN NULL ELSE su.FirstName+' '+ISNULL(su.SurName,'')+' '+su.LastName END as ServiceProviderName   
   ,CASE WHEN su.Id IS NULL THEN 'N/A' ELSE IsNull(su.Email,'N/A') END as ServiceProviderEmail  
   ,CASE WHEN su.Id IS NULL THEN 'N/A' ELSE IsNull(su.PhoneNo,'N/A') END as ServiceProviderPhone  
   ,CASE WHEN su.Id IS NULL THEN 'N/A' ELSE IsNull(su.MobileNo,'N/A') END as ServiceProviderMobile  
   ,st.IsConfirmed  
   ,f.[Name] as FranchiseName
   ,T.[Name] as TaskStatus
   ,u.FranchiseId
   ,st.[Status]
   ,st.ClientId
   ,CAST(ST.[CheckIn] AT TIME ZONE 'UTC' AT TIME ZONE sc.TimeZone AS DATETIME) as CheckInTime
   ,CAST(ST.[CheckOut] AT TIME ZONE 'UTC' AT TIME ZONE sc.TimeZone AS DATETIME) as CheckOutTime
   ,ISNULL(sty.[Name], 'N/A') as ServiceType
   ,ISNULL(s.[Name], 'N/A') as ServiceName

 from [dbo].[tblServicesTask] (Nolock) st  
   JOIN dbo.tblScheduler sc ON SC.Id = st.ScheduleId
   JOIN [dbo].[tblUser] (Nolock) u on u.Id=st.ClientId   
   LEFT JOIN [dbo].[tblUser] (Nolock) su on su.Id=st.ServiceProviderId
   JOIN [dbo].[tblFranchise]  (Nolock) f on f.Id=u.FranchiseId 
   JOIN [tblLookupItems] T on T.LookupType ='TaskStatus' and T.Id=st.[Status]
   LEFT JOIN [dbo].[tblServicesType] (Nolock) sty on sty.Id = sc.ServiceType
   LEFT JOIN [dbo].[tblServices] (Nolock) s on s.Id = sc.CSVServiceIds
 Where 1=1  
 and u.FranchiseId=@pFranchiseId  
 and st.Id=@pTaskId
 

  
END