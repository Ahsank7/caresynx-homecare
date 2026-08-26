using Scheduler.API.Models.Expense;

namespace Scheduler.API.Services.Expense
{
    public interface IExpense
    {
        Task<UserExpenseSearchResponse> GetUserExpensesAsync(UserExpenseSearchRequest request);
        Task<UserExpenseInfo> GetUserExpenseInfoAsync(Guid UserExpenseID);
        Task<Guid?> CreateUpdateUserExpenseAsync(SaveUserExpenseInfoViewModel saveUserExpenseInfoViewModel);
        Guid? DeleteUserExpense(Guid id);
    }
}
