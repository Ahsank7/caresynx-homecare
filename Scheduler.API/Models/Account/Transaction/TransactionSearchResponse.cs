namespace Scheduler.API.Models.Account.Transaction
{
    public class TransactionSearchResponse
    {
        public List<TransactionInfo> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
