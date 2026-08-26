using Scheduler.API.Models.Package;

namespace Scheduler.API.Services.Package
{
    public interface IInvoicePdfService
    {
        /// <summary>
        /// Generate PDF for an invoice and upload to Azure
        /// </summary>
        /// <param name="invoice">Invoice data</param>
        /// <param name="organizationName">Organization name</param>
        /// <returns>Azure Blob URL of the generated PDF</returns>
        Task<string> GenerateAndUploadInvoicePdfAsync(PackageInvoiceViewModel invoice, string organizationName);
        
        /// <summary>
        /// Generate PDF bytes from invoice data
        /// </summary>
        Task<byte[]> GenerateInvoicePdfBytesAsync(PackageInvoiceViewModel invoice, string organizationName);
    }
}
