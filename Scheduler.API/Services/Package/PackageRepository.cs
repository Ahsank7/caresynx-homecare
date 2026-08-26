using Dapper;
using Scheduler.API.Models.Package;
using Scheduler.API.Models.Email;
using Scheduler.API.Services.Payment;
using Scheduler.API.Services.Email;
using Scheduler.API.Services.Security;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Collections.Generic;

namespace Scheduler.API.Services.Package
{
    public class PackageRepository : IPackage
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly IStripeConnectedAccountService _stripeService;
        private readonly ISaaSStripeService _saasStripeService;
        private readonly IEmailService _emailService;
        private readonly IInvoicePdfService _invoicePdfService;
        private readonly ICrypto _crypto;
        private readonly ILogger<PackageRepository> _logger;

        public PackageRepository(
            IDapperRepository dapperRepository, 
            IStripeConnectedAccountService stripeService,
            ISaaSStripeService saasStripeService,
            IEmailService emailService,
            IInvoicePdfService invoicePdfService,
            ICrypto crypto,
            ILogger<PackageRepository> logger)
        {
            _dapperRepository = dapperRepository;
            _stripeService = stripeService;
            _saasStripeService = saasStripeService;
            _emailService = emailService;
            _invoicePdfService = invoicePdfService;
            _crypto = crypto;
            _logger = logger;
        }

