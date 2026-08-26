using System.Net;
using System.Net.Mail;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduler.API.Models.Email;

namespace Scheduler.API.Services.Email
{
    public class EmailService : IEmailService
    {
        private const string ProviderAwsSes = "AwsSes";
        private const string ProviderSmtp = "Smtp";

        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _settings;
        private readonly string _provider;
        private readonly AmazonSimpleEmailServiceClient? _sesClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;

            var configuredProvider = configuration["Email:Provider"] ?? ProviderAwsSes;
            _provider = NormalizeProvider(configuredProvider);

            _settings = new EmailSettings
            {
                Provider = _provider,
                Region = configuration["Email:Region"] ?? "us-east-1",
                FromEmail = configuration["Email:FromEmail"] ?? string.Empty,
                FromDisplayName = configuration["Email:FromDisplayName"] ?? "caresynX",
                SmtpServer = configuration["Email:Smtp:Host"]
                    ?? configuration["Email:Smtp:Server"]
                    ?? configuration["Email:SmtpServer"]
                    ?? string.Empty,
                SmtpPort = configuration.GetValue<int?>("Email:Smtp:Port")
                    ?? configuration.GetValue<int?>("Email:SmtpPort")
                    ?? 587,
                SmtpUsername = configuration["Email:Smtp:UserName"]
                    ?? configuration["Email:Smtp:Username"]
                    ?? configuration["Email:SmtpUsername"]
                    ?? string.Empty,
                SmtpPassword = configuration["Email:Smtp:Password"]
                    ?? configuration["Email:SmtpPassword"]
                    ?? string.Empty,
                EnableSsl = configuration.GetValue<bool?>("Email:Smtp:EnableSsl")
                    ?? configuration.GetValue<bool?>("Email:EnableSsl")
                    ?? true,
                UseDefaultCredentials = configuration.GetValue<bool?>("Email:Smtp:UseDefaultCredentials")
                    ?? configuration.GetValue<bool?>("Email:UseDefaultCredentials")
                    ?? false,
                Timeout = configuration.GetValue<int?>("Email:Smtp:Timeout")
                    ?? configuration.GetValue<int?>("Email:Timeout")
                    ?? 30000
            };

            if (!IsKnownProvider(configuredProvider))
            {
                _logger.LogWarning(
                    "Unsupported email provider {Provider}. Falling back to AWS SES.",
                    configuredProvider);
            }

            if (_provider == ProviderAwsSes)
            {
                _sesClient = CreateSesClient(configuration, _settings.Region);
                _logger.LogInformation("AWS SES email provider initialized for region {Region}", _settings.Region);
            }
            else
            {
                _logger.LogInformation(
                    "SMTP email provider initialized for host {Host}:{Port}",
                    string.IsNullOrWhiteSpace(_settings.SmtpServer) ? "(not configured)" : _settings.SmtpServer,
                    _settings.SmtpPort);
            }
        }

        public Task<bool> SendEmailAsync(EmailMessage message)
        {
            return _provider == ProviderSmtp
                ? SendSmtpEmailAsync(message)
                : SendSesEmailAsync(message);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var message = new EmailMessage
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = isHtml
            };

