using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Billing;
using Scheduler.API.Services.Billing;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillingController : BaseController
    {
        IBilling _billing;
        public BillingController(IBilling billing, ILogger<BillingController> logger) : base(logger)
        {
            _billing = billing;
        }

        [HttpPost]
        [Route("Info/List")]
        public async Task<IActionResult> GetStaffList(BillingInfoRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _billing.GetBillingInvoiceInfoAsync(model),
                "Billing info list retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Detail/List")]
        public async Task<IActionResult> GetBillingInvoiceDetailA(BillingDetailRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _billing.GetBillingInvoiceDetailAsync(model),
                "Billing invoice detail retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Generate")]
        public async Task<IActionResult> GenerateBillingInvoices(GenerateBillingRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _billing.GenerateBillingInvoicesAsync(model),
                "Billing invoices generated successfully!"
            );
        }

        [HttpPost]
        [Route("Preview")]
        public async Task<IActionResult> PreviewBillingInvoices(BillingPreviewRequest model)
        {
            if (model == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _billing.PreviewBillingInvoicesAsync(model),
                "Billing invoices preview retrieved successfully!"
            );
        }
    }
}
