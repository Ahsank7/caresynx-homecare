using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Scheduler;
using Scheduler.API.Services.Scheduler;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulerController : ControllerBase
    {
        IScheduler _scheduler;
        public SchedulerController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }
        [HttpPost]
        [Route("CreateAppointment")]
        public async Task<IActionResult> ScheduleAppointment(ScheduleAppointmentRequest request)
        {
            var result = await _scheduler.CreateSceduleAppointmentAsync(request);
            return Ok(new Response<int> { Status = StatusCodes.Status200OK, Message = "Appointment Scheduled successfully!", Data = result.ScheduleId });
        }
        [HttpPost]
        [Route("GetClientTasks")]
        public async Task<IActionResult> GetClientTasks(GetClientTasksRequest request)
        {
            var result = await _scheduler.GetClientTasks(request);
            return Ok(new Response<List<GetClientTasksResponse>> { Status = StatusCodes.Status200OK, Message = "Appointment Scheduled successfully!", Data = result });
        }
        [HttpPost]
        [Route("GetServiceProviderTasks")]
        public async Task<IActionResult> GetServiceProviderTasks(GetServiceProviderTasksRequest request)
        {
            var result = await _scheduler.GetServiceProviderTasks(request);
            return Ok(new Response<List<GetServiceProviderTasksResponse>> { Status = StatusCodes.Status200OK, Message = "Appointment Scheduled successfully!", Data = result });
        }
    }
}
