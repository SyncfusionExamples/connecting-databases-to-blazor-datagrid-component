using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Grid_DynamoDB.Components;
using Grid_DynamoDB.Services;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Syncfusion Blazor services
builder.Services.AddSyncfusionBlazor();

// Configure AWS DynamoDB
var awsOptions = builder.Configuration.GetAWSOptions();

// For Development environment, use DynamoDB Local with dummy credentials
if (builder.Environment.IsDevelopment())
{
    // Override credentials with dummy values for local development
    awsOptions.Credentials = new BasicAWSCredentials(
        builder.Configuration["AWS:AccessKeyId"] ?? "local",
        builder.Configuration["AWS:SecretAccessKey"] ?? "local"
    );
}

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();

// Register DynamoDB Service for dependency injection
builder.Services.AddScoped<DynamoDBService>();

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
