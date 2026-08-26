using Scheduler.API.Common;
using Scheduler.API.Models.Complaint;
using Scheduler.API.Services.Complaint;
using Scheduler.API.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComplaintController : BaseController
    {
        private readonly IComplaint _complaintService;
        private readonly IUser _userService;

        public ComplaintController(IComplaint complaintService, IUser userService, ILogger<ComplaintController> logger)
            : base(logger)
        {
            _complaintService = complaintService;
            _userService = userService;
        }

        /// <summary>
        /// Get complaints with optional filters
        /// </summary>
        [HttpPost]
        [Route("Search")]
        public async Task<IActionResult> GetComplaints([FromBody] GetComplaintsRequest request)
        {
            return await ExecuteAsync(
                async () =>
                {
                    var complaints = await _complaintService.GetComplaintsAsync(request);
                    return new GetComplaintsResponse 
                    { 
                        Complaints = complaints,
                        TotalCount = complaints.Count
                    };
                },
                "Complaints retrieved successfully!"
            );
        }

        /// <summary>
        /// Get a specific complaint by ID
        /// </summary>
        [HttpGet]
        [Route("{complaintId}")]
        public async Task<IActionResult> GetComplaintById(Guid complaintId)
        {
            if (complaintId == Guid.Empty)
                return ValidationError("Valid Complaint ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var complaint = await _complaintService.GetComplaintByIdAsync(complaintId);
                    return complaint;
                },
                "Complaint retrieved successfully!"
            );
        }

        /// <summary>
        /// Get complaints for a specific user (as complainant or complained against)
        /// </summary>
        [HttpGet]
        [Route("User/{userId}")]
        public async Task<IActionResult> GetUserComplaints(Guid userId)
        {
            if (userId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var request = new GetComplaintsRequest { UserId = userId };
                    var complaints = await _complaintService.GetComplaintsAsync(request);
                    return new GetComplaintsResponse 
                    { 
                        Complaints = complaints,
                        TotalCount = complaints.Count
                    };
                },
                "User complaints retrieved successfully!"
            );
        }

        /// <summary>
        /// Create a new complaint
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequest request)
        {
            if (request == null)
                return ValidationError("Complaint data is required");

            if (request.ComplainantId == Guid.Empty)
                return ValidationError("Valid Complainant ID is required");

            if (string.IsNullOrWhiteSpace(request.ComplainedAgainstUserNo))
                return ValidationError("User Number is required");

            if (string.IsNullOrWhiteSpace(request.Title))
                return ValidationError("Complaint title is required");

            if (string.IsNullOrWhiteSpace(request.Description))
                return ValidationError("Complaint description is required");

            return await ExecuteAsync(
                async () =>
                {
                    // Look up the user by UserNo
                    var complainedAgainstUser = await _userService.GetUserInfoByUserNoAsync(request.ComplainedAgainstUserNo);
                    
                    if (complainedAgainstUser == null)
                    {
                        throw new ArgumentException($"User with User Number '{request.ComplainedAgainstUserNo}' not found");
                    }

                    // Create the complaint with the actual user ID
                    var createRequest = new CreateComplaintRequest
                    {
                        ComplainantId = request.ComplainantId,
                        ComplainantType = request.ComplainantType,
                        ComplainedAgainstUserNo = request.ComplainedAgainstUserNo,
                        FranchiseId = request.FranchiseId,
                        Title = request.Title,
                        Description = request.Description,
                        Category = request.Category,
                        Severity = request.Severity,
                        Status = request.Status
                    };

                    var userId = GetCurrentUserId();
                    var complaint = await _complaintService.CreateComplaintAsync(
                        complainedAgainstUser.UserId,
                        complainedAgainstUser.UserType,
                        createRequest,
                        userId
                    );
                    return complaint;
                },
                "Complaint created successfully!"
            );
        }

        /// <summary>
        /// Update an existing complaint
        /// </summary>
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateComplaint([FromBody] UpdateComplaintRequest request)
        {
            if (request == null)
                return ValidationError("Complaint data is required");

            if (request.ComplaintId == Guid.Empty)
                return ValidationError("Valid Complaint ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var userId = GetCurrentUserId();
                    var complaint = await _complaintService.UpdateComplaintAsync(request, userId);
                    return complaint;
                },
                "Complaint updated successfully!"
            );
        }

        /// <summary>
        /// Delete (soft delete) a complaint
        /// </summary>
        [HttpDelete]
        [Route("{complaintId}")]
        public async Task<IActionResult> DeleteComplaint(Guid complaintId)
        {
            if (complaintId == Guid.Empty)
                return ValidationError("Valid Complaint ID is required");

            return await ExecuteAsync(
                async () =>
                {
                    var userId = GetCurrentUserId();
                    var result = await _complaintService.DeleteComplaintAsync(complaintId, userId);
                    return result;
                },
                "Complaint deleted successfully!"
            );
        }

        #region Helper Methods

        /// <summary>
        /// Get current user ID from JWT claims
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("sub")?.Value
                ?? User?.FindFirst("userId")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }

        #endregion
    }
}

