using Scheduler.API.Models.Package;
using Scheduler.API.Services.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IronPdf;

namespace Scheduler.API.Services.Package
{
    public class InvoicePdfService : IInvoicePdfService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<InvoicePdfService> _logger;
        private readonly string _connectionString;
        private readonly string _containerName;

        public InvoicePdfService(IConfiguration configuration, ILogger<InvoicePdfService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration["Storage:Azure:ConnectionString"];
            _containerName = "invoices"; // Separate container for invoices
        }

        public async Task<string> GenerateAndUploadInvoicePdfAsync(PackageInvoiceViewModel invoice, string organizationName)
        {
            try
            {
                // Generate PDF bytes
                var pdfBytes = await GenerateInvoicePdfBytesAsync(invoice, organizationName);

                // Generate filename
                var fileName = $"invoice-{invoice.InvoiceNumber}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
                var blobName = $"{invoice.OrganizationId}/{fileName}";

                // Upload to Azure Blob Storage
                var blobServiceClient = new BlobServiceClient(_connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                // Create container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                var blobClient = containerClient.GetBlobClient(blobName);

                // Upload PDF
                using (var stream = new MemoryStream(pdfBytes))
                {
                    var uploadOptions = new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = "application/pdf"
                        }
                    };
                    
                    await blobClient.UploadAsync(stream, uploadOptions);
                }

                var url = blobClient.Uri.ToString();
                _logger.LogInformation($"Invoice PDF uploaded successfully: {url}");

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating/uploading invoice PDF for invoice {invoice.InvoiceNumber}");
                throw;
            }
        }

        public async Task<byte[]> GenerateInvoicePdfBytesAsync(PackageInvoiceViewModel invoice, string organizationName)
        {
            try
            {
                // Generate HTML content
                var html = GenerateInvoiceHtml(invoice, organizationName);
                
                // Convert HTML to PDF using IronPdf
                var renderer = new ChromePdfRenderer();
                
                // Configure PDF settings
                renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A4;
                renderer.RenderingOptions.MarginTop = 10;
                renderer.RenderingOptions.MarginBottom = 10;
                renderer.RenderingOptions.MarginLeft = 10;
                renderer.RenderingOptions.MarginRight = 10;
                renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;
                renderer.RenderingOptions.PrintHtmlBackgrounds = true;
                
                // Render HTML to PDF
                var pdf = await Task.Run(() => renderer.RenderHtmlAsPdf(html));
                
                // Return PDF as byte array
                return pdf.BinaryData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating PDF bytes for invoice {invoice.InvoiceNumber}");
                throw;
            }
        }

        private string GenerateInvoiceHtml(PackageInvoiceViewModel invoice, string organizationName)
        {
            var invoiceDate = invoice.CreatedDate.ToString("MMMM dd, yyyy");
            var billingPeriod = $"{invoice.BillingPeriodStart:MMMM dd, yyyy} - {invoice.BillingPeriodEnd:MMMM dd, yyyy}";
            var statusClass = invoice.PaymentStatus.ToLower();
            var initialChargeRow = invoice.IsInitialCharge 
                ? $@"<tr>
                        <td>Initial One-Time Cost:</td>
                        <td style=""text-align: right;"">${invoice.InitialOneTimeCost:F2}</td>
                    </tr>" 
                : string.Empty;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4a90e2; color: white; padding: 30px; text-align: center; }}
        .content {{ padding: 30px; }}
        .invoice-details {{ margin: 20px 0; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        th, td {{ padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f2f2f2; font-weight: bold; }}
        .total-box {{ background-color: #4a90e2; color: white; padding: 20px; text-align: center; margin: 20px 0; font-size: 24px; }}
        .status-badge {{ display: inline-block; padding: 8px 15px; border-radius: 4px; font-weight: bold; }}
        .status-pending {{ background-color: #ffc107; color: #000; }}
        .status-paid {{ background-color: #28a745; color: white; }}
        .status-failed {{ background-color: #dc3545; color: white; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>INVOICE</h1>
            <p style=""font-size: 20px;"">#{invoice.InvoiceNumber}</p>
        </div>
        <div class=""content"">
            <h2>{organizationName}</h2>
            
            <div class=""invoice-details"">
                <table>
                    <tr>
                        <th>Invoice Date</th>
                        <td>{invoiceDate}</td>
                    </tr>
                    <tr>
                        <th>Billing Period</th>
                        <td>{billingPeriod}</td>
                    </tr>
                    <tr>
                        <th>Payment Status</th>
                        <td><span class=""status-badge status-{statusClass}"">{invoice.PaymentStatus}</span></td>
                    </tr>
                </table>
            </div>

            <h3>Package Charges</h3>
            <table>
                <tr>
                    <td>Per User Charge ({invoice.ClientCount} active users):</td>
                    <td style=""text-align: right;"">${invoice.PerClientCharge * invoice.ClientCount:F2}</td>
                </tr>
                {initialChargeRow}
                <tr>
                    <td>Infrastructure Cost:</td>
                    <td style=""text-align: right;"">${invoice.InfrastructureCost:F2}</td>
                </tr>
                <tr>
                    <td>Support Charges:</td>
                    <td style=""text-align: right;"">${invoice.SupportCharges:F2}</td>
                </tr>
                <tr>
                    <td>New Feature/Report Charges:</td>
                    <td style=""text-align: right;"">${invoice.NewFeatureReportCharges:F2}</td>
                </tr>
                <tr style=""font-weight: bold;"">
                    <td>Subtotal:</td>
                    <td style=""text-align: right;"">${invoice.SubTotal:F2}</td>
                </tr>
                <tr>
                    <td>Tax:</td>
                    <td style=""text-align: right;"">${invoice.TaxAmount:F2}</td>
                </tr>
            </table>

            <div class=""total-box"">
                <strong>TOTAL AMOUNT: ${invoice.TotalAmount:F2}</strong>
            </div>

            <p style=""text-align: center; color: #666; margin-top: 40px;"">
                Thank you for your business!<br>
                eXtremeScheduler
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
