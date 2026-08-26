namespace Scheduler.API.Models.Account.Transaction
{
    public class TransactionSearchRequest
    {
        public Guid? UserId { get; set; }
        public Guid? FranchiseId { get; set; }
        public int? TypeId { get; set; }
        public DateTime? Date { get; set; }
        public string? UserNo { get; set; }
        public string? ReferenceId { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
