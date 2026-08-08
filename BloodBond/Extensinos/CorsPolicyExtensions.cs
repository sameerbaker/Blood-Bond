using Microsoft.Extensions.DependencyInjection;

namespace BloodBond.Extensinos
{
    /// <summary>
    /// CORS policy for the BloodBond API.
    /// </summary>
    public static class CorsPolicyExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("BloodBondCors", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
