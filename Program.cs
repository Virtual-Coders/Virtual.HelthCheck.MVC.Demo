using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Virtual.HealthCheck;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseWebRoot("wwwroot");
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddCheck<MyServiceHealthCheck>("My Service Health Check")
    .AddUrlGroup(new Uri("https://qa-ophir-tracker-web.azurewebsites.net/"), name: "UI Health Check")
    .AddUrlGroup(new Uri("https://qa-ophir-obs-api.azurewebsites.net/swagger/v1/swagger.json"), name: "Third-Party Service Check")
    .AddSqlServer(builder.Configuration.GetConnectionString("Default"), name: "SQL DB Health Check");

// Add HealthChecks UI
builder.Services.AddHealthChecksUI(setup =>
{
    setup.AddHealthCheckEndpoint("default", "/health");
}).AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Map the HealthChecks UI
app.MapHealthChecksUI(setup =>
{
    setup.UIPath = "/vc-health"; // Custom path for UI
    setup.AddCustomStylesheet("wwwroot/css/dotnet.css");
});

app.Run();
