using Scheduler.API.Models.Account.Transaction;

namespace Scheduler.API.Services.Account.Transaction
{
    public interface ITransaction
    {
        Task<TransactionSearchResponse> GetTransactionsAsync(TransactionSearchRequest request);
        Task<TransactionInfo> GetTransactionInfoAsync(Guid TransactionID);
        Task<Guid?> CreateUpdateTransactionAsync(UpsertTransactionInfoViewModel saveTransactionInfoViewModel);
        Guid? DeleteTransaction(Guid id);
    }
}
