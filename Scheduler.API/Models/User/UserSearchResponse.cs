namespace Scheduler.API.Models.User
{
    public class UserSearchResponse
    {
        public List<SearchUserViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
