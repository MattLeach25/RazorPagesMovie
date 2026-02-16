using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesMovie.Data;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using System.Diagnostics;
using System.Diagnostics.Metrics;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register ActivitySource and Meter for custom telemetry
var activitySource = new ActivitySource("RazorPagesMovie");
var meter = new Meter("RazorPagesMovie");
builder.Services.AddSingleton(activitySource);
builder.Services.AddSingleton(meter);

// Configure OpenTelemetry
ConfigureOpenTelemetry(builder);

var connectionString = builder.Configuration.GetConnectionString("RazorPagesMovieContext")
    ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.");

if (builder.Environment.IsDevelopment())
{
    // Local development: Use SQLite
    builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
    {
        Authentication = SqlAuthenticationMethod.ActiveDirectoryManagedIdentity
    };

    builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
            options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString)
                   .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
}

var app = builder.Build();

// Automatically create and migrate the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<RazorPagesMovieContext>();
        
        logger.LogInformation("Attempting to connect to database...");
        
        if (app.Environment.IsDevelopment())
        {
            // Development: Use SQLite with EnsureCreated (bypasses migrations)
            logger.LogInformation("Development environment detected. Using SQLite.");
            
            var created = context.Database.EnsureCreated();
            if (created)
            {
                logger.LogInformation("SQLite database created successfully from model.");
            }
            else
            {
                logger.LogInformation("SQLite database already exists.");
            }
        }
        else
        {
            // Production: Use SQL Server with migrations
            logger.LogInformation("Production environment detected. Using SQL Server with migrations.");
            
            var canConnect = context.Database.CanConnect();
            if (canConnect)
            {
                logger.LogInformation("Database connection successful.");
                
                // Check if there are pending migrations
                var pendingMigrations = context.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Found {Count} pending migrations: {Migrations}", 
                        pendingMigrations.Count, string.Join(", ", pendingMigrations));
                    
                    // Apply migrations in production
                    context.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("Database is up to date. No pending migrations.");
                }
            }
            else
            {
                logger.LogError("Cannot connect to database. Application will start but database operations will fail.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up the database. Error: {Message}", ex.Message);
        
        if (app.Environment.IsDevelopment())
        {
            throw; // Fail fast in development
        }
        else
        {
            logger.LogWarning("Application will continue to start despite database connection issues.");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
{
    var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrEmpty(appInsightsConnectionString))
    {
        builder.Services.AddOpenTelemetry()
            .UseAzureMonitor(options =>
            {
                options.ConnectionString = appInsightsConnectionString;
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource("RazorPagesMovie");
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter("RazorPagesMovie");
            });
    }
}
