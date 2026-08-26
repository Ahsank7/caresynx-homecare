using Scheduler.API.Models.Wage;

namespace Scheduler.API.Services.Wage
{
    public interface IWage
    {
        Task<WageInfoResponse> GetWageInfoAsync(WageInfoRequest request);
        Task<WageDetailResponse> GetWageDetailAsync(WageDetailRequest request);
        Task<GenerateWageResponse> GenerateServiceProviderWageAsync(GenerateWageRequest request);
        Task<WagePreviewResponse> PreviewServiceProviderWageAsync(WagePreviewRequest request);
    }
}
