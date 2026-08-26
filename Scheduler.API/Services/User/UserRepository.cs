using Dapper;
using Scheduler.API.Models.User;
using Scheduler.API.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Services.User
{
    public class UserRepository : IUser
    {
        IDapperRepository _dapperRepository = null;
        IUrlService _urlService = null;
        private readonly ILogger<UserRepository> _logger;

        /// <summary>
        /// [User].[uspGetAllUsers] expects @pSortType to be 'ASC' or 'DESC'.
        /// A legacy repository bug passed <see cref="UserSearchRequest.SortColumn"/> into @pSortType (e.g. 'FirstName').
        /// </summary>
        private static string NormalizeSortDirection(string? sortType, string? sortColumn)
        {
            if (string.IsNullOrWhiteSpace(sortType))
                return "ASC";
            var t = sortType.Trim();
            if (!string.IsNullOrEmpty(sortColumn) &&
                string.Equals(t, sortColumn, StringComparison.OrdinalIgnoreCase))
                return "ASC";
            if (string.Equals(t, "DESC", StringComparison.OrdinalIgnoreCase))
                return "DESC";
            return "ASC";
        }

        public UserRepository(IDapperRepository DapperRepository, IUrlService urlService, ILogger<UserRepository> logger)
        {
            _dapperRepository = DapperRepository;
            _urlService = urlService;
            _logger = logger;
        }

        public async Task<Guid?> CreateUpdateUserAsync(SaveUserInfoViewModel saveUserInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserType", saveUserInfoViewModel.UserType, DbType.Int32);
                dp_params.Add("@pFirstName", saveUserInfoViewModel.FirstName, DbType.String);
                dp_params.Add("@pSurName", saveUserInfoViewModel.SurName, DbType.String);
                dp_params.Add("@pLastName", saveUserInfoViewModel.LastName, DbType.String);
                dp_params.Add("@pAlias", saveUserInfoViewModel.Alias, DbType.String);
                dp_params.Add("@pUserName", saveUserInfoViewModel.UserName, DbType.String);
                dp_params.Add("@pPhoneNo", saveUserInfoViewModel.PhoneNo, DbType.String);
                dp_params.Add("@pMobileNo", saveUserInfoViewModel.MobileNo, DbType.String);
                dp_params.Add("@pPassportNo", saveUserInfoViewModel.PassportNo, DbType.String);
                dp_params.Add("@pIdentityNo", saveUserInfoViewModel.IdentityNo, DbType.String);
                dp_params.Add("@pEthnicityId", saveUserInfoViewModel.EthnicityId, DbType.Int32);
                dp_params.Add("@pAge", saveUserInfoViewModel.Age, DbType.Int32);
                dp_params.Add("@pBirthDate", saveUserInfoViewModel.BirthDate, DbType.Date);
                dp_params.Add("@pJoiningDate", saveUserInfoViewModel.JoiningDate, DbType.Date);
                dp_params.Add("@pCountyId", saveUserInfoViewModel.CountyId, DbType.Int32);
                dp_params.Add("@pMaritalStatusId", saveUserInfoViewModel.MaritalStatusId, DbType.Int32);
                dp_params.Add("@pEmail", saveUserInfoViewModel.Email, DbType.String);
                dp_params.Add("@pAddressLine1", saveUserInfoViewModel.AddressLine1, DbType.String);
                dp_params.Add("@pAddressLine2", saveUserInfoViewModel.AddressLine2, DbType.String);
                dp_params.Add("@pAddressLine3", saveUserInfoViewModel.AddressLine3, DbType.String);
                dp_params.Add("@pLatitude", saveUserInfoViewModel.Latitude, DbType.Decimal);
                dp_params.Add("@pLongitude", saveUserInfoViewModel.Longitude, DbType.Decimal);
                dp_params.Add("@pStateId", saveUserInfoViewModel.StateId, DbType.Int32);
                dp_params.Add("@pNationalityId", saveUserInfoViewModel.NationalityId, DbType.Int32);
                dp_params.Add("@pCountryId", saveUserInfoViewModel.CountryId, DbType.Int32);
                dp_params.Add("@pGenderId", saveUserInfoViewModel.GenderId, DbType.Int32);
                dp_params.Add("@pTitleId", saveUserInfoViewModel.TitleId, DbType.Int32);
                dp_params.Add("@pPasswordHash", saveUserInfoViewModel.PasswordHash, DbType.String);
                dp_params.Add("@pFranchiseId", saveUserInfoViewModel.FranchiseId, DbType.Guid);
                dp_params.Add("@pAddressId", saveUserInfoViewModel.AddressId, DbType.Guid);
                dp_params.Add("@pNotes", saveUserInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pId", saveUserInfoViewModel.Id, DbType.Guid);

                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await _dapperRepository.InsertAsync<Guid?>("[User].[InsertUpdateUser]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                return dp_params.Get<Guid?>("@pOutId");

            }
            catch (SqlException ex)
            {
                // Re-throw SqlException so it can be properly handled by the controller
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Guid> DeleteUserAsync(Guid id, int userStatusAction)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pUserId", id, DbType.Guid);
            dp_params.Add("@pUserStatusAction", userStatusAction, DbType.Int16);
            var result = await _dapperRepository.UpdateAsync<Guid>("[User].[DeleteUser]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<UserInfo> GetUserInfoAsync(Guid userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", userId, DbType.Guid);
                var result = await _dapperRepository.GetListAsync<UserInfo>("[User].[GetUserInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                var userResponse = result.FirstOrDefault()!;
                userResponse.ProfileImagePath = _urlService.BuildWebPath(userResponse.ProfileImagePath);

                return userResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user info for user {UserId}", userId);
                return null;
            }
        }



        public async Task<UserInfo> GetUserInfoAsync(string login, string password)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pLogin", login, DbType.String);
                dp_params.Add("@pPassword", password, DbType.String);
                var result = await _dapperRepository.GetListAsync<UserInfo>("[User].[GetLoginUserInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                var userResponse = result.FirstOrDefault()!;
                if (userResponse != null)
                {
                    userResponse.ProfileImagePath = _urlService.BuildWebPath(userResponse.ProfileImagePath);
                }

                return userResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load login user info for username {Username}", login);
                throw;
            }
        }

        public async Task<UserInfo> GetUserInfoByUserNoAsync(string userNo)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserNo", userNo, DbType.String);
                var result = await _dapperRepository.GetListAsync<UserInfo>("[User].[uspGetUserByUserNo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                var userResponse = result.FirstOrDefault()!;
                if (userResponse != null)
                {
                    userResponse.ProfileImagePath = _urlService.BuildWebPath(userResponse.ProfileImagePath);
                }

                return userResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserSearchResponse> GetUsersAsync(UserSearchRequest request)
        {
            UserSearchResponse UserResponse = new UserSearchResponse();

            var dp_params = new DynamicParameters();
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            dp_params.Add("@pUserId", request.UserId, DbType.Guid);
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", NormalizeSortDirection(request.SortType, request.SortColumn), DbType.String);
            dp_params.Add("@pEmail", request.Email, DbType.String);
            dp_params.Add("@pMobileNumber", request.MobileNumber, DbType.String);
            dp_params.Add("@pPhoneNumber", request.PhoneNumber, DbType.String);
            dp_params.Add("@pFirstName", request.FirstName, DbType.String);
            dp_params.Add("@pEthnicityId", request.EthnicityId, DbType.Int32);
            dp_params.Add("@pLastName", request.LastName, DbType.String);
            dp_params.Add("@pJoiningDate", request.JoiningDate, DbType.Date);
            dp_params.Add("@pGenderId", request.GenderId, DbType.Int32);
            dp_params.Add("@pUserType", request.UserType, DbType.Int32);
            dp_params.Add("@pUserNo", request.UserNo, DbType.String);
            dp_params.Add("@pStatusId", request.StatusId, DbType.Int32);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            dp_params.Add("@pCurrentUserId", request.CurrentUserId, DbType.Guid); // Role hierarchy filtering
            var result = await _dapperRepository.GetAllAsync<SearchUserViewModel>("[User].[uspGetAllUsers]"
                , dp_params,
                commandType: CommandType.StoredProcedure);

            UserResponse.Response = result.Item1;
            UserResponse.TotalRecords = result.Item2;

            return UserResponse;
        }

        public async Task<bool> UploadProfileImageAsync(Guid userId, string profileImagePath)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", userId, DbType.Guid);
                dp_params.Add("@pProfileImagePath", profileImagePath, DbType.String);
                var result = await _dapperRepository.UpdateAsync<bool>("[User].[UploadProfileImage]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Keep the old sync method for backward compatibility
        public Guid DeleteUser(Guid id, int userStatusAction)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pUserId", id, DbType.Guid);
            dp_params.Add("@pUserStatusAction", userStatusAction, DbType.Int16);
            var result = _dapperRepository.Update<Guid>("[User].[DeleteUser]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<bool> UploadProfileImage(Guid userId, string profileImagePath)
        {
            return await UploadProfileImageAsync(userId, profileImagePath);
        }
    }
}
