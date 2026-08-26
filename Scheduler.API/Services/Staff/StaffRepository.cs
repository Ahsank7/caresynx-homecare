using Dapper;
using Scheduler.API.Models.Staff;
using System.Data;

namespace Scheduler.API.Services.Staff
{
    public class StaffRepository : IStaff
    {
        IDapperRepository _dapperRepository = null;
        public StaffRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<Guid?> CreateUpdateStaffAsync(SaveStaffInfoViewModel saveStaffInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pFirstName", saveStaffInfoViewModel.FirstName, DbType.String);
                dp_params.Add("@pSurName", saveStaffInfoViewModel.SurName, DbType.String);
                dp_params.Add("@pLastName", saveStaffInfoViewModel.LastName, DbType.String);
                dp_params.Add("@pAlias", saveStaffInfoViewModel.Alias, DbType.String);
                dp_params.Add("@pUserName", saveStaffInfoViewModel.UserName, DbType.String);
                dp_params.Add("@pPhoneNo", saveStaffInfoViewModel.PhoneNo, DbType.String);
                dp_params.Add("@pMobileNo", saveStaffInfoViewModel.MobileNo, DbType.String);
                dp_params.Add("@pPassportNo", saveStaffInfoViewModel.PassportNo, DbType.String);
                dp_params.Add("@pIdentityNo", saveStaffInfoViewModel.IdentityNo, DbType.String);
                dp_params.Add("@pEthnicityId", saveStaffInfoViewModel.EthnicityId, DbType.Int32);
                dp_params.Add("@pAge", saveStaffInfoViewModel.Age, DbType.Int32);
                dp_params.Add("@pBirthDate", saveStaffInfoViewModel.BirthDate, DbType.Date);
                dp_params.Add("@pJoiningDate", saveStaffInfoViewModel.JoiningDate, DbType.Date);
                dp_params.Add("@pCountyId", saveStaffInfoViewModel.CountyId, DbType.Int32);
                dp_params.Add("@pMaritalStatusId", saveStaffInfoViewModel.MaritalStatusId, DbType.Int32);
                dp_params.Add("@pEmail", saveStaffInfoViewModel.Email, DbType.String);
                dp_params.Add("@pAddressLine1", saveStaffInfoViewModel.AddressLine1, DbType.String);
                dp_params.Add("@pAddressLine2", saveStaffInfoViewModel.AddressLine2, DbType.String);
                dp_params.Add("@pAddressLine3", saveStaffInfoViewModel.AddressLine3, DbType.String);
                dp_params.Add("@pLatitude", saveStaffInfoViewModel.Latitude, DbType.Decimal);
                dp_params.Add("@pLongitude", saveStaffInfoViewModel.Longitude, DbType.Decimal);
                dp_params.Add("@pStateId", saveStaffInfoViewModel.StateId, DbType.Int32);
                dp_params.Add("@pNationalityId", saveStaffInfoViewModel.NationalityId, DbType.Int32);
                dp_params.Add("@pCountryId", saveStaffInfoViewModel.CountryId, DbType.Int32);
                dp_params.Add("@pGenderId", saveStaffInfoViewModel.GenderId, DbType.Int32);
                dp_params.Add("@pTitleId", saveStaffInfoViewModel.TitleId, DbType.Int32);
                dp_params.Add("@pPasswordHash", saveStaffInfoViewModel.PasswordHash, DbType.String);
                dp_params.Add("@pFranchiseId", saveStaffInfoViewModel.FranchiseId, DbType.Guid);
                dp_params.Add("@pNotes", saveStaffInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pRoleId", saveStaffInfoViewModel.RoleId, DbType.Int32);
                dp_params.Add("@pId", saveStaffInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await _dapperRepository.InsertAsync<StaffInfo>("[Staff].[InsertUpdateStaff]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                return dp_params.Get<Guid?>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid> DeleteStaffAsync(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", id, DbType.Guid);
            var result = await _dapperRepository.UpdateAsync<Guid>("[Staff].[DeleteStaff]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<StaffInfo> GetStaffInfoAsync(Guid userId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", userId, DbType.Guid);
            var result = await _dapperRepository.GetListAsync<StaffInfo>("[Staff].[GetStaffInfo]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault()!;
        }

        public async Task<StaffSearchResponse> GetStaffsAsync(StaffSearchRequest request)
        {
            StaffSearchResponse staffResponse = new StaffSearchResponse();

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
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            dp_params.Add("@pCurrentUserId", request.CurrentUserId, DbType.Guid); // Role hierarchy filtering
            var result = await _dapperRepository.GetAllAsync<SearchStaffViewModel>("[Staff].[uspGetAllStaffs]"
                , dp_params,
                commandType: CommandType.StoredProcedure);

            staffResponse.Response = result.Item1;
            staffResponse.TotalRecords = result.Item2;

            return staffResponse;
        }

        // Keep the old sync method for backward compatibility
        public Guid DeleteStaff(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pID", id, DbType.Guid);
            var result = _dapperRepository.Update<Guid>("[Staff].[DeleteStaff]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
