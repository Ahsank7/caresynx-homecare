using Dapper;
using Scheduler.API.Models.Address;
using System.Data;

namespace Scheduler.API.Services.Address
{
    public class AddressRepository : IAddress
    {
        IDapperRepository _dapperRepository = null;
        public AddressRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }
        public async Task<Guid?> CreateUpdateAddressAsync(SaveAddressInfoViewModel saveAddressInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pCountyId", saveAddressInfoViewModel.CountyId, DbType.Int32);
                dp_params.Add("@pAddressLine1", saveAddressInfoViewModel.AddressLine1, DbType.String);
                dp_params.Add("@pAddressLine2", saveAddressInfoViewModel.AddressLine2, DbType.String);
                dp_params.Add("@pAddressLine3", saveAddressInfoViewModel.AddressLine3, DbType.String);
                dp_params.Add("@pLatitude", saveAddressInfoViewModel.Latitude, DbType.Decimal);
                dp_params.Add("@pLongitude", saveAddressInfoViewModel.Longitude, DbType.Decimal);
                dp_params.Add("@pStateId", saveAddressInfoViewModel.StateId, DbType.Int32);
                dp_params.Add("@pCountryId", saveAddressInfoViewModel.CountryId, DbType.Int32);
                dp_params.Add("@pUserId", saveAddressInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pAddressTypeId", saveAddressInfoViewModel.AddressTypeId, DbType.Int32);
                dp_params.Add("@pId", saveAddressInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InsertUpdateUserAddress]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return saveAddressInfoViewModel.Id= dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> DeleteAddressAsync(Guid id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", id, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = _dapperRepository.Update<Guid>("[dbo].[DeleteUserAddress]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<AddressInfo> GetAddressInfoAsync(Guid AddressID)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", AddressID, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.GetList<AddressInfo>("[dbo].[GetUserAddressInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AddressSearchResponse> GetAddresssAsync(AddressSearchRequest request)
        {

            try
            {
                AddressSearchResponse addressResponse = new AddressSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pAddressTypeId", request.AddressTypeId, DbType.Int32);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<SearchAddressViewModel>("[dbo].[uspGetAllUserAddress]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                addressResponse.Response = result.Item1;
                addressResponse.TotalRecords = result.Item2;

                return addressResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
