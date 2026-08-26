using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Services.Account.Transaction;
using Scheduler.API.Models.Account.Transaction;
using Scheduler.API.Common;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : BaseController
    {
        ITransaction _transaction;
        public TransactionsController(ITransaction transaction, ILogger<TransactionsController> logger) : base(logger)
        {
            _transaction = transaction;
        }

        [HttpPost]
        [Route("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateTransaction(UpsertTransactionInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Transaction data is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _transaction.CreateUpdateTransactionAsync(model);

                if (result == null)
                    throw new InvalidOperationException("Failed to create/update transaction");

                return result;
            }, "Transaction created/Updated successfully!");
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> GetTransactionDetails(Guid transactionID)
        {
            if (transactionID == Guid.Empty)
                return ValidationError("Valid transaction ID is required");

            return await ExecuteAsync(
                () => _transaction.GetTransactionInfoAsync(transactionID),
                "Transaction details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> GetTransactionList(TransactionSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search request data is required");

            return await ExecuteAsync(
                () => _transaction.GetTransactionsAsync(model),
                "Transaction list retrieved successfully!"
            );
        }
    }
}
