namespace Scheduler.API.Models.Expense
{
    public class SearchUserExpenseViewModel
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public int TaskId { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string? Notes { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsActive { get; set; }
    }
}
