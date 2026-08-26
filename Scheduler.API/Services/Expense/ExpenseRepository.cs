using Dapper;
using Scheduler.API.Models.Expense;
using Scheduler.API.Services.TaskLog;
using System.Data;

namespace Scheduler.API.Services.Expense
{
    public class ExpenseRepository : IExpense
    {
        IDapperRepository _dapperRepository = null;
        ITaskLog _taskLog = null;
        
        public ExpenseRepository(IDapperRepository DapperRepository, ITaskLog taskLog)
        {
            _dapperRepository = DapperRepository;
            _taskLog = taskLog;
        }

        public async Task<Guid?> CreateUpdateUserExpenseAsync(SaveUserExpenseInfoViewModel saveUserExpenseInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pNotes", saveUserExpenseInfoViewModel.Notes, DbType.String);
                dp_params.Add("@pCreatedBy", saveUserExpenseInfoViewModel.CreatedBy, DbType.Guid);
                dp_params.Add("@pAmount", saveUserExpenseInfoViewModel.Amount, DbType.Decimal);
                dp_params.Add("@pDate", saveUserExpenseInfoViewModel.Date, DbType.Date);
                dp_params.Add("@pTaskId", saveUserExpenseInfoViewModel.TaskId, DbType.Int32);
                dp_params.Add("@pUserId", saveUserExpenseInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pType", saveUserExpenseInfoViewModel.Type, DbType.Int32);
                dp_params.Add("@pId",saveUserExpenseInfoViewModel.Id, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                
                bool isNew = saveUserExpenseInfoViewModel.Id == null || saveUserExpenseInfoViewModel.Id == Guid.Empty;
                string previousValue = isNew ? "None" : "Existing expense";
                
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InsertUpdateUserExpense]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                var expenseId = dp_params.Get<Guid>("@pOutId");
                
                // Log to task log
                if (saveUserExpenseInfoViewModel.TaskId > 0)
                {
                    string actionType = isNew ? "Expense Added" : "Expense Updated";
                    string newValue = $"Type: {saveUserExpenseInfoViewModel.Type}, Amount: ${saveUserExpenseInfoViewModel.Amount}, Date: {saveUserExpenseInfoViewModel.Date:yyyy-MM-dd}";
                    await _taskLog.InsertTaskLogAsync(
                        saveUserExpenseInfoViewModel.TaskId,
                        actionType,
                        previousValue,
                        newValue,
                        "Expense",
                        saveUserExpenseInfoViewModel.Notes,
                        saveUserExpenseInfoViewModel.CreatedBy
                    );
                }

                return saveUserExpenseInfoViewModel.Id = expenseId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Guid? DeleteUserExpense(Guid id)
        {
            try
            {
                // Get expense info before deleting to log it
                var expenseInfo = GetUserExpenseInfoAsync(id).Result;
                
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", id, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = _dapperRepository.Update<Guid>("[dbo].[DeleteExpenseInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                
                // Log to task log if expense info was found
                if (expenseInfo != null && expenseInfo.TaskId > 0)
                {
                    string previousValue = $"Type: {expenseInfo.Type}, Amount: ${expenseInfo.Amount}, Date: {expenseInfo.Date:yyyy-MM-dd}";
                    _taskLog.InsertTaskLogAsync(
                        expenseInfo.TaskId,
                        "Expense Removed",
                        previousValue,
                        "None",
                        "Expense",
                        expenseInfo.Notes,
                        expenseInfo.UserId // Using UserId as CreatedBy since model doesn't have CreatedBy
                    ).Wait();
                }
                
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserExpenseInfo> GetUserExpenseInfoAsync(Guid UserExpenseID)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", UserExpenseID, DbType.Guid);
                //dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.GetList<UserExpenseInfo>("[dbo].[GetUserExpenseInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }


            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserExpenseSearchResponse> GetUserExpensesAsync(UserExpenseSearchRequest request)
        {
            try
            {
                UserExpenseSearchResponse response = new UserExpenseSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pTypeId", request.TypeId, DbType.Int32);
                dp_params.Add("@pTaskId", request.TaskId, DbType.Int32);
                dp_params.Add("@pDate", request.Date, DbType.Date);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
                var result = await Task.FromResult(_dapperRepository.GetAll<SearchUserExpenseViewModel>("[dbo].[uspGetUserExpense]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                response.Response = result.Item1;
                response.TotalRecords = result.Item2;

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
