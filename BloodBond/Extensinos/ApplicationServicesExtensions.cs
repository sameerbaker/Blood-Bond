using BloodBond.BLL.Mapping;
using BloodBond.BLL.Service;
using BloodBond.DAL.Repository;
using BloodBond.DAL.utils;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBond.Extensinos
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Generic repository (open)
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Specific repositories
            services.AddScoped<IBloodBankRepository, BloodBankRepository>();
            services.AddScoped<IBloodInventoryRepository, BloodInventoryRepository>();
            services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMonetaryDonationRepository, MonetaryDonationRepository>();
            services.AddScoped<IBloodBankRatingRepository, BloodBankRatingRepository>();
            services.AddScoped<IBadgeRepository, BadgeRepository>();
            services.AddScoped<IBloodDriveEventRepository, BloodDriveEventRepository>();
            services.AddScoped<IEventAttendanceRepository, EventAttendanceRepository>();

            // Application services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IBloodBankService, BloodBankService>();
            services.AddScoped<IBloodRequestService, BloodRequestService>();
            services.AddScoped<IEligibilityService, EligibilityService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IMonetaryDonationService, MonetaryDonationService>();
            services.AddScoped<IBloodBankRatingService, BloodBankRatingService>();
            services.AddScoped<IBadgeService, BadgeService>();
            services.AddScoped<IBloodDriveEventService, BloodDriveEventService>();

            // Seeders
            services.AddScoped<ISeedData, RoleSeedData>();
            services.AddScoped<ISeedData, BadgeSeedData>();

            // Stripe settings (bind from config). The default binder reads
            // from appsettings.json + appsettings.{Env}.json + user-secrets
            // (in Development) + environment variables — in that order.
            // Adding a new key to user-secrets or env vars will simply
            // override whatever is in appsettings.json.
            services.Configure<StripeSettings>(options =>
            {
                // No manual reading here — the default options binder
                // already populates the properties from the merged
                // IConfiguration. This comment is left so future
                // contributors don't add a manual `BuildServiceProvider`
                // (which is an anti-pattern and breaks scoped lifetimes).
            });
            services.AddOptions<StripeSettings>()
                .BindConfiguration("Stripe");

            // Mapster mappings
            MapsterConfig.RegisterMappings();

            return services;
        }
    }
}
