using Dapper;
using Scheduler.API.Models.Client;
using Scheduler.API.Models.Organization;
using System.Data;

namespace Scheduler.API.Services.Organization
{
    public class OrganizationRepository : IOrganization
    {
        IDapperRepository _dapperRepository = null;
        public OrganizationRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }
        public async Task<Guid?> CreateUpdateOrganizationAsync(AddUpdateOrganizationViewModel saveOrganizationViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pName", saveOrganizationViewModel.Name, DbType.String);
                dp_params.Add("@pDescription", saveOrganizationViewModel.Description, DbType.String);
                dp_params.Add("@pDefaultBillingRate", saveOrganizationViewModel.DefaultBillingRate, DbType.Decimal);
                dp_params.Add("@pDefaultWageRate", saveOrganizationViewModel.DefaultWageRate, DbType.Decimal);
                dp_params.Add("@pCompleteAddress", saveOrganizationViewModel.CompleteAddress, DbType.String);
                dp_params.Add("@pContactNo", saveOrganizationViewModel.ContactNo, DbType.String);
                dp_params.Add("@pEmail", saveOrganizationViewModel.Email, DbType.String);
                dp_params.Add("@pWebSite", saveOrganizationViewModel.WebSite, DbType.String);
                dp_params.Add("@pTimeZone", saveOrganizationViewModel.TimeZone, DbType.String);
                dp_params.Add("@pCurrencyId", saveOrganizationViewModel.CurrencyId, DbType.Int32);
                dp_params.Add("@pCurrencySignId", saveOrganizationViewModel.CurrencySignId, DbType.Int32);
                dp_params.Add("@pDiscountPercentage", saveOrganizationViewModel.DiscountPercentage, DbType.Decimal);
                dp_params.Add("@pTaxPercentage", saveOrganizationViewModel.TaxPercentage, DbType.Decimal);
                dp_params.Add("@pCalculationTypeId", saveOrganizationViewModel.CalculationTypeId, DbType.Int32);
                dp_params.Add("@pServiceRateForBilling", saveOrganizationViewModel.ServiceRateForBilling, DbType.Int32);
                dp_params.Add("@pId", saveOrganizationViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid?>("[Organization].[InsertUpdateOrganization]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));


                return saveOrganizationViewModel.Id = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Guid DeleteOrganization(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", id, DbType.Guid);
            var result = _dapperRepository.Update<Guid>("[Organization].[DeleteOrganization]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        public async Task<OrganizationInfo> GetOrganisationInfoByIdAsync(Guid organizationId)
        {
            OrganizationInfo OrganisationInfos = new OrganizationInfo(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetList<OrganizationInfo>("[organization].[uspGetOrganisationInfoById]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            OrganisationInfos = result.FirstOrDefault()!;

            return OrganisationInfos;
        }
        public async Task<List<OrganizationInfo>> GetOrganizationsByUserIdAsync(Guid userId)
        {
            List<OrganizationInfo> OrganisationInfos = new List<OrganizationInfo>(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pUserId", userId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetAll<OrganizationInfo>("[organization].[uspGetOrganizationsByUserId]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            OrganisationInfos = result.Item1;

            return OrganisationInfos;
        }

        public async Task<List<OrganizationInfo>> GetAllOrganizationsAsync()
        {
            List<OrganizationInfo> OrganisationInfos = new List<OrganizationInfo>();

            var dp_params = new DynamicParameters();
            var result = await Task.FromResult(_dapperRepository.GetAll<OrganizationInfo>("[Organization].[uspGetAllOrganizations]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            OrganisationInfos = result.Item1;

            return OrganisationInfos;
        }

        public async Task<bool> UpdateOrganizationLogoAsync(Guid organizationId, string logoPath)
        {
            try
            {
                var dp_params = new Dapper.DynamicParameters();
                dp_params.Add("@pOrganizationId", organizationId, System.Data.DbType.Guid);
                dp_params.Add("@pLogoPath", logoPath, System.Data.DbType.String);
                var result = await Task.FromResult(_dapperRepository.Update<bool>("[Organization].[UpdateOrganizationLogoPath]", dp_params, commandType: System.Data.CommandType.StoredProcedure));
                return result;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearOrganizationLogoAsync(Guid organizationId)
        {
            return await UpdateOrganizationLogoAsync(organizationId, "");
        }
    }
}
