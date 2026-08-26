using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Franchise;
using Scheduler.API.Models.Organization;
using Scheduler.API.Services.Franchise;
using Scheduler.API.Services.Organization;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FranchiseController : BaseController
    {
        private readonly IFranchise _Franchise;

        public FranchiseController(IFranchise Franchise, ILogger<FranchiseController> logger) 
            : base(logger)
        {
            _Franchise = Franchise;
        }

        // [HttpPost("SaveUpdate")]
        [HttpPost]
        [Route("SaveUpdateFranchise")]
        public async Task<IActionResult> SaveUpdateFranchise(AddOrUpdateFranchiseViewModel model)
        {
            if (model == null)
                return ValidationError("Franchise data is required");

            return await ExecuteAsync(
                () => _Franchise.CreateOrUpdateFranchiseAsync(model),
                "Franchise created/Updated successfully!"
            );
        }

        [HttpPost]
        [Route("CreateFranchiseAdminUser")]
        public async Task<IActionResult> CreateFranchiseAdminUser(CreateFranchiseAdminUserViewModel model)
        {
            if (model == null)
                return ValidationError("Franchise admin user data is required");

            if (model.FranchiseId == Guid.Empty)
                return ValidationError("Valid Franchise ID is required");

            if (string.IsNullOrEmpty(model.FranchiseName))
                return ValidationError("Franchise name is required");

            if (string.IsNullOrEmpty(model.OrganizationName))
                return ValidationError("Organization name is required");

            return await ExecuteAsync(
                () => _Franchise.CreateFranchiseAdminUserAsync(model),
                "Franchise admin user created successfully!"
            );
        }

        //[HttpGet("GetFranchisesByOrganization")]
        [HttpGet]
        [Route("{organizationid}")]
        public async Task<IActionResult> GetFranchisesByOrganization(Guid OrganizationId)
        {
            if (OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _Franchise.GetFranchisesByOrganizationIdAsync(OrganizationId),
                "Franchise list retrieved successfully!"
            );
        }

        //[HttpGet("GetFranchisesByOrganizationAndUser")]
        [HttpGet]
        [Route("{organizationid}/{userid}")]
        public async Task<IActionResult> GetFranchisesByOrganizationAndUser(Guid OrganizationId, Guid UserId)
        {
            if (OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _Franchise.GetFranchisesByOrganizationIdAsync(OrganizationId, UserId),
                "Franchise list retrieved successfully!"
            );
        }

        [HttpDelete]
        [Route("DeleteFranchise")]
        public IActionResult DeleteClient(Guid Id)
        {
            var result = _Franchise.DeleteFranchise(Id);
            return Ok(new Response<Guid> { Status = StatusCodes.Status200OK, Message = "Franchise deleted successfully!", Data = result });
        }

        [HttpGet]
        [Route("Dashboard/{franchiseId}")]
        public async Task<IActionResult> GetFranchiseDashboard(Guid franchiseId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            if (franchiseId == Guid.Empty)
                return ValidationError("Valid Franchise ID is required");

            // Set default dates to current month if not provided
            if (!startDate.HasValue)
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!endDate.HasValue)
                endDate = startDate.Value.AddMonths(1).AddDays(-1);

            return await ExecuteAsync(
                () => _Franchise.GetFranchiseDashboardDataAsync(franchiseId, startDate.Value, endDate.Value),
                "Dashboard data retrieved successfully!"
            );
        }

        // User Franchise Assignment endpoints
        [HttpGet]
        [Route("UserAssignments/{userId}/{organizationId}")]
        public async Task<IActionResult> GetUserFranchiseAssignments(Guid userId, Guid organizationId)
        {
            if (userId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _Franchise.GetUserFranchiseAssignmentsAsync(userId, organizationId),
                "User franchise assignments retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("AssignUserToFranchise")]
        public async Task<IActionResult> AssignUserToFranchise(AssignUserFranchiseRequest request)
        {
            if (request == null)
                return ValidationError("Assignment data is required");

            if (request.UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            if (request.FranchiseId == Guid.Empty)
                return ValidationError("Valid Franchise ID is required");

            return await ExecuteAsync(
                () => _Franchise.AssignUserToFranchiseAsync(request),
                "User assigned to franchise successfully!"
            );
        }

        [HttpDelete]
        [Route("RemoveUserFromFranchise/{userId}/{franchiseId}")]
        public async Task<IActionResult> RemoveUserFromFranchise(Guid userId, Guid franchiseId)
        {
            if (userId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            if (franchiseId == Guid.Empty)
                return ValidationError("Valid Franchise ID is required");

            return await ExecuteAsync(
                () => _Franchise.RemoveUserFromFranchiseAsync(userId, franchiseId),
                "User removed from franchise successfully!"
            );
        }
    }
}
