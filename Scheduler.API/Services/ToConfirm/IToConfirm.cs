using Scheduler.API.Models.ToConfirm;

namespace Scheduler.API.Services.ToConfirm
{
    public interface IToConfirm
    {
        Task<ToConfirmResponse> GetToConfirmTasksAsync(ToConfirmRequest request);
        bool CalculateBillingAndWageAmounts(string servicesTaskIds, Guid organizationId);
        bool ConfirmExpenses(string expenseIds);
    }
}
