
using Scheduler.API.Models.Billing;

namespace Scheduler.API.Services.Billing
{
    public interface IBilling
    {
        Task<BillingInfoResponse> GetBillingInvoiceInfoAsync(BillingInfoRequest request);
        Task<BillingDetailResponse> GetBillingInvoiceDetailAsync(BillingDetailRequest request);
        Task<GenerateBillingResponse> GenerateBillingInvoicesAsync(GenerateBillingRequest request);
        Task<BillingPreviewResponse> PreviewBillingInvoicesAsync(BillingPreviewRequest request);
    }
}
