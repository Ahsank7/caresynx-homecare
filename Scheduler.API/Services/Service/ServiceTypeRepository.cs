using Dapper;
using Scheduler.API.Models.Service;
using System.Data;

namespace Scheduler.API.Services.Service
{
    public class ServiceTypeRepository : IServiceType
    {
        IDapperRepository _dapperRepository = null;
        public ServiceTypeRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<GetServiceTypeResponse> GetServiceTypesAsync(Guid OrganizationId)
        {
            try
            {
                GetServiceTypeResponse response =new GetServiceTypeResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pOrganizationId", OrganizationId, DbType.Guid);
                var result = await _dapperRepository.GetListAsync<ServiceTypeInfo>("[dbo].[GetServiceTypes]"
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

        public async Task<ServiceTypeInfo> CreateServiceTypeAsync(ServiceTypeInfo model)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pName", model.Name, DbType.String);
            dp_params.Add("@pDescription", model.Description, DbType.String);
            dp_params.Add("@pOrganizationId", model.OrganizationId, DbType.Guid);
            var result = await _dapperRepository.InsertAsync<ServiceTypeInfo>("[dbo].[CreateServiceType]", dp_params, CommandType.StoredProcedure);
            return result;
        }

        public async Task<ServiceTypeInfo> UpdateServiceTypeAsync(ServiceTypeInfo model)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pId", model.Id, DbType.Int32);
            dp_params.Add("@pName", model.Name, DbType.String);
            dp_params.Add("@pDescription", model.Description, DbType.String);
            var result = await _dapperRepository.UpdateAsync<ServiceTypeInfo>("[dbo].[UpdateServiceType]", dp_params, CommandType.StoredProcedure);
            return result;
        }

        public async Task<DeleteEntityResult> DeleteServiceTypeAsync(int id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pId", id, DbType.Int32);
            var result = await _dapperRepository.GetAsync<DeleteEntityResult>("[dbo].[DeleteServiceType]", dp_params, CommandType.StoredProcedure);
            return result ?? new DeleteEntityResult
            {
                Deleted = false,
                Message = "Service type not found or could not be deleted"
            };
        }
    }
}
