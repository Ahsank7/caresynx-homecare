namespace Scheduler.API.Models.User
{
    public class SaveUserInfoViewModel
    {
        public Guid? Id { get; set; }
        public int UserType { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? LastName { get; set; }
        public string? Alias { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? PassportNo { get; set; }
        public string? IdentityNo { get; set; }
        public int? EthnicityId { get; set; }
        public string? PasswordHash { get; set; }
        public int Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Notes { get; set; }
        public string? UserNo { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public Guid? AddressId { get; set; }
        public int? CountyId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int? TitleId { get; set; }
        public int? GenderId { get; set; }
        public int? NationalityId { get; set; }
        public Guid FranchiseId { get; set; }
        public int userStatus { get; set; }
    }
}