            return await SendEmailAsync(message);
        }

        public async Task<bool> SendBulkEmailAsync(List<EmailMessage> messages)
        {
            int successCount = 0;
            int failureCount = 0;

            foreach (var message in messages)
            {
                var result = await SendEmailAsync(message);
                if (result)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                }
            }

            _logger.LogInformation(
                "Bulk email send completed. Success: {Success}, Failed: {Failed}",
                successCount,
                failureCount);

            return failureCount == 0;
        }

        public async Task<bool> SendEmailWithAttachmentAsync(EmailMessage message, List<EmailAttachment> attachments)
        {
            message.Attachments = attachments;
            return await SendEmailAsync(message);
        }

        private async Task<bool> SendSmtpEmailAsync(EmailMessage message)
        {
            try
            {
                var toAddresses = SplitAddresses(message.To);
                if (toAddresses.Count == 0)
                {
                    _logger.LogWarning("Email recipient is required. Skipping email send.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
                {
                    _logger.LogWarning("SMTP host is not configured. Set Email:Smtp:Host before sending email.");
                    return false;
                }

                if (!TryResolveSender(message, out var fromEmail, out var fromDisplay, out _))
                {
                    return false;
                }

                using var mailMessage = new MailMessage
                {
                    From = string.IsNullOrWhiteSpace(fromDisplay)
                        ? new MailAddress(fromEmail)
                        : new MailAddress(fromEmail, fromDisplay),
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = message.IsHtml
                };

                foreach (var address in toAddresses)
                {
                    mailMessage.To.Add(address);
                }

                foreach (var address in SplitAddresses(message.Cc))
                {
                    mailMessage.CC.Add(address);
                }

                foreach (var address in SplitAddresses(message.Bcc))
                {
                    mailMessage.Bcc.Add(address);
                }

                if (message.Attachments != null)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        var stream = new MemoryStream(attachment.Content);
                        mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                    }
                }

                using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
                {
                    EnableSsl = _settings.EnableSsl,
                    Timeout = _settings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = _settings.UseDefaultCredentials
                };

                if (!_settings.UseDefaultCredentials && !string.IsNullOrWhiteSpace(_settings.SmtpUsername))
                {
                    smtpClient.Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword);
                }

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {To} using SMTP.", message.To);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMTP email to {To}: {Message}", message.To, ex.Message);
                return false;
            }
        }

        private async Task<bool> SendSesEmailAsync(EmailMessage message)
        {
            try
            {
                if (_sesClient == null)
                {
                    _logger.LogWarning("AWS SES email client is not initialized. Skipping email send.");
                    return false;
                }

                var toAddresses = SplitAddresses(message.To);
                if (toAddresses.Count == 0)
                {
                    _logger.LogWarning("Email recipient is required. Skipping email send.");
                    return false;
                }

                if (!TryResolveSender(message, out _, out _, out var senderAddress))
                {
                    return false;
                }

                var destination = new Destination
                {
                    ToAddresses = toAddresses.ToList(),
                    CcAddresses = SplitAddresses(message.Cc).ToList(),
                    BccAddresses = SplitAddresses(message.Bcc).ToList()
                };

                var body = new Body();
                if (message.IsHtml)
                {
                    body.Html = new Content { Charset = "UTF-8", Data = message.Body };
                }
                else
                {
                    body.Text = new Content { Charset = "UTF-8", Data = message.Body };
                }

                if (message.Attachments != null && message.Attachments.Any())
                {
                    return await SendRawSesEmailWithAttachmentsAsync(senderAddress, message, destination);
                }

                var request = new SendEmailRequest
                {
                    Source = senderAddress,
                    Destination = destination,
                    Message = new Amazon.SimpleEmail.Model.Message
                    {
                        Subject = new Content { Charset = "UTF-8", Data = message.Subject },
                        Body = body
                    }
                };

                var response = await _sesClient.SendEmailAsync(request);
                _logger.LogInformation(
                    "Email sent successfully to {To} using AWS SES. MessageId: {MessageId}",
                    message.To,
                    response.MessageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending AWS SES email to {To}: {Message}", message.To, ex.Message);
                return false;
            }
        }

        private async Task<bool> SendRawSesEmailWithAttachmentsAsync(
            string senderAddress,
            EmailMessage message,
            Destination destination)
        {
            try
            {
                if (_sesClient == null)
                {
                    _logger.LogWarning("AWS SES email client is not initialized. Skipping raw email send.");
                    return false;
                }

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream);

                var boundary = $"----=_Part_{Guid.NewGuid():N}";

                writer.WriteLine($"From: {senderAddress}");
                writer.WriteLine($"To: {message.To}");
                if (!string.IsNullOrWhiteSpace(message.Cc))
                {
                    writer.WriteLine($"Cc: {message.Cc}");
                }

                writer.WriteLine($"Subject: {message.Subject}");
                writer.WriteLine("MIME-Version: 1.0");
                writer.WriteLine($"Content-Type: multipart/mixed; boundary=\"{boundary}\"");
                writer.WriteLine();

                writer.WriteLine($"--{boundary}");
                writer.WriteLine($"Content-Type: {(message.IsHtml ? "text/html" : "text/plain")}; charset=UTF-8");
                writer.WriteLine("Content-Transfer-Encoding: 7bit");
                writer.WriteLine();
                writer.WriteLine(message.Body);

                foreach (var attachment in message.Attachments!)
                {
                    writer.WriteLine($"--{boundary}");
                    writer.WriteLine($"Content-Type: {attachment.ContentType}; name=\"{attachment.FileName}\"");
                    writer.WriteLine("Content-Transfer-Encoding: base64");
                    writer.WriteLine($"Content-Disposition: attachment; filename=\"{attachment.FileName}\"");
                    writer.WriteLine();
                    writer.WriteLine(Convert.ToBase64String(attachment.Content));
                }

                writer.WriteLine($"--{boundary}--");
                writer.Flush();

                memoryStream.Position = 0;

                var rawRequest = new SendRawEmailRequest
                {
                    Source = senderAddress,
                    Destinations = destination.ToAddresses
                        .Concat(destination.CcAddresses ?? new List<string>())
                        .Concat(destination.BccAddresses ?? new List<string>())
                        .ToList(),
                    RawMessage = new RawMessage { Data = memoryStream }
                };

                var response = await _sesClient.SendRawEmailAsync(rawRequest);
                _logger.LogInformation(
                    "Raw email with attachments sent to {To} using AWS SES. MessageId: {MessageId}",
                    message.To,
                    response.MessageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending raw AWS SES email to {To}: {Message}", message.To, ex.Message);
                return false;
            }
        }

        private bool TryResolveSender(
            EmailMessage message,
            out string fromEmail,
            out string fromDisplay,
            out string senderAddress)
        {
            fromEmail = message.From ?? _settings.FromEmail;
            fromDisplay = message.FromDisplayName ?? _settings.FromDisplayName;

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                senderAddress = string.Empty;
                _logger.LogWarning("From email address is not configured. Skipping email send.");
                return false;
            }

            senderAddress = string.IsNullOrWhiteSpace(fromDisplay)
                ? fromEmail
                : $"{fromDisplay} <{fromEmail}>";

            return true;
        }

        private static AmazonSimpleEmailServiceClient CreateSesClient(IConfiguration configuration, string region)
        {
            var accessKey = configuration["Email:AccessKey"];
            var secretKey = configuration["Email:SecretKey"];
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            return !string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey)
                ? new AmazonSimpleEmailServiceClient(accessKey, secretKey, regionEndpoint)
                : new AmazonSimpleEmailServiceClient(regionEndpoint);
        }

        private static string NormalizeProvider(string provider)
        {
            if (IsSmtpProvider(provider))
            {
                return ProviderSmtp;
            }

            return ProviderAwsSes;
        }

        private static bool IsKnownProvider(string provider)
        {
            return IsSmtpProvider(provider)
                || provider.Equals(ProviderAwsSes, StringComparison.OrdinalIgnoreCase)
                || provider.Equals("Aws", StringComparison.OrdinalIgnoreCase)
                || provider.Equals("SES", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSmtpProvider(string provider)
        {
            return provider.Equals(ProviderSmtp, StringComparison.OrdinalIgnoreCase)
                || provider.Equals("SMTP", StringComparison.OrdinalIgnoreCase);
        }

        private static IReadOnlyList<string> SplitAddresses(string? addresses)
        {
            return addresses?
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(address => !string.IsNullOrWhiteSpace(address))
                .ToList()
                ?? new List<string>();
        }
    }
}
