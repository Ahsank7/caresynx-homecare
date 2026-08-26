using Dapper;
using Scheduler.API.Models.Preference;
using System.Data;

namespace Scheduler.API.Services.Preference
{
    public class PreferenceRepository : IPreference
    {
        private readonly IDapperRepository _dapperRepository;

        public PreferenceRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        #region Client Preferences

        public async Task<List<ClientPreferenceInfo>> GetClientPreferencesAsync(Guid clientId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ClientId", clientId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.GetList<ClientPreferenceInfo>(
                    "[dbo].[uspGetClientPreferences]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result?.ToList() ?? new List<ClientPreferenceInfo>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting client preferences: {ex.Message}", ex);
            }
        }

        public async Task<Guid> UpsertClientPreferenceAsync(UpsertClientPreferenceRequest request, Guid? userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@Id", request.Id, DbType.Guid);
                dp_params.Add("@ClientId", request.ClientId, DbType.Guid);
                dp_params.Add("@PreferenceType", request.PreferenceType, DbType.String);
                dp_params.Add("@PreferenceValue", request.PreferenceValue, DbType.String);
                dp_params.Add("@PreferenceItemId", request.PreferenceItemId, DbType.Int32);
                dp_params.Add("@IsRequired", request.IsRequired, DbType.Boolean);
                dp_params.Add("@UserId", userId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Get<Guid>(
                    "[dbo].[uspUpsertClientPreference]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error upserting client preference: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteClientPreferenceAsync(Guid id, Guid? userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@Id", id, DbType.Guid);
                dp_params.Add("@UserId", userId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Execute(
                    "[dbo].[uspDeleteClientPreference]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting client preference: {ex.Message}", ex);
            }
        }

        #endregion

        #region Service Provider Attributes

        public async Task<List<ServiceProviderAttributeInfo>> GetServiceProviderAttributesAsync(Guid serviceProviderId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ServiceProviderId", serviceProviderId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.GetList<ServiceProviderAttributeInfo>(
                    "[dbo].[uspGetServiceProviderAttributes]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result?.ToList() ?? new List<ServiceProviderAttributeInfo>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting service provider attributes: {ex.Message}", ex);
            }
        }

        public async Task<Guid> UpsertServiceProviderAttributeAsync(UpsertServiceProviderAttributeRequest request, Guid? userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@Id", request.Id, DbType.Guid);
                dp_params.Add("@ServiceProviderId", request.ServiceProviderId, DbType.Guid);
                dp_params.Add("@AttributeType", request.AttributeType, DbType.String);
                dp_params.Add("@AttributeValue", request.AttributeValue, DbType.String);
                dp_params.Add("@AttributeItemId", request.AttributeItemId, DbType.Int32);
                dp_params.Add("@UserId", userId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Get<Guid>(
                    "[dbo].[uspUpsertServiceProviderAttribute]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error upserting service provider attribute: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteServiceProviderAttributeAsync(Guid id, Guid? userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@Id", id, DbType.Guid);
                dp_params.Add("@UserId", userId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Execute(
                    "[dbo].[uspDeleteServiceProviderAttribute]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting service provider attribute: {ex.Message}", ex);
            }
        }

        #endregion

        #region Matching

        public async Task<List<MatchingServiceProviderInfo>> GetMatchingServiceProvidersAsync(Guid clientId, Guid? franchiseId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ClientId", clientId, DbType.Guid);
                dp_params.Add("@FranchiseId", franchiseId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.GetList<MatchingServiceProviderInfo>(
                    "[dbo].[uspGetMatchingServiceProviders]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result?.ToList() ?? new List<MatchingServiceProviderInfo>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting matching service providers: {ex.Message}", ex);
            }
        }

        #endregion
    }
}

