namespace Scheduler.API.Models.RolePermission
{
    public class MenuPermission
    {
        public string MenuId { get; set; } = string.Empty;
        public string MenuName { get; set; } = string.Empty;
        public string? ParentMenuId { get; set; }
        public string? MenuPath { get; set; }
        public string? MenuIcon { get; set; }
        public int MenuOrder { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

    public class SaveRolePermissionsRequest
    {
        public int RoleId { get; set; }
        public Guid OrganizationId { get; set; }
        public List<MenuPermission> Permissions { get; set; } = new List<MenuPermission>();
    }

    public class UserMenuPermissionsResponse
    {
        public List<MenuPermission> Permissions { get; set; } = new List<MenuPermission>();
    }
} 