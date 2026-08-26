using Scheduler.API.Models.Email;
using Scheduler.API.Models.Package;

namespace Scheduler.API.Services.Package
{
    public static class EmailTemplates
    {
        public static EmailMessage CreateInvoiceEmail(PackageInvoiceViewModel invoice, string organizationName, string organizationEmail)
        {
            var invoiceDate = invoice.CreatedDate.ToString("MMMM dd, yyyy");
            var billingPeriod = $"{invoice.BillingPeriodStart:MMMM dd, yyyy} - {invoice.BillingPeriodEnd:MMMM dd, yyyy}";
            var dueDate = invoice.BillingPeriodEnd.AddDays(7).ToString("MMMM dd, yyyy");
            var statusClass = invoice.PaymentStatus.ToLower();
            var initialChargeRow = invoice.IsInitialCharge 
                ? $@"<tr>
                        <td>Initial One-Time Cost:</td>
                        <td style=""text-align: right;"">${invoice.InitialOneTimeCost:F2}</td>
                    </tr>" 
                : string.Empty;

            string paymentInstructions;
            if (invoice.PaymentStatus == "Pending")
            {
                paymentInstructions = @"
            <p><strong>Payment Instructions:</strong></p>
            <p>This invoice will be automatically charged to your registered payment method on the due date. Please ensure your payment method is up to date.</p>";
            }
            else if (invoice.PaymentStatus == "Paid")
            {
                var transactionId = invoice.PaymentTransactionId ?? "N/A";
                paymentInstructions = $@"
            <p><strong>Payment Confirmation:</strong></p>
            <p>This invoice has been successfully paid. Transaction ID: {transactionId}</p>";
            }
            else
            {
                var failureReason = invoice.PaymentFailureReason ?? "Unknown error";
                paymentInstructions = $@"
            <p><strong>Payment Failed:</strong></p>
            <p>We were unable to process your payment. Reason: {failureReason}</p>
            <p>Please update your payment method and contact support if you need assistance.</p>";
            }

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4a90e2; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .invoice-details {{ background-color: white; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .amount-box {{ background-color: #4a90e2; color: white; padding: 15px; text-align: center; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f2f2f2; }}
        .status-badge {{ display: inline-block; padding: 5px 10px; border-radius: 3px; font-weight: bold; }}
        .status-pending {{ background-color: #ffc107; color: #000; }}
        .status-paid {{ background-color: #28a745; color: white; }}
        .status-failed {{ background-color: #dc3545; color: white; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Invoice #{invoice.InvoiceNumber}</h1>
        </div>
        <div class=""content"">
            <p>Dear {organizationName},</p>
            <p>Please find your monthly package invoice details below:</p>
            
            <div class=""invoice-details"">
                <h3>Invoice Information</h3>
                <table>
                    <tr>
                        <td><strong>Invoice Number:</strong></td>
                        <td>{invoice.InvoiceNumber}</td>
                    </tr>
                    <tr>
                        <td><strong>Invoice Date:</strong></td>
                        <td>{invoiceDate}</td>
                    </tr>
                    <tr>
                        <td><strong>Billing Period:</strong></td>
                        <td>{billingPeriod}</td>
                    </tr>
                    <tr>
                        <td><strong>Due Date:</strong></td>
                        <td>{dueDate}</td>
                    </tr>
                    <tr>
                        <td><strong>Payment Status:</strong></td>
                        <td>
                            <span class=""status-badge status-{statusClass}"">
                                {invoice.PaymentStatus}
                            </span>
                        </td>
                    </tr>
                </table>
            </div>

            <div class=""invoice-details"">
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
                    <tr>
                        <td><strong>Subtotal:</strong></td>
                        <td style=""text-align: right;""><strong>${invoice.SubTotal:F2}</strong></td>
                    </tr>
                    <tr>
                        <td>Tax:</td>
                        <td style=""text-align: right;"">${invoice.TaxAmount:F2}</td>
                    </tr>
                </table>
            </div>

            <div class=""amount-box"">
                <h2 style=""margin: 0;"">Total Amount: ${invoice.TotalAmount:F2}</h2>
            </div>

            {paymentInstructions}
            
            <p><strong>Note:</strong> A PDF version of this invoice is attached to this email for your records.</p>

            <p>If you have any questions about this invoice, please contact our support team.</p>
            
            <p>Best regards,<br>eXtremeScheduler Billing Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = organizationEmail,
                Subject = $"Invoice #{invoice.InvoiceNumber} - {organizationName}",
                Body = body,
                IsHtml = true
            };
        }

        public static EmailMessage CreatePaymentSuccessEmail(PackageInvoiceViewModel invoice, string organizationName, string organizationEmail, string transactionId)
        {
            var paymentDate = DateTime.UtcNow.ToString("MMMM dd, yyyy HH:mm");
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .success-box {{ background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Payment Successful</h1>
        </div>
        <div class=""content"">
            <p>Dear {organizationName},</p>
            
            <div class=""success-box"">
                <h3 style=""margin-top: 0;"">✓ Payment Received</h3>
                <p>Your payment for Invoice #{invoice.InvoiceNumber} has been successfully processed.</p>
                <p><strong>Amount Paid:</strong> ${invoice.TotalAmount:F2}</p>
                <p><strong>Transaction ID:</strong> {transactionId}</p>
                <p><strong>Payment Date:</strong> {paymentDate} UTC</p>
            </div>

            <p>Thank you for your payment. Your account is up to date.</p>
            
            <p>Best regards,<br>eXtremeScheduler Billing Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = organizationEmail,
                Subject = $"Payment Confirmation - Invoice #{invoice.InvoiceNumber}",
                Body = body,
                IsHtml = true
            };
        }

        public static EmailMessage CreatePaymentFailedEmail(PackageInvoiceViewModel invoice, string organizationName, string organizationEmail, string failureReason)
        {
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .error-box {{ background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Payment Failed</h1>
        </div>
        <div class=""content"">
            <p>Dear {organizationName},</p>
            
            <div class=""error-box"">
                <h3 style=""margin-top: 0;"">⚠ Payment Processing Failed</h3>
                <p>We were unable to process your payment for Invoice #{invoice.InvoiceNumber}.</p>
                <p><strong>Amount:</strong> ${invoice.TotalAmount:F2}</p>
                <p><strong>Reason:</strong> {failureReason}</p>
            </div>

            <p><strong>What you need to do:</strong></p>
            <ul>
                <li>Please update your payment method in the admin portal</li>
                <li>Ensure your card is valid and has sufficient funds</li>
                <li>Contact support if you need assistance</li>
            </ul>

            <p>We will automatically retry the payment. Please update your payment information as soon as possible to avoid service interruption.</p>
            
            <p>Best regards,<br>eXtremeScheduler Billing Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = organizationEmail,
                Subject = $"Payment Failed - Invoice #{invoice.InvoiceNumber}",
                Body = body,
                IsHtml = true
            };
        }
    }
}
