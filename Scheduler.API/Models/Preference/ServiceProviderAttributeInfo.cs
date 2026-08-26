namespace Scheduler.API.Models.Preference
{
    public class ServiceProviderAttributeInfo
    {
        public Guid Id { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string AttributeType { get; set; }
        public string? AttributeValue { get; set; }
        public int? AttributeItemId { get; set; }
        public string? AttributeItemName { get; set; }
        public string? AttributeItemDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

