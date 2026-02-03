using Grid_PostgreSQL.Components;
using Grid_PostgreSQL.Data;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ========== SYNCFUSION BLAZOR CONFIGURATION ==========
// Register Syncfusion Blazor services for UI components
builder.Services.AddSyncfusionBlazor();
// ====================================================

// ========== ENTITY FRAMEWORK CORE CONFIGURATION ==========
// Get PostgreSQL connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Validate that connection string is configured
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found in appsettings.json. " +
        "Please configure the PostgreSQL connection string before running the application.");
}

// Register PurchaseOrderDbContext with PostgreSQL provider (Npgsql)
builder.Services.AddDbContext<PurchaseOrderDbContext>(options =>
{
    // Use Npgsql as the database provider for PostgreSQL
    options.UseNpgsql(connectionString);

    // Enable detailed error messages and sensitive data logging in development environment
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register PurchaseOrderRepository as a scoped service
// Scoped means a new instance is created for each HTTP request
builder.Services.AddScoped<PurchaseOrderRepository>();
// ====================================================


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
