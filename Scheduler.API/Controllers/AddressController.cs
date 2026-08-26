using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Address;
using Scheduler.API.Services.Address;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [ApiController]
    public class AddressController : BaseController
    {
        private readonly IAddress _Address;
        
        public AddressController(IAddress Address, ILogger<AddressController> logger) 
            : base(logger)
        {
            _Address = Address;
        }

        [HttpPost("SaveUpdateAddress")]
        public async Task<IActionResult> SaveUpdateAddress(SaveAddressInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Address data is required");

            return await ExecuteAsync(
                () => _Address.CreateUpdateAddressAsync(model),
                "Address created/Updated successfully!"
            );
        }

        [HttpGet("GetAddressDetails")]
        public async Task<IActionResult> GetAddressDetails(Guid AddressID)
        {
            if (AddressID == Guid.Empty)
                return ValidationError("Valid Address ID is required");

            return await ExecuteAsync(
                () => _Address.GetAddressInfoAsync(AddressID),
                "Address details retrieved successfully!"
            );
        }

        [HttpPost("GetAddressList")]
        public async Task<IActionResult> GetAddressList(AddressSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            return await ExecuteAsync(
                () => _Address.GetAddresssAsync(model),
                "Address list retrieved successfully!"
            );
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAddress(Guid AddressID)
        {
            if (AddressID == Guid.Empty)
                return ValidationError("Valid Address ID is required");

            return await ExecuteAsync(
                () => _Address.DeleteAddressAsync(AddressID),
                "Address deleted successfully!"
            );
        }
    }
}
