using Scheduler.API.Models.Package;

namespace Scheduler.API.Services.Package
{
    public interface IPackage
    {
        Task<Guid?> SaveUpdatePackageAsync(PackageViewModel model);
        Task<(List<PackageViewModel>, int)> GetAllPackagesAsync(bool includeInactive = false);
        Task<PackageViewModel?> GetPackageByIdAsync(Guid packageId);
        Task<bool> AssignPackageToOrganizationAsync(AssignPackageToOrganizationViewModel model);
        Task<List<OrganizationPackageHistoryViewModel>> GetOrganizationPackageHistoryAsync(Guid organizationId);
        Task<OrganizationPackageHistoryViewModel?> GetCurrentOrganizationPackageAsync(Guid organizationId);
        Task<bool> UpdateOrganizationPackagePricingAsync(UpdateOrganizationPackagePricingViewModel model);
        
        // Organization Card Management
        Task<OrganizationCardInfoViewModel?> SaveUpdateOrganizationCardAsync(OrganizationCardInfoViewModel model);
        Task<OrganizationCardInfoViewModel?> GetOrganizationCardAsync(Guid organizationId);
        Task<bool> ValidateOrganizationCardAsync(Guid organizationId);
        
        // Package Invoice Management
        Task<List<PackageInvoiceViewModel>> GetOrganizationInvoicesAsync(Guid organizationId);
        Task<PackageInvoiceViewModel?> GetInvoiceByIdAsync(int invoiceId);
        Task<int> GenerateMonthlyInvoicesAsync(GenerateMonthlyInvoicesRequest request);
        Task<bool> ProcessInvoicePaymentAsync(ProcessPackageInvoicePaymentRequest request);
    }
}

