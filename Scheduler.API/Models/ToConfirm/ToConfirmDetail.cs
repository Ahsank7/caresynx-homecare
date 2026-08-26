namespace Scheduler.API.Models.ToConfirm
{
    public class ToConfirmDetail
    {
        public int TaskId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Date { get; set; }
        public Guid ClientId { get; set; }
        public string? ClientUserNo { get; set; }
        public string? ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientMobile { get; set; }
        public string? ClientAddress { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string? ServiceProviderName { get; set; }
        public string? ServiceProviderUserNo { get; set; }
        public string? ServiceProviderEmail { get; set; }
        public string? ServiceProviderPhone { get; set; }
        public string? ServiceProviderMobile { get; set; }
        public string? ServiceProviderAddress { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsBillingConfirmed { get; set; }
        public bool IsPayrollConfirmed { get; set; }
        public string? ServiceType { get; set; }
        public string? ServiceName { get; set; }
        
        // New fields for expenses
        public string? RecordType { get; set; } // "Task" or "Expense"
        public Guid? ExpenseId { get; set; }
        public string? ExpenseType { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? ExpenseNotes { get; set; }
        public bool? ExpenseIsConfirmed { get; set; }
        
        // Attendance fields
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
