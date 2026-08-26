using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.RolePermission;
using Scheduler.API.Services.Role;
using Scheduler.API.Services.RolePermission;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : BaseController
    {
        private readonly IRolePermission _rolePermission;
        private readonly IRole _roleService;

        public RolePermissionController(IRolePermission rolePermission, IRole roleService, ILogger<RolePermissionController> logger)
            : base(logger)
        {
            _rolePermission = rolePermission;
            _roleService = roleService;
        }

        /// <summary>
        /// Ensures the current user may manage menu permissions for the target role (same rules as Role/available-for-assignment).
        /// </summary>
        private async Task<IActionResult?> ValidateCurrentUserCanManageRolePermissionsAsync(int targetRoleId, Guid? organizationId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var currentUserId))
                return ValidationError("Unable to determine current user");

            var allowed = await _roleService.GetAvailableRolesForUserAsync(organizationId, currentUserId);
            if (allowed == null || !allowed.Any(r => r.Id == targetRoleId))
                return ValidationError("You do not have permission to manage permissions for this role.");

            return null;
        }

        [HttpGet("user-permissions/{userId}")]
        public async Task<IActionResult> GetUserMenuPermissions(Guid userId, Guid? organizationId = null)
        {
            if (userId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _rolePermission.GetUserMenuPermissionsAsync(userId, organizationId),
                "User menu permissions retrieved successfully!"
            );
        }

        [HttpGet("menus/{organizationId}")]
        public async Task<IActionResult> GetAllMenus(Guid? organizationId)
        {
            return await ExecuteAsync(
                () => _rolePermission.GetAllMenusAsync(organizationId),
                "Menus retrieved successfully!"
            );
        }

        [HttpGet("menus-for-admin/{organizationId}")]
        public async Task<IActionResult> GetAllMenusForAdmin(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _rolePermission.GetAllMenusForAdminAsync(organizationId),
                "Menus retrieved successfully!"
            );
        }

        [HttpPost("update-menu-status")]
        public async Task<IActionResult> UpdateMenuStatus(UpdateMenuStatusRequest request)
        {
            if (request == null)
                return ValidationError("Menu status data is required");

            if (request.MenuId == Guid.Empty)
                return ValidationError("Valid Menu ID is required");

            return await ExecuteAsync(
                () => _rolePermission.UpdateMenuStatusAsync(request.MenuId, request.IsActive),
                "Menu status updated successfully!"
            );
        }

        [HttpGet("role-permissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(int roleId, Guid? organizationId = null)
        {
            if (roleId <= 0)
                return ValidationError("Valid Role ID is required");

            var denied = await ValidateCurrentUserCanManageRolePermissionsAsync(roleId, organizationId);
            if (denied != null)
                return denied;

            return await ExecuteAsync(
                () => _rolePermission.GetRolePermissionsAsync(roleId, organizationId),
                "Role permissions retrieved successfully!"
            );
        }

        [HttpPost("save-permissions")]
        public async Task<IActionResult> SaveRolePermissions(SaveRolePermissionsRequest request)
        {
            if (request == null)
                return ValidationError("Role permissions data is required");

            if (request.RoleId <= 0)
                return ValidationError("Valid Role ID is required");

            if (request.OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            var denied = await ValidateCurrentUserCanManageRolePermissionsAsync(request.RoleId, request.OrganizationId);
            if (denied != null)
                return denied;

            // Debug logging
            _logger.LogInformation("Saving role permissions - RoleId: {RoleId}, OrganizationId: {OrganizationId}, PermissionsCount: {Count}",
                request.RoleId, request.OrganizationId, request.Permissions?.Count ?? 0);

            return await ExecuteAsync(
                () => _rolePermission.SaveRolePermissionsAsync(request),
                "Role permissions saved successfully!"
            );
        }
    }
}