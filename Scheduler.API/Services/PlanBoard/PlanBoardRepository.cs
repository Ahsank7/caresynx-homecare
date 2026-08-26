using Dapper;
using Scheduler.API.Models.PlanBoard;
using Scheduler.API.Models.ServicesTask;
using Scheduler.API.Services.Email;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Services.PlanBoard
{
    public class PlanBoardRepository : IPlanBoard
    {
        IDapperRepository _dapperRepository = null;
        private readonly IEmailService _emailService;
        private readonly ILogger<PlanBoardRepository> _logger;

        public PlanBoardRepository(
            IDapperRepository DapperRepository,
            IEmailService emailService,
            ILogger<PlanBoardRepository> logger)
        {
            _dapperRepository = DapperRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ServicesTaskResponse> GetPlanBoardTasksAsync(ServicesTaskRequest request)
        {
            ServicesTaskResponse toConfirmResponse = new ServicesTaskResponse(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
            dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
            dp_params.Add("@pTaskId", request.TaskId, DbType.String);
            dp_params.Add("@pTaskStatusIds", request.TaskStatusIds, DbType.String);
            dp_params.Add("@pClientName", request.ClientName, DbType.String);
            dp_params.Add("@pClientUserNo", request.ClientUserNo, DbType.String);
            dp_params.Add("@pServiceProviderUserNo", request.ServiceProviderUserNo, DbType.String);
            dp_params.Add("@pServiceProviderName", request.ServiceProviderName, DbType.String);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetAll<ServicesTaskDetail>("[dbo].[uspGetPlanboardTasks]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            toConfirmResponse.Response = result.Item1;
            toConfirmResponse.TotalRecords = result.Item2;

            return toConfirmResponse;
        }
        public async Task<bool> UpdateTaskNotesAsync(UpdateNotesRequest updateNotes)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskNotes", updateNotes.Notes, DbType.String);
                dp_params.Add("@pTaskId", updateNotes.TaskId, DbType.String);
                dp_params.Add("@pUpdatedBy", updateNotes.UpdatedBy, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.Update<int>("[dbo].[UpdateTaskNotes]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> UpdateTaskStatusAsync(UpdateTaskStatusRequest updateStatus)
        {
            try
            {
                // Get task details before updating to get previous status
                var taskDetails = await GetTaskDetailsForEmailAsync(updateStatus.TaskId.ToString());
                var previousStatus = taskDetails?.TaskStatus ?? "Unknown";

                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskStatus", updateStatus.TaskStatus, DbType.Int16);
                dp_params.Add("@pTaskId", updateStatus.TaskId, DbType.String);
                dp_params.Add("@pTaskNotes", updateStatus.StatusNotes, DbType.String);
                dp_params.Add("@pUpdatedBy", updateStatus.UpdatedBy, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.Update<int>("[dbo].[UpdateTaskStatus]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                // Get updated task details and send email notifications
                await SendTaskStatusChangeEmailsAsync(updateStatus.TaskId.ToString(), previousStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task status for TaskId: {updateStatus.TaskId}");
                return false;
            }
        }
        public async Task<bool> AssignServiceProviderToTaskAsync(AssignServiceProviderRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskId", request.TaskId, DbType.String);
                dp_params.Add("@pServiceProviderId", request.ServiceProviderId, DbType.Guid);
                dp_params.Add("@pUpdatedBy", request.UpdatedBy, DbType.Guid);
                
                var result = await Task.FromResult(_dapperRepository.Update<int>("[dbo].[AssignServiceProviderToTask]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning service provider to task: {request.TaskId}");
                return false;
            }
        }

        public async Task<bool> UnassignServiceProviderFromTaskAsync(UnassignServiceProviderRequest request)
        {
            try
            {
                var taskDetails = await GetTaskDetailsForEmailAsync(request.TaskId.ToString());
                var previousStatus = taskDetails?.TaskStatus ?? "Unknown";

                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskId", request.TaskId.ToString(), DbType.String);
                dp_params.Add("@pUpdatedBy", request.UpdatedBy, DbType.Guid);
                dp_params.Add("@pNotes", request.Notes, DbType.String);

                await Task.FromResult(_dapperRepository.Update<int>(
                    "[dbo].[UnassignServiceProviderFromTask]",
                    dp_params,
                    commandType: CommandType.StoredProcedure));

                await SendTaskStatusChangeEmailsAsync(request.TaskId.ToString(), previousStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unassigning service provider from task: {request.TaskId}");
                return false;
            }
        }
        public async Task<bool> UpdateTaskAttendanceAsync(UpdateTaskAttendanceRequest updateAttendace)
        {
            try
            {
                // Get task details before updating to get previous status
                var taskDetails = await GetTaskDetailsForEmailAsync(updateAttendace.TaskId.ToString());
                var previousStatus = taskDetails?.TaskStatus ?? "Unknown";

                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskTime", updateAttendace.AttendanceTime, DbType.DateTime);
                dp_params.Add("@pTaskId", updateAttendace.TaskId, DbType.String);
                dp_params.Add("@pUpdatedBy", updateAttendace.UpdatedBy, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.Update<int>("[dbo].[UpdateTaskAttendance]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                // Get updated task details and send email notifications
                await SendTaskStatusChangeEmailsAsync(updateAttendace.TaskId.ToString(), previousStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task attendance for TaskId: {updateAttendace.TaskId}");
                return false;
            }
        }
        public async Task<bool> AddAttendanceAsync(AddAttendanceRequest addAttendance)
        {
            try
            {
                // Get task details before updating to get previous status
                var taskDetails = await GetTaskDetailsForEmailAsync(addAttendance.TaskId.ToString());
                var previousStatus = taskDetails?.TaskStatus ?? "Unknown";

                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskCheckInTime", addAttendance.CheckInTime, DbType.DateTime);
                dp_params.Add("@pTaskCheckOutTime", addAttendance.CheckOutTime, DbType.DateTime);
                dp_params.Add("@pTaskId", addAttendance.TaskId, DbType.Int16);
                dp_params.Add("@pUpdatedBy", addAttendance.UpdatedBy, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.Update<int>("[dbo].[AddTaskAttendance]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                // Get updated task details and send email notifications
                await SendTaskStatusChangeEmailsAsync(addAttendance.TaskId.ToString(), previousStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding attendance for TaskId: {addAttendance.TaskId}");
                return false;
            }
        }

        public async Task<ServicesTaskDetail> GetServicesTaskInfo(string taskId, Guid franchiseId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pTaskId", taskId, DbType.Int32);
                dp_params.Add("@pFranchiseId", franchiseId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<ServicesTaskDetail>("[dbo].[uspGetServiceTaskInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<ServicesTaskDetail?> GetTaskDetailsForEmailAsync(string taskId)
        {
            try
            {
                // Query to get task details without franchise filter
                var query = @"
                    SELECT st.Id as TaskId   
                          ,st.ScheduleId  
                          ,CAST(st.StartTime AT TIME ZONE 'UTC' AT TIME ZONE ISNULL(sc.TimeZone, 'UTC') AS DATETIME) as StartTime
                          ,CAST(st.EndTime AT TIME ZONE 'UTC' AT TIME ZONE ISNULL(sc.TimeZone, 'UTC') AS DATETIME) as EndTime
                          ,st.Date  
                          ,st.ClientId  
                          ,u.FirstName+' '+ISNULL(U.SurName,'')+' '+u.LastName as ClientName   
                          ,ISNULL(u.Email,'N/A') as ClientEmail  
                          ,ISNULL(u.PhoneNo,'N/A') as ClientPhone  
                          ,ISNULL(u.MobileNo,'N/A') as ClientMobile  
                          ,ua.AddressLine1 + ISNULL(', ' + ua.AddressLine2, '') as ClientAddress
                          ,ST.ServiceProviderId  
                          ,su.FirstName+' '+ISNULL(su.SurName,'')+' '+su.LastName as ServiceProviderName   
                          ,ISNULL(su.Email,'N/A') as ServiceProviderEmail  
                          ,ISNULL(su.PhoneNo,'N/A') as ServiceProviderPhone  
                          ,ISNULL(su.MobileNo,'N/A') as ServiceProviderMobile  
                          ,sua.AddressLine1 + ISNULL(', ' + sua.AddressLine2, '') as ServiceProviderAddress
                          ,st.IsConfirmed  
                          ,f.[Name] as FranchiseName
                          ,T.[Name] as TaskStatus
                          ,u.FranchiseId
                          ,st.[Status]
                          ,CAST(ST.[CheckIn] AT TIME ZONE 'UTC' AT TIME ZONE ISNULL(sc.TimeZone, 'UTC') AS DATETIME) as CheckInTime
                          ,CAST(ST.[CheckOut] AT TIME ZONE 'UTC' AT TIME ZONE ISNULL(sc.TimeZone, 'UTC') AS DATETIME) as CheckOutTime
                          ,ISNULL(sty.[Name], 'N/A') as ServiceType
                          ,ISNULL(s.[Name], 'N/A') as ServiceName
                     FROM [dbo].[tblServicesTask] st  
                       JOIN dbo.tblScheduler sc ON SC.Id = st.ScheduleId
                       JOIN [dbo].[tblUser] u on u.Id=st.ClientId   
                       LEFT JOIN [dbo].[tblUserAddress] ua on ua.UserId = u.Id AND ua.IsPrimaryAddress = 1
                       LEFT JOIN [dbo].[tblUser] su on su.Id=st.ServiceProviderId
                       LEFT JOIN [dbo].[tblUserAddress] sua on sua.UserId = su.Id AND sua.IsPrimaryAddress = 1
                       JOIN [dbo].[tblFranchise] f on f.Id=u.FranchiseId 
                       JOIN [tblLookupItems] T on T.LookupType ='TaskStatus' and T.Id=st.[Status]
                       LEFT JOIN [dbo].[tblServicesType] sty on sty.Id = sc.ServiceType
                       LEFT JOIN [dbo].[tblServices] s on s.Id = sc.CSVServiceIds
                     WHERE st.Id=@TaskId";

                var dp_params = new DynamicParameters();
                dp_params.Add("@TaskId", taskId, DbType.Int32);

                var result = await Task.FromResult(_dapperRepository.GetList<ServicesTaskDetail>(
                    query,
                    dp_params,
                    commandType: CommandType.Text));

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting task details for email notification. TaskId: {taskId}");
                return null;
            }
        }

        private async Task SendTaskStatusChangeEmailsAsync(string taskId, string previousStatus)
        {
            try
            {
                var taskDetails = await GetTaskDetailsForEmailAsync(taskId);
                if (taskDetails == null)
                {
                    _logger.LogWarning($"Task details not found for email notification. TaskId: {taskId}");
                    return;
                }

                var newStatus = taskDetails.TaskStatus ?? "Unknown";

                // Only send emails if status actually changed
                if (previousStatus == newStatus)
                {
                    return;
                }

                // Send email to client
                if (!string.IsNullOrWhiteSpace(taskDetails.ClientEmail) && 
                    taskDetails.ClientEmail != "N/A")
                {
                    try
                    {
                        var clientEmail = TaskEmailTemplates.CreateTaskStatusChangeEmail(
                            taskDetails,
                            taskDetails.ClientEmail,
                            taskDetails.ClientName ?? "Valued Client",
                            "Client",
                            previousStatus,
                            newStatus
                        );
                        await _emailService.SendEmailAsync(clientEmail);
                        _logger.LogInformation($"Task status change email sent to client for TaskId: {taskId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending task status change email to client for TaskId: {taskId}");
                    }
                }

                // Send email to service provider
                if (!string.IsNullOrWhiteSpace(taskDetails.ServiceProviderEmail) && 
                    taskDetails.ServiceProviderEmail != "N/A")
                {
                    try
                    {
                        var providerEmail = TaskEmailTemplates.CreateTaskStatusChangeEmail(
                            taskDetails,
                            taskDetails.ServiceProviderEmail,
                            taskDetails.ServiceProviderName ?? "Valued Service Provider",
                            "Service Provider",
                            previousStatus,
                            newStatus
                        );
                        await _emailService.SendEmailAsync(providerEmail);
                        _logger.LogInformation($"Task status change email sent to service provider for TaskId: {taskId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending task status change email to service provider for TaskId: {taskId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending task status change emails for TaskId: {taskId}");
            }
        }
    }
}
