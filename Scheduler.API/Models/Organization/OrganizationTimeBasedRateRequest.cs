using System.ComponentModel.DataAnnotations;

namespace Scheduler.API.Models.Organization
{
    public class OrganizationTimeBasedRateRequest
    {
        public int? Id { get; set; }
        
        [Required]
        public Guid OrganizationId { get; set; }
        
        public int? ServiceTypeId { get; set; }
        
        public int? ServiceId { get; set; }
        
        [Required]
        [Range(0, 6, ErrorMessage = "DayOfWeek must be between 0 and 6")]
        public int DayOfWeek { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "ClientRate must be non-negative")]
        public decimal ClientRate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "WageRate must be non-negative")]
        public decimal WageRate { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
