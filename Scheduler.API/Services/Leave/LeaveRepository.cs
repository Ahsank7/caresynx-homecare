using Dapper;
using Scheduler.API.Models.Leave;
using System.Data;

namespace Scheduler.API.Services.Leave
{
    public class LeaveRepository : ILeave
    {
        IDapperRepository _dapperRepository = null;
        public LeaveRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }
        public async Task<Guid?> CreateUpdateUserLeaveAsync(SaveUserLeaveInfoViewModel saveUserLeaveInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pNotes", saveUserLeaveInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pCreatedBy", saveUserLeaveInfoViewModel.CreatedBy, DbType.Guid);
                dp_params.Add("@pEndTime", saveUserLeaveInfoViewModel.EndTime, DbType.DateTime);
                dp_params.Add("@pStartTime", saveUserLeaveInfoViewModel.StartTime, DbType.DateTime);
                dp_params.Add("@pDate", saveUserLeaveInfoViewModel.Date, DbType.Date);
                dp_params.Add("@pStatus", saveUserLeaveInfoViewModel.Status, DbType.Int32);
                dp_params.Add("@pUserId", saveUserLeaveInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pType", saveUserLeaveInfoViewModel.Type, DbType.Int32);
                dp_params.Add("@pId", saveUserLeaveInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InsertUpdateUserLeave]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return dp_params.Get<Guid?>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Guid? DeleteUserLeave(Guid id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", id, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = _dapperRepository.Update<Guid>("[dbo].[DeleteLeaveInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserLeaveInfo> GetUserLeaveInfoAsync(Guid UserLeaveID)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", UserLeaveID, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.GetList<UserLeaveInfo>("[dbo].[GetUserleaveInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserLeaveSearchResponse> GetUserLeavesAsync(UserLeaveSearchRequest request)
        {
            try
            {
                UserLeaveSearchResponse response = new UserLeaveSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pTypeId", request.TypeId, DbType.Int32);
                dp_params.Add("@pStatusId", request.StatusId, DbType.Int32);
                dp_params.Add("@pDate", request.Date, DbType.Date);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<SearchUserLeaveViewModel>("[dbo].[uspGetUserLeave]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                response.Response = result.Item1;
                response.TotalRecords = result.Item2;

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
