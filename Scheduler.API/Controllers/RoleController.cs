using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Role;
using Scheduler.API.Services.Role;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRole _roleService;

        public RoleController(IRole roleService, ILogger<RoleController> logger)
            : base(logger)
        {
            _roleService = roleService;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRoles(Guid? organizationId = null)
        {
            return await ExecuteAsync(
                () => _roleService.GetAvailableRolesAsync(organizationId),
                "Available roles retrieved successfully!"
            );
        }

        [HttpGet("available-for-assignment")]
        public async Task<IActionResult> GetAvailableRolesForAssignment(Guid? organizationId = null)
        {
            // Get current user ID from JWT claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                return ValidationError("Unable to determine current user");
            }

            return await ExecuteAsync(
                () => _roleService.GetAvailableRolesForUserAsync(organizationId, currentUserId),
                "Available roles for assignment retrieved successfully!"
            );
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            if (roleId <= 0)
                return ValidationError("Valid Role ID is required");

            return await ExecuteAsync(
                () => _roleService.GetRoleByIdAsync(roleId),
                "Role retrieved successfully!"
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleInfo role)
        {
            if (role == null)
                return ValidationError("Role data is required");

            if (string.IsNullOrWhiteSpace(role.Name))
                return ValidationError("Role name is required");

            return await ExecuteAsync(
                () => _roleService.CreateRoleAsync(role),
                "Role created successfully!"
            );
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(int roleId, RoleInfo role)
        {
            if (role == null)
                return ValidationError("Role data is required");

            if (roleId <= 0)
                return ValidationError("Valid Role ID is required");

            if (string.IsNullOrWhiteSpace(role.Name))
                return ValidationError("Role name is required");

            role.Id = roleId;

            return await ExecuteAsync(
                () => _roleService.UpdateRoleAsync(role),
                "Role updated successfully!"
            );
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            if (roleId <= 0)
                return ValidationError("Valid Role ID is required");

            return await ExecuteAsync(
                () => _roleService.DeleteRoleAsync(roleId),
                "Role deleted successfully!"
            );
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request)
        {
            if (request == null)
                return ValidationError("Assignment data is required");

            if (request.UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            if (request.RoleId <= 0)
                return ValidationError("Valid Role ID is required");

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var assignerUserId))
                return ValidationError("Unable to determine current user");

            // Always use the authenticated user as assigner for hierarchy checks and CreatedBy audit (ignore client CreatedBy)
            return await ExecuteAsync(
                () => _roleService.AssignRoleToUserAsync(request.UserId, request.RoleId, assignerUserId),
                "Role assigned to user successfully!"
            );
        }
    }

    public class AssignRoleRequest
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
