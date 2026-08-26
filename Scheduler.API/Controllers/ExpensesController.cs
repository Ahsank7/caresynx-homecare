using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Expense;
using Scheduler.API.Services.Expense;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : BaseController
    {
        IExpense _Expense;
        public ExpensesController(IExpense Expense, ILogger<ExpensesController> logger) : base(logger)
        {
            _Expense = Expense;
        }

        [HttpPost]
        [Route("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateUserExpense(SaveUserExpenseInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Expense data is required");

            return await ExecuteAsync(
                () => _Expense.CreateUpdateUserExpenseAsync(model),
                "User Expense created/Updated successfully!"
            );
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> GetUserExpenseDetails(Guid userExpenseId)
        {
            if (userExpenseId == Guid.Empty)
                return ValidationError("Valid expense ID is required");

            return await ExecuteAsync(
                () => _Expense.GetUserExpenseInfoAsync(userExpenseId),
                "User Expense details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> GetUserExpenseList(UserExpenseSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search request data is required");

            return await ExecuteAsync(
                () => _Expense.GetUserExpensesAsync(model),
                "User Expense list retrieved successfully!"
            );
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeleteUserExpense(Guid userExpenseId)
        {
            if (userExpenseId == Guid.Empty)
                return ValidationError("Valid expense ID is required");

            return Execute(() =>
            {
                var result = _Expense.DeleteUserExpense(userExpenseId);
                return result.Value;
            }, "User Expense deleted successfully!");
        }
    }
}
