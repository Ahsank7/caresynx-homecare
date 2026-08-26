namespace Scheduler.API.Models.Account.Transaction
{
    public class UpsertTransactionInfoViewModel
    {
        public Guid? TransactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid? CardId { get; set; }
        public Guid? BankAccountId { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Remarks { get; set; }
        public string? ReferenceId { get; set; }
    }
}