        public async Task<Guid?> SaveUpdatePackageAsync(PackageViewModel model)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", model.Id, DbType.Guid);
                dp_params.Add("@pName", model.Name, DbType.String);
                dp_params.Add("@pDescription", model.Description, DbType.String);
                dp_params.Add("@pPerClientCharge", model.PerClientCharge, DbType.Decimal);
                dp_params.Add("@pInitialOneTimeCost", model.InitialOneTimeCost, DbType.Decimal);
                dp_params.Add("@pInfrastructureCost", model.InfrastructureCost, DbType.Decimal);
                dp_params.Add("@pSupportCharges", model.SupportCharges, DbType.Decimal);
                dp_params.Add("@pNewFeatureReportCharges", model.NewFeatureReportCharges, DbType.Decimal);
                dp_params.Add("@pIsActive", model.IsActive, DbType.Boolean);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);

                await _dapperRepository.InsertAsync<Guid?>(
                    "[dbo].[InsertUpdatePackage]",
                    dp_params,
                    commandType: CommandType.StoredProcedure);

                return dp_params.Get<Guid?>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(List<PackageViewModel>, int)> GetAllPackagesAsync(bool includeInactive = false)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pIncludeInactive", includeInactive, DbType.Boolean);

            var result = await _dapperRepository.GetAllAsync<PackageViewModel>(
                "[dbo].[GetAllPackages]",
                dp_params,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<PackageViewModel?> GetPackageByIdAsync(Guid packageId)
        {
            var packages = await GetAllPackagesAsync(true);
            return packages.Item1.FirstOrDefault(p => p.Id == packageId);
        }

        public async Task<bool> AssignPackageToOrganizationAsync(AssignPackageToOrganizationViewModel model)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pOrganizationId", model.OrganizationId, DbType.Guid);
                dp_params.Add("@pPackageId", model.PackageId, DbType.Guid);
                dp_params.Add("@pStartDate", model.StartDate, DbType.DateTime2);
                dp_params.Add("@pNotes", model.Notes, DbType.String);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);

                await _dapperRepository.InsertAsync<Guid?>(
                    "[dbo].[AssignPackageToOrganization]",
                    dp_params,
                    commandType: CommandType.StoredProcedure);

                var resultId = dp_params.Get<Guid?>("@pOutId");
                return resultId.HasValue;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<OrganizationPackageHistoryViewModel>> GetOrganizationPackageHistoryAsync(Guid organizationId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

            var result = await _dapperRepository.QueryAsync<OrganizationPackageHistoryViewModel>(
                "[dbo].[GetOrganizationPackageHistory]",
                dp_params,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<OrganizationPackageHistoryViewModel?> GetCurrentOrganizationPackageAsync(Guid organizationId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

            var result = await _dapperRepository.QueryAsync<OrganizationPackageHistoryViewModel>(
                "[dbo].[GetCurrentOrganizationPackage]",
                dp_params,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateOrganizationPackagePricingAsync(UpdateOrganizationPackagePricingViewModel model)
        {
            try
            {
                // First verify that the package is active
                var packageCheck = await _dapperRepository.QueryAsync<dynamic>(
                    "SELECT IsActive FROM [dbo].[tblOrganizationPackage] WHERE Id = @OrganizationPackageId",
                    new DynamicParameters(new { OrganizationPackageId = model.OrganizationPackageId }),
                    CommandType.Text);

                var package = packageCheck.FirstOrDefault();
                if (package == null)
                {
                    _logger.LogWarning($"Organization package {model.OrganizationPackageId} not found");
                    return false;
                }

                if (!(bool)package.IsActive)
                {
                    _logger.LogWarning($"Attempted to update inactive package {model.OrganizationPackageId}");
                    return false;
                }

                var dp_params = new DynamicParameters();
                dp_params.Add("@pOrganizationPackageId", model.OrganizationPackageId, DbType.Guid);
                dp_params.Add("@pPerClientCharge", model.PerClientCharge, DbType.Decimal);
                dp_params.Add("@pInitialOneTimeCost", model.InitialOneTimeCost, DbType.Decimal);
                dp_params.Add("@pInfrastructureCost", model.InfrastructureCost, DbType.Decimal);
                dp_params.Add("@pSupportCharges", model.SupportCharges, DbType.Decimal);
                dp_params.Add("@pNewFeatureReportCharges", model.NewFeatureReportCharges, DbType.Decimal);

                var result = await _dapperRepository.ExecuteAsync(
                    "[dbo].[UpdateOrganizationPackagePricing]",
                    dp_params,
                    commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization package pricing");
                return false;
            }
        }

        public async Task<OrganizationCardInfoViewModel?> SaveUpdateOrganizationCardAsync(OrganizationCardInfoViewModel model)
        {
            try
            {
                // Note: Card data is already encrypted when it reaches this method (encrypted in controller)
                // We need to decrypt temporarily for Stripe validation, then re-encrypt for storage
                // Since the data is encrypted, we'll assume validation will happen at controller level
                // before encryption, or we skip validation if encrypting at this level
                
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", model.Id, DbType.Int32);
                dp_params.Add("@pOrganizationId", model.OrganizationId, DbType.Guid);
                dp_params.Add("@pCardHolderName", model.CardHolderName, DbType.String);
                dp_params.Add("@pCardNumber", model.CardNumber, DbType.String);
                dp_params.Add("@pExpiryMonth", model.ExpiryMonth, DbType.Byte);
                dp_params.Add("@pExpiryYear", model.ExpiryYear, DbType.Int16);
                dp_params.Add("@pCVV", model.CVV, DbType.String);
                dp_params.Add("@pTypeId", model.TypeId, DbType.Int32);
                dp_params.Add("@pIsActive", model.IsActive, DbType.Boolean);
                dp_params.Add("@pOutId", null, DbType.Int32, direction: ParameterDirection.Output);

                await _dapperRepository.InsertAsync<int>(
                    "[dbo].[InsertUpdateOrganizationCardInfo]",
                    dp_params,
                    commandType: CommandType.StoredProcedure);

                var resultId = dp_params.Get<int?>("@pOutId");
                if (resultId.HasValue)
                {
                    return await GetOrganizationCardAsync(model.OrganizationId);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving organization card");
                return null;
            }
        }

        public async Task<OrganizationCardInfoViewModel?> GetOrganizationCardAsync(Guid organizationId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

            var result = await _dapperRepository.QueryAsync<OrganizationCardInfoViewModel>(
                "[dbo].[GetOrganizationCardInfo]",
                dp_params,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        // Helper method to get decrypted card for payment processing
        private async Task<OrganizationCardInfoViewModel?> GetDecryptedOrganizationCardAsync(Guid organizationId)
        {
            var card = await GetOrganizationCardAsync(organizationId);
            
            if (card != null)
            {
                // Decrypt card data for payment processing
                if (!string.IsNullOrEmpty(card.CardNumber))
                {
                    card.CardNumber = _crypto.Decrypt(card.CardNumber);
                }
                if (!string.IsNullOrEmpty(card.CVV))
                {
                    card.CVV = _crypto.Decrypt(card.CVV);
                }
            }
            
            return card;
        }

        public async Task<bool> ValidateOrganizationCardAsync(Guid organizationId)
        {
            var card = await GetOrganizationCardAsync(organizationId);
            if (card == null) return false;

            // Check if card is expired
            var currentDate = DateTime.UtcNow;
            var expiryDate = new DateTime(card.ExpiryYear, card.ExpiryMonth, 1).AddMonths(1).AddDays(-1);
            
            return expiryDate >= currentDate && card.IsActive;
        }

        public async Task<List<PackageInvoiceViewModel>> GetOrganizationInvoicesAsync(Guid organizationId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);

            var result = await _dapperRepository.QueryAsync<PackageInvoiceViewModel>(
                "[dbo].[GetOrganizationPackageInvoices]",
                dp_params,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<PackageInvoiceViewModel?> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoices = await _dapperRepository.QueryAsync<PackageInvoiceViewModel>(
                @"SELECT 
                    pi.Id,
                    pi.OrganizationId,
                    pi.OrganizationPackageId,
                    p.Name AS PackageName,
                    pi.InvoiceDate,
                    pi.BillingPeriodStart,
                    pi.BillingPeriodEnd,
                    pi.PerClientCharge,
                    pi.ClientCount,
                    pi.InitialOneTimeCost,
                    pi.InfrastructureCost,
                    pi.SupportCharges,
                    pi.NewFeatureReportCharges,
                    pi.SubTotal,
                    pi.TaxAmount,
                    pi.TotalAmount,
                    pi.IsInitialCharge,
                    pi.PaymentStatus,
                    pi.PaymentDate,
                    pi.PaymentTransactionId,
                    pi.PaymentFailureReason,
                    pi.InvoiceNumber,
                    pi.CreatedDate
                FROM [dbo].[tblPackageInvoice] pi
                INNER JOIN [dbo].[tblOrganizationPackage] op ON pi.OrganizationPackageId = op.Id
                INNER JOIN [dbo].[tblPackage] p ON op.PackageId = p.Id
                WHERE pi.Id = @InvoiceId",
                new DynamicParameters(new { InvoiceId = invoiceId }),
                commandType: CommandType.Text);

            return invoices.FirstOrDefault();
        }

        public async Task<int> GenerateMonthlyInvoicesAsync(GenerateMonthlyInvoicesRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pBillingMonth", request.BillingMonth, DbType.Int32);
                dp_params.Add("@pBillingYear", request.BillingYear, DbType.Int32);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);

                var result = await _dapperRepository.QueryAsync<dynamic>(
                    "[dbo].[GenerateMonthlyPackageInvoices]",
                    dp_params,
                    commandType: CommandType.StoredProcedure);

                var firstResult = result.FirstOrDefault();
                int invoiceCount = 0;
                if (firstResult != null && firstResult.GeneratedInvoiceCount != null)
                {
                    invoiceCount = (int)firstResult.GeneratedInvoiceCount;
                }

                // Send email notifications for newly generated invoices
                if (invoiceCount > 0)
                {
                    await SendInvoiceEmailsAsync(request.BillingMonth, request.BillingYear, request.OrganizationId);
                }

                return invoiceCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating monthly invoices for {request.BillingMonth}/{request.BillingYear}");
                return 0;
            }
        }

        private async Task SendInvoiceEmailsAsync(int billingMonth, int billingYear, Guid? organizationId)
        {
            try
            {
                // Get all pending invoices for the billing period
                var query = @"
                    SELECT pi.*, o.Name AS OrganizationName, o.Email AS OrganizationEmail
                    FROM [dbo].[tblPackageInvoice] pi
                    INNER JOIN [dbo].[tblOrganization] o ON pi.OrganizationId = o.Id
                    WHERE pi.BillingPeriodStart >= @PeriodStart 
                      AND pi.BillingPeriodStart <= @PeriodEnd
                      AND pi.PaymentStatus = 'Pending'
                      AND (@OrganizationId IS NULL OR pi.OrganizationId = @OrganizationId)
                      AND o.Email IS NOT NULL AND o.Email != ''";

                var periodStart = new DateTime(billingYear, billingMonth, 1);
                var periodEnd = periodStart.AddMonths(1).AddDays(-1);

                var invoices = await _dapperRepository.QueryAsync<dynamic>(
                    query,
                    new DynamicParameters(new 
                    { 
                        PeriodStart = periodStart, 
                        PeriodEnd = periodEnd,
                        OrganizationId = organizationId
                    }),
                    CommandType.Text);

                foreach (var inv in invoices)
                {
                    try
                    {
                        var invoice = await GetInvoiceByIdAsync((int)inv.Id);
                        if (invoice != null && !string.IsNullOrWhiteSpace(inv.OrganizationEmail?.ToString()))
                        {
                            var orgName = inv.OrganizationName?.ToString() ?? "Organization";
                            var orgEmail = inv.OrganizationEmail.ToString();
                            
                            // Generate PDF and upload to Azure
                            string pdfUrl = null;
                            byte[] pdfBytes = null;
                            try
                            {
                                pdfUrl = await _invoicePdfService.GenerateAndUploadInvoicePdfAsync(invoice, orgName);
                                
                                // Update invoice with PDF URL
                                await _dapperRepository.ExecuteAsync(
                                    "[dbo].[UpdateInvoiceDocumentUrl]",
                                    new DynamicParameters(new { 
                                        pInvoiceId = invoice.Id, 
                                        pDocumentUrl = pdfUrl 
                                    }),
                                    CommandType.StoredProcedure);
                                
                                // Get PDF bytes for attachment
                                pdfBytes = await _invoicePdfService.GenerateInvoicePdfBytesAsync(invoice, orgName);
                                
                                _logger.LogInformation($"Invoice PDF generated and saved: {pdfUrl}");
                            }
                            catch (Exception pdfEx)
                            {
                                _logger.LogError(pdfEx, $"Error generating PDF for invoice {invoice.InvoiceNumber}. Sending email without attachment.");
                            }
                            
                            // Create email with PDF attachment
                            var emailMessage = EmailTemplates.CreateInvoiceEmail(
                                invoice,
                                orgName,
                                orgEmail
                            );
                            
                            // Attach PDF if generated successfully
                            if (pdfBytes != null)
                            {
                                emailMessage.Attachments = new List<Models.Email.EmailAttachment>
                                {
                                    new Models.Email.EmailAttachment
                                    {
                                        FileName = $"Invoice-{invoice.InvoiceNumber}.pdf",
                                        Content = pdfBytes,
                                        ContentType = "application/pdf"
                                    }
                                };
                            }

                            await _emailService.SendEmailAsync(emailMessage);
                            _logger.LogInformation($"Invoice email sent for invoice {invoice.InvoiceNumber} to {orgEmail}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending invoice email for invoice {inv.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invoice emails");
            }
        }

        public async Task<bool> ProcessInvoicePaymentAsync(ProcessPackageInvoicePaymentRequest request)
        {
            try
            {
                // Get invoice
                var invoice = await GetInvoiceByIdAsync(request.InvoiceId);
                if (invoice == null || invoice.PaymentStatus == "Paid")
                    return false;

                // Get organization card (DECRYPTED for payment processing)
                var card = await GetDecryptedOrganizationCardAsync(request.OrganizationId);
                if (card == null)
                {
                    // Update invoice with failure reason
                    await UpdateInvoicePaymentStatus(request.InvoiceId, "Failed", null, null, "No payment card on file");
                    return false;
                }

                // Validate card
                if (!await ValidateOrganizationCardAsync(request.OrganizationId))
                {
                    await UpdateInvoicePaymentStatus(request.InvoiceId, "Failed", null, null, "Card expired or invalid");
                    return false;
                }

                // Create Stripe token from DECRYPTED card info
                var tokenRequest = new CardTokenRequest
                {
                    CardHolderName = card.CardHolderName,
                    CardNumber = card.CardNumber, // Already decrypted by GetDecryptedOrganizationCardAsync
                    ExpiryMonth = card.ExpiryMonth.ToString("00"),
                    ExpiryYear = card.ExpiryYear.ToString(),
                    Cvc = card.CVV // Already decrypted
                };

                string stripeToken;
                try
                {
                    stripeToken = await _stripeService.CreateStripeTokenAsync(tokenRequest);
                }
                catch (Exception ex)
                {
                    await UpdateInvoicePaymentStatus(request.InvoiceId, "Failed", null, null, $"Token creation failed: {ex.Message}");
                    return false;
                }

                // Get organization info for email
                var orgInfo = await _dapperRepository.QueryAsync<dynamic>(
                    "SELECT Name, Email FROM [dbo].[tblOrganization] WHERE Id = @OrganizationId",
                    new DynamicParameters(new { OrganizationId = request.OrganizationId }),
                    CommandType.Text);

                var org = orgInfo.FirstOrDefault();
                var orgName = org?.Name?.ToString() ?? "Organization";
                var orgEmail = org?.Email?.ToString() ?? string.Empty;

                // Create charge request
                var chargeRequest = new InvoiceChargeRequest
                {
                    Amount = invoice.TotalAmount,
                    Currency = "usd", // TODO: Get from organization currency settings
                    StripeToken = stripeToken,
                    Description = $"Package Invoice #{invoice.InvoiceNumber}",
                    CustomerEmail = orgEmail,
                    Metadata = new Dictionary<string, string>
                    {
                        { "InvoiceId", invoice.Id.ToString() },
                        { "OrganizationId", request.OrganizationId.ToString() },
                        { "InvoiceNumber", invoice.InvoiceNumber },
                        { "PaymentType", "PackageInvoice" }
                    }
                };

                // Process payment using SaaS Admin Stripe account (not organization's account)
                var paymentResult = await _saasStripeService.CreateChargeForPackageInvoiceAsync(chargeRequest);
                
                _logger.LogInformation($"Package invoice payment processed using SaaS Admin Stripe account. Invoice: {invoice.InvoiceNumber}, Organization: {request.OrganizationId}");

                if (paymentResult.Success)
                {
                    await UpdateInvoicePaymentStatus(
                        request.InvoiceId, 
                        "Paid", 
                        DateTime.UtcNow, 
                        paymentResult.ReferenceId, 
                        null);

                    // Send payment success email
                    if (!string.IsNullOrWhiteSpace(orgEmail))
                    {
                        try
                        {
                            var updatedInvoice = await GetInvoiceByIdAsync(request.InvoiceId);
                            if (updatedInvoice != null)
                            {
                                var emailMessage = EmailTemplates.CreatePaymentSuccessEmail(
                                    updatedInvoice,
                                    orgName,
                                    orgEmail,
                                    paymentResult.ReferenceId
                                );
                                await _emailService.SendEmailAsync(emailMessage);
                                _logger.LogInformation($"Payment success email sent for invoice {invoice.InvoiceNumber} to {orgEmail}");
                            }
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, $"Error sending payment success email for invoice {invoice.InvoiceNumber}");
                        }
                    }

                    return true;
                }
                else
                {
                    var failureReason = paymentResult.ErrorMessage ?? "Payment processing failed";
                    await UpdateInvoicePaymentStatus(
                        request.InvoiceId, 
                        "Failed", 
                        null, 
                        null, 
                        failureReason);

                    // Send payment failed email
                    if (!string.IsNullOrWhiteSpace(orgEmail))
                    {
                        try
                        {
                            var updatedInvoice = await GetInvoiceByIdAsync(request.InvoiceId);
                            if (updatedInvoice != null)
                            {
                                var emailMessage = EmailTemplates.CreatePaymentFailedEmail(
                                    updatedInvoice,
                                    orgName,
                                    orgEmail,
                                    failureReason
                                );
                                await _emailService.SendEmailAsync(emailMessage);
                                _logger.LogInformation($"Payment failed email sent for invoice {invoice.InvoiceNumber} to {orgEmail}");
                            }
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, $"Error sending payment failed email for invoice {invoice.InvoiceNumber}");
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> UpdateInvoicePaymentStatus(int invoiceId, string status, DateTime? paymentDate, string transactionId, string failureReason)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pInvoiceId", invoiceId, DbType.Int32);
                dp_params.Add("@pPaymentStatus", status, DbType.String);
                dp_params.Add("@pPaymentDate", paymentDate, DbType.DateTime2);
                dp_params.Add("@pPaymentTransactionId", transactionId, DbType.String);
                dp_params.Add("@pPaymentFailureReason", failureReason, DbType.String);

                var result = await _dapperRepository.ExecuteAsync(
                    "[dbo].[UpdatePackageInvoicePaymentStatus]",
                    dp_params,
                    commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

