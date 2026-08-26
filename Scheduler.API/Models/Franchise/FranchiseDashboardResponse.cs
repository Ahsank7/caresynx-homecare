namespace Scheduler.API.Models.Franchise
{
    public class FranchiseDashboardResponse
    {
        public DashboardStats Stats { get; set; } = new();
        public List<ServiceTypeData> PopularServices { get; set; } = new();
        public List<ServiceTaskData> ServiceTaskStatuses { get; set; } = new();
        public List<TaskStatusData> TaskStatusDistribution { get; set; } = new();
        public List<BillingWageData> BillingTrend { get; set; } = new();
        public List<BillingWageData> WageTrend { get; set; } = new();
        public BillingWageSummary BillingSummary { get; set; } = new();
        public BillingWageSummary WageSummary { get; set; } = new();
    }

    public class DashboardStats
    {
        public int TotalClients { get; set; }
        public int TotalServiceProviders { get; set; }
        public int TotalStaff { get; set; }
        public int TotalTasks { get; set; }
        public int TotalBillingInvoices { get; set; }
        public int TotalWages { get; set; }
    }

    public class ServiceTypeData
    {
        public string ServiceType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ServiceTaskData
    {
        public string TaskStatus { get; set; } = string.Empty;  // Changed from ServiceType
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public string Color { get; set; } = string.Empty;      // Added Color property
    }

    public class TaskStatusData
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class BillingWageData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }

    public class BillingWageSummary
    {
        public int TotalCount { get; set; }
        public int PaidCount { get; set; }
        public int UnpaidCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
    }
}
