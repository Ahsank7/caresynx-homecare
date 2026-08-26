namespace Scheduler.API.Models.Package
{
    public class PackageViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PerClientCharge { get; set; }
        public decimal InitialOneTimeCost { get; set; }
        public decimal InfrastructureCost { get; set; }
        public decimal SupportCharges { get; set; }
        public decimal NewFeatureReportCharges { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AssignPackageToOrganizationViewModel
    {
        public Guid OrganizationId { get; set; }
        public Guid PackageId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Notes { get; set; }
    }

    public class OrganizationPackageHistoryViewModel
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string? PackageDescription { get; set; }
        public decimal PerClientCharge { get; set; }
        public decimal InitialOneTimeCost { get; set; }
        public decimal InfrastructureCost { get; set; }
        public decimal SupportCharges { get; set; }
        public decimal NewFeatureReportCharges { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateOrganizationPackagePricingViewModel
    {
        public Guid OrganizationPackageId { get; set; }
        public decimal PerClientCharge { get; set; }
        public decimal InitialOneTimeCost { get; set; }
        public decimal InfrastructureCost { get; set; }
        public decimal SupportCharges { get; set; }
        public decimal NewFeatureReportCharges { get; set; }
    }
}

