using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.ToConfirm;
using Scheduler.API.Services.ToConfirm;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToConfirmController : BaseController
    {
        IToConfirm _toConfirm;
        public ToConfirmController(IToConfirm toConfirm, ILogger<ToConfirmController> logger) : base(logger)
        {
            _toConfirm = toConfirm;
        }

        [HttpPost]
        [Route("ServicesTask")]
        public async Task<IActionResult> GetToConfirmTasksAsync(ToConfirmRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _toConfirm.GetToConfirmTasksAsync(request),
                "To confirm tasks retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("CalculateBillingAndWageAmounts")]
        public async Task<IActionResult> CalculateBillingAndWageAmounts(string servicesTaskIds,Guid organizationId)
        {
            if (string.IsNullOrEmpty(servicesTaskIds))
                return ValidationError("Services task IDs are required");

            if (organizationId == Guid.Empty)
                return ValidationError("Valid organization ID is required");

            return Execute(() =>
            {
                var result = _toConfirm.CalculateBillingAndWageAmounts(servicesTaskIds, organizationId);
                return result;
            }, "Task confirmed successfully!");
        }

        [HttpPost]
        [Route("ConfirmExpenses")]
        public async Task<IActionResult> ConfirmExpenses(string expenseIds)
        {
            if (string.IsNullOrEmpty(expenseIds))
                return ValidationError("Expense IDs are required");

            return Execute(() =>
            {
                var result = _toConfirm.ConfirmExpenses(expenseIds);
                return result;
            }, "Expenses confirmed successfully!");
        }
    }
}
