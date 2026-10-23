using Microsoft.EntityFrameworkCore;
using SmartShop.Api.Data;
using SmartShop.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SmartShop AI API",
        Version = "v1",
        Description = "Generative AI for .NET Developers with Google AI (Gemini) - Exam Paper SET01"
    });
});

// Configure Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=.\\SQLEXPRESS;Database=SMARTSHOP;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<SmartShopDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Gemini & Chat Services
builder.Services.AddHttpClient<IGeminiService, GeminiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(100);
});

builder.Services.AddSingleton<IChatSessionService, ChatSessionService>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable Swagger UI in both Dev and Prod for grading convenience
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartShop AI API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

// Serve frontend website from wwwroot (Requirement 5)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();
