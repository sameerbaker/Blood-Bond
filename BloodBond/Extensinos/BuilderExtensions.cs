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

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("BloodBondCors");
            app.UseAuthentication();
            app.UseAuthorization();

            // MapControllers requires IEndpointRouteBuilder (provided by WebApplication).
            var endpoints = (IEndpointRouteBuilder)app;
            endpoints.MapControllers();

            return endpoints;
        }
    }
}
