using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace Scheduler.API.Extensions
{
    public static class AwsSecretsManagerExtensions
    {
        public static IConfigurationBuilder AddAwsSecretsManager(this IConfigurationBuilder builder, string secretName, string region = "us-east-1")
        {
            builder.Add(new AwsSecretsManagerSource(secretName, region));
            return builder;
        }
    }

    public class AwsSecretsManagerSource : IConfigurationSource
    {
        private readonly string _secretName;
        private readonly string _region;

        public AwsSecretsManagerSource(string secretName, string region)
        {
            _secretName = secretName;
            _region = region;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AwsSecretsManagerProvider(_secretName, _region);
        }
    }

    public class AwsSecretsManagerProvider : ConfigurationProvider
    {
        private readonly string _secretName;
        private readonly string _region;

        public AwsSecretsManagerProvider(string secretName, string region)
        {
            _secretName = secretName;
            _region = region;
        }

        public override void Load()
        {
            var secret = GetSecret();
            if (secret == null) return;

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);
            if (data == null) return;

            foreach (var kvp in data)
            {
                // Convert double underscore (__) to colon (:) for .NET config hierarchy
                var key = kvp.Key.Replace("__", ":");
                Data[key] = kvp.Value;
            }
        }

        private string? GetSecret()
        {
            try
            {
                using var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region));
                var request = new GetSecretValueRequest { SecretId = _secretName };
                var response = client.GetSecretValueAsync(request).GetAwaiter().GetResult();
                return response.SecretString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load secrets from AWS Secrets Manager: {ex.Message}");
                return null;
            }
        }
    }
}
