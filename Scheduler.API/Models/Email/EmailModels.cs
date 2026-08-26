namespace Scheduler.API.Models.Email
{
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public string? From { get; set; }
        public string? FromDisplayName { get; set; }
        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }

    public class EmailSettings
    {
        public string Provider { get; set; } = "AwsSes";
        public string Region { get; set; } = "us-east-1";
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; }
        public string FromEmail { get; set; } = string.Empty;
        public string FromDisplayName { get; set; } = "caresynX";
        public int Timeout { get; set; } = 30000; // 30 seconds
    }
}

