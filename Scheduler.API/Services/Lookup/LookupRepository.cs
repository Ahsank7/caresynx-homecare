using Dapper;
using Scheduler.API.Helper;
using Scheduler.API.Models.Client;
using Scheduler.API.Models.Leave;
using Scheduler.API.Models.Lookup;
using System.Data;
using System.Linq;

namespace Scheduler.API.Services.Lookup
{
    public class LookupRepository : ILookup
    {
        IDapperRepository _dapperRepository = null;
        public LookupRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<int?> CreateUpdateLookupAsync(UpsertLookupRequest saveLookupInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pName", saveLookupInfoViewModel.Name, DbType.String);
                dp_params.Add("@pIsActive", saveLookupInfoViewModel.IsActive, DbType.Boolean);
                dp_params.Add("@pDescription", saveLookupInfoViewModel.Description, DbType.String);
                dp_params.Add("@pLookupType", saveLookupInfoViewModel.LookupType, DbType.String);
                dp_params.Add("@pId", saveLookupInfoViewModel.Id, DbType.Int32);
                dp_params.Add("@pOutId", null, DbType.Int32, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<int>("[Lookup].[InsertUpdateLookup]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return saveLookupInfoViewModel.Id = dp_params.Get<int>("@pOutId");
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int? DeleteLookup(int id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", id, DbType.Int32);

                var result = _dapperRepository.Update<int>("[Lookup].[DeleteLookupItem]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return id;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<LookupResponse> GetLookupAsync(LookupSearchRequest request)
        {
            try
            {
                LookupResponse response = new LookupResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pLookupType", request.LookupType, DbType.String);
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pName", request.Name, DbType.String);
                dp_params.Add("@pIsActive", request.IsActive, DbType.Boolean);
                dp_params.Add("@pPageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<LookupDetail>("[Lookup].[uspGeLookupItems]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                response.Result = result.Item1;
                response.TotalRecords = result.Item2;
                response.LookupType = request.LookupType;

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<LookupDetail> GetLookupInfoAsync(int Id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", Id, DbType.Int32);
                var result = await Task.FromResult(_dapperRepository.GetList<LookupDetail>("[Lookup].[uspGetLookupItemById]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Dictionary<string, string>> GetLookupsList()
        {
            try
            {
                var result = await Task.FromResult(_dapperRepository.GetList<KeyValuePair<string, string>>("[Lookup].[uspGetLookupsList]"
                    , null,
                    commandType: CommandType.StoredProcedure).ToDictionary(x => x.Key, x => x.Value));
                return result;


            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
