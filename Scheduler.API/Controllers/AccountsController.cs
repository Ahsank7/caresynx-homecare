using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Services.Account.BankAccount;
using Scheduler.API.Services.Account.Card;
using Scheduler.API.Models.Account.BankAccount;
using Scheduler.API.Models.Account.Card;
using Scheduler.API.Common;
using Scheduler.API.Services.Security;
using Scheduler.API.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : BaseController
    {
        IBankAccount  _bankAccount;
        ICard _card;
        ICrypto _crypto;
        public AccountsController(IBankAccount bankAccount,ICard card, ICrypto crypto, ILogger<AccountsController> logger) : base(logger)
        {
            _bankAccount = bankAccount;
            _card = card;
            _crypto = crypto;
        }

        [HttpPost]
        [Route("UpsertUserBankAccount")]
        public async Task<IActionResult> UpsertBankAccount(UpsertBankAccountViewModel upsertBankAccountViewModel)
        {
            if (upsertBankAccountViewModel == null)
                return ValidationError("Bank account data is required");

            return await ExecuteAsync(async () =>
            {
                upsertBankAccountViewModel.AccountNumber = _crypto.Encrypt(upsertBankAccountViewModel.AccountNumber!);
                var result = await _bankAccount.UpsertBankAccount(upsertBankAccountViewModel);

                if (result == null)
                    throw new InvalidOperationException("Failed to add/update bank account");

                return result;
            }, "User Bank Account added/updated successfully!");
        }

        [HttpGet]
        [Route("GetUserBankAccount")]
        public async Task<IActionResult> GetBankAccountAsync(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _bankAccount.GetBankAccountAsync(UserId);

                if (result != null && result.AccountNumber != null)
                {
                    result.AccountNumber = _crypto.Decrypt(result.AccountNumber!).MaskString();
                }

                return result;
            }, "Bank Account retrieved successfully!");
        }

        [HttpPost]
        [Route("UpsertUserCardInfo")]
        public async Task<IActionResult> UpsertCardInfo(UpsertCardInfoViewModel upsertCardInfoViewModel)
        {
            if (upsertCardInfoViewModel == null)
                return ValidationError("Card data is required");

            return await ExecuteAsync(async () =>
            {
                upsertCardInfoViewModel.CardNumber = _crypto.Encrypt(upsertCardInfoViewModel.CardNumber!);
                upsertCardInfoViewModel.CVV = _crypto.Encrypt(upsertCardInfoViewModel.CVV!);
                var result = await _card.UpsertCardInfo(upsertCardInfoViewModel);

                if (result == null)
                    throw new InvalidOperationException("Failed to add/update card");

                return result;
            }, "User Card added/updated successfully!");
        }

        [HttpGet]
        [Route("GetUserCardInfo")]
        public async Task<IActionResult> GetCardInfoAsync(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _card.GetCardInfoAsync(UserId);
                if (result != null && result.CardNumber != null)
                {
                    result.CardNumber = _crypto.Decrypt(result.CardNumber!).MaskString();
                }
                if (result != null && result.CVV != null)
                {
                    result.CVV = _crypto.Decrypt(result.CVV!).MaskString();
                }

                return result;
            }, "Card info retrieved successfully!");
        }

        
    }
}
