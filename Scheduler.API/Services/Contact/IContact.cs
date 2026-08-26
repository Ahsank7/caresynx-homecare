using Scheduler.API.Models.Contact;

namespace Scheduler.API.Services.Contact
{
    public interface IContact
    {
        Task<Guid?> CreateUpdateContactAsync(SaveContactInfoViewModel saveContactInfoViewModel);
        Task<bool> DeleteContact(Guid Id);
        Task<ContactInfo> GetContactInfoAsync(Guid ContactID);
        Task<ContactSearchResponse> GetContactsAsync(ContactSearchRequest request);
    }
}
