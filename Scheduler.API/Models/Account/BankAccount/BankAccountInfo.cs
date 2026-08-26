namespace Scheduler.API.Models.Account.BankAccount
{
    public class BankAccountInfo
    {
        public Guid? BankAccountId { get; set; }
        public Guid UserId { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public int? bankId { get; set; }
        public string? BranchCode { get; set; }
        public string? IBAN { get; set; }
        public string? ConnectedAccountId { get; set; }
    }
}
