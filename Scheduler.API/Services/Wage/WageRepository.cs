using Dapper;
using System;
using Scheduler.API.Models.Wage;
using Scheduler.API.Services.Email;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Services.Wage
{
    public class WageRepository : IWage
    {
        IDapperRepository _dapperRepository = null;
        private readonly IEmailService _emailService;
        private readonly ILogger<WageRepository> _logger;

        public WageRepository(
            IDapperRepository DapperRepository,
            IEmailService emailService,
            ILogger<WageRepository> logger)
        {
            _dapperRepository = DapperRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<WageDetailResponse> GetWageDetailAsync(WageDetailRequest request)
        {
            WageDetailResponse billingDetailResponse = new WageDetailResponse();

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pWageId", request.WageId, DbType.Int32);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.String);
            var result = await Task.FromResult(_dapperRepository.GetAll<WageDetail>("[dbo].[uspGetWageDetail]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            billingDetailResponse.Response = result.Item1;
            billingDetailResponse.TotalRecords = result.Item2;

            return billingDetailResponse;
        }

        public async Task<WageInfoResponse> GetWageInfoAsync(WageInfoRequest request)
        {
            WageInfoResponse billingInfoResponse = new WageInfoResponse(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);
            dp_params.Add("@pUserId", request.UserId, DbType.Guid);
            dp_params.Add("@pDate", request.Date, DbType.Date);
            dp_params.Add("@pTransactionId", request.TransactionId, DbType.String);
            dp_params.Add("@pUserNo", request.UserNo, DbType.String);
            dp_params.Add("@pFranchiseId", request.FranchiseId, DbType.Guid);
            dp_params.Add("@PageNumber", request.PageNumber, DbType.Int32);
            var result = await Task.FromResult(_dapperRepository.GetAll<WageInfo>("[dbo].[uspGetWageInfo]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            billingInfoResponse.Response = result.Item1;
            billingInfoResponse.TotalRecords = result.Item2;

            return billingInfoResponse;
        }

        public async Task<GenerateWageResponse> GenerateServiceProviderWageAsync(GenerateWageRequest request)
        {
            GenerateWageResponse response = new GenerateWageResponse();

            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
                dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
                dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);

                // Execute the stored procedure and get the result
                var result = await _dapperRepository.GetAsync<dynamic>("[dbo].[uspGenerateServiceProviderWage]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);

                // Extract the generated wage count from the result
                int generatedCount = 0;
                if (result != null)
                {
                    var resultDict = (IDictionary<string, object>)result;
                    if (resultDict.ContainsKey("GeneratedWageCount"))
                    {
                        int.TryParse(resultDict["GeneratedWageCount"].ToString(), out generatedCount);
                    }
                }

                response.IsSuccess = true;
                response.Message = generatedCount > 0 
                    ? $"Service provider wages generated successfully. {generatedCount} wage record(s) created."
                    : "No service provider wages were generated for the selected date range.";
                response.GeneratedWages = generatedCount;

                // Send email notifications for generated wages
                if (generatedCount > 0 && request.OrganizationId.HasValue)
                {
                    await SendWageGenerationEmailsAsync(request.StartDate, request.EndDate, request.OrganizationId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating service provider wages");
                response.IsSuccess = false;
                response.Message = $"Error generating service provider wages: {ex.Message}";
                response.GeneratedWages = 0;
            }

            return response;
        }

        public async Task<WagePreviewResponse> PreviewServiceProviderWageAsync(WagePreviewRequest request)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pStartDate", request.StartDate, DbType.Date);
            dp_params.Add("@pEndDate", request.EndDate, DbType.Date);
            dp_params.Add("@pOrganizationId", request.OrganizationId, DbType.Guid);
            dp_params.Add("@pPageNumber", request.PageNumber, DbType.Int32);
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
            dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
            dp_params.Add("@pSortType", request.SortType, DbType.String);

            var result = await Task.FromResult(_dapperRepository.GetAll<WagePreviewInfo>("[dbo].[uspPreviewServiceProviderWage]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            return new WagePreviewResponse
            {
                Response = result.Item1,
                TotalRecords = result.Item2
            };
        }

        private async Task SendWageGenerationEmailsAsync(DateTime startDate, DateTime endDate, Guid organizationId)
        {
            try
            {
                // Query to get newly generated wages with service provider details
                var query = @"
                    SELECT 
                        w.Id AS WageId,
                        w.Date AS WageDate,
                        w.StartDate,
                        w.EndDate,
                        w.DueDate,
                        w.Description,
                        w.ServiceProviderId,
                        u.FirstName + ' ' + ISNULL(u.SurName, '') + ' ' + u.LastName AS ServiceProviderName,
                        u.Email AS ServiceProviderEmail,
                        ISNULL(SUM(wd.Amount), 0) AS TotalAmount,
                        COUNT(DISTINCT wd.TaskId) AS TaskCount
                    FROM [dbo].[tblServiceProviderWage] w
                    INNER JOIN [dbo].[tblUser] u ON w.ServiceProviderId = u.Id
                    LEFT JOIN [dbo].[tblServiceProviderWageDetail] wd ON wd.ServiceProviderWageId = w.Id
                    INNER JOIN dbo.tbUserFranchise uf ON uf.UserId = u.Id
                    INNER JOIN dbo.tblFranchise f ON f.Id = uf.FranchiseId
                    WHERE w.StartDate = @StartDate 
                      AND w.EndDate = @EndDate
                      AND f.OrganizationId = @OrganizationId
                      AND u.Email IS NOT NULL 
                      AND u.Email != ''
                      AND u.Email != 'N/A'
                    GROUP BY w.Id, w.Date, w.StartDate, w.EndDate, w.DueDate, w.Description, 
                             w.ServiceProviderId, u.FirstName, u.SurName, u.LastName, u.Email";

                var dp_params = new DynamicParameters();
                dp_params.Add("@StartDate", startDate, DbType.Date);
                dp_params.Add("@EndDate", endDate, DbType.Date);
                dp_params.Add("@OrganizationId", organizationId, DbType.Guid);

                var wages = await _dapperRepository.QueryAsync<dynamic>(
                    query,
                    dp_params,
                    CommandType.Text);

                foreach (var wage in wages)
                {
                    try
                    {
                        var email = BillingWageEmailTemplates.CreateWageGeneratedEmail(
                            (string)wage.ServiceProviderEmail,
                            (string)wage.ServiceProviderName,
                            (int)wage.WageId,
                            (DateTime)wage.WageDate,
                            (DateTime)wage.StartDate,
                            (DateTime)wage.EndDate,
                            (DateTime)wage.DueDate,
                            (decimal)wage.TotalAmount,
                            (int)wage.TaskCount,
                            (string)wage.Description
                        );

                        await _emailService.SendEmailAsync(email);
                        _logger.LogInformation($"Wage generation email sent to service provider for WageId: {wage.WageId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending wage generation email for WageId: {wage.WageId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending wage generation emails");
            }
        }
    }
}
