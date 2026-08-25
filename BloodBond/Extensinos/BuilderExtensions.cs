using BloodBond.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BloodBond.Extensinos
{
    
    public static class BuilderExtensions
    {
        
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

            
            app.UseCors("BloodBondCors");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            var endpoints = (IEndpointRouteBuilder)app;
            endpoints.MapControllers();

            return endpoints;
        }
    }
}
