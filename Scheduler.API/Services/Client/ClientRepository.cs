using Dapper;
using Scheduler.API.Models.Client;
using System.Data;

namespace Scheduler.API.Services.Client
{
    public class ClientRepository : IClient
    {
        IDapperRepository _dapperRepository = null;
        public ClientRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<Guid?> CreateUpdateClientAsync(SaveClientInfoViewModel saveClientInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pFirstName", saveClientInfoViewModel.FirstName, DbType.String);
                dp_params.Add("@pSurName", saveClientInfoViewModel.SurName, DbType.String);
                dp_params.Add("@pLastName", saveClientInfoViewModel.LastName, DbType.String);
                dp_params.Add("@pAlias", saveClientInfoViewModel.Alias, DbType.String);
                dp_params.Add("@pUserName", saveClientInfoViewModel.UserName, DbType.String);
                dp_params.Add("@pPhoneNo", saveClientInfoViewModel.PhoneNo, DbType.String);
                dp_params.Add("@pMobileNo", saveClientInfoViewModel.MobileNo, DbType.String);
                dp_params.Add("@pPassportNo", saveClientInfoViewModel.PassportNo, DbType.String);
                dp_params.Add("@pIdentityNo", saveClientInfoViewModel.IdentityNo, DbType.String);
                dp_params.Add("@pEthnicityId", saveClientInfoViewModel.EthnicityId, DbType.Int32);
                dp_params.Add("@pAge", saveClientInfoViewModel.Age, DbType.Int32);
                dp_params.Add("@pBirthDate", saveClientInfoViewModel.BirthDate, DbType.Date);
                dp_params.Add("@pJoiningDate", saveClientInfoViewModel.JoiningDate, DbType.Date);
                dp_params.Add("@pCountyId", saveClientInfoViewModel.CountyId, DbType.Int32);
                dp_params.Add("@pMaritalStatusId", saveClientInfoViewModel.MaritalStatusId, DbType.Int32);
                dp_params.Add("@pEmail", saveClientInfoViewModel.Email, DbType.String);
                dp_params.Add("@pAddressLine1", saveClientInfoViewModel.AddressLine1, DbType.String);
                dp_params.Add("@pAddressLine2", saveClientInfoViewModel.AddressLine2, DbType.String);
                dp_params.Add("@pAddressLine3", saveClientInfoViewModel.AddressLine3, DbType.String);
                dp_params.Add("@pLatitude", saveClientInfoViewModel.Latitude, DbType.Decimal);
                dp_params.Add("@pLongitude", saveClientInfoViewModel.Longitude, DbType.Decimal);
                dp_params.Add("@pStateId", saveClientInfoViewModel.StateId, DbType.Int32);
                dp_params.Add("@pNationalityId", saveClientInfoViewModel.NationalityId, DbType.Int32);
                dp_params.Add("@pCountryId", saveClientInfoViewModel.CountryId, DbType.Int32);
                dp_params.Add("@pGenderId", saveClientInfoViewModel.GenderId, DbType.Int32);
                dp_params.Add("@pTitleId", saveClientInfoViewModel.TitleId, DbType.Int32);
                dp_params.Add("@pPasswordHash", saveClientInfoViewModel.PasswordHash, DbType.String);
                dp_params.Add("@pFranchiseId", saveClientInfoViewModel.FranchiseId, DbType.Guid);
                dp_params.Add("@pNotes", saveClientInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pId", saveClientInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await _dapperRepository.InsertAsync<Guid?>("[CLIENT].[InsertUpdateClient]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                return dp_params.Get<Guid?>("@pOutId");

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid> DeleteClientAsync(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", id, DbType.Guid);
            var result = await _dapperRepository.UpdateAsync<Guid>("[CLIENT].[DeleteClient]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<ClientInfo> GetClientInfoAsync(Guid userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", userId, DbType.Guid);
                var result = await _dapperRepository.GetListAsync<ClientInfo>("[CLIENT].[GetClientInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ClientSearchResponse> GetClientsAsync(ClientSearchRequest request)
        {
            ClientSearchResponse ClientResponse = new ClientSearchResponse();

            var dp_params = new DynamicParameters();
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            dp_params.Add("@pUserId", request.UserId, DbType.Guid);
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
            dp_params.Add("@pGenderId", request.GenderId, DbType.Int32);
            dp_params.Add("@pStatusId", request.StatusId, DbType.Int32);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            var result = await _dapperRepository.GetAllAsync<SearchClientViewModel>("[CLIENT].[uspGetAllClients]"
                , dp_params,
                commandType: CommandType.StoredProcedure);

            ClientResponse.Response = result.Item1;
            ClientResponse.TotalRecords = result.Item2;

            return ClientResponse;
        }

        // Keep the old sync method for backward compatibility
        public Guid DeleteClient(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", id, DbType.Guid);
            var result = _dapperRepository.Update<Guid>("[CLIENT].[DeleteClient]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
