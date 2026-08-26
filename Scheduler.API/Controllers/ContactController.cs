using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Contact;
using Scheduler.API.Services.Contact;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [ApiController]
    public class ContactController : BaseController
    {
        private readonly IContact _contact;
        
        public ContactController(IContact contact, ILogger<ContactController> logger) 
            : base(logger)
        {
            _contact = contact;
        }

        [HttpPost]
        [Route("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateContact(SaveContactInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Contact data is required");

            return await ExecuteAsync(
                () => _contact.CreateUpdateContactAsync(model),
                "Contact created/Updated successfully!"
            );
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> GetContactDetails(Guid contactID)
        {
            if (contactID == Guid.Empty)
                return ValidationError("Valid Contact ID is required");

            return await ExecuteAsync(
                () => _contact.GetContactInfoAsync(contactID),
                "Contact details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> GetContactList(ContactSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            return await ExecuteAsync(
                () => _contact.GetContactsAsync(model),
                "Contact list retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid Contact ID is required");

            return await ExecuteAsync(
                () => _contact.DeleteContact(Id),
                "Contact deleted successfully!"
            );
        }
    }
}
