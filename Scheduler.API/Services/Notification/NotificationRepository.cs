using Dapper;
using Scheduler.API.Models.Notification;
using System.Data;

namespace Scheduler.API.Services.Notification
{
    public class NotificationRepository : INotification
    {
        private readonly IDapperRepository _dapperRepository;

        public NotificationRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<NotificationSearchResponse> GetUserNotificationsAsync(NotificationSearchRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@UserId", request.UserId, DbType.Guid);
                dp_params.Add("@OrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@FranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@RoleId", request.RoleId, DbType.Int32);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.Int32);
                dp_params.Add("@PageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@UnreadOnly", request.UnreadOnly, DbType.Boolean);

                var notifications = await _dapperRepository.GetListAsync<NotificationInfo>(
                    "[dbo].[uspGetUserNotifications]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                );

                var response = new NotificationSearchResponse
                {
                    Notifications = notifications.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                // Get totals from the first record
                if (notifications.Any())
                {
                    // The stored procedure returns TotalRecords and UnreadCount in each row
                    // We need to add these as dynamic properties in NotificationInfo or retrieve separately
                    response.TotalRecords = notifications.Count(); // Will be overridden if we get from SP
                    response.UnreadCount = await GetUnreadNotificationCountAsync(
                        request.UserId, 
                        request.OrganizationId, 
                        request.FranchiseId, 
                        request.RoleId
                    );
                }

                return response;
            }
            catch (Exception ex)
            {
                return new NotificationSearchResponse();
            }
        }

        public async Task<Guid> CreateNotificationAsync(CreateNotificationRequest request, Guid createdBy)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@Title", request.Title, DbType.String);
                dp_params.Add("@Message", request.Message, DbType.String);
                dp_params.Add("@Type", request.Type, DbType.String);
                dp_params.Add("@Priority", request.Priority, DbType.Int32);
                dp_params.Add("@OrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@FranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@TargetRoleId", request.TargetRoleId, DbType.Int32);
                dp_params.Add("@TargetUserId", request.TargetUserId, DbType.Guid);
                dp_params.Add("@ActivityType", request.ActivityType, DbType.String);
                dp_params.Add("@ActivityEntity", request.ActivityEntity, DbType.String);
                dp_params.Add("@ActivityEntityId", request.ActivityEntityId, DbType.Guid);
                dp_params.Add("@ActionUrl", request.ActionUrl, DbType.String);
                dp_params.Add("@CreatedBy", createdBy, DbType.Guid);
                dp_params.Add("@ExpiresAt", request.ExpiresAt, DbType.DateTime);
                dp_params.Add("@NotificationId", null, DbType.Guid, direction: ParameterDirection.Output);

                await _dapperRepository.InsertAsync<Guid>(
                    "[dbo].[uspCreateNotification]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                );

                return dp_params.Get<Guid>("@NotificationId");
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@NotificationId", notificationId, DbType.Guid);
                dp_params.Add("@UserId", userId, DbType.Guid);

                await _dapperRepository.ExecuteAsync(
                    "[dbo].[uspMarkNotificationAsRead]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> MarkAllNotificationsAsReadAsync(Guid userId, Guid? organizationId, Guid? franchiseId, int? roleId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@UserId", userId, DbType.Guid);
                dp_params.Add("@OrganizationId", organizationId, DbType.Guid);
                dp_params.Add("@FranchiseId", franchiseId, DbType.Guid);
                dp_params.Add("@RoleId", roleId, DbType.Int32);

                await _dapperRepository.ExecuteAsync(
                    "[dbo].[uspMarkAllNotificationsAsRead]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                );

                return 1; // Success
            }
            catch (Exception ex)
            {
                return 0; // Failure
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(Guid userId, Guid? organizationId, Guid? franchiseId, int? roleId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@UserId", userId, DbType.Guid);
                dp_params.Add("@OrganizationId", organizationId, DbType.Guid);
                dp_params.Add("@FranchiseId", franchiseId, DbType.Guid);
                dp_params.Add("@RoleId", roleId, DbType.Int32);

                var result = await _dapperRepository.GetAsync<int>(
                    "[dbo].[uspGetUnreadNotificationCount]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@NotificationId", notificationId, DbType.Guid);
                dp_params.Add("@UserId", userId, DbType.Guid);

                await _dapperRepository.ExecuteAsync(
                    "[dbo].[uspDeleteNotification]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Guid> CreateActivityNotificationAsync(
            string activityType,
            string activityEntity,
            Guid activityEntityId,
            string title,
            string message,
            Guid? organizationId,
            Guid? franchiseId,
            Guid createdBy,
            string? actionUrl = null)
        {
            var request = new CreateNotificationRequest
            {
                Title = title,
                Message = message,
                Type = "Activity",
                Priority = 0,
                OrganizationId = organizationId,
                FranchiseId = franchiseId,
                ActivityType = activityType,
                ActivityEntity = activityEntity,
                ActivityEntityId = activityEntityId,
                ActionUrl = actionUrl
            };

            return await CreateNotificationAsync(request, createdBy);
        }
    }
}

