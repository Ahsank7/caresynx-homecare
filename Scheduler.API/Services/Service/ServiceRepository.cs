using Dapper;
using Scheduler.API.Models.Service;
using System.Data;

namespace Scheduler.API.Services.Service
{
    public class ServiceRepository : IServices
    {
        IDapperRepository _dapperRepository = null;
        public ServiceRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<GetServicesResponse> GetServiceListAsync(int ServiceTypeId)
        {
            try
            {
                GetServicesResponse response =new GetServicesResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pServiceTypeID", ServiceTypeId, DbType.Int32);
                var result = await _dapperRepository.GetListAsync<ServiceInfo>("[dbo].[GetServicesByTypeId]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                response.Response =result.ToList();

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ServiceInfo> CreateServiceAsync(ServiceInfo model)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pName", model.Name, DbType.String);
            dp_params.Add("@pDescription", model.Description, DbType.String);
            dp_params.Add("@pServiceTypeId", model.ServiceTypeId, DbType.Int32);
            dp_params.Add("@pRate", model.Rate, DbType.Decimal);
            var result = await _dapperRepository.InsertAsync<ServiceInfo>("[dbo].[CreateService]", dp_params, CommandType.StoredProcedure);
            return result;
        }

        public async Task<ServiceInfo> UpdateServiceAsync(ServiceInfo model)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pId", model.Id, DbType.Int32);
            dp_params.Add("@pName", model.Name, DbType.String);
            dp_params.Add("@pDescription", model.Description, DbType.String);
            dp_params.Add("@pServiceTypeId", model.ServiceTypeId, DbType.Int32);
            dp_params.Add("@pRate", model.Rate, DbType.Decimal);
            var result = await _dapperRepository.UpdateAsync<ServiceInfo>("[dbo].[UpdateService]", dp_params, CommandType.StoredProcedure);
            return result;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pId", id, DbType.Int32);
            var result = await _dapperRepository.ExecuteAsync("[dbo].[DeleteService]", dp_params, CommandType.StoredProcedure);
            return result > 0;
        }
    }
}
