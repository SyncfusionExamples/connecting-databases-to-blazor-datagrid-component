using Grid_Firebase_Realtime.Components;
using Grid_Firebase_Realtime.Services;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Syncfusion Blazor services
builder.Services.AddSyncfusionBlazor();

// ========== FIREBASE CONFIGURATION ==========
// Register HttpClient for Firebase
builder.Services.AddHttpClient<FirebaseService>();

// Register Firebase Service for dependency injection
builder.Services.AddScoped<FirebaseService>();
// =============================================

// Add Syncfusion License if you have one (optional)
// Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("your_license_key");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
