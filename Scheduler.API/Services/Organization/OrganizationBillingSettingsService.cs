using Dapper;
using Scheduler.API.Models.Organization;
using System.Data;

namespace Scheduler.API.Services.Organization
{
    public class OrganizationBillingSettingsService : IOrganizationBillingSettingsService
    {
        private readonly IDapperRepository _dapperRepository;

        public OrganizationBillingSettingsService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<OrganizationBillingSettings> GetBillingSettingsAsync(Guid organizationId)
        {
            var settings = new OrganizationBillingSettings
            {
                OrganizationId = organizationId
            };

            // Get organization basic info
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

            var orgInfo = await _dapperRepository.GetAsync<OrganizationBillingSettings>("[dbo].[uspGetOrganizationBillingSettings]", dp_params, CommandType.StoredProcedure);
            
            if (orgInfo != null)
            {
                settings.OrganizationName = orgInfo.OrganizationName;
                settings.ServiceRateForBilling = orgInfo.ServiceRateForBilling;
                settings.DefaultBillingRate = orgInfo.DefaultBillingRate;
                settings.DefaultWageRate = orgInfo.DefaultWageRate;
            }

            // Get time-based rates
            settings.TimeBasedRates = await GetTimeBasedRatesAsync(organizationId);

            return settings;
        }

        public async Task<bool> SaveBillingSettingsAsync(OrganizationBillingSettingsRequest request)
        {
            try
            {
                // Update organization billing settings
                var dp_params = new DynamicParameters();
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@pServiceRateForBilling", request.ServiceRateForBilling, DbType.Int32);
                dp_params.Add("@pDefaultBillingRate", request.DefaultBillingRate, DbType.Decimal);
                dp_params.Add("@pDefaultWageRate", request.DefaultWageRate, DbType.Decimal);

                await _dapperRepository.ExecuteAsync("[dbo].[uspUpdateOrganizationBillingSettings]", dp_params, CommandType.StoredProcedure);

                // Handle time-based rates if ServiceRateForBilling is 3 (Time-Based)
                if (request.ServiceRateForBilling == 3)
                {
                    // Delete existing time-based rates for this organization
                    var deleteParams = new DynamicParameters();
                    deleteParams.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                    await _dapperRepository.ExecuteAsync("[dbo].[uspDeleteOrganizationTimeBasedRatesByOrganization]", deleteParams, CommandType.StoredProcedure);

                    // Insert new time-based rates
                    foreach (var rate in request.TimeBasedRates)
                    {
                        await SaveTimeBasedRateAsync(rate);
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<OrganizationTimeBasedRate>> GetTimeBasedRatesAsync(Guid organizationId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

            var rates = await _dapperRepository.GetListAsync<OrganizationTimeBasedRate>("[dbo].[uspGetOrganizationTimeBasedRates]", dp_params, CommandType.StoredProcedure);

            return rates.ToList();
        }

        public async Task<OrganizationTimeBasedRate> SaveTimeBasedRateAsync(OrganizationTimeBasedRateRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", request.Id ?? (object)DBNull.Value, DbType.Int32);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@pServiceTypeId", request.ServiceTypeId ?? (object)DBNull.Value, DbType.Int32);
                dp_params.Add("@pServiceId", request.ServiceId ?? (object)DBNull.Value, DbType.Int32);
                dp_params.Add("@pDayOfWeek", request.DayOfWeek, DbType.Int32);
                dp_params.Add("@pStartTime", request.StartTime, DbType.Time);
                dp_params.Add("@pEndTime", request.EndTime, DbType.Time);
                dp_params.Add("@pClientRate", request.ClientRate, DbType.Decimal);
                dp_params.Add("@pWageRate", request.WageRate, DbType.Decimal);
                dp_params.Add("@pIsActive", request.IsActive, DbType.Boolean);
                dp_params.Add("@pOutId", null, DbType.Int32, direction: ParameterDirection.Output);

                await _dapperRepository.ExecuteAsync("[dbo].[uspInsertUpdateOrganizationTimeBasedRate]", dp_params, CommandType.StoredProcedure);

                var newId = dp_params.Get<int>("@pOutId");

                // Return the saved rate
                return new OrganizationTimeBasedRate
                {
                    Id = newId,
                    OrganizationId = request.OrganizationId,
                    ServiceTypeId = request.ServiceTypeId,
                    ServiceId = request.ServiceId,
                    DayOfWeek = request.DayOfWeek,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    ClientRate = request.ClientRate,
                    WageRate = request.WageRate,
                    IsActive = request.IsActive
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteTimeBasedRateAsync(int id, Guid organizationId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", id, DbType.Int32);
                dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

                var result = await _dapperRepository.ExecuteAsync("[dbo].[uspDeleteOrganizationTimeBasedRate]", dp_params, CommandType.StoredProcedure);
                return result > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
