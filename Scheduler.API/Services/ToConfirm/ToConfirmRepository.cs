using Dapper;
using Scheduler.API.Models.ToConfirm;
using System.Data;

namespace Scheduler.API.Services.ToConfirm
{
    public class ToConfirmRepository : IToConfirm
    {
        IDapperRepository _dapperRepository = null;
        public ToConfirmRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public bool CalculateBillingAndWageAmounts(string servicesTaskIds,Guid organizationId)
        {
            var result = true;
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pServicesTasks", servicesTaskIds, DbType.String);
                dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);
                _dapperRepository.Update<bool>("[dbo].[uspCalculateBillingAndWageAmounts]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                 result=false;
            }

            return result;
        }

        public async Task<ToConfirmResponse> GetToConfirmTasksAsync(ToConfirmRequest request)
        {
            ToConfirmResponse toConfirmResponse = new ToConfirmResponse(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pClientEmail", request.ClientEmail, DbType.String);
            dp_params.Add("@pClientId", request.ClientID.ToString()??null, DbType.String);
            dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
            dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
            dp_params.Add("@pTaskId", request.TaskId, DbType.String);
            dp_params.Add("@pTaskStatusIds", request.TaskStatusIds, DbType.String);
            dp_params.Add("@pClientName", request.ClientName, DbType.String);
            dp_params.Add("@pClientUserNo", request.ClientUserNo, DbType.String);
            dp_params.Add("@pServiceProviderUserNo", request.ServiceProviderUserNo, DbType.String);
            dp_params.Add("@pClientPhoneNumber", request.ClientPhoneNumber, DbType.String);
            dp_params.Add("@pServiceProviderId", request.ServiceProviderID.ToString()??null, DbType.String);
            dp_params.Add("@pServiceProviderName", request.ServiceProviderName, DbType.String);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetAll<ToConfirmDetail>("[dbo].[uspGetToConfirmTasksWithExpenses]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            toConfirmResponse.Response = result.Item1;
            toConfirmResponse.TotalRecords = result.Item2;

            return toConfirmResponse;
        }

        public bool ConfirmExpenses(string expenseIds)
        {
            var result = true;
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pExpenseIds", expenseIds, DbType.String);
                _dapperRepository.Update<bool>("[dbo].[uspConfirmExpenses]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}
