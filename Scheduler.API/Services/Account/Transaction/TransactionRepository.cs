using Dapper;
using Scheduler.API.Models.Account.Transaction;
using System.Data;

namespace Scheduler.API.Services.Account.Transaction
{
    public class TransactionRepository : ITransaction
    {
        IDapperRepository _dapperRepository = null;
        public TransactionRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<Guid?> CreateUpdateTransactionAsync(UpsertTransactionInfoViewModel saveTransactionInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();


                dp_params.Add("@pReferenceId", saveTransactionInfoViewModel.ReferenceId, DbType.String);
                dp_params.Add("@pRemarks", saveTransactionInfoViewModel.Remarks, DbType.String);
                dp_params.Add("@pBankAccountId", saveTransactionInfoViewModel.BankAccountId, DbType.Guid);
                dp_params.Add("@pCardId", saveTransactionInfoViewModel.CardId, DbType.Guid);
                dp_params.Add("@pStatusId", saveTransactionInfoViewModel.StatusId, DbType.Int32);
                //dp_params.Add("@pTransactionDate", saveTransactionInfoViewModel.TransactionDate, DbType.Date);
                dp_params.Add("@pUserId", saveTransactionInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pTypeId", saveTransactionInfoViewModel.TypeId, DbType.Int32);
                dp_params.Add("@pTransactionId", saveTransactionInfoViewModel.TransactionId, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InsertUpdateUserTransaction]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return saveTransactionInfoViewModel.TransactionId = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Guid? DeleteTransaction(Guid id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", id, DbType.Guid);
                var result = _dapperRepository.Update<Guid>("[dbo].[DeleteUserTransaction]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TransactionInfo> GetTransactionInfoAsync(Guid TransactionID)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", TransactionID, DbType.Guid);
               var result = await Task.FromResult(_dapperRepository.GetList<TransactionInfo>("[dbo].[GetUserTransactionInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TransactionSearchResponse> GetTransactionsAsync(TransactionSearchRequest request)
        {
            try
            {
                TransactionSearchResponse TransactionResponse = new TransactionSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pTransactionTypeId", request.TypeId, DbType.Int32);
                dp_params.Add("@pTransactionDate", request.Date, DbType.Date);
                dp_params.Add("@pReferenceId", request.ReferenceId, DbType.String);
                dp_params.Add("@pUserNo", request.UserNo, DbType.String);
                dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<TransactionInfo>("[dbo].[uspGetAllUserTransaction]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                TransactionResponse.Response = result.Item1;
                TransactionResponse.TotalRecords = result.Item2;

                return TransactionResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
