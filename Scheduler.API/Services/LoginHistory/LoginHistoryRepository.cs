using Dapper;
using Scheduler.API.Models.LoginHistory;
using System.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.API.Services.LoginHistory
{
    public class LoginHistoryRepository : ILoginHistory
    {
        private readonly IDapperRepository _dapperRepository;

        public LoginHistoryRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<LoginHistoryResponse> GetLoginHistoryAsync(LoginHistoryRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pUserType", request.UserType, DbType.Int32);
                dp_params.Add("@pStartDate", request.StartDate, DbType.DateTime);
                dp_params.Add("@pEndDate", request.EndDate, DbType.DateTime);
                dp_params.Add("@pLoginStatus", request.LoginStatus, DbType.String);
                dp_params.Add("@pIPAddress", request.IPAddress, DbType.String);
                dp_params.Add("@pPageNumber", request.PageNumber, DbType.Int32);
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortDirection", request.SortDirection, DbType.String);

                // Get login history entries
                var entries = await _dapperRepository.GetListAsync<LoginHistoryEntry>("[dbo].[GetLoginHistory]", dp_params, commandType: CommandType.StoredProcedure);
                
                // Get total count using separate stored procedure
                var countParams = new DynamicParameters();
                countParams.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                countParams.Add("@pUserId", request.UserId, DbType.Guid);
                countParams.Add("@pUserType", request.UserType, DbType.Int32);
                countParams.Add("@pStartDate", request.StartDate, DbType.DateTime);
                countParams.Add("@pEndDate", request.EndDate, DbType.DateTime);
                countParams.Add("@pLoginStatus", request.LoginStatus, DbType.String);
                countParams.Add("@pIPAddress", request.IPAddress, DbType.String);
                
                var totalCountResult = await _dapperRepository.GetAsync<dynamic>("[dbo].[GetLoginHistoryCount]", countParams, commandType: CommandType.StoredProcedure);
                
                var response = new LoginHistoryResponse();
                response.Entries = entries?.ToList() ?? new List<LoginHistoryEntry>();
                response.TotalRecords = totalCountResult?.TotalRecords ?? 0;

                return response;
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                throw;
            }
        }

        public async Task<int> InsertLoginHistoryAsync(InsertLoginHistoryRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pUserName", request.UserName, DbType.String);
                dp_params.Add("@pUserEmail", request.UserEmail, DbType.String);
                dp_params.Add("@pUserType", request.UserType, DbType.Int32);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@pIPAddress", request.IPAddress, DbType.String);
                dp_params.Add("@pUserAgent", request.UserAgent, DbType.String);
                dp_params.Add("@pBrowserName", request.BrowserName, DbType.String);
                dp_params.Add("@pBrowserVersion", request.BrowserVersion, DbType.String);
                dp_params.Add("@pOperatingSystem", request.OperatingSystem, DbType.String);
                dp_params.Add("@pDeviceType", request.DeviceType, DbType.String);
                dp_params.Add("@pScreenResolution", request.ScreenResolution, DbType.String);
                dp_params.Add("@pTimezone", request.Timezone, DbType.String);
                dp_params.Add("@pLanguage", request.Language, DbType.String);
                dp_params.Add("@pCountry", request.Country, DbType.String);
                dp_params.Add("@pCity", request.City, DbType.String);
                dp_params.Add("@pLoginStatus", request.LoginStatus, DbType.String);
                dp_params.Add("@pFailureReason", request.FailureReason, DbType.String);

                var result = await _dapperRepository.InsertAsync<int>("[dbo].[InsertLoginHistory]", dp_params, commandType: CommandType.StoredProcedure);
                
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                throw;
            }
        }

        public async Task<bool> UpdateLogoutTimeAsync(Guid userId, DateTime? loginTime = null)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", userId, DbType.Guid);
                dp_params.Add("@pLoginTime", loginTime, DbType.DateTime);

                var result = await _dapperRepository.UpdateAsync<int>("[dbo].[UpdateLogoutTime]", dp_params, commandType: CommandType.StoredProcedure);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                throw;
            }
        }
    }
}
