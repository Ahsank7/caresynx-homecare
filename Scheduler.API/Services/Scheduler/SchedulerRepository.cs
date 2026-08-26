using Dapper;
using Scheduler.API.Models.Scheduler;
using Scheduler.API.Services.Email;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Services.Scheduler
{
    public class SchedulerRepository: IScheduler
    {
        IDapperRepository _dapperRepository = null;
        private readonly IEmailService _emailService;
        private readonly ILogger<SchedulerRepository> _logger;

        public SchedulerRepository(
            IDapperRepository DapperRepository,
            IEmailService emailService,
            ILogger<SchedulerRepository> logger)
        {
            _dapperRepository = DapperRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ScheduleAppointmentResponse> CreateSceduleAppointmentAsync(ScheduleAppointmentRequest request)
        {
            ScheduleAppointmentResponse response = new ScheduleAppointmentResponse(); ;

            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pDescription", request.ScheduleDescription, DbType.String);
                dp_params.Add("@pStartTime", request.StartTime, DbType.DateTime);
                dp_params.Add("@pEndTime", request.EndTime, DbType.DateTime);
                dp_params.Add("@pRecurrencePattern", request.RecurrencePattern, DbType.Int32);
                dp_params.Add("@pRecurrenceInterval", request.RecurrenceInterval, DbType.Int32);
                dp_params.Add("@pRecurrenceDaysOfWeek", request.RecurrenceDaysOfWeek, DbType.String);
                dp_params.Add("@pRecurrenceDayOfMonth", request.RecurrenceDayOfMonth, DbType.String);
                dp_params.Add("@pRecurrenceMonthOfYear", request.RecurrenceDayOfYear, DbType.String);

                dp_params.Add("@pServiceType", request.ServiceType, DbType.Int32);
                dp_params.Add("@pCSVServiceIds", request.CSVServiceIds, DbType.String);
                dp_params.Add("@pClientId", request.ClientId, DbType.Guid);
                dp_params.Add("@pCSVServiceProviderIds", request.CSVServiceProviderIds, DbType.String);
                dp_params.Add("@pCreatedBy", request.CreatedBy, DbType.Guid);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.Insert<int>("[dbo].[CreateSchedule]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                response.ScheduleId = result;

                // Send email notifications for schedule creation
                await SendScheduleCreationEmailsAsync(result, request);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule appointment");
                throw;
            }
          
        }
        public async Task<List<GetClientTasksResponse>> GetClientTasks(GetClientTasksRequest request)
        {

            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pStartDate", request.StartDate, DbType.DateTime);
                dp_params.Add("@pEndDate", request.EndDate, DbType.DateTime);
                dp_params.Add("@pClientId", request.ClientId, DbType.Guid);
                dp_params.Add("@pStatusIds", request.StatusIds, DbType.String);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<GetClientTasksResponse>("[dbo].[GetClientTasks]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.ToList();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<List<GetServiceProviderTasksResponse>> GetServiceProviderTasks(GetServiceProviderTasksRequest request)
        {

            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pStartDate", request.StartDate, DbType.DateTime);
                dp_params.Add("@pEndDate", request.EndDate, DbType.DateTime);
                dp_params.Add("@pServiceProviderId", request.ServiceProviderId, DbType.Guid);
                dp_params.Add("@pStatusIds", request.StatusIds, DbType.String);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<GetServiceProviderTasksResponse>("[dbo].[GetServiceProviderTasks]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return result.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task SendScheduleCreationEmailsAsync(int scheduleId, ScheduleAppointmentRequest request)
        {
            try
            {
                // Get schedule details including service type, service name, client and service provider information
                var query = @"
                    SELECT 
                        s.Id AS ScheduleId,
                        s.Description,
                        s.StartTime,
                        s.EndTime,
                        s.RecurrencePattern,
                        CASE 
                            WHEN s.RecurrencePattern = 1 THEN 'Daily'
                            WHEN s.RecurrencePattern = 2 THEN 'Weekly'
                            WHEN s.RecurrencePattern = 3 THEN 'Monthly'
                            WHEN s.RecurrencePattern = 4 THEN 'Yearly'
                            ELSE 'One Time'
                        END AS RecurrencePatternName,
                        st.Name AS ServiceType,
                        srv.Name AS ServiceName,
                        c.Id AS ClientId,
                        c.FirstName + ' ' + ISNULL(c.SurName, '') + ' ' + c.LastName AS ClientName,
                        c.Email AS ClientEmail
                    FROM [dbo].[tblScheduler] s
                    LEFT JOIN [dbo].[tblServicesType] st ON st.Id = s.ServiceType
                    LEFT JOIN [dbo].[tblServices] srv ON srv.Id = s.CSVServiceIds
                    INNER JOIN [dbo].[tblUser] c ON c.Id = s.ClientId
                    WHERE s.Id = @ScheduleId";

                var dp_params = new DynamicParameters();
                dp_params.Add("@ScheduleId", scheduleId, DbType.Int32);

                var scheduleDetails = await _dapperRepository.QueryAsync<dynamic>(
                    query,
                    dp_params,
                    CommandType.Text);

                var schedule = scheduleDetails.FirstOrDefault();
                if (schedule == null)
                {
                    _logger.LogWarning($"Schedule details not found for email notification. ScheduleId: {scheduleId}");
                    return;
                }

                // Get service provider details
                var serviceProviderIds = request.CSVServiceProviderIds?.Split(',') ?? Array.Empty<string>();

                foreach (var spId in serviceProviderIds)
                {
                    if (string.IsNullOrWhiteSpace(spId)) continue;

                    try
                    {
                        // Get service provider information
                        var spQuery = @"
                            SELECT 
                                Id AS ServiceProviderId,
                                FirstName + ' ' + ISNULL(SurName, '') + ' ' + LastName AS ServiceProviderName,
                                Email AS ServiceProviderEmail
                            FROM [dbo].[tblUser]
                            WHERE Id = @ServiceProviderId";

                        var spParams = new DynamicParameters();
                        spParams.Add("@ServiceProviderId", Guid.Parse(spId), DbType.Guid);

                        var spDetails = await _dapperRepository.QueryAsync<dynamic>(
                            spQuery,
                            spParams,
                            CommandType.Text);

                        var serviceProvider = spDetails.FirstOrDefault();
                        if (serviceProvider != null && !string.IsNullOrWhiteSpace(serviceProvider.ServiceProviderEmail) && 
                            serviceProvider.ServiceProviderEmail != "N/A")
                        {
                            // Send email to service provider
                            var spEmail = TaskEmailTemplates.CreateScheduleCreatedEmail(
                                (string)serviceProvider.ServiceProviderEmail,
                                (string)serviceProvider.ServiceProviderName,
                                "Service Provider",
                                (int)schedule.ScheduleId,
                                (DateTime)schedule.StartTime,
                                (DateTime)schedule.EndTime,
                                (string)schedule.RecurrencePatternName ?? "One Time",
                                (string)schedule.ServiceType ?? "N/A",
                                (string)schedule.ServiceName ?? "N/A",
                                (string)schedule.ClientName,
                                (string)serviceProvider.ServiceProviderName,
                                (string)schedule.Description ?? ""
                            );

                            await _emailService.SendEmailAsync(spEmail);
                            _logger.LogInformation($"Schedule creation email sent to service provider for ScheduleId: {scheduleId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending schedule creation email to service provider. ScheduleId: {scheduleId}, ServiceProviderId: {spId}");
                    }
                }

                // Send email to client
                if (!string.IsNullOrWhiteSpace(schedule.ClientEmail) && schedule.ClientEmail != "N/A")
                {
                    try
                    {
                        // Get first service provider name for the email
                        var firstSpId = serviceProviderIds.FirstOrDefault();
                        string serviceProviderName = "Service Provider";

                        if (!string.IsNullOrWhiteSpace(firstSpId))
                        {
                            var spQuery = @"
                                SELECT FirstName + ' ' + ISNULL(SurName, '') + ' ' + LastName AS ServiceProviderName
                                FROM [dbo].[tblUser]
                                WHERE Id = @ServiceProviderId";

                            var spParams = new DynamicParameters();
                            spParams.Add("@ServiceProviderId", Guid.Parse(firstSpId), DbType.Guid);

                            var spDetails = await _dapperRepository.QueryAsync<dynamic>(
                                spQuery,
                                spParams,
                                CommandType.Text);

                            var sp = spDetails.FirstOrDefault();
                            if (sp != null)
                            {
                                serviceProviderName = (string)sp.ServiceProviderName;
                            }
                        }

                        var clientEmail = TaskEmailTemplates.CreateScheduleCreatedEmail(
                            (string)schedule.ClientEmail,
                            (string)schedule.ClientName,
                            "Client",
                            (int)schedule.ScheduleId,
                            (DateTime)schedule.StartTime,
                            (DateTime)schedule.EndTime,
                            (string)schedule.RecurrencePatternName ?? "One Time",
                            (string)schedule.ServiceType ?? "N/A",
                            (string)schedule.ServiceName ?? "N/A",
                            (string)schedule.ClientName,
                            serviceProviderName,
                            (string)schedule.Description ?? ""
                        );

                        await _emailService.SendEmailAsync(clientEmail);
                        _logger.LogInformation($"Schedule creation email sent to client for ScheduleId: {scheduleId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending schedule creation email to client. ScheduleId: {scheduleId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending schedule creation emails for ScheduleId: {scheduleId}");
            }
        }

    }
}
