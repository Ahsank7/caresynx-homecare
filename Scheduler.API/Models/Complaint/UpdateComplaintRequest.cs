using System.ComponentModel.DataAnnotations;

namespace Scheduler.API.Models.Complaint
{
    public class UpdateComplaintRequest
    {
        [Required]
        public Guid ComplaintId { get; set; }

        [StringLength(200, MinimumLength = 5)]
        public string? Title { get; set; }

        [StringLength(2000, MinimumLength = 10)]
        public string? Description { get; set; }

        public int? Category { get; set; }

        public int? Severity { get; set; }

        public int? Status { get; set; }

        [StringLength(2000)]
        public string? Resolution { get; set; }

        public Guid? ResolvedBy { get; set; }
    }
}

