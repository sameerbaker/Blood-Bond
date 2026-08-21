using BloodBond.Extensinos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (Connection String from appsettings)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new System.InvalidOperationException("Missing ConnectionStrings:DefaultConnection in appsettings.json.");
builder.Services.AddDatabase(connectionString);

// Identity (Users, Roles, Sign-in, Tokens)
builder.Services.AddIdentityServices();

// JWT Bearer
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddCorsPolicy();

// Rate Limiting (anti-spam / anti-abuse)
builder.Services.AddRateLimiting();

// Localization (English + Arabic via Accept-Language or ?lang=)
builder.Services.AddBloodBondLocalization();

// Application services (Repositories, BLL services, Seeders, Mapster)
builder.Services.AddApplicationServices();

// -------------------- Pipeline --------------------
var app = builder.Build();

// Apply migrations and seed data (only on real startup, not at design time).
if (!app.Environment.IsEnvironment("Testing"))
{
    await app.ApplyMigrationsAndSeedAsync();
}

app.UseBloodBondPipeline(app.Environment);

app.Run();
