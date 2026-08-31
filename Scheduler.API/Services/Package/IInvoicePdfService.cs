using Scheduler.API.Models.Package;

namespace Scheduler.API.Services.Package
{
    public interface IInvoicePdfService
    {
        /// <summary>
        /// Generate PDF for an invoice and save it using the configured storage provider.
        /// </summary>
        /// <returns>Public URL or web path of the generated PDF</returns>
        Task<string> GenerateAndUploadInvoicePdfAsync(PackageInvoiceViewModel invoice, string organizationName);
        
        /// <summary>
        /// Generate PDF bytes from invoice data
        /// </summary>
        Task<byte[]> GenerateInvoicePdfBytesAsync(PackageInvoiceViewModel invoice, string organizationName);
    }
}
