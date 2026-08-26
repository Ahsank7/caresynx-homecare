using Dapper;
using Scheduler.API.Models.Availability;
using System.Data;

namespace Scheduler.API.Services.Availability
{
    public class AvailabilityRepository : IAvailability
    {
        IDapperRepository _dapperRepository = null;
        public AvailabilityRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<Guid?> CreateUpdateAvailabilityAsync(SaveAvailabilityInfoViewModel saveAvailabilityInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pStartTime", saveAvailabilityInfoViewModel.StartTime, DbType.Time);
                dp_params.Add("@pEndTime", saveAvailabilityInfoViewModel.EndTime, DbType.Time);
                dp_params.Add("@pDay", saveAvailabilityInfoViewModel.Day, DbType.String);
                dp_params.Add("@pUserId", saveAvailabilityInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pId", saveAvailabilityInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InsertUpdateUserAvailability]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return saveAvailabilityInfoViewModel.Id = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Guid? DeleteAvailability(Guid id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", id, DbType.Guid);
                var result = _dapperRepository.Update<Guid>("[dbo].[DeleteUserAvailability]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AvailabilityInfo> GetAvailabilityInfoAsync(Guid AvailabilityID)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", AvailabilityID, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<AvailabilityInfo>("[dbo].[GetUserAvailabilityInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AvailabilitySearchResponse> GetAvailabilitysAsync(AvailabilitySearchRequest request)
        {
            try
            {
                AvailabilitySearchResponse availabilitySearchRequestResponse = new AvailabilitySearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<SearchAvailabilityViewModel>("[dbo].[uspGetAllAvailability]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                availabilitySearchRequestResponse.Response = result.Item1;
                availabilitySearchRequestResponse.TotalRecords = result.Item2;

                return availabilitySearchRequestResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
