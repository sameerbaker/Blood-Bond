using BloodBond.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBond.Extensinos
{
    /// <summary>
    /// ASP.NET Core Identity registration with the ApplicationUser as the user type.
    /// </summary>
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<BloodBond.DAL.Data.ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Needed for IHttpContextAccessor inside ApplicationDbContext
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
