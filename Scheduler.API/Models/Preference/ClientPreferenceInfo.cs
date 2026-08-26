namespace Scheduler.API.Models.Preference
{
    public class ClientPreferenceInfo
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string PreferenceType { get; set; }
        public string? PreferenceValue { get; set; }
        public int? PreferenceItemId { get; set; }
        public string? PreferenceItemName { get; set; }
        public string? PreferenceItemDescription { get; set; }
        public bool IsRequired { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

