using System.ComponentModel.DataAnnotations;

namespace Scheduler.API.Models.Complaint
{
    public class CreateComplaintRequest
    {
        [Required]
        public Guid ComplainantId { get; set; }

        [Required]
        public int ComplainantType { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User Number is required")]
        public string ComplainedAgainstUserNo { get; set; }

        public Guid? FranchiseId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Description { get; set; }

        public int? Category { get; set; }

        public int? Severity { get; set; }

        public int? Status { get; set; }
    }
}

