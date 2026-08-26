namespace Scheduler.API.Models.Account.Transaction
{
    public class TransactionInfo
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public string? Status { get; set; }
        public int StatusId { get; set; }
        public string? Type { get; set; }
        public int? TypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Remarks { get; set; }
        public string? ReferenceId { get; set; }
        public string? CardNumber { get; set; }
        public Guid? CardId { get; set; }
        public string? AccountNumber { get; set; }
        public Guid? BankAccountId { get; set; }
    }
}
