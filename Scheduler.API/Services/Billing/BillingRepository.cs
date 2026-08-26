using Dapper;
using System;
using System.Data;
using Scheduler.API.Models.Client;
using Scheduler.API.Models.Billing;
using Scheduler.API.Helper;
using Scheduler.API.Models.Billing;
using Scheduler.API.Services.Email;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Services.Billing
{
    public class BillingRepository : IBilling
    {
        IDapperRepository _dapperRepository = null;
        private readonly IEmailService _emailService;
        private readonly ILogger<BillingRepository> _logger;

        public BillingRepository(
            IDapperRepository DapperRepository,
            IEmailService emailService,
            ILogger<BillingRepository> logger)
        {
            _dapperRepository = DapperRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<BillingDetailResponse> GetBillingInvoiceDetailAsync(BillingDetailRequest request)
        {
            BillingDetailResponse billingDetailResponse = new BillingDetailResponse();

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pBillingId", request.BillingId, DbType.Int32);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            var result = await Task.FromResult(_dapperRepository.GetAll<BillingDetail>("[dbo].[uspGetBillingDetail]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            billingDetailResponse.Response = result.Item1;
            billingDetailResponse.TotalRecords = result.Item2;

            return billingDetailResponse;
        }
    

        public async Task<BillingInfoResponse> GetBillingInvoiceInfoAsync(BillingInfoRequest request)
        {
            BillingInfoResponse billingInfoResponse = new BillingInfoResponse(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pUserId", request.UserId, DbType.Guid);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.Int32);
            dp_params.Add("@pDate", request.Date, DbType.Date);
            dp_params.Add("@pTransactionId", request.TransactionId, DbType.String);
            dp_params.Add("@pUserNo", request.UserNo, DbType.String);
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetAll<BillingInfo>("[dbo].[uspGetBillingInfo]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            billingInfoResponse.Response = result.Item1;
            billingInfoResponse.TotalRecords = result.Item2;

            return billingInfoResponse;
        }

        public async Task<GenerateBillingResponse> GenerateBillingInvoicesAsync(GenerateBillingRequest request)
        {
            GenerateBillingResponse response = new GenerateBillingResponse();

            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
                dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);

                // Execute the stored procedure and get the result
                var result = await _dapperRepository.GetAsync<dynamic>("[dbo].[uspGenerateBillingInvoices]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                // Extract the generated invoice count from the result
                int generatedCount = 0;
                if (result != null)
                {
                    var resultDict = (IDictionary<string, object>)result;
                    if (resultDict.ContainsKey("GeneratedInvoiceCount"))
                    {
                        int.TryParse(resultDict["GeneratedInvoiceCount"].ToString(), out generatedCount);
                    }
                }

                response.IsSuccess = true;
                response.Message = generatedCount > 0 
                    ? $"Billing invoices generated successfully. {generatedCount} invoice(s) created."
                    : "No billing invoices were generated for the selected date range.";
                response.GeneratedInvoices = generatedCount;

                // Send email notifications for generated invoices
                if (generatedCount > 0 && request.StartDate.HasValue && request.EndDate.HasValue && request.OrganizationId.HasValue)
                {
                    await SendInvoiceGenerationEmailsAsync(request.StartDate.Value, request.EndDate.Value, request.OrganizationId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating billing invoices");
                response.IsSuccess = false;
                response.Message = $"Error generating billing invoices: {ex.Message}";
                response.GeneratedInvoices = 0;
            }

            return response;
        }

        public async Task<BillingPreviewResponse> PreviewBillingInvoicesAsync(BillingPreviewRequest request)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
            dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
            dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
            dp_params.Add("@pPageNumber", request.PageNumber, DbType.Int32);
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);

            var result = await Task.FromResult(_dapperRepository.GetAll<BillingPreviewInfo>("[dbo].[uspPreviewBillingInvoices]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            return new BillingPreviewResponse
            {
                Response = result.Item1,
                TotalRecords = result.Item2
            };
        }

        private async Task SendInvoiceGenerationEmailsAsync(DateTime startDate, DateTime endDate, Guid organizationId)
        {
            try
            {
                // Query to get newly generated invoices with client details
                var query = @"
                    SELECT 
                        inv.Id AS InvoiceId,
                        inv.Date AS InvoiceDate,
                        inv.StartDate,
                        inv.EndDate,
                        inv.DueDate,
                        inv.Details,
                        inv.ClientId,
                        CAST(inv.Id AS NVARCHAR(50)) AS InvoiceNumber,
                        COALESCE(NULLIF(LTRIM(RTRIM(inv.BillToDisplayName)), ''), u.FirstName + ' ' + ISNULL(u.SurName, '') + ' ' + u.LastName) AS ClientName,
                        COALESCE(NULLIF(LTRIM(RTRIM(inv.DebtorEmail)), ''), u.Email) AS ClientEmail,
                        ISNULL(SUM(invd.Amount), 0) AS TotalAmount,
                        COUNT(DISTINCT invd.TaskId) AS TaskCount
                    FROM [dbo].[tblBillingInvoice] inv
                    INNER JOIN [dbo].[tblUser] u ON inv.ClientId = u.Id
                    LEFT JOIN [dbo].[tblBillingInvoiceDetail] invd ON invd.BillingInvoiceId = inv.Id
                    INNER JOIN dbo.tbUserFranchise uf ON uf.UserId = u.Id
                    INNER JOIN dbo.tblFranchise f ON f.Id = uf.FranchiseId
                    WHERE inv.StartDate = @StartDate 
                      AND inv.EndDate = @EndDate
                      AND f.OrganizationId = @OrganizationId
                      AND COALESCE(NULLIF(LTRIM(RTRIM(inv.DebtorEmail)), ''), u.Email) IS NOT NULL 
                      AND COALESCE(NULLIF(LTRIM(RTRIM(inv.DebtorEmail)), ''), u.Email) != ''
                      AND COALESCE(NULLIF(LTRIM(RTRIM(inv.DebtorEmail)), ''), u.Email) != 'N/A'
                    GROUP BY inv.Id, inv.Date, inv.StartDate, inv.EndDate, inv.DueDate, inv.Details, 
                             inv.ClientId, inv.BillToDisplayName, inv.DebtorEmail, u.FirstName, u.SurName, u.LastName, u.Email";

                var dp_params = new DynamicParameters();
                dp_params.Add("@StartDate", startDate, DbType.Date);
                dp_params.Add("@EndDate", endDate, DbType.Date);
                dp_params.Add("@OrganizationId", organizationId, DbType.Guid);

                var invoices = await _dapperRepository.QueryAsync<dynamic>(
                    query,
                    dp_params,
                    CommandType.Text);

                foreach (var invoice in invoices)
                {
                    try
                    {
                        var email = BillingWageEmailTemplates.CreateInvoiceGeneratedEmail(
                            (string)invoice.ClientEmail,
                            (string)invoice.ClientName,
                            (int)invoice.InvoiceId,
                            (string)invoice.InvoiceNumber,
                            (DateTime)invoice.InvoiceDate,
                            (DateTime)invoice.StartDate,
                            (DateTime)invoice.EndDate,
                            (DateTime)invoice.DueDate,
                            (decimal)invoice.TotalAmount,
                            (int)invoice.TaskCount,
                            (string)invoice.Details
                        );

                        await _emailService.SendEmailAsync(email);
                        _logger.LogInformation($"Invoice generation email sent to client for InvoiceId: {invoice.InvoiceId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending invoice generation email for InvoiceId: {invoice.InvoiceId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invoice generation emails");
            }
        }
    }
}
