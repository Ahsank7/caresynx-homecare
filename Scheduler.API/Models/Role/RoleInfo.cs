using System.ComponentModel.DataAnnotations;

namespace Scheduler.API.Models.Role
{
    public class RoleInfo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public Guid? OrganizationId { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Guid? CreatedBy { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        [Required]
        public int RoleLevel { get; set; }
    }
}
