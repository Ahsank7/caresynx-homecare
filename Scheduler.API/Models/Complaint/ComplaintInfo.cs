namespace Scheduler.API.Models.Complaint
{
    public class ComplaintInfo
    {
        public Guid Id { get; set; }
        public Guid ComplainantId { get; set; }
        public int ComplainantType { get; set; }
        public Guid ComplainedAgainstId { get; set; }
        public int ComplainedAgainstType { get; set; }
        public Guid? FranchiseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Category { get; set; }
        public int? Severity { get; set; }
        public int Status { get; set; }
        public string? Resolution { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public Guid? ResolvedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; }

        // Related entity information
        public string? ComplainantName { get; set; }
        public string? ComplainantEmail { get; set; }
        public int? ComplainantUserType { get; set; }
        public string? ComplainedAgainstName { get; set; }
        public string? ComplainedAgainstEmail { get; set; }
        public int? ComplainedAgainstUserType { get; set; }
        public string? ResolvedByName { get; set; }

        // Lookup names
        public string? CategoryName { get; set; }
        public string? SeverityName { get; set; }
        public string? StatusName { get; set; }
    }
}

