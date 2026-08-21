using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBond.Extensinos
{
    /// <summary>
    /// Configures multi-language support (English + Arabic).
    /// Clients can request a language via the Accept-Language header (e.g. "ar" or "en").
    /// </summary>
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddBloodBondLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Order matters: query string > cookie > Accept-Language header
                options.RequestCultureProviders = new IRequestCultureProvider[]
                {
                    new QueryStringRequestCultureProvider
                    {
                        QueryStringKey = "lang"
                    },
                    new CookieRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });
            return services;
        }

        public static IApplicationBuilder UseBloodBondLocalization(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>()
                .Value;
            app.UseRequestLocalization(options);
            return app;
        }
    }
}
