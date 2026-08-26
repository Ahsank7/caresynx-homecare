using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Models.Service;
using Scheduler.API.Services.Service;
using Microsoft.Extensions.Logging;
using Scheduler.API.Common;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServices _service;
        private readonly IServiceType _serviceType;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(IServices service, IServiceType serviceType, ILogger<ServicesController> logger)
        {
            _serviceType = serviceType;
            _service = service;
            _logger = logger;
        }

        // --- Service CRUD ---
        [HttpGet("List")]
        public async Task<IActionResult> GetServicesList(int serviceTypeId)
        {
            var result = await _service.GetServiceListAsync(serviceTypeId);
            return Ok(Response<GetServicesResponse>.Success(result, "Service list retrieved successfully!"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ServiceInfo model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(Response<GetServicesResponse >.BadRequest("Service name is required"));
            var result = await _service.CreateServiceAsync(model);
            return Ok(Response<ServiceInfo>.Success(result, "Service created successfully!"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService([FromBody] ServiceInfo model)
        {
            if (model == null || model.Id <= 0 || string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(Response<ServiceInfo>.BadRequest("Valid service ID and name are required"));
            var result = await _service.UpdateServiceAsync(model);
            return Ok(Response<ServiceInfo>.Success(result, "Service updated successfully!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (id <= 0)
                return BadRequest(Response<bool>.BadRequest("Valid service ID is required"));
            var result = await _service.DeleteServiceAsync(id);
            if (!result)
                return NotFound(Response<bool>.NotFound("Service not found or could not be deleted"));
            return Ok(Response<bool>.Success(result,"Service deleted successfully!"));
        }

        // --- ServiceType CRUD ---
        [HttpGet("List/ServiceType")]
        public async Task<IActionResult> GetServiceTypesList(Guid organizationId)
        {
            var result = await _serviceType.GetServiceTypesAsync(organizationId);
            return Ok(Response<GetServiceTypeResponse>.Success(result, "Service type list retrieved successfully!"));
        }

        [HttpPost("ServiceType")]
        public async Task<IActionResult> CreateServiceType([FromBody] ServiceTypeInfo model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(Response<ServiceTypeInfo>.BadRequest("Service type name is required"));
            var result = await _serviceType.CreateServiceTypeAsync(model);
            return Ok(Response<ServiceTypeInfo>.Success(result, "Service type created successfully!"));
        }

        [HttpPut("ServiceType")]
        public async Task<IActionResult> UpdateServiceType([FromBody] ServiceTypeInfo model)
        {
            if (model == null || model.Id <= 0 || string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(Response<ServiceTypeInfo>.BadRequest("Valid service type ID and name are required"));
            var result = await _serviceType.UpdateServiceTypeAsync(model);
            return Ok(Response<ServiceTypeInfo>.Success(result, "Service type updated successfully!"));
        }

        [HttpDelete("ServiceType/{id}")]
        public async Task<IActionResult> DeleteServiceType(int id)
        {
            if (id <= 0)
                return BadRequest(Response<bool>.BadRequest("Valid service type ID is required"));
            var result = await _serviceType.DeleteServiceTypeAsync(id);
            if (!result)
                return NotFound(Response<bool>.NotFound("Service type not found or could not be deleted"));
            return Ok(Response<bool>.Success(result,"Service type deleted successfully!"));
        }
    }
}
