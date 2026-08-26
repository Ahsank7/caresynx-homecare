namespace Scheduler.API.Models.Wage
{
    public class WageInfo
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string? UserNo { get; set; }
        public string? ServiceProviderName { get; set; }
        public Guid? TransactionId { get; set; }
        public string? WagePaymentsId { get; set; }
    }
}
