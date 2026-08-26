using Scheduler.API.Common.Extensions;
using Scheduler.API.Models.Payment;
using Scheduler.API.Services.Security;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Scheduler.API.Services.Payment
{
    public class StripeConnectedAccountService : IStripeConnectedAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripeConnectedAccountService> _logger;
        private readonly string _stripeSecretKey;

        public StripeConnectedAccountService(IConfiguration configuration, ILogger<StripeConnectedAccountService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            // This service uses organization-level Stripe operations
            // Organizations use this service to charge their clients/service providers
            // Package invoice payments (SaaS admin charging organizations) use SaaSStripeService
            _stripeSecretKey = _configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<ConnectedAccountResult> CreateConnectedAccountAsync(ConnectedAccountRequest request)
        {
            try
            {
                var options = new AccountCreateOptions
                {
                    Type = "express",   // can also be "custom"
                    Country = request.Country ?? "US",
                    Email = request.Email,
                    BusinessType = request.BusinessType ?? "individual",
                    Company = new AccountCompanyOptions
                    {
                        Name = request.CompanyName
                    },
                    Individual = new AccountIndividualOptions
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Phone = request.Phone,
                        Address = new AddressOptions
                        {
                            Line1 = request.Address,
                            City = request.City,
                            State = request.State,
                            PostalCode = request.PostalCode,
                            Country = request.Country ?? "US"
                        }
                    },
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                    },
                };

                var service = new AccountService();
                var account = await service.CreateAsync(options);

                // Create account link for onboarding
                var accountLinkOptions = new AccountLinkCreateOptions
                {
                    Account = account.Id,
                    RefreshUrl = $"{_configuration["AppSettings:BaseUrl"] ?? "https://localhost:7094"}/stripe/refresh",
                    ReturnUrl = $"{_configuration["AppSettings:BaseUrl"] ?? "https://localhost:7094"}/stripe/return",
                    Type = "account_onboarding"
                };

                var accountLinkService = new AccountLinkService();
                var accountLink = await accountLinkService.CreateAsync(accountLinkOptions);

                return new ConnectedAccountResult
                {
                    Success = true,
                    ConnectedAccountId = account.Id,
                    AccountLink = accountLink.Url,
                    Status = account.ChargesEnabled ? "active" : "pending"
                };
            }
            catch (StripeException ex)
            {
                return new ConnectedAccountResult
                {
                    Success = false,
                    ErrorMessage = $"Stripe error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ConnectedAccountResult
                {
                    Success = false,
                    ErrorMessage = $"Error creating connected account: {ex.Message}"
                };
            }
        }

        public async Task<ConnectedAccountResult> GetConnectedAccountAsync(string connectedAccountId)
        {
            try
            {
                var service = new AccountService();
                var account = await service.GetAsync(connectedAccountId);

                return new ConnectedAccountResult
                {
                    Success = true,
                    ConnectedAccountId = account.Id,
                    Status = account.ChargesEnabled ? "active" : "pending"
                };
            }
            catch (StripeException ex)
            {
                return new ConnectedAccountResult
                {
                    Success = false,
                    ErrorMessage = $"Stripe error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ConnectedAccountResult
                {
                    Success = false,
                    ErrorMessage = $"Error retrieving connected account: {ex.Message}"
                };
            }
        }

        public async Task<bool> UpdateConnectedAccountAsync(string connectedAccountId, ConnectedAccountUpdateRequest request)
        {
            try
            {
                var options = new AccountUpdateOptions
                {
                    Email = request.Email,
                    Company = new AccountCompanyOptions
                    {
                        Name = request.CompanyName
                    },
                    Individual = new AccountIndividualOptions
                    {
                        Phone = request.Phone,
                        Address = new AddressOptions
                        {
                            Line1 = request.Address,
                            City = request.City,
                            State = request.State,
                            PostalCode = request.PostalCode
                        }
                    }
                };

                var service = new AccountService();
                await service.UpdateAsync(connectedAccountId, options);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteConnectedAccountAsync(string connectedAccountId)
        {
            try
            {
                var service = new AccountService();
                await service.DeleteAsync(connectedAccountId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<AccountLinkResult> CreateAccountLinkAsync(string connectedAccountId)
        {
            try
            {
                var accountLinkOptions = new AccountLinkCreateOptions
                {
                    Account = connectedAccountId,
                    RefreshUrl = $"{_configuration["AppSettings:BaseUrl"] ?? "https://localhost:7094"}/stripe/refresh",
                    ReturnUrl = $"{_configuration["AppSettings:BaseUrl"] ?? "https://localhost:7094"}/stripe/return",
                    Type = "account_onboarding"
                };

                var accountLinkService = new AccountLinkService();
                var accountLink = await accountLinkService.CreateAsync(accountLinkOptions);

                return new AccountLinkResult
                {
                    Success = true,
                    AccountLink = accountLink.Url,
                    ExpiresAt = accountLink.ExpiresAt,
                    ErrorMessage = null
                };
            }
            catch (StripeException ex)
            {
                return new AccountLinkResult
                {
                    Success = false,
                    AccountLink = string.Empty,
                    ErrorMessage = ex.Message
                };
            }
        }

        // Direct payout methods
        public async Task<PaymentResult> CreateDirectPayoutAsync(DirectPayoutRequest request)
        {
            try
            {
                // Create bank account token
                var tokenOptions = new TokenCreateOptions
                {
                    BankAccount = new TokenBankAccountOptions
                    {
                        Country = request.Country,
                        Currency = request.Currency,
                        AccountNumber = request.AccountNumber,
                        RoutingNumber = request.RoutingNumber,
                        AccountHolderName = request.AccountHolderName,
                        AccountHolderType = request.AccountHolderType
                    }
                };

                var tokenService = new TokenService();
                var token = await tokenService.CreateAsync(tokenOptions);

                // Create payout using the token
                var payoutOptions = new PayoutCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = request.Currency.ToLower(),
                    Method = request.Method ?? "standard", // "standard" or "instant"
                    Description = request.Description,
                    StatementDescriptor = request.StatementDescriptor ?? "PAYMENT"
                };

                var payoutService = new PayoutService();
                var payout = await payoutService.CreateAsync(payoutOptions);

                return new PaymentResult
                {
                    ReferenceId = payout.Id,
                    Success = true,
                    ErrorMessage = null
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Direct payout failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating direct payout: {ex.Message}"
                };
            }
        }

        // Payout methods
        public async Task<PaymentResult> CreatePayoutToConnectedAccountAsync(PayoutRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ConnectedAccountId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Connected account ID is required for payout"
                    };
                }

                var payoutService = new PayoutService();
                var payoutOptions = new PayoutCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = request.Currency?.ToLower() ?? "usd",
                    Method = "standard", // Use standard method for reliability
                    Description = request.Description,
                    StatementDescriptor = request.StatementDescriptor ?? "WAGE PAYMENT"
                };

                var payout = await payoutService.CreateAsync(payoutOptions, new RequestOptions
                {
                    StripeAccount = request.ConnectedAccountId // Use the connected account
                });

                return new PaymentResult
                {
                    ReferenceId = payout.Id,
                    Success = true,
                    ErrorMessage = null
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Payout failed with connected account: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating payout: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResult> GetPayoutStatusAsync(string payoutId)
        {
            try
            {
                if (string.IsNullOrEmpty(payoutId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Payout ID is required"
                    };
                }

                var payoutService = new PayoutService();
                var payout = await payoutService.GetAsync(payoutId);

                return new PaymentResult
                {
                    ReferenceId = payout.Id,
                    Success = true,
                    ErrorMessage = payout.Status
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrieving payout status: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrieving payout status: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResult> RetryFailedPayoutAsync(string payoutId)
        {
            try
            {
                if (string.IsNullOrEmpty(payoutId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Payout ID is required"
                    };
                }

                var payoutService = new PayoutService();
                var originalPayout = await payoutService.GetAsync(payoutId);

                if (originalPayout.Status != "failed")
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Only failed payouts can be retried"
                    };
                }

                // Create a new payout with the same details
                var retryOptions = new PayoutCreateOptions
                {
                    Amount = originalPayout.Amount,
                    Currency = originalPayout.Currency,
                    Method = originalPayout.Method ?? "standard",
                    Description = $"Retry: {originalPayout.Description}",
                    Metadata = new Dictionary<string, string>
                    {
                        {"payment_type", "wage_retry"},
                        {"original_payout", payoutId},
                        {"retry_count", "1"}
                    }
                };

                var retryPayout = await payoutService.CreateAsync(retryOptions);

                return new PaymentResult
                {
                    ReferenceId = retryPayout.Id,
                    Success = true,
                    ErrorMessage = null
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Failed to retry payout: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrying payout: {ex.Message}"
                };
            }
        }

        // Invoice charging methods
        public async Task<PaymentResult> CreateChargeForInvoiceAsync(InvoiceChargeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.StripeToken))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Stripe token is required for invoice charge"
                    };
                }

                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = request.Currency?.ToLower() ?? "usd",
                    Source = request.StripeToken,
                    Description = request.Description,
                    ReceiptEmail = request.CustomerEmail,
                    Metadata = request.Metadata ?? new Dictionary<string, string>()
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions);

                return new PaymentResult
                {
                    ReferenceId = charge.Id,
                    Success = charge.Status == "succeeded",
                    ErrorMessage = charge.Status
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Invoice charge failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating invoice charge: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResult> GetChargeStatusAsync(string chargeId)
        {
            try
            {
                if (string.IsNullOrEmpty(chargeId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Charge ID is required"
                    };
                }

                var chargeService = new ChargeService();
                var charge = await chargeService.GetAsync(chargeId);

                return new PaymentResult
                {
                    ReferenceId = charge.Id,
                    Success = charge.Status == "succeeded",
                    ErrorMessage = charge.Status
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrieving charge status: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error retrieving charge status: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResult> RefundChargeAsync(string chargeId, decimal? amount = null)
        {
            try
            {
                if (string.IsNullOrEmpty(chargeId))
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = "Charge ID is required"
                    };
                }

                var refundOptions = new RefundCreateOptions
                {
                    Charge = chargeId
                };

                if (amount.HasValue)
                {
                    refundOptions.Amount = (long)(amount.Value * 100); // Convert to cents
                }

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions);

                return new PaymentResult
                {
                    ReferenceId = refund.Id,
                    Success = refund.Status == "succeeded",
                    ErrorMessage = refund.Status
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating refund: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error creating refund: {ex.Message}"
                };
            }
        }

        // Card validation with test charge and refund
        public async Task<PaymentResult> ValidateCardWithTestChargeAsync(CardTokenRequest request)
        {
            try
            {
                // Create token from card details
                var token = await CreateStripeTokenAsync(request);

                // Create a $1.00 test charge
                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = 100, // $1.00 in cents
                    Currency = "usd",
                    Source = token,
                    Description = "Card validation test charge - will be refunded immediately",
                    Metadata = new Dictionary<string, string>
                    {
                        { "validation", "true" },
                        { "auto_refund", "true" }
                    }
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions);

                if (charge.Status != "succeeded")
                {
                    return new PaymentResult
                    {
                        ReferenceId = charge.Id,
                        Success = false,
                        ErrorMessage = "Card validation failed: Charge was not successful"
                    };
                }

                // Immediately refund the test charge
                var refundOptions = new RefundCreateOptions
                {
                    Charge = charge.Id,
                    Reason = RefundReasons.RequestedByCustomer
                };

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions);

                if (refund.Status != "succeeded")
                {
                    _logger.LogWarning($"Card validation charge succeeded but refund failed. Charge ID: {charge.Id}, Refund ID: {refund.Id}");
                }

                return new PaymentResult
                {
                    ReferenceId = charge.Id,
                    Success = true,
                    ErrorMessage = null
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Card validation failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    ReferenceId = string.Empty,
                    Success = false,
                    ErrorMessage = $"Error validating card: {ex.Message}"
                };
            }
        }

        // Token creation methods
        public async Task<string> CreateStripeTokenAsync(CardTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CardNumber) ||
                    string.IsNullOrEmpty(request.ExpiryMonth) ||
                    string.IsNullOrEmpty(request.ExpiryYear) ||
                    string.IsNullOrEmpty(request.Cvc))
                {
                    throw new ArgumentException("All card details are required for token creation");
                }

                var options = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Name = request.CardHolderName,
                        Number = request.CardNumber,
                        ExpYear = request.ExpiryYear,
                        ExpMonth = request.ExpiryMonth,
                        Cvc = request.Cvc
                    },
                };

                
                // Check if we're in development/test mode and use test tokens
                if (_configuration["Environment"] == "Development" || _configuration["Environment"] == "Test")
                {
                    // Use Stripe test tokens based on card type
                    return request.CardNumber.GetStripeTestToken();
                }
                else
                {
                    // For production, create token from encrypted card data using centralized service

                    var service = new TokenService();
                    var token = await service.CreateAsync(options);
                    return token.Id;
                }
            }
            catch (StripeException ex)
            {
                throw new Exception($"Stripe error creating token: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating Stripe token: {ex.Message}");
            }
        }

    }
}
