namespace Scheduler.API.Models.Wage
{
    public class WageDetail
    {
        public int Id { get; set; }
        public int ServiceProviderWageId { get; set; }
        public int TaskId { get; set; }
        public string? ServiceType { get; set; }
        public DateTime Date { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
