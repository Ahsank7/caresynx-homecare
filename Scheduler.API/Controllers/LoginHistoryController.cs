using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.LoginHistory;
using Scheduler.API.Services.LoginHistory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginHistoryController : ControllerBase
    {
        private readonly ILoginHistory _loginHistory;

        public LoginHistoryController(ILoginHistory loginHistory)
        {
            _loginHistory = loginHistory;
        }

        [HttpPost]
        [Authorize]
        [Route("GetLoginHistory")]
        public async Task<IActionResult> GetLoginHistoryAsync(LoginHistoryRequest request)
        {
            try
            {
                var result = await _loginHistory.GetLoginHistoryAsync(request);
                return Ok(new Response<LoginHistoryResponse>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Login history retrieved successfully!",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<LoginHistoryResponse>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving login history.",
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("InsertLoginHistory")]
        public async Task<IActionResult> InsertLoginHistoryAsync(InsertLoginHistoryRequest request)
        {
            try
            {
                // Capture the real IP address from the server side
                request.IPAddress = SystemInfoHelper.GetIPAddress(HttpContext);
                
                var result = await _loginHistory.InsertLoginHistoryAsync(request);
                return Ok(new Response<int>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Login history inserted successfully!",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<int>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while inserting login history.",
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("UpdateLogoutTime")]
        public async Task<IActionResult> UpdateLogoutTimeAsync(Guid userId, DateTime? loginTime = null)
        {
            try
            {
                var result = await _loginHistory.UpdateLogoutTimeAsync(userId, loginTime);
                return Ok(new Response<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Logout time updated successfully!",
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating logout time.",
                    IsSuccess = false
                });
            }
        }
    }
}
