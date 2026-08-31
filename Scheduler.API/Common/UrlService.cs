using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Scheduler.API.Common
{
    public interface IUrlService
    {
        string BuildWebPath(string filePath);
        string BuildImageUrl(string imagePath);
        bool IsCompleteUrl(string path);
        string GetStorageBaseUrl();
        string GetStorageType();
    }

    public class UrlService : IUrlService
    {
        private static readonly string[] PublicFolders =
        {
            "ProfileImages",
            "OrganizationLogos",
            "UserDocument",
            "Invoices"
        };

        private readonly IConfiguration _configuration;
        private readonly string _absoluteBaseDirectory;

        public UrlService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _absoluteBaseDirectory = ResolveAbsoluteBaseDirectory(configuration, environment);
        }

        public string BuildWebPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;

            if (IsCompleteUrl(filePath))
                return filePath;

            var storageType = GetStorageType();
            var baseUrl = GetStorageBaseUrl();

            switch (storageType?.ToLowerInvariant())
            {
                case "s3":
                case "aws":
                    return BuildS3Url(filePath, baseUrl);
                case "azure":
                    return BuildAzureUrl(filePath, baseUrl);
                case "gcp":
                case "google":
                    return BuildGcpUrl(filePath, baseUrl);
                case "local":
                default:
                    return BuildLocalUrl(filePath, baseUrl);
            }
        }

        public string BuildImageUrl(string imagePath)
        {
            return BuildWebPath(imagePath);
        }

        public bool IsCompleteUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public string GetStorageBaseUrl()
        {
            return _configuration["Storage:BaseUrl"] ??
                   _configuration["FileStorage:BaseUrl"] ??
                   "";
        }

        public string GetStorageType()
        {
            return _configuration["Storage:Type"] ??
                   _configuration["FileStorage:Type"] ??
                   "local";
        }

        private string BuildLocalUrl(string filePath, string baseUrl)
        {
            var webPath = ToPublicWebPath(filePath);
            if (string.IsNullOrEmpty(baseUrl))
                return webPath;

            return baseUrl.TrimEnd('/') + webPath;
        }

        private string ToPublicWebPath(string filePath)
        {
            var normalized = filePath.Replace('\\', '/');

            foreach (var folder in PublicFolders)
            {
                var token = "/" + folder + "/";
                var index = normalized.IndexOf(token, StringComparison.OrdinalIgnoreCase);
                if (index < 0 && normalized.StartsWith(folder + "/", StringComparison.OrdinalIgnoreCase))
                    index = 0;

                if (index >= 0)
                {
                    var relative = normalized.Substring(index).TrimStart('/');
                    return "/" + relative;
                }
            }

            try
            {
                var fullFile = Path.GetFullPath(filePath);
                var fullBase = _absoluteBaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                               + Path.DirectorySeparatorChar;

                if (fullFile.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase))
                {
                    var relative = fullFile.Substring(fullBase.Length).Replace('\\', '/');
                    return "/" + relative.TrimStart('/');
                }
            }
            catch (Exception)
            {
                // Not a filesystem path — fall through.
            }

            var markers = new[] { "/wwwroot/FileStorage/", "/FileStorage/", "wwwroot/FileStorage/" };
            foreach (var marker in markers)
            {
                var index = normalized.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    var relative = normalized.Substring(index + marker.Length).TrimStart('/');
                    return "/" + relative;
                }
            }

            return normalized.StartsWith('/') ? normalized : "/" + normalized;
        }

        private static string ResolveAbsoluteBaseDirectory(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var basePath = configuration["Storage:LocalBaseDir"] ??
                           configuration["FileStorage:LocalBaseDir"] ??
                           "wwwroot/FileStorage";

            if (basePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(basePath);
                basePath = uri.AbsolutePath.TrimStart('/');
            }

            if (!Path.IsPathRooted(basePath))
                basePath = Path.Combine(environment.ContentRootPath, basePath);

            return Path.GetFullPath(basePath);
        }

        private string BuildS3Url(string filePath, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                var bucketName = _configuration["Storage:S3:BucketName"];
                var region = _configuration["Storage:S3:Region"];
                return $"https://{bucketName}.s3.{region}.amazonaws.com/{filePath}";
            }

            return baseUrl.TrimEnd('/') + "/" + filePath.TrimStart('/');
        }

        private string BuildAzureUrl(string filePath, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                var accountName = _configuration["Storage:Azure:AccountName"];
                var containerName = _configuration["Storage:Azure:ContainerName"];
                return $"https://{accountName}.blob.core.windows.net/{containerName}/{filePath}";
            }

            return baseUrl.TrimEnd('/') + "/" + filePath.TrimStart('/');
        }

        private string BuildGcpUrl(string filePath, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                var bucketName = _configuration["Storage:GCP:BucketName"];
                return $"https://storage.googleapis.com/{bucketName}/{filePath}";
            }

            return baseUrl.TrimEnd('/') + "/" + filePath.TrimStart('/');
        }
    }
}
