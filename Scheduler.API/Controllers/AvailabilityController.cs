using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Availability;
using Scheduler.API.Services.Availability;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AvailabilityController : BaseController
    {
        IAvailability _Availability;
        public AvailabilityController(IAvailability Availability, ILogger<AvailabilityController> logger) : base(logger)
        {
            _Availability = Availability;
        }


        [HttpPost]
        [Route("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateAvailability(SaveAvailabilityInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Availability data is required");

            return await ExecuteAsync(async () =>
            {
                var result = await _Availability.CreateUpdateAvailabilityAsync(model);

                if (result == null)
                    throw new InvalidOperationException("Failed to create/update availability");

                return result;
            }, "Availability created/Updated successfully!");
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> GetAvailabilityDetails(Guid Id)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid ID is required");

            return await ExecuteAsync(
                () => _Availability.GetAvailabilityInfoAsync(Id),
                "Availability details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> GetStaffList(AvailabilitySearchRequest model)
        {
            if (model == null)
                return ValidationError("Search request data is required");

            return await ExecuteAsync(
                () => _Availability.GetAvailabilitysAsync(model),
                "Availability list retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult DeleteAvailability(Guid Id)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid ID is required");

            return Execute(() =>
            {
                var result = _Availability.DeleteAvailability(Id);
                return result.Value;
            }, "Availability deleted successfully!");
        }
    }
}
