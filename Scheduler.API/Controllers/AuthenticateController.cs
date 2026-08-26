using Scheduler.API.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Scheduler.API.Services.Authentication;
using Scheduler.API.Models.Authentication;
using Scheduler.API.Services.Security;
using Scheduler.API.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IAuthentication _authentication;
        private readonly ICrypto _crypto;

        public AuthenticateController(IConfiguration configuration, IAuthentication authentication, ICrypto crypto, ILogger<AuthenticateController> logger)
            : base(logger)
        {
            _configuration = configuration;
            _authentication = authentication;
            _crypto = crypto;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            if (model == null)
                return ValidationError("Login data is required");

            return await ExecuteAsync(async () =>
            {
                model.Password = _crypto.Encrypt(model.Password!);
                var authResponse = await _authentication.Authenticate(model);

                if (authResponse == null)
                    throw new UnauthorizedAccessException("Invalid username or password");

                return authResponse;
            }, "Login successful");
        }

        [HttpPost]
        [Route("UpdateUserCredentials")]
        public async Task<IActionResult> UpdateUserCredentials([FromBody] UpdateUserAuthenticationInfoViewModel model)
        {
            _logger.LogInformation("UpdateUserCredentials called with model: {@Model}", model);

            if (model == null)
                return ValidationError("User credentials data is required");

            if (string.IsNullOrEmpty(model.UserName))
                return ValidationError("Username is required");

            if (model.UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            var existing = await _authentication.GetAuthenticationInfoAsync(model.UserId);
            string? existingPlain = null;
            var existingCipher = existing?.Password;
            if (!string.IsNullOrEmpty(existingCipher))
            {
                try
                {
                    existingPlain = _crypto.Decrypt(existingCipher);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not decrypt existing password for user {UserId}", model.UserId);
                }
            }

            var passwordUnchanged = !string.IsNullOrEmpty(existingPlain)
                && !string.IsNullOrEmpty(model.Password)
                && model.Password == existingPlain.MaskString();

            if (!passwordUnchanged)
            {
                if (string.IsNullOrEmpty(model.Password))
                    return ValidationError("Password is required");

                if (!CredentialPasswordPolicy.IsSatisfied(model.Password!))
                    return ValidationError(CredentialPasswordPolicy.RequirementMessage);

                model.Password = _crypto.Encrypt(model.Password!);
            }
            else
            {
                model.Password = existingCipher;
            }

            // Check if username already exists for another user
            var userExist = await _authentication.UserNameExistsAsync(model.UserName!);
            if (userExist!=null && userExist.IsUserNameExists==1 && userExist.UserId!=model.UserId)
                return ValidationError("Username already exists");

            _logger.LogInformation("Model validation passed. UserId: {UserId}, UserName: {UserName}, RoleId: {RoleId}",
                model.UserId, model.UserName, model.RoleId);

            return await ExecuteAsync(
                async () => {
                    var result = await _authentication.UpdateUserAuthenticationInfo(model);
                    return result;
                },
                "User Credentials Updated successfully!"
            );
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (model == null)
                return ValidationError("Change password data is required");

            if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
                return ValidationError("Old password and new password are required");

            if (model.OldPassword == model.NewPassword)
                return ValidationError("New password must be different from old password");

            return await ExecuteAsync(async () =>
            {
                // Encrypt passwords
                model.OldPassword = _crypto.Encrypt(model.OldPassword);
                model.NewPassword = _crypto.Encrypt(model.NewPassword);

                var result = await _authentication.ChangePassword(model);

                if (!result)
                    throw new InvalidOperationException("Invalid old password or user not found");

                return result;
            }, "Password changed successfully!");
        }

        [HttpGet]
        [Route("GetUserCredentials")]
        public async Task<IActionResult> GetUserCredentials(Guid UserId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _authentication.GetAuthenticationInfoAsync(UserId);
                
                if (result == null)
                    throw new InvalidOperationException("No credentials found");

                if (result.Password != null)
                {
                    result.Password = _crypto.Decrypt(result.Password!).MaskString();
                }

                return result;
            }, "Authentication info retrieved successfully!");
        }

    }
}
