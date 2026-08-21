using BloodBond.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BloodBond.Extensinos
{
    /// <summary>
    /// Extension methods that finalize the request pipeline.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Wires the full middleware pipeline. Returns IEndpointRouteBuilder
        /// so callers can still chain endpoint mappings (e.g., MapControllers).
        /// </summary>
        public static IEndpointRouteBuilder UseBloodBondPipeline(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            // Global exception handling should be the FIRST middleware.
            app.UseMiddleware<GlobalExceptionHandling>();

            // Localization must come early so localized strings are picked up everywhere
            app.UseBloodBondLocalization();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // NOTE: UseHttpsRedirection disabled in dev so the HTTP listener on :5000
            // works without losing the Authorization header on redirect.
            // Enable again in production.
            // app.UseHttpsRedirection();
            app.UseCors("BloodBondCors");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            // MapControllers requires IEndpointRouteBuilder (provided by WebApplication).
            var endpoints = (IEndpointRouteBuilder)app;
            endpoints.MapControllers();

            return endpoints;
        }
    }
}
