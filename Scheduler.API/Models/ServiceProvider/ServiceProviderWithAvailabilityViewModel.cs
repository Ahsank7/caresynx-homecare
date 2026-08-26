namespace Scheduler.API.Models.ServiceProvider
{
    public class ServiceProviderWithAvailabilityViewModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserNo { get; set; }
        public string ProfileImagePath { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string FinalAvailabilityStatus { get; set; }
        public string RawAvailabilityStatus { get; set; }
        public string LeaveStatus { get; set; }
        public string TaskStatus { get; set; }
        public string AvailableStartTime { get; set; }
        public string AvailableEndTime { get; set; }
        public string AvailableDay { get; set; }
        public DateTime? LeaveStartDate { get; set; }
        public DateTime? LeaveEndDate { get; set; }
        public int TaskCount { get; set; }
    }
}
