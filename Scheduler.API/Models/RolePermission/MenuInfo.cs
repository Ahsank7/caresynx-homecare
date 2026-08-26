namespace Scheduler.API.Models.RolePermission
{
    public class MenuInfo
    {
        public Guid Id { get; set; }
        public string MenuId { get; set; } = string.Empty;
        public string MenuName { get; set; } = string.Empty;
        public string? ParentMenuId { get; set; }
        public string? MenuPath { get; set; }
        public string? MenuIcon { get; set; }
        public int MenuOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateMenuStatusRequest
    {
        public Guid MenuId { get; set; }
        public bool IsActive { get; set; }
    }
}


