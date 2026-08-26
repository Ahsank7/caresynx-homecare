using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Client;
using Scheduler.API.Models.Organization;
using Scheduler.API.Services.Client;
using Scheduler.API.Services.Organization;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : BaseController
    {
        private readonly IOrganization _Organization;

        public OrganizationController(IOrganization Organization, ILogger<OrganizationController> logger) 
            : base(logger)
        {
            _Organization = Organization;
        }

        [HttpPost]
        [Route("SaveUpdateOrganization")]
        public async Task<IActionResult> SaveUpdateOrganization(AddUpdateOrganizationViewModel model)
        {
            if (model == null)
                return ValidationError("Organization data is required");

            return await ExecuteAsync(
                () => _Organization.CreateUpdateOrganizationAsync(model),
                "Organization created/Updated successfully!"
            );
        }

        [HttpGet]
        [Route("List/{userid}")]
        public async Task<IActionResult> GetOrganizationList(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _Organization.GetOrganizationsByUserIdAsync(UserId),
                "Organization list retrieved successfully!"
            );
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAllOrganizations()
        {
            return await ExecuteAsync(
                () => _Organization.GetAllOrganizationsAsync(),
                "All organizations retrieved successfully!"
            );
        }

        [HttpGet]
        [Route("Info/{organizationid}")]
        public async Task<IActionResult> GetOrganizationDetails(Guid OrganizationID)
        {
            if (OrganizationID == Guid.Empty)
                return ValidationError("Valid Organization ID is required");

            return await ExecuteAsync(
                () => _Organization.GetOrganisationInfoByIdAsync(OrganizationID),
                "Organization details retrieved successfully!"
            );
        }

        [HttpDelete]
        [Route("DeleteOrganization")]
        public IActionResult DeleteClient(Guid Id)
        {
            var result = _Organization.DeleteOrganization(Id);
            return Ok(new Response<Guid> { Status = StatusCodes.Status200OK, Message = "Organization deleted successfully!", Data = result });
        }
    }
}
