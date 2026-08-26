using Dapper;
using Microsoft.IdentityModel.Tokens;
using Scheduler.API.Helper;
using Scheduler.API.Models.Authentication;
using Scheduler.API.Models.User;
using Scheduler.API.Services.User;
using Scheduler.API.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Scheduler.API.Services.Authentication
{
    public class AuthenticationRepository : IAuthentication
    {
        IDapperRepository _dapperRepository = null;
        IConfiguration _configuration = null;
        IUser _user = null;
        IUrlService _urlService = null;

        
        public AuthenticationRepository(IDapperRepository DapperRepository, IConfiguration configuration, IUser user, IUrlService urlService)
        {
            _dapperRepository = DapperRepository;
            _configuration = configuration;
            _user = user;
            _urlService = urlService;
        }

        public async Task<AuthResponse> Authenticate(UserLoginRequest request)
        {
            // Validate the user (replace with actual DB query)
            var user = await _user.GetUserInfoAsync(request.Username!,request.Password!);

            if (user == default) return null;  // Invalid credentials

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return token;

        }

        private AuthResponse GenerateJwtToken(UserInfo userInfo)
        {
            
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = jwtSettings["Secret"]; 
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

            var claims = new[]
            {
            new Claim("OrganizationId", Convert.ToString(userInfo.OrganizationId)),
            new Claim("FranchiseId", Convert.ToString(userInfo.FranchiseId)),
            new Claim("UserNo", userInfo.UserNo!),
            new Claim("UserID", userInfo.UserId.ToString()),
            new Claim("UserType", userInfo.UserType.ToString()),
            new Claim("FullName", userInfo.FirstName+" "+userInfo.LastName),
            new Claim("Email", userInfo.Email!),
            new Claim("ProfileImagePath", _urlService.BuildWebPath(userInfo.ProfileImagePath) ??""),
            new Claim("Name", userInfo.FirstName!),
            new Claim("RoleId", Convert.ToString(userInfo.RoleId))
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                notBefore: DateTime.UtcNow,
                signingCredentials: creds);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public async Task<UserAuthenticationInfo> GetAuthenticationInfoAsync(Guid userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", userId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<UserAuthenticationInfo>("[dbo].[GetUserCreadentialsInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> UpdateUserAuthenticationInfo(UpdateUserAuthenticationInfoViewModel saveAuthenticationInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pUserName", saveAuthenticationInfoViewModel.UserName, DbType.String);
                dp_params.Add("@pPassword", saveAuthenticationInfoViewModel.Password, DbType.String);
                dp_params.Add("@pRoleId", saveAuthenticationInfoViewModel.RoleId ?? (object)DBNull.Value, DbType.Int32);
                dp_params.Add("@pUserId", saveAuthenticationInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[UpdateUserAuthenticationInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return saveAuthenticationInfoViewModel.UserId = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pUserId", changePasswordViewModel.UserId, DbType.Guid);
                dp_params.Add("@pOldPassword", changePasswordViewModel.OldPassword, DbType.String);
                dp_params.Add("@pNewPassword", changePasswordViewModel.NewPassword, DbType.String);
                dp_params.Add("@pIsValid", false, DbType.Boolean, direction: ParameterDirection.Output);
                
                var result = await Task.FromResult(_dapperRepository.Insert<bool>("[dbo].[ChangeUserPassword]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return dp_params.Get<bool>("@pIsValid");
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UserNameCheckResult> UserNameExistsAsync(string userName)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserName", userName, DbType.String);

                // Use a simpler approach - just get the first result set directly
                var result = await _dapperRepository.GetAsync<UserNameCheckResult>(
                    "[User].[CheckUserNameExists]",
                    dp_params,
                    CommandType.StoredProcedure
                );
                
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return null;
            }
        }
    }
}
