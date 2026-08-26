using Scheduler.API.Models.Email;

namespace Scheduler.API.Services.Email
{
    public static class BillingWageEmailTemplates
    {
        public static EmailMessage CreateInvoiceGeneratedEmail(
            string clientEmail,
            string clientName,
            int invoiceId,
            string invoiceNumber,
            DateTime invoiceDate,
            DateTime startDate,
            DateTime endDate,
            DateTime dueDate,
            decimal totalAmount,
            int taskCount,
            string details)
        {
            var invoiceDateStr = invoiceDate.ToString("MMMM dd, yyyy");
            var dueDateStr = dueDate.ToString("MMMM dd, yyyy");
            var billingPeriod = $"{startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}";
            
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
        .amount-box {{ background-color: #4a90e2; color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 15px 0; }}
        .due-date-warning {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>📄 Invoice Generated</h1>
        </div>
        <div class=""content"">
            <p>Dear {clientName},</p>
            <p>Your invoice has been successfully generated for the services provided during the billing period.</p>
            
            <div class=""invoice-details"">
                <h3>Invoice Information</h3>
                <table>
                    <tr>
                        <td><strong>Invoice ID:</strong></td>
                        <td>#{invoiceId}</td>
                    </tr>
                    <tr>
                        <td><strong>Invoice Number:</strong></td>
                        <td>{invoiceNumber}</td>
                    </tr>
                    <tr>
                        <td><strong>Invoice Date:</strong></td>
                        <td>{invoiceDateStr}</td>
                    </tr>
                    <tr>
                        <td><strong>Billing Period:</strong></td>
                        <td>{billingPeriod}</td>
                    </tr>
                    <tr>
                        <td><strong>Number of Services:</strong></td>
                        <td>{taskCount} task(s)</td>
                    </tr>
                    <tr>
                        <td><strong>Details:</strong></td>
                        <td>{details}</td>
                    </tr>
                </table>
            </div>

            <div class=""amount-box"">
                <h2 style=""margin: 0;"">Total Amount: ${totalAmount:F2}</h2>
            </div>

            <div class=""due-date-warning"">
                <strong>⚠ Payment Due Date:</strong> {dueDateStr}
                <p style=""margin: 5px 0 0 0;"">Please ensure payment is made by the due date to avoid any service interruption.</p>
            </div>

            <p><strong>What's Next?</strong></p>
            <ul>
                <li>Review your invoice details in the system</li>
                <li>Download a detailed PDF copy from your dashboard</li>
                <li>Make payment before the due date</li>
                <li>Contact support if you have any questions</li>
            </ul>

            <p>Thank you for using our services!</p>
            
            <p>Best regards,<br>eXtremeScheduler Billing Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
            <p>&copy; {DateTime.Now.Year} eXtremeScheduler. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = clientEmail,
                Subject = $"Invoice Generated - Invoice #{invoiceId} (Due: {dueDateStr})",
                Body = body,
                IsHtml = true
            };
        }

        public static EmailMessage CreateWageGeneratedEmail(
            string serviceProviderEmail,
            string serviceProviderName,
            int wageId,
            DateTime wageDate,
            DateTime startDate,
            DateTime endDate,
            DateTime dueDate,
            decimal totalAmount,
            int taskCount,
            string description)
        {
            var wageDateStr = wageDate.ToString("MMMM dd, yyyy");
            var dueDateStr = dueDate.ToString("MMMM dd, yyyy");
            var paymentPeriod = $"{startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}";
            
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .wage-details {{ background-color: white; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .amount-box {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 15px 0; }}
        .payment-info {{ background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>💰 Wage Payment Generated</h1>
        </div>
        <div class=""content"">
            <p>Dear {serviceProviderName},</p>
            <p>Your wage payment has been successfully generated for the services you provided during the payment period.</p>
            
            <div class=""wage-details"">
                <h3>Wage Information</h3>
                <table>
                    <tr>
                        <td><strong>Wage ID:</strong></td>
                        <td>#{wageId}</td>
                    </tr>
                    <tr>
                        <td><strong>Generated Date:</strong></td>
                        <td>{wageDateStr}</td>
                    </tr>
                    <tr>
                        <td><strong>Payment Period:</strong></td>
                        <td>{paymentPeriod}</td>
                    </tr>
                    <tr>
                        <td><strong>Number of Services:</strong></td>
                        <td>{taskCount} task(s) completed</td>
                    </tr>
                    <tr>
                        <td><strong>Description:</strong></td>
                        <td>{description}</td>
                    </tr>
                </table>
            </div>

            <div class=""amount-box"">
                <h2 style=""margin: 0;"">Total Wage Amount: ${totalAmount:F2}</h2>
            </div>

            <div class=""payment-info"">
                <strong>✓ Payment Schedule:</strong>
                <p style=""margin: 5px 0 0 0;"">Expected payment date: <strong>{dueDateStr}</strong></p>
                <p style=""margin: 5px 0 0 0;"">Payment will be processed to your registered bank account.</p>
            </div>

            <p><strong>Wage Details:</strong></p>
            <ul>
                <li>You completed {taskCount} task(s) during this period</li>
                <li>Total earnings: ${totalAmount:F2}</li>
                <li>Payment will be transferred on or before {dueDateStr}</li>
            </ul>

            <p><strong>What's Next?</strong></p>
            <ul>
                <li>Review your wage details in the system</li>
                <li>Download a detailed breakdown from your dashboard</li>
                <li>Ensure your bank account information is up to date</li>
                <li>Contact support if you have any questions</li>
            </ul>

            <p>Thank you for your excellent service!</p>
            
            <p>Best regards,<br>eXtremeScheduler Payroll Team</p>
        </div>
        <div class=""footer"">
            <p>This is an automated email. Please do not reply to this message.</p>
            <p>&copy; {DateTime.Now.Year} eXtremeScheduler. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = serviceProviderEmail,
                Subject = $"Wage Payment Generated - Wage #{wageId} (${totalAmount:F2})",
                Body = body,
                IsHtml = true
            };
        }
    }
}

