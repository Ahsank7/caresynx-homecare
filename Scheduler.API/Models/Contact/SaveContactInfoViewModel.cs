namespace Scheduler.API.Models.Contact
{
    public class SaveContactInfoViewModel
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? LastName { get; set; }
        public string? Alias { get; set; }
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? IdentityNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Notes { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public int? CountyId     { get; set; }
        public int? StateId      { get; set; }
        public int? CountryId    { get; set; }
        public float Latitude     { get; set; }
        public float Longitude { get; set; }
        public int? TitleId { get; set; }
        public int? GenderId { get; set; }
        public int? ContactTypeId { get; set; }
        public Guid FranchiseId { get; set; }
        public bool? IsBillingContact { get; set; }
    }
}
