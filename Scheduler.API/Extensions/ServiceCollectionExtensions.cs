using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scheduler.API.Services;
using Scheduler.API.Services.Address;
using Scheduler.API.Services.Authentication;
using Scheduler.API.Services.Availability;
using Scheduler.API.Services.Billing;

using Scheduler.API.Services.Client;
using Scheduler.API.Services.Contact;
using Scheduler.API.Services.Document;
using Scheduler.API.Services.Expense;
using Scheduler.API.Services.Franchise;
using Scheduler.API.Services.Leave;
using Scheduler.API.Services.Lookup;
using Scheduler.API.Services.Organization;
using Scheduler.API.Services.Package;
using Scheduler.API.Services.PlanBoard;
using Scheduler.API.Services.Scheduler;
using Scheduler.API.Services.Staff;
using Scheduler.API.Services.ToConfirm;
using Scheduler.API.Services.TaskLog;
using Scheduler.API.Services.LoginHistory;
using Scheduler.API.Services.Wage;
using Scheduler.API.Services.Account.BankAccount;
using Scheduler.API.Services.Account.Card;
using Scheduler.API.Services.Account.Transaction;
using Scheduler.API.Services.Security;
using Scheduler.API.Services.User;
using Scheduler.API.Services.FileStorage;
using Scheduler.API.Services.Service;
using Scheduler.API.Services.Payment;
using Scheduler.API.Services.RolePermission;
using Scheduler.API.Services.Email;
using Scheduler.API.Common;
using FluentValidation;
using Scheduler.API.Models.Client;
using FluentValidation.AspNetCore;
using Scheduler.API.Services.Role;
using Scheduler.API.Services.Notification;
using Scheduler.API.Services.Preference;
using Scheduler.API.Services.Complaint;
using Scheduler.API.Services.Payer;

namespace Scheduler.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Controllers
            services.AddControllers();

            // Swagger
            services.AddSwaggerGen();

            // Dependency Injection
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<Scheduler.API.Services.ServiceProvider.IServiceProvider, Scheduler.API.Services.ServiceProvider.ServiceProviderRepository>();
            services.AddScoped<IClient, ClientRepository>();
            services.AddScoped<IStaff, StaffRepository>();
            services.AddScoped<IContact, ContactRepository>();
            services.AddScoped<IAddress, AddressRepository>();
            services.AddScoped<IOrganization, OrganizationRepository>();
            services.AddScoped<IOrganizationBillingSettingsService, OrganizationBillingSettingsService>();
            services.AddScoped<IPackage, PackageRepository>();
            //services.AddHostedService<MonthlyBillingService>();
            services.AddScoped<IFranchise, FranchiseRepository>();
            services.AddScoped<IPlanBoard, PlanBoardRepository>();
            services.AddScoped<IToConfirm, ToConfirmRepository>();
            services.AddScoped<ITaskLog, TaskLogRepository>();
            services.AddScoped<ILoginHistory, LoginHistoryRepository>();
            services.AddScoped<IExpense, ExpenseRepository>();
            services.AddScoped<ILeave, LeaveRepository>();
            services.AddScoped<IScheduler, SchedulerRepository>();
            services.AddScoped<IDocument, DocumentRepository>();
            services.AddScoped<ILookup, LookupRepository>();

            services.AddScoped<IAvailability, AvailabilityRepository>();
            services.AddScoped<IAuthentication, AuthenticationRepository>();
            services.AddScoped<IWage, WageRepository>();
            services.AddScoped<IBilling, BillingRepository>();
            services.AddScoped<IClientPayerService, ClientPayerService>();
            services.AddScoped<IBankAccount, BankAccountRepository>();
            services.AddScoped<ICard, CardRepository>();
            services.AddScoped<ITransaction, TransactionRepository>();
            services.AddScoped<ICrypto, CryptoRepository>();
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<IFileStorageFactory, FileStorageFactory>();
            services.AddScoped<IFileStorageService>(provider => 
                provider.GetRequiredService<IFileStorageFactory>().CreateStorageService());
            services.AddScoped<IServices, ServiceRepository>();
            services.AddScoped<IServiceType, ServiceTypeRepository>();
            services.AddScoped<IPayment, PaymentRepository>();
            services.AddScoped<IRolePermission, RolePermissionRepository>();
            services.AddScoped<IRole, RoleRepository>();
            services.AddScoped<IUrlService, UrlService>();
            services.AddScoped<IStripeConnectedAccountService, StripeConnectedAccountService>();
            services.AddScoped<ISaaSStripeService, SaaSStripeService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IInvoicePdfService, InvoicePdfService>();
            services.AddScoped<INotification, NotificationRepository>();
            services.AddScoped<NotificationHelper>();
            services.AddScoped<IPreference, PreferenceRepository>();
            services.AddScoped<IComplaint, ComplaintRepository>();

            // FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<SaveClientInfoViewModel>, SaveClientInfoViewModelValidator>();

            return services;
        }

        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scheduler API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and your token."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddAppCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:3000",
                                      "https://homecare.caresynx.com",
                                      "http://homecare.caresynx.com",
                                      "https://caresynx.com",
                                      "https://www.caresynx.com",
                                      "https://schedulerapi-demo-bbedcye9htd5ajfg.centralindia-01.azurewebsites.net",
                                      "https://kind-ocean-064e6cf00.1.azurestaticapps.net",
                                      "https://d1d81fooaztt9r.cloudfront.net",
                                      "http://44.210.107.247")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
            return services;
        }
    }
} 