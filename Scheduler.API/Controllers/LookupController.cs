using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Helper;
using Scheduler.API.Models.Lookup;
using Scheduler.API.Services.Lookup;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LookupController : ControllerBase
    {
        ILookup _Lookup;
        public LookupController(ILookup Lookup)
        {
            _Lookup = Lookup;

        }

        [HttpPost]
        [Route("Item/SaveUpdate")]
        public async Task<IActionResult> SaveUpdateLookup(UpsertLookupRequest upsertLookupRequest)
        {
            var result = await _Lookup.CreateUpdateLookupAsync(upsertLookupRequest);

            return Ok(new Response<int?> { Status = StatusCodes.Status200OK, Message = " Lookup created/Updated successfully!", Data = result, IsSuccess = true });
        }

        [HttpGet]
        [Route("GetDetails")]
        public async Task<IActionResult> GetLookupDetails(int lookupId)
        {
            var result = await _Lookup.GetLookupInfoAsync(lookupId);

            return Ok(new Response<LookupDetail> { Status = StatusCodes.Status200OK, Message = " Lookup details get successfully!", Data = result, IsSuccess = true });
        }

        [HttpPost]
        [Route("GetItemsList")]
        public async Task<IActionResult> GetLookupList(LookupSearchRequest model)
        {
            var result = await _Lookup.GetLookupAsync(model);
            return Ok(new Response<LookupResponse> { Status = StatusCodes.Status200OK, Message = " Lookup details get successfully!", Data = result, IsSuccess = true });
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult DeleteLookup(int lookupId)
        {
            var result = _Lookup.DeleteLookup(lookupId);

            return Ok(new Response<int> { Status = StatusCodes.Status200OK, Message = " Lookup deleted successfully!", Data = result.Value, IsSuccess = true });
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> GetLookupsList()
        {
            var result = await _Lookup.GetLookupsList();
            return Ok(new Response<Dictionary<string,string>> { Status = StatusCodes.Status200OK, Message = " Lookup deleted successfully!", Data = result, IsSuccess = true });
        }
    }
}
