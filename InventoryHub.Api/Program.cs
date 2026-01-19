using InventoryHub.Api.Middleware;
using InventoryHub.Api.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Optimize JSON serialization
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register repository and service
builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5174",
                "https://localhost:5174")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add response compression for performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Add Swagger UI for easier testing
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "InventoryHub API v1");
    });
}

// Performance: Use response compression
app.UseResponseCompression();

// Custom middleware for request logging
app.UseRequestLogging();

app.UseHttpsRedirection();
app.UseCors("BlazorClient");

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
