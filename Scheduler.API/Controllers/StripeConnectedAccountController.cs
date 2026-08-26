using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Services.Payment;
using Scheduler.API.Models.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Scheduler.API.Services.Account.BankAccount;
using Scheduler.API.Models.Account.BankAccount;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StripeConnectedAccountController : BaseController
    {
        private readonly IStripeConnectedAccountService _connectedAccountService;
        private readonly IBankAccount _bankAccountService;

        public StripeConnectedAccountController(
            IStripeConnectedAccountService connectedAccountService,
            IBankAccount bankAccountService,
            ILogger<StripeConnectedAccountController> logger) : base(logger)
        {
            _connectedAccountService = connectedAccountService;
            _bankAccountService = bankAccountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateConnectedAccount([FromBody] ConnectedAccountRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _connectedAccountService.CreateConnectedAccountAsync(request);
                
                if (!result.Success)
                    throw new InvalidOperationException(result.ErrorMessage);

                return result;
            }, "Connected account created successfully");
        }

        [HttpPost("create-for-user")]
        public async Task<IActionResult> CreateConnectedAccountForUser([FromBody] CreateConnectedAccountForUserRequest request)
        {
            try
            {
                // Get user's bank account info
                var bankAccount = await _bankAccountService.GetBankAccountAsync(request.UserId);
                if (bankAccount == null)
                {
                    return BadRequest(new Response<ConnectedAccountResult>
                    {
                        Message = "User does not have a bank account configured",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Create connected account request
                var connectedAccountRequest = new ConnectedAccountRequest
                {
                    Email = request.Email,
                    Country = request.Country ?? "US",
                    BusinessType = "individual",
                    CompanyName = bankAccount.AccountHolderName,
                    FirstName = request.FirstName ?? bankAccount.AccountHolderName?.Split(' ').FirstOrDefault(),
                    LastName = request.LastName ?? bankAccount.AccountHolderName?.Split(' ').Skip(1).FirstOrDefault(),
                    Phone = request.Phone,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    PostalCode = request.PostalCode
                };

                var result = await _connectedAccountService.CreateConnectedAccountAsync(connectedAccountRequest);
                
                if (result.Success)
                {
                    // Update bank account with connected account ID
                    var updateRequest = new UpsertBankAccountViewModel
                    {
                        //BankAccountId = bankAccount.BankAccountId,
                        UserId = bankAccount.UserId,
                        AccountHolderName = bankAccount.AccountHolderName,
                        AccountNumber = bankAccount.AccountNumber,
                        bankId = bankAccount.bankId,
                        BranchCode = bankAccount.BranchCode,
                        IBAN = bankAccount.IBAN,
                        ConnectedAccountId = result.ConnectedAccountId
                    };

                    await _bankAccountService.UpsertBankAccount(updateRequest);

                    return Ok(new Response<ConnectedAccountResult>
                    {
                        Data = result,
                        Message = "Connected account created and linked to user successfully",
                        Status = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return BadRequest(new Response<ConnectedAccountResult>
                    {
                        Data = result,
                        Message = result.ErrorMessage,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<ConnectedAccountResult>
                {
                    Message = $"Error creating connected account for user: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet("{connectedAccountId}")]
        public async Task<IActionResult> GetConnectedAccount(string connectedAccountId)
        {
            if (string.IsNullOrEmpty(connectedAccountId))
                return ValidationError("Connected account ID is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _connectedAccountService.GetConnectedAccountAsync(connectedAccountId);
                
                if (!result.Success)
                    throw new InvalidOperationException(result.ErrorMessage);

                return result;
            }, "Connected account retrieved successfully");
        }

        [HttpPut("{connectedAccountId}")]
        public async Task<IActionResult> UpdateConnectedAccount(string connectedAccountId, [FromBody] ConnectedAccountUpdateRequest request)
        {
            try
            {
                var success = await _connectedAccountService.UpdateConnectedAccountAsync(connectedAccountId, request);
                
                if (success)
                {
                    return Ok(new Response<bool>
                    {
                        Data = success,
                        Message = "Connected account updated successfully",
                        Status = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return BadRequest(new Response<bool>
                    {
                        Data = success,
                        Message = "Failed to update connected account",
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Message = $"Error updating connected account: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost("{connectedAccountId}/account-link")]
        public async Task<IActionResult> CreateAccountLink(string connectedAccountId)
        {
            try
            {
                var result = await _connectedAccountService.CreateAccountLinkAsync(connectedAccountId);
                
                if (result.Success)
                {
                    return Ok(new Response<AccountLinkResult>
                    {
                        Data = result,
                        Message = "Account link created successfully",
                        Status = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return BadRequest(new Response<AccountLinkResult>
                    {
                        Data = result,
                        Message = result.ErrorMessage,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<AccountLinkResult>
                {
                    Message = $"Error creating account link: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpDelete("{connectedAccountId}")]
        public async Task<IActionResult> DeleteConnectedAccount(string connectedAccountId)
        {
            try
            {
                var success = await _connectedAccountService.DeleteConnectedAccountAsync(connectedAccountId);
                
                if (success)
                {
                    return Ok(new Response<bool>
                    {
                        Data = success,
                        Message = "Connected account deleted successfully",
                        Status = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return BadRequest(new Response<bool>
                    {
                        Data = success,
                        Message = "Failed to delete connected account",
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Message = $"Error deleting connected account: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }

    public class CreateConnectedAccountForUserRequest
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
