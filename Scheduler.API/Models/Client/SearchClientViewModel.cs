namespace Scheduler.API.Models.Client
{
    public class SearchClientViewModel
    {
        public Guid UserId { get; set; }
        public string UserNo { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? LastName { get; set; }
        public string? Alias { get; set; }
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? PassportNo { get; set; }
        public string? IdentityNo { get; set; }
        public string? Ethnicity { get; set; }
        public int Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Notes { get; set; }
        public string? Title { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? UserType { get; set; }
        public string? Status { get; set; }
    }
}
