namespace Scheduler.API.Models.Address
{
    public class AddressInfo
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public int AddressTypeId { get; set; }
        public int? CountyId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
