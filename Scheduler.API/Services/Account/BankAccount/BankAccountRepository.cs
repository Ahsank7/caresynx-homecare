using Scheduler.API.Models.Account.BankAccount;
using Dapper;
using System.Data;

namespace Scheduler.API.Services.Account.BankAccount
{
    public class BankAccountRepository : IBankAccount
    {
        IDapperRepository _dapperRepository = null;
        public BankAccountRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }
        public async Task<BankAccountInfo> GetBankAccountAsync(Guid UserId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", UserId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<BankAccountInfo>("[dbo].[GetUserBankAccountInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> UpsertBankAccount(UpsertBankAccountViewModel upsertBankAccountViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pBankId", upsertBankAccountViewModel.bankId, DbType.Int32);
                dp_params.Add("@pAccountNumber", upsertBankAccountViewModel.AccountNumber, DbType.String);
                dp_params.Add("@pBranchCode", upsertBankAccountViewModel.BranchCode, DbType.String);
                dp_params.Add("@pAccountHolderName", upsertBankAccountViewModel.AccountHolderName, DbType.String);
                dp_params.Add("@pIBAN", upsertBankAccountViewModel.IBAN, DbType.String);
                dp_params.Add("@pConnectedAccountId", upsertBankAccountViewModel.ConnectedAccountId, DbType.String);
                dp_params.Add("@pUserId", upsertBankAccountViewModel.UserId, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InserUpdateUserBankAccountInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateConnectedAccountIdAsync(Guid userId, string connectedAccountId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", userId, DbType.Guid);
                dp_params.Add("@pConnectedAccountId", connectedAccountId, DbType.String);

                var result = await Task.FromResult(_dapperRepository.Execute("[dbo].[UpdateUserBankAccountConnectedAccountId]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UserHasBankAccountAsync(Guid userId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", userId, DbType.Guid);
                
                var result = await Task.FromResult(_dapperRepository.GetList<BankAccountInfo>("[dbo].[GetUserBankAccountInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                
                return result != null && result.Any();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<BankAccountInfo> GetOrCreateBankAccountAsync(UpsertBankAccountViewModel upsertBankAccountViewModel)
        {
            try
            {
                // First, try to get existing bank account
                var existingAccount = await GetBankAccountAsync(upsertBankAccountViewModel.UserId);
                
                if (existingAccount != null)
                {
                    // User already has a bank account, update it
                    var updatedId = await UpsertBankAccount(upsertBankAccountViewModel);
                    if (updatedId.HasValue)
                    {
                        // Return the updated account
                        return await GetBankAccountAsync(upsertBankAccountViewModel.UserId);
                    }
                    else
                    {
                        throw new Exception("Failed to update existing bank account");
                    }
                }
                else
                {
                    // User doesn't have a bank account, create new one
                    var newId = await UpsertBankAccount(upsertBankAccountViewModel);
                    if (newId.HasValue)
                    {
                        // Return the newly created account
                        return await GetBankAccountAsync(upsertBankAccountViewModel.UserId);
                    }
                    else
                    {
                        throw new Exception("Failed to create new bank account");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetOrCreateBankAccountAsync: {ex.Message}");
            }
        }
    }
}
