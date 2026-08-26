using Scheduler.API.Models.Email;

namespace Scheduler.API.Services.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailMessage message);
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendBulkEmailAsync(List<EmailMessage> messages);
        Task<bool> SendEmailWithAttachmentAsync(EmailMessage message, List<EmailAttachment> attachments);
    }
}

