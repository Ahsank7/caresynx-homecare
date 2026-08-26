namespace Scheduler.API.Models.User
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public Guid FranchiseId { get; set; }
        public Guid OrganizationId { get; set; }
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
        public int? EthnicityId { get; set; }
        public int Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Notes { get; set; }
        public int? TitleId { get; set; }
        public int? GenderId { get; set; }
        public int? NationalityID { get; set; }
        public int? MaritalStatusId { get; set; }
        public int UserType { get; set; }
        public int StatusId { get; set; }
        public string? Status { get; set; }
        public string? ProfileImagePath { get; set; }
        public int RoleId { get; set; }
        public Guid? AddressId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public int? CountyId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
