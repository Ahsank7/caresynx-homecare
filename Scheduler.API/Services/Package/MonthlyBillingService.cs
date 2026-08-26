using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.API.Services.Package;
using Scheduler.API.Services;
using Scheduler.API.Models.Package;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace Scheduler.API.Services.Package
{
    public class MonthlyBillingService : BackgroundService
    {
        private readonly ILogger<MonthlyBillingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

        public MonthlyBillingService(ILogger<MonthlyBillingService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monthly Billing Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var lastDayOfMonth = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
                    
                    // Check if it's the last day of the month and after 11 PM
                    if (now.Date == lastDayOfMonth.Date && now.Hour >= 23)
                    {
                        _logger.LogInformation("Last day of month detected. Generating monthly invoices...");
                        
                        // Generate invoices for current month
                        await GenerateMonthlyInvoices(now.Month, now.Year);
                        
                        // Process payments for pending invoices
                        await ProcessPendingPayments();
                        
                        // Wait until next month to avoid duplicate runs
                        var nextMonth = now.AddMonths(1);
                        var nextCheck = new DateTime(nextMonth.Year, nextMonth.Month, 1, 23, 0, 0);
                        var delay = nextCheck - now;
                        
                        if (delay.TotalMilliseconds > 0)
                        {
                            _logger.LogInformation($"Next billing check scheduled for {nextCheck}");
                            await Task.Delay(delay, stoppingToken);
                        }
                    }
                    else
                    {
                        // Wait for the next check interval
                        await Task.Delay(_checkInterval, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Monthly Billing Service");
                    // Wait before retrying
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            _logger.LogInformation("Monthly Billing Service stopped");
        }

        private async Task GenerateMonthlyInvoices(int month, int year)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var packageService = scope.ServiceProvider.GetRequiredService<IPackage>();

                var request = new GenerateMonthlyInvoicesRequest
                {
                    BillingMonth = month,
                    BillingYear = year,
                    OrganizationId = null // Generate for all organizations
                };

                var count = await packageService.GenerateMonthlyInvoicesAsync(request);
                _logger.LogInformation($"Generated {count} monthly invoices for {month}/{year}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating monthly invoices for {month}/{year}");
            }
        }

        private async Task ProcessPendingPayments()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var packageService = scope.ServiceProvider.GetRequiredService<IPackage>();
                var dapperRepository = scope.ServiceProvider.GetRequiredService<IDapperRepository>();

                // Get all pending invoices
                var pendingInvoices = await dapperRepository.QueryAsync<dynamic>(
                    "SELECT Id, OrganizationId FROM [dbo].[tblPackageInvoice] WHERE PaymentStatus = 'Pending' OR PaymentStatus = 'Failed'",
                    new DynamicParameters(),
                    CommandType.Text);

                int processedCount = 0;
                int failedCount = 0;

                foreach (var invoice in pendingInvoices)
                {
                    try
                    {
                        var result = await packageService.ProcessInvoicePaymentAsync(new ProcessPackageInvoicePaymentRequest
                        {
                            InvoiceId = invoice.Id,
                            OrganizationId = invoice.OrganizationId
                        });

                        if (result)
                        {
                            processedCount++;
                            _logger.LogInformation($"Successfully processed payment for invoice {invoice.Id}");
                        }
                        else
                        {
                            failedCount++;
                            _logger.LogWarning($"Failed to process payment for invoice {invoice.Id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        _logger.LogError(ex, $"Error processing payment for invoice {invoice.Id}");
                    }
                }

                _logger.LogInformation($"Payment processing completed. Processed: {processedCount}, Failed: {failedCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending payments");
            }
        }
    }
}

