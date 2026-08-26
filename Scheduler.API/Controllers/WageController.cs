using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Wage;
using Scheduler.API.Services.Wage;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WageController : BaseController
    {
        IWage _wage;
        public WageController(IWage wage, ILogger<WageController> logger) : base(logger)
        {
            _wage = wage;
        }

        [HttpPost]
        [Route("Info/List")]
        public async Task<IActionResult> GetWageInfoList(WageInfoRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _wage.GetWageInfoAsync(model),
                "Wage info list retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Detail/List")]
        public async Task<IActionResult> GetWageDetail(WageDetailRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _wage.GetWageDetailAsync(model),
                "Wage detail retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> GenerateServiceProviderWage(GenerateWageRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _wage.GenerateServiceProviderWageAsync(model),
                "Service provider wage generated successfully!"
            );
        }

        [HttpPost]
        [Route("Preview")]
        public async Task<IActionResult> PreviewServiceProviderWage(WagePreviewRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _wage.PreviewServiceProviderWageAsync(model),
                "Service provider wage preview retrieved successfully!"
            );
        }
    }
}
