using Scheduler.API.Models.Account.BankAccount;

namespace Scheduler.API.Services.Account.BankAccount
{
    public interface IBankAccount
    {
        Task<BankAccountInfo> GetBankAccountAsync(Guid UserId);
        Task<Guid?> UpsertBankAccount(UpsertBankAccountViewModel upsertBankAccountViewModel);
        Task<bool> UpdateConnectedAccountIdAsync(Guid userId, string connectedAccountId);
        Task<bool> UserHasBankAccountAsync(Guid userId);
        Task<BankAccountInfo> GetOrCreateBankAccountAsync(UpsertBankAccountViewModel upsertBankAccountViewModel);
    }
}
