namespace Scheduler.API.Models.Expense
{
    public class UserExpenseSearchResponse
    {
        public List<SearchUserExpenseViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
