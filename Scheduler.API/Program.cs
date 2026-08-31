using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scheduler.API.Application.Middlewares;
using Scheduler.API.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Preserve legacy AWS Secrets Manager behavior unless the VPS explicitly disables it.
var awsSecretsEnabled = builder.Configuration.GetValue<bool?>("AWS:SecretsManager:Enabled") ?? true;
if (builder.Environment.IsProduction() && awsSecretsEnabled)
{
    var secretName = builder.Configuration["AWS:SecretsManager:SecretName"] ?? "caresynx/production";
    var region = builder.Configuration["AWS:SecretsManager:Region"] ?? "us-east-1";
    builder.Configuration.AddAwsSecretsManager(secretName, region);
}

// Service registration
builder.Services.AddAppServices();
builder.Services.AddAppSwagger();
builder.Services.AddAppCors();

// Authentication
builder.Services.AddCustomAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// CORS must be applied BEFORE authentication
app.UseAppCorsPolicy();

// Add global exception handler middleware BEFORE authentication
app.UseGlobalExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseAppSwaggerUI();

app.UseStaticFiles(); // For wwwroot by default

// Add static file serving for different file types
var fileStoragePath = builder.Configuration["Storage:LocalBaseDir"] ?? "wwwroot/FileStorage";

// Extract the actual path from the URL if it's a full URL
if (fileStoragePath.StartsWith("http"))
{
    // Extract path from URL like "https://localhost:7094/FileStorage"
    var uri = new Uri(fileStoragePath);
    fileStoragePath = uri.AbsolutePath.TrimStart('/');
}

// Ensure we have an absolute path
if (!Path.IsPathRooted(fileStoragePath))
{
    fileStoragePath = Path.Combine(builder.Environment.ContentRootPath, fileStoragePath);
}

// Create directories if they don't exist
Directory.CreateDirectory(fileStoragePath);
var directories = new[] { "ProfileImages", "OrganizationLogos", "UserDocument", "Invoices" };
foreach (var dir in directories)
{
    var fullPath = Path.Combine(fileStoragePath, dir);
    Directory.CreateDirectory(fullPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(fileStoragePath, "ProfileImages")),
    RequestPath = "/ProfileImages"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(fileStoragePath, "OrganizationLogos")),
    RequestPath = "/OrganizationLogos"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(fileStoragePath, "UserDocument")),
    RequestPath = "/UserDocument"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(fileStoragePath, "Invoices")),
    RequestPath = "/Invoices"
});

app.Run();


