namespace Scheduler.API.Models.Expense
{
    public class UserExpenseSearchRequest
    {
        public Guid? UserId { get; set; }
        public string? Date { get; set; }
        public int? TypeId { get; set; }
        public int? TaskId { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
