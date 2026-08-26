using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Helper;
using Scheduler.API.Models.Account.Transaction;
using Scheduler.API.Models.Payment;
using Scheduler.API.Services.Account.Transaction;
using Scheduler.API.Services.Account.BankAccount;
using Scheduler.API.Services.Payment;
using Scheduler.API.Services.Security;
using Scheduler.API.Common;
using Stripe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Scheduler.API.Common.Extensions;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PaymentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IPayment _payment;
        private readonly ICrypto _crypto;
        private readonly ITransaction _transaction;
        private readonly IStripeConnectedAccountService _stripeConnectedAccountService;
        private readonly IBankAccount _bankAccount;

        public PaymentController(
            IConfiguration configuration, 
            IPayment payment, 
            ICrypto crypto, 
            ITransaction transaction,
            IStripeConnectedAccountService stripeConnectedAccountService,
            IBankAccount bankAccount,
            ILogger<PaymentController> logger) : base(logger)
        {
            _configuration = configuration;
            // NOTE: This sets the DEFAULT Stripe key for organization operations
            // Organizations charging their clients/service providers should use their own
            // StripeConnectedAccountId from tblOrganization table (future enhancement)
            // Package invoice payments (SaaS charging organizations) use SaaSStripeService with separate key
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            _payment = payment;
            _crypto = crypto;
            _transaction = transaction;
            _stripeConnectedAccountService = stripeConnectedAccountService;
            _bankAccount = bankAccount; 
        }

        [HttpPost("wage")]
        public async Task<IActionResult> ProcessWagePayment()
        {
            try
            {
                // Fetch wage payment data from SP
                var paymentData = await _payment.GetPaymentData("WAGE");

                if (paymentData == null || !paymentData.Any())
                {
                    return Ok(new { success = true, message = "No wage payments to process" });
                }

                var processedCount = 0;
                var failedCount = 0;
                var errors = new List<string>();

                foreach (var data in paymentData)
                {
                    try
                    {
                        // Log the payment data being processed
                        Console.WriteLine($"Processing wage payment ID: {data.Id}, Amount: {data.Amount}, User: {data.UserId}");
                        Console.WriteLine($"Bank Account: {data.AccountHolderName} - {data.AccountNumber} - {data.BranchCode} - {data.IBAN}");
                        Console.WriteLine($"Currency:({data.CurrencySign}) - Code: {data.CurrencyCode}");

                        // Validate payment data
                        if (!ValidatePaymentData(data, "WAGE"))
                        {
                            var errorMsg = $"Invalid payment data for ID {data.Id}";
                            Console.WriteLine($"Validation failed: {errorMsg}");
                            errors.Add(errorMsg);
                            failedCount++;
                            continue;
                        }

                        // Validate that user has exactly one bank account
                        /*
                        if (!await ValidateUserBankAccount(data.UserId))
                        {
                            var errorMsg = $"User {data.UserId} does not have a valid bank account for wage payment ID {data.Id}";
                            Console.WriteLine($"Bank account validation failed: {errorMsg}");
                            errors.Add(errorMsg);
                            failedCount++;
                            continue;
                        }
                        */

                        // Process payment through Stripe (WAGE = transfer money to employee)
                        var paymentResult = await ProcessStripePayment(data, "WAGE");

                        if (paymentResult.Success)
                        {
                            // Update payment status in database
                            await _payment.UpdatePaymentStatus("WAGE", data.Id, paymentResult.ReferenceId);
                            Console.WriteLine($"Wage payment successful for ID {data.Id}: {paymentResult.ReferenceId}");
                            processedCount++;

                            var upsertTransactionInfoViewModel = new UpsertTransactionInfoViewModel
                            {
                                TransactionId = data.Row_Guid,
                                ReferenceId = paymentResult.ReferenceId,
                                TransactionDate = DateTime.Now,
                                UserId = data.UserId,
                                Remarks = "Transaction sucessfull",
                                StatusId = 1

                            };

                            _transaction.CreateUpdateTransactionAsync(upsertTransactionInfoViewModel);
                        }
                        else
                        {
                            var errorMsg = $"Payment failed for ID {data.Id}: {paymentResult.ErrorMessage}";
                            Console.WriteLine($"Payment failed: {errorMsg}");
                            errors.Add(errorMsg);
                            failedCount++;

                            var upsertTransactionInfoViewModel = new UpsertTransactionInfoViewModel
                            {
                                TransactionId = data.Row_Guid,
                                ReferenceId = paymentResult.ReferenceId,
                                TransactionDate = DateTime.Now,
                                UserId = data.UserId,
                                Remarks = paymentResult.ErrorMessage,
                                StatusId = 2

                            };

                            _transaction.CreateUpdateTransactionAsync(upsertTransactionInfoViewModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"Error processing payment ID {data.Id}: {ex.Message}";
                        Console.WriteLine($"Exception occurred: {errorMsg}");
                        errors.Add(errorMsg);
                        failedCount++;

                        var upsertTransactionInfoViewModel = new UpsertTransactionInfoViewModel
                        {
                            TransactionId = data.Row_Guid,
                            ReferenceId = "",
                            TransactionDate = DateTime.Now,
                            UserId = data.UserId,
                            Remarks = errorMsg,
                            StatusId = 2

                        };

                        _transaction.CreateUpdateTransactionAsync(upsertTransactionInfoViewModel);

                    }
                }

                return Ok(new
                {
                    success = true,
                    processedCount,
                    failedCount,
                    errors = errors.Any() ? errors : null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wage payment processing failed: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-cards")]
        public IActionResult GetTestCards()
        {
            return Execute(() => new
            {
                Visa = TestCardData.GetTestCard("visa"),
                Mastercard = TestCardData.GetTestCard("mastercard"),
                AmericanExpress = TestCardData.GetTestCard("amex"),
                Discover = TestCardData.GetTestCard("discover"),
                DeclinedCard = TestCardData.GetTestCard("declined"),
                InsufficientFunds = TestCardData.GetTestCard("insufficient"),
                ExpiredCard = TestCardData.GetTestCard("expired"),
                WrongCVC = TestCardData.GetTestCard("wrongcvc")
            }, "Test cards retrieved successfully");
        }

        [HttpPost("test-charge")]
        public async Task<IActionResult> TestCardCharge([FromBody] TestChargeRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.CardNumber) ||
                string.IsNullOrEmpty(request.CardHolderName) || string.IsNullOrEmpty(request.CVV))
            {
                return ValidationError("Invalid card data");
            }

            return await ExecuteAsync(async () =>
            {
                // Create a test charge of $1.00
                var testAmount = 1.00m;
                string stripeToken;

                // For test-charge endpoint, always use test tokens regardless of environment
                // This ensures consistent behavior between local and Azure deployments
                stripeToken = request.CardNumber.GetStripeTestToken();
                
                Console.WriteLine($"Test charge using token: {stripeToken} for card ending in: {request.CardNumber.Substring(Math.Max(0, request.CardNumber.Length - 4))}");

                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = (long)(testAmount * 100), // Convert to cents
                    Currency = "usd",
                    Source = stripeToken,
                    Description = "Card validation test charge ($1.00)",
                    Capture = false // Don't capture the charge, just authorize it
                };

                var chargeService = new ChargeService();
                var charge = await chargeService.CreateAsync(chargeOptions);
                
                Console.WriteLine($"Test charge created with status: {charge.Status}, ID: {charge.Id}");

                if (charge.Status == "succeeded")
                {
                    // Immediately void the authorization since this is just a test
                    var refundOptions = new RefundCreateOptions
                    {
                        Charge = charge.Id,
                        Amount = (long)(testAmount * 100),
                        Reason = "requested_by_customer"
                    };

                    var refundService = new RefundService();
                    var refund = await refundService.CreateAsync(refundOptions);

                    return new
                    {
                        success = true,
                        message = "Card validated successfully",
                        referenceId = charge.Id,
                        refundId = refund.Id
                    };
                }
                else
                {
                    throw new InvalidOperationException($"Card validation failed: {charge.FailureMessage ?? "Unknown error"}");
                }
            }, "Card validation completed");
        }

        [HttpGet("status/{paymentType}/{id}")]
        public async Task<IActionResult> GetPaymentStatus(string paymentType, int id)
        {
            if (string.IsNullOrEmpty(paymentType))
                return ValidationError("Payment type is required");

            if (id <= 0)
                return ValidationError("Valid payment ID is required");

            return await ExecuteAsync(async () =>
            {
                var paymentStatus = await _payment.GetPaymentStatus(paymentType, id);

                if (paymentStatus == null)
                    throw new InvalidOperationException("Payment status not found");

                return paymentStatus;
            }, "Payment status retrieved successfully");
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePayment([FromBody] PaymentValidationRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            if (string.IsNullOrEmpty(request.PaymentType))
                return ValidationError("Payment type is required");

            if (request.PaymentId <= 0)
                return ValidationError("Valid payment ID is required");

            return await ExecuteAsync(async () =>
            {
                var paymentData = await _payment.GetPaymentData(request.PaymentType);
                var targetPayment = paymentData?.FirstOrDefault(p => p.Id == request.PaymentId);

                if (targetPayment == null)
                    throw new InvalidOperationException("Payment not found");

                var isValid = ValidatePaymentData(targetPayment, request.PaymentType);
                var validationErrors = new List<string>();

                if (request.PaymentType == "INVOICE" &&
                    string.Equals(targetPayment.ChargeSource, "PayerManual", StringComparison.OrdinalIgnoreCase))
                {
                    isValid = false;
                    validationErrors.Add(
                        "This invoice bills an organization payer with no payment method on file; add a payer card or use manual collection.");
                }

                if (!isValid)
                {
                    if (targetPayment.Amount <= 0)
                        validationErrors.Add("Amount must be greater than zero");

                    if (request.PaymentType == "INVOICE" &&
                        !string.Equals(targetPayment.ChargeSource, "PayerManual", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(targetPayment.CardHolderName))
                            validationErrors.Add("Card holder name is required");
                        if (string.IsNullOrEmpty(targetPayment.CardNumber))
                            validationErrors.Add("Card number is required");
                        if (string.IsNullOrEmpty(targetPayment.CVV))
                            validationErrors.Add("CVV is required");
                        if (targetPayment.ExpiryMonth <= 0 || targetPayment.ExpiryMonth > 12)
                            validationErrors.Add("Invalid expiry month");
                        if (targetPayment.ExpiryYear < DateTime.Now.Year)
                            validationErrors.Add("Card has expired");
                    }
                    else if (request.PaymentType == "WAGE")
                    {
                        if (string.IsNullOrEmpty(targetPayment.AccountHolderName))
                            validationErrors.Add("Account holder name is required");
                        if (string.IsNullOrEmpty(targetPayment.AccountNumber))
                            validationErrors.Add("Account number is required");
                        if (string.IsNullOrEmpty(targetPayment.BranchCode))
                            validationErrors.Add("Branch code (routing number) is required");
                        if (string.IsNullOrEmpty(targetPayment.IBAN))
                            validationErrors.Add("IBAN is required");
                    }
                }

                return new PaymentValidationResponse
                {
                    IsValid = isValid,
                    ValidationErrors = validationErrors
                };
            }, "Payment validation completed");
        }

        [HttpPost("invoice")]
        public async Task<IActionResult> ProcessInvoiceDeduction()
        {
            try
            {
                // Fetch invoice deduction data from SP
                var paymentData = await _payment.GetPaymentData("INVOICE");

                if (paymentData == null || !paymentData.Any())
                {
                    return Ok(new { success = true, message = "No invoice payments to process" });
                }

                var processedCount = 0;
                var failedCount = 0;
                var errors = new List<string>();

                foreach (var data in paymentData)
                {
                    try
                    {
                        if (string.Equals(data.ChargeSource, "PayerManual", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation(
                                "Skipping auto-charge for invoice {InvoiceId}: organization payer has no card on file (manual collection).",
                                data.Id);
                            continue;
                        }

                        // Validate payment data
                        if (!ValidatePaymentData(data, "INVOICE"))
                        {
                            errors.Add($"Invalid payment data for ID {data.Id}");
                            failedCount++;
                            continue;
                        }

                        // Process payment through Stripe (INVOICE = charge money from customer)
                        var paymentResult = await ProcessStripePayment(data, "INVOICE");

                        if (paymentResult.Success)
                        {
                            // Update payment status in database
                            await _payment.UpdatePaymentStatus("INVOICE", data.Id, paymentResult.ReferenceId);
                            processedCount++;

                            var upsertTransactionInfoViewModel = new UpsertTransactionInfoViewModel
                            {
                                TransactionId = data.Row_Guid,
                                ReferenceId = paymentResult.ReferenceId,
                                TransactionDate = DateTime.Now,
                                UserId = data.UserId,
                                Remarks = paymentResult.ErrorMessage,
                                StatusId = 1

                            };

                            _transaction.CreateUpdateTransactionAsync(upsertTransactionInfoViewModel);
                        }
                        else
                        {
                            errors.Add($"Payment failed for ID {data.Id}: {paymentResult.ReferenceId}");
                            failedCount++;

                            var upsertTransactionInfoViewModel = new UpsertTransactionInfoViewModel
                            {
                                TransactionId = data.Row_Guid,
                                ReferenceId = paymentResult.ReferenceId,
                                TransactionDate = DateTime.Now,
                                UserId = data.UserId,
                                Remarks = paymentResult.ErrorMessage,
                                StatusId = 2

                            };

                            _transaction.CreateUpdateTransactionAsync(upsertTransactionInfoViewModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing payment ID {data.Id}: {ex.Message}");
                        failedCount++;
                        var upsertTransactionInfoViewModel = new UpsertTransactionInfoViewModel
                        {
                            TransactionId = data.Row_Guid,
                            ReferenceId = "",
                            TransactionDate = DateTime.Now,
                            UserId = data.UserId,
                            Remarks = $"Error processing payment ID {data.Id}: {ex.Message}",
                            StatusId = 2

                        };

                        _transaction.CreateUpdateTransactionAsync(upsertTransactionInfoViewModel);
                    }
                }

                return Ok(new
                {
                    success = true,
                    processedCount,
                    failedCount,
                    errors = errors.Any() ? errors : null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("payout-status/{payoutId}")]
        public async Task<IActionResult> GetPayoutStatus(string payoutId)
        {
            try
            {
                if (string.IsNullOrEmpty(payoutId))
                {
                    return BadRequest(new { success = false, message = "Payout ID is required" });
                }

                var result = await _stripeConnectedAccountService.GetPayoutStatusAsync(payoutId);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            Id = result.ReferenceId,
                            Status = result.ErrorMessage, // Status is stored in ErrorMessage for payout status
                            Message = "Payout status retrieved successfully"
                        }
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("retry-payout/{payoutId}")]
        public async Task<IActionResult> RetryFailedPayout(string payoutId)
        {
            try
            {
                if (string.IsNullOrEmpty(payoutId))
                {
                    return BadRequest(new { success = false, message = "Payout ID is required" });
                }

                var result = await _stripeConnectedAccountService.RetryFailedPayoutAsync(payoutId);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Payout retry initiated successfully",
                        originalPayoutId = payoutId,
                        newPayoutId = result.ReferenceId
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private bool ValidatePaymentData(PaymentData paymentData, string paymentType)
        {
            if (paymentData == null)
                return false;

            if (paymentData.Amount <= 0)
                return false;

            if (paymentType == "INVOICE")
            {
                if (string.Equals(paymentData.ChargeSource, "PayerManual", StringComparison.OrdinalIgnoreCase))
                    return false;

                // For invoice payments, we need card information
                if (string.IsNullOrEmpty(paymentData.CardHolderName) ||
                    string.IsNullOrEmpty(paymentData.CardNumber) ||
                    string.IsNullOrEmpty(paymentData.CVV) ||
                    paymentData.ExpiryMonth <= 0 || paymentData.ExpiryMonth > 12 ||
                    paymentData.ExpiryYear < DateTime.Now.Year)
                {
                    return false;
                }
            }
            else if (paymentType == "WAGE")
            {
                // For wage payments, we need bank account information
                if (string.IsNullOrEmpty(paymentData.AccountHolderName) ||
                    string.IsNullOrEmpty(paymentData.IBAN))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<PaymentResult> ProcessStripePayment(PaymentData paymentData, string paymentType)
        {
            if (paymentType == "INVOICE")
            {
                try
                {
                    // INVOICE: Charge money from customer/client (organization charging their customers)
                    // TODO: Get OrganizationId and use their StripeConnectedAccountId for this charge
                    // This ensures each organization uses their own Stripe account, not the SaaS admin account
                    // For now, uses default Stripe key until organization-specific accounts are configured
                    
                    // For testing, we'll use Stripe's test tokens instead of raw card data
                    string stripeToken;

                    // Check if we're in development/test mode and use test tokens
                    if (_configuration["Environment"] == "Development" || _configuration["Environment"] == "Test")
                    {
                        // Use Stripe test tokens based on card type
                        stripeToken = _crypto.Decrypt(paymentData.CardNumber!).GetStripeTestToken();// GetStripeTestToken(_crypto.Decrypt(paymentData.CardNumber!));
                    }
                    else
                    {
                        // For production, create token from encrypted card data using centralized service
                        try
                        {
                            var tokenRequest = new CardTokenRequest
                            {
                                CardHolderName = paymentData.CardHolderName,
                                CardNumber = _crypto.Decrypt(paymentData.CardNumber!),
                                ExpiryYear = paymentData.ExpiryYear.ToString(),
                                ExpiryMonth = paymentData.ExpiryMonth.ToString(),
                                Cvc = _crypto.Decrypt(paymentData.CVV!)
                            };

                            stripeToken = await _stripeConnectedAccountService.CreateStripeTokenAsync(tokenRequest);
                        }
                        catch (Exception ex)
                        {
                            return new PaymentResult
                            {
                                ReferenceId = string.Empty,
                                Success = false,
                                ErrorMessage = $"Token creation failed: {ex.Message}"
                            };
                        }
                    }

                    // Use StripeConnectedAccountService for the actual charge
                    var chargeRequest = new InvoiceChargeRequest
                    {
                        StripeToken = stripeToken,
                        Amount = paymentData.Amount,
                        Currency = paymentData.CurrencyCode?.ToLower() ?? "usd",
                        Description = $"Invoice payment for ID: {paymentData.Id}",
                        CustomerEmail = paymentData.Email,
                        CustomerName = paymentData.CardHolderName,
                        Metadata = new Dictionary<string, string>
                        {
                            {"payment_type", "invoice"},
                            {"payment_id", paymentData.Id.ToString()},
                            {"user_id", paymentData.UserId.ToString()},
                            {"charge_source", paymentData.ChargeSource ?? "ClientCard"}
                        }
                    };

                    var paymentResult = await _stripeConnectedAccountService.CreateChargeForInvoiceAsync(chargeRequest);
                    return paymentResult;
                }
                catch (Exception ex)
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = $"Invoice payment failed: {ex.Message}"
                    };
                }
            }
            else if (paymentType == "WAGE")
            {
                // WAGE: Send money directly to employee's bank account using Direct Payouts
                try
                {
                    // Validate required bank account information
                    if (string.IsNullOrEmpty(paymentData.AccountHolderName) ||
                        string.IsNullOrEmpty(paymentData.AccountNumber) ||
                        string.IsNullOrEmpty(paymentData.BranchCode) ||
                        string.IsNullOrEmpty(paymentData.IBAN))
                    {
                        return new PaymentResult
                        {
                            ReferenceId = string.Empty,
                            Success = false,
                            ErrorMessage = "Bank account information is required for wage payments"
                        };
                    }

                    // Create direct payout request
                    var directPayoutRequest = new DirectPayoutRequest
                    {
                        Amount = paymentData.Amount,
                        Currency = paymentData.CurrencyCode?.ToLower() ?? "usd",
                        Country = paymentData.IBAN.GetCountryFromIBAN(), // Derive country from IBAN
                        AccountNumber = _crypto.Decrypt(paymentData.AccountNumber!),
                        RoutingNumber = paymentData.BranchCode,
                        AccountHolderName = paymentData.AccountHolderName,
                        AccountHolderType = "individual",
                        Description = $"Wage payment for ID: {paymentData.Id}",
                        StatementDescriptor = "WAGE PAYMENT",
                        Method = "standard"
                    };

                    // Create direct payout
                    var payoutResult = await _stripeConnectedAccountService.CreateDirectPayoutAsync(directPayoutRequest);

                    if (payoutResult.Success)
                    {
                        return new PaymentResult
                        {
                            ReferenceId = payoutResult.ReferenceId,
                            Success = true,
                            ErrorMessage = null
                        };
                    }
                    else
                    {
                        return new PaymentResult
                        {
                            ReferenceId = string.Empty,
                            Success = false,
                            ErrorMessage = $"Direct payout failed: {payoutResult.ErrorMessage}"
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new PaymentResult
                    {
                        ReferenceId = string.Empty,
                        Success = false,
                        ErrorMessage = $"Wage payment failed: {ex.Message}"
                    };
                }
            }

            return new PaymentResult
            {
                ReferenceId = string.Empty,
                Success = false
            };
        }


        //private string GetStripeTestToken(string cardNumber)
        //{
        //    // Stripe test tokens for different card scenarios
        //    // These are safe to use in test mode
        //    var lastFourDigits = cardNumber?.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : "0000";

        //    switch (lastFourDigits)
        //    {
        //        case "0000": // Generic success
        //            return "tok_visa";
        //        case "1111": // Visa success
        //            return "tok_visa";
        //        case "2222": // Visa debit success
        //            return "tok_visa_debit";
        //        case "3333": // Mastercard success
        //            return "tok_mastercard";
        //        case "4444": // American Express success
        //            return "tok_amex";
        //        case "5555": // Discover success
        //            return "tok_discover";
        //        case "6666": // Declined card
        //            return "tok_chargeDeclined";
        //        case "7777": // Insufficient funds
        //            return "tok_chargeDeclinedInsufficientFunds";
        //        case "8888": // Expired card
        //            return "tok_chargeDeclinedExpiredCard";
        //        case "9999": // Incorrect CVC
        //            return "tok_chargeDeclinedIncorrectCvc";
        //        default:
        //            return "tok_visa"; // Default to successful Visa
        //    }
        //}




        [HttpGet("charge-status/{chargeId}")]
        public async Task<IActionResult> GetChargeStatus(string chargeId)
        {
            try
            {
                if (string.IsNullOrEmpty(chargeId))
                {
                    return BadRequest(new { success = false, message = "Charge ID is required" });
                }

                var result = await _stripeConnectedAccountService.GetChargeStatusAsync(chargeId);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            Id = result.ReferenceId,
                            Status = result.ErrorMessage, // Status is stored in ErrorMessage for charge status
                            Message = "Charge status retrieved successfully"
                        }
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("refund-charge/{chargeId}")]
        public async Task<IActionResult> RefundCharge(string chargeId, [FromBody] RefundChargeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(chargeId))
                {
                    return BadRequest(new { success = false, message = "Charge ID is required" });
                }

                var result = await _stripeConnectedAccountService.RefundChargeAsync(chargeId, request.Amount);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Charge refunded successfully",
                        refundId = result.ReferenceId
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("direct-payout")]
        public async Task<IActionResult> CreateDirectPayout([FromBody] DirectPayoutRequest request)
        {
            try
            {
                if (request == null || request.Amount <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid payout request" });
                }

                var result = await _stripeConnectedAccountService.CreateDirectPayoutAsync(request);
                
                if (result.Success)
                {
                    return Ok(new { success = true, payoutId = result.ReferenceId, message = "Payout created successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("create-stripe-token")]
        public async Task<IActionResult> CreateStripeToken([FromBody] CreateStripeTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CardNumber) ||
                    string.IsNullOrEmpty(request.ExpiryMonth) ||
                    string.IsNullOrEmpty(request.ExpiryYear) ||
                    string.IsNullOrEmpty(request.Cvc))
                {
                    return BadRequest(new { success = false, message = "All card details are required" });
                }

                var tokenRequest = new CardTokenRequest
                {
                    CardHolderName = request.CardHolderName,
                    CardNumber = request.CardNumber,
                    ExpiryYear = request.ExpiryYear,
                    ExpiryMonth = request.ExpiryMonth,
                    Cvc = request.Cvc
                };

                var stripeToken = await _stripeConnectedAccountService.CreateStripeTokenAsync(tokenRequest);

                return Ok(new
                {
                    success = true,
                    message = "Stripe token created successfully",
                    token = stripeToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("manual-mark-as-paid")]
        public async Task<IActionResult> ManualMarkAsPaid([FromBody] ManualMarkAsPaidRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            if (string.IsNullOrEmpty(request.PaymentType))
                return ValidationError("Payment type is required");

            if (request.PaymentId <= 0)
                return ValidationError("Valid payment ID is required");

            if (string.IsNullOrWhiteSpace(request.Reason))
                return ValidationError("Reason for manual payment is required");

            return await ExecuteAsync(async () =>
            {
                // Verify payment exists and is not already paid
                var paymentStatus = await _payment.GetPaymentStatus(request.PaymentType, request.PaymentId);
                
                if (paymentStatus == null)
                    throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found");

                if (paymentStatus.Status?.ToLower() == "paid" || paymentStatus.ProcessedDate.HasValue)
                    throw new InvalidOperationException("This payment has already been marked as paid");

                // Mark payment as paid manually
                var affectedRows = await _payment.ManualMarkAsPaid(
                    request.PaymentType,
                    request.PaymentId,
                    request.Reason,
                    request.PaymentDate ?? DateTime.UtcNow
                );

                if (affectedRows <= 0)
                    throw new InvalidOperationException("Failed to mark payment as paid. The payment may have already been processed.");

                return new ManualMarkAsPaidResponse
                {
                    Success = true,
                    Message = $"Payment ID {request.PaymentId} has been manually marked as paid",
                    AffectedRows = affectedRows
                };
            }, "Payment manually marked as paid successfully");
        }

        /// <summary>
        /// Updates the user's bank account with the Stripe Connected Account ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="connectedAccountId">The Stripe Connected Account ID</param>
        /// <returns>True if successful, false otherwise</returns>
        private async Task<bool> UpdateBankAccountConnectedAccountId(Guid userId, string connectedAccountId)
        {
            try
            {
                // Use the injected IBankAccount service to update the ConnectedAccountId
                var result = await _bankAccount.UpdateConnectedAccountIdAsync(userId, connectedAccountId);

                if (result)
                {
                    Console.WriteLine($"Successfully updated bank account for user {userId} with ConnectedAccountId: {connectedAccountId}");
                }
                else
                {
                    Console.WriteLine($"Failed to update bank account for user {userId} with ConnectedAccountId: {connectedAccountId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating bank account ConnectedAccountId: {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// Validates that a user has exactly one bank account
        /// </summary>
        /*
        private async Task<bool> ValidateUserBankAccount(Guid userId)
        {
            try
            {
                var hasBankAccount = await _bankAccount.UserHasBankAccountAsync(userId);
                
                if (!hasBankAccount)
                {
                    Console.WriteLine($"User {userId} does not have a bank account");
                    return false;
                }
                
                // Get the bank account to ensure it's valid
                var bankAccount = await _bankAccount.GetBankAccountAsync(userId);
                
                if (bankAccount == null)
                {
                    Console.WriteLine($"User {userId} bank account is null");
                    return false;
                }
                
                // Validate required fields
                if (string.IsNullOrEmpty(bankAccount.AccountHolderName) ||
                    string.IsNullOrEmpty(bankAccount.AccountNumber) ||
                    string.IsNullOrEmpty(bankAccount.IBAN))
                {
                    Console.WriteLine($"User {userId} bank account is missing required information");
                    return false;
                }
                
                Console.WriteLine($"User {userId} has valid bank account: {bankAccount.AccountHolderName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating user bank account: {ex.Message}");
                return false;
            }
        }
        */
    }



    public class RefundChargeRequest
    {
        public decimal? Amount { get; set; } // If null, refunds the full amount
    }

    public class CreateStripeTokenRequest
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvc { get; set; }
    }
}