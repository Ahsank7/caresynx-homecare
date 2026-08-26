using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Helper;
using Scheduler.API.Models.Document;
using Scheduler.API.Services.Document;
using Scheduler.API.Services.FileStorage;
using Scheduler.API.Common;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Linq;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : BaseController
    {
        private readonly IDocument _document;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUrlService _urlService;

        public DocumentsController(IDocument document, IFileStorageService fileStorageService, IUrlService urlService, ILogger<DocumentsController> logger)
            : base(logger)
        {
            _document = document;
            _fileStorageService = fileStorageService;
            _urlService = urlService;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadModel model)
        {
            if (model.DocumentData == null || model.DocumentData.Length == 0)
                return ValidationError("Invalid file.");

            var directoryPath = Path.Combine("UserDocument", model.UserId.ToString());
            var documentPath = await _fileStorageService.SaveFileAsync(model.DocumentData, directoryPath);

            var requestModel = new DocumentUploadModel()
            {
                AccessRoles = model.AccessRoles,
                Name = model.Name,
                Description = model.Description,
                DocumentPath = documentPath,
                UserId = model.UserId,
                DocumentTypeId = model.DocumentTypeId
            };

            var result = await _document.UploadDocumentAsync(requestModel);
            
            //// Return the document with the proper URL
            //if (result != null)
            //{
            //    result.DocumentPath = _urlService.BuildWebPath(result.DocumentPath);
            //}

            return Ok(Response<object>.Success(result, "Document uploaded successfully!"));
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> GetDocumentDetails(int Id)
        {
            if (Id <= 0)
                return ValidationError("Valid Document ID is required");

            var result = await _document.GeDocumentInfoByIdAsync(Id);
            
            // Return the document with the proper URL
            if (result != null)
            {
                result.DocumentPath = _urlService.BuildWebPath(result.DocumentPath);
            }

            return Ok(Response<object>.Success(result, "Document details retrieved successfully!"));
        }

        [HttpGet]
        [Route("DetailsByUserId")]
        public async Task<IActionResult> GetDocumentDetailsByUserId(Guid Id, int documentTypeId)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid User ID is required");

            var result = await _document.GeDocumentInfoByUserIdAsync(Id, documentTypeId);
            
            // Return the document with the proper URL
            if (result != null)
            {
                result.DocumentPath = _urlService.BuildWebPath(result.DocumentPath);
            }

            return Ok(Response<object>.Success(result, "Document details retrieved successfully!"));
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> GetDocumentList(DocumentSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            var result = await _document.GetUserDocumentsAsync(model);
            
            // Return the documents with proper URLs
            if (result != null && result.Response!.Any())
            {
                foreach (var doc in result.Response!)
                {
                    if (doc != null)
                    {
                        doc.DocumentPath = _urlService.BuildWebPath(doc.DocumentPath);
                    }
                }
            }

            return Ok(Response<object>.Success(result, "Document list retrieved successfully!"));
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeleteDocument(int documentID)
        {
            if (documentID <= 0)
                return ValidationError("Valid Document ID is required");

            return Execute(
                () => _document.DeleteDocument(documentID),
                "Document deleted successfully!"
            );
        }

        [HttpPost("upload-user-image")]
        public async Task<IActionResult> UploadUserImage([FromForm] UploadUserImageRequest request)
        {
            if (request.UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");
            if (request.File == null || request.File.Length == 0)
                return ValidationError("Valid file is required");
            try
            {
                // Save the new image
                var directoryPath = Path.Combine("ProfileImages", request.UserId.ToString());
                var filePath = await _fileStorageService.SaveFileAsync(request.File, directoryPath);
                // Update user image path
                var userService = HttpContext.RequestServices.GetService(typeof(Scheduler.API.Services.User.IUser)) as Scheduler.API.Services.User.IUser;
                var result = await userService.UploadProfileImageAsync(request.UserId, filePath);
                if (!result)
                    return ValidationError("Failed to update user image");
                return Ok(Response<object>.Success(new { FilePath = _urlService.BuildWebPath(filePath) }, "User image uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading user image for user {UserId}", request.UserId);
                return StatusCode(500, Response<object>.InternalServerError("Failed to upload user image"));
            }
        }

   [HttpDelete("delete-user-image")]
        public async Task<IActionResult> DeleteUserImage(Guid userId)
        {
            if (userId == Guid.Empty)
                return ValidationError("Valid User ID is required");
            try
            {
                var userService = HttpContext.RequestServices.GetService(typeof(Scheduler.API.Services.User.IUser)) as Scheduler.API.Services.User.IUser;
                var user = await userService.GetUserInfoAsync(userId);
                if (user == null)
                    return NotFoundError("User not found");
                if (string.IsNullOrEmpty(user.ProfileImagePath))
                    return ValidationError("No user image to delete");
                await _fileStorageService.DeleteFileAsync(user.ProfileImagePath);
                var result = await userService.UploadProfileImageAsync(userId, "");
                if (!result)
                    return ValidationError("Failed to remove user image");
                return Ok("User image deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user image for user {UserId}", userId);
                return StatusCode(500, Response<object>.InternalServerError("Failed to delete user image"));
            }
        }

        [HttpPost("upload-organization-logo")]
        public async Task<IActionResult> UploadOrganizationLogo([FromForm] UploadOrganizationImageRequest request)
        {
            if (request.OrganizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");
            if (request.File == null || request.File.Length == 0)
                return ValidationError("Valid file is required");
            try
            {
                var directoryPath = Path.Combine("OrganizationLogos", request.OrganizationId.ToString());
                var filePath = await _fileStorageService.SaveFileAsync(request.File, directoryPath);
                var orgService = HttpContext.RequestServices.GetService(typeof(Scheduler.API.Services.Organization.IOrganization)) as Scheduler.API.Services.Organization.IOrganization;
                var result = await orgService.UpdateOrganizationLogoAsync(request.OrganizationId, filePath);
                if (!result)
                    return ValidationError("Failed to update organization logo");
                return Ok(Response<object>.Success(new { FilePath = _urlService.BuildWebPath(filePath) }, "Organization logo uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading organization logo for org {OrganizationId}", request.OrganizationId);
                return StatusCode(500, Response<object>.InternalServerError("Failed to upload organization logo"));
            }
        }

        [HttpDelete("delete-organization-logo")]
        public async Task<IActionResult> DeleteOrganizationLogo(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");
            try
            {
                var orgService = HttpContext.RequestServices.GetService(typeof(Scheduler.API.Services.Organization.IOrganization)) as Scheduler.API.Services.Organization.IOrganization;
                var org = await orgService.GetOrganisationInfoByIdAsync(organizationId);
                if (org == null)
                    return NotFoundError("Organization not found");
                if (string.IsNullOrEmpty(org.LogoPath))
                    return ValidationError("No organization logo to delete");
                await _fileStorageService.DeleteFileAsync(org.LogoPath);
                var result = await orgService.ClearOrganizationLogoAsync(organizationId);
                if (!result)
                    return ValidationError("Failed to remove organization logo");
                return Ok("Organization logo deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting organization logo for org {OrganizationId}", organizationId);
                return StatusCode(500, Response<object>.InternalServerError("Failed to delete organization logo"));
            }
        }

        [HttpGet("get-user-image")]
        public async Task<IActionResult> GetUserImage(Guid userId)
        {
            if (userId == Guid.Empty)
                return ValidationError("Valid User ID is required");
            var userService = HttpContext.RequestServices.GetService(typeof(Scheduler.API.Services.User.IUser)) as Scheduler.API.Services.User.IUser;
            var user = await userService.GetUserInfoAsync(userId);
            if (user == null)
                return NotFoundError("User not found");
            if (string.IsNullOrEmpty(user.ProfileImagePath))
                return NotFoundError("User image not found");
            return Ok(Response<string>.Success(user.ProfileImagePath, "User image path retrieved successfully"));
        }

        [HttpGet("get-organization-logo")]
        public async Task<IActionResult> GetOrganizationLogo(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return ValidationError("Valid Organization ID is required");
            var orgService = HttpContext.RequestServices.GetService(typeof(Scheduler.API.Services.Organization.IOrganization)) as Scheduler.API.Services.Organization.IOrganization;
            var org = await orgService.GetOrganisationInfoByIdAsync(organizationId);
            if (org == null)
                return NotFoundError("Organization not found");
            if (string.IsNullOrEmpty(org.LogoPath))
                return NotFoundError("Organization logo not found");
            return Ok(Response<string>.Success(_urlService.BuildWebPath(org.LogoPath), "Organization logo path retrieved successfully"));
        }

      
    }
}
