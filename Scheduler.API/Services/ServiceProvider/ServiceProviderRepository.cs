using Dapper;
using Scheduler.API.Models.ServiceProvider;
using System.Data;

namespace Scheduler.API.Services.ServiceProvider
{
    public class ServiceProviderRepository : IServiceProvider
    {
        IDapperRepository _dapperRepository = null;
        public ServiceProviderRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<Guid?> CreateUpdateServiceProviderAsync(SaveServiceProviderInfoViewModel saveServiceProviderInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pFirstName", saveServiceProviderInfoViewModel.FirstName, DbType.String);
                dp_params.Add("@pSurName", saveServiceProviderInfoViewModel.SurName, DbType.String);
                dp_params.Add("@pLastName", saveServiceProviderInfoViewModel.LastName, DbType.String);
                dp_params.Add("@pAlias", saveServiceProviderInfoViewModel.Alias, DbType.String);
                dp_params.Add("@pUserName", saveServiceProviderInfoViewModel.UserName, DbType.String);
                dp_params.Add("@pPhoneNo", saveServiceProviderInfoViewModel.PhoneNo, DbType.String);
                dp_params.Add("@pMobileNo", saveServiceProviderInfoViewModel.MobileNo, DbType.String);
                dp_params.Add("@pPassportNo", saveServiceProviderInfoViewModel.PassportNo, DbType.String);
                dp_params.Add("@pIdentityNo", saveServiceProviderInfoViewModel.IdentityNo, DbType.String);
                dp_params.Add("@pEthnicityId", saveServiceProviderInfoViewModel.EthnicityId, DbType.Int32);
                dp_params.Add("@pAge", saveServiceProviderInfoViewModel.Age, DbType.Int32);
                dp_params.Add("@pBirthDate", saveServiceProviderInfoViewModel.BirthDate, DbType.Date);
                dp_params.Add("@pJoiningDate", saveServiceProviderInfoViewModel.JoiningDate, DbType.Date);
                dp_params.Add("@pCountyId", saveServiceProviderInfoViewModel.CountyId, DbType.Int32);
                dp_params.Add("@pMaritalStatusId", saveServiceProviderInfoViewModel.MaritalStatusId, DbType.Int32);
                dp_params.Add("@pEmail", saveServiceProviderInfoViewModel.Email, DbType.String);
                dp_params.Add("@pAddressLine1", saveServiceProviderInfoViewModel.AddressLine1, DbType.String);
                dp_params.Add("@pAddressLine2", saveServiceProviderInfoViewModel.AddressLine2, DbType.String);
                dp_params.Add("@pAddressLine3", saveServiceProviderInfoViewModel.AddressLine3, DbType.String);
                dp_params.Add("@pLatitude", saveServiceProviderInfoViewModel.Latitude, DbType.Decimal);
                dp_params.Add("@pLongitude", saveServiceProviderInfoViewModel.Longitude, DbType.Decimal);
                dp_params.Add("@pStateId", saveServiceProviderInfoViewModel.StateId, DbType.Int32);
                dp_params.Add("@pNationalityId", saveServiceProviderInfoViewModel.NationalityId, DbType.Int32);
                dp_params.Add("@pCountryId", saveServiceProviderInfoViewModel.CountryId, DbType.Int32);
                dp_params.Add("@pGenderId", saveServiceProviderInfoViewModel.GenderId, DbType.Int32);
                dp_params.Add("@pTitleId", saveServiceProviderInfoViewModel.TitleId, DbType.Int32);
                dp_params.Add("@pPasswordHash", saveServiceProviderInfoViewModel.PasswordHash, DbType.String);
                dp_params.Add("@pFranchiseId", saveServiceProviderInfoViewModel.FranchiseId, DbType.Guid);
                dp_params.Add("@pNotes", saveServiceProviderInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pId", saveServiceProviderInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid?>("[ServiceProvider].[InsertUpdateServiceProvider]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return dp_params.Get<Guid?>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public Guid DeleteServiceProvider(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", id, DbType.Int32);
            //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
            var result = _dapperRepository.Update<Guid>("[ServiceProvider].[DeleteServiceProvider]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<ServiceProviderInfo> GetServiceProviderInfoAsync(Guid userId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", userId, DbType.Guid);
            //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
            var result = await Task.FromResult(_dapperRepository.GetList<ServiceProviderInfo>("[ServiceProvider].[GetServiceProviderInfo]"
                , dp_params,
                commandType: CommandType.StoredProcedure));
            return result.FirstOrDefault()!;
        }

        public async Task<ServiceProviderSearchResponse> GetServiceProvidersAsync(ServiceProviderSearchRequest request)
        {
            ServiceProviderSearchResponse ServiceProviderResponse = new ServiceProviderSearchResponse();

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pEmail", request.Email, DbType.String);
            dp_params.Add("@pMobileNumber", request.MobileNumber, DbType.String);
            dp_params.Add("@pPhoneNumber", request.PhoneNumber, DbType.String);
            dp_params.Add("@pFirstName", request.FirstName, DbType.String);
            dp_params.Add("@pEthnicityId", request.EthnicityId, DbType.Int32);
            dp_params.Add("@pLastName", request.LastName, DbType.String);
            dp_params.Add("@pJoiningDate", request.JoiningDate, DbType.Date);
            dp_params.Add("@pUserId", request.UserId, DbType.Guid);
            dp_params.Add("@pGenderId", request.GenderId, DbType.Int32);
            dp_params.Add("@pStatusId", request.StatusId, DbType.Int32);
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.Int32);
            
            // Preference filtering parameters
            dp_params.Add("@pClientId", request.ClientId, DbType.Guid);
            dp_params.Add("@pApplyPreferenceFilter", request.ApplyPreferenceFilter, DbType.Boolean);
            
            // Use the preference-aware stored procedure if preference filtering is requested
            string storedProcedure = request.ApplyPreferenceFilter && request.ClientId.HasValue
                ? "[ServiceProvider].[uspGetServiceProvidersWithPreferences]"
                : "[ServiceProvider].[uspGetAllServiceProviders]";
            
            var result = await Task.FromResult(_dapperRepository.GetAll<SearchServiceProviderViewModel>(storedProcedure
                , dp_params,
                commandType: CommandType.StoredProcedure));

            ServiceProviderResponse.Response = result.Item1;
            ServiceProviderResponse.TotalRecords = result.Item2;

            return ServiceProviderResponse;
        }

        public async Task<GetAvailableServiceProviderSearchResponse> GetAvailableServiceProvidersAsync(AvailableServiceProviderSearchRequest request)
        {
            try
            {
                GetAvailableServiceProviderSearchResponse ServiceProviderResponse = new GetAvailableServiceProviderSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pStartDateTime", request.StartDateTime, DbType.DateTime);
                dp_params.Add("@pEndDateTime", request.EndDateTime, DbType.DateTime);
                dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@pSearchText", request.SearchText, DbType.String);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.Int32);
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);

               var result = await Task.FromResult(_dapperRepository.GetAll<GetAvailableServiceProviderSearchViewModel>("[ServiceProvider].[uspGetAllAvailableServiceProviders]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

                ServiceProviderResponse.Response = result.Item1;
                ServiceProviderResponse.TotalRecords = result.Item2;

                return ServiceProviderResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public async Task<ServiceProviderWithAvailabilityResponse> GetServiceProvidersWithAvailabilityAsync(ServiceProviderWithAvailabilityRequest request)
        {
            try
            {
                ServiceProviderWithAvailabilityResponse response = new ServiceProviderWithAvailabilityResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
                dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
                dp_params.Add("@pStartTime", request.StartTime, DbType.Time);
                dp_params.Add("@pEndTime", request.EndTime, DbType.Time);
                dp_params.Add("@pSearchText", request.SearchText, DbType.String);

                var result = await Task.FromResult(_dapperRepository.GetAll<ServiceProviderWithAvailabilityViewModel>(
                    "[ServiceProvider].[uspGetServiceProvidersWithAvailability]",
                    dp_params,
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

        public async Task<ContractInfo> GetContractInfoAsync(Guid UserId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", UserId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<ContractInfo>("[dbo].[GetServiceProviderContractInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> UpsertContractInfo(UpsertContractInfoViewModel upsertContractInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pStartDate", upsertContractInfoViewModel.StartDate, DbType.Date);
                dp_params.Add("@pEndDate", upsertContractInfoViewModel.EndDate, DbType.Date);

                dp_params.Add("@pRate", upsertContractInfoViewModel.Rate, DbType.Decimal);
                dp_params.Add("@pOptionId", upsertContractInfoViewModel.OptionId, DbType.Int32);
                dp_params.Add("@pContractType", upsertContractInfoViewModel.ContractType, DbType.Int32);
                dp_params.Add("@pFrequencyId", upsertContractInfoViewModel.FrequencyId, DbType.Int32);
                dp_params.Add("@pUserId", upsertContractInfoViewModel.ServiceProviderUserId, DbType.Guid);
                dp_params.Add("@pId", upsertContractInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InserUpdateServiceProviderContractInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return upsertContractInfoViewModel.Id = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
