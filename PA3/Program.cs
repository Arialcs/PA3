using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Get the database provider configuration from app settings
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

// Conditionally register the database provider
if (useInMemoryDatabase)
{
    builder.Services.AddDbContext<QuoteDbContext>(options =>
        options.UseInMemoryDatabase("Pa3_8843688"));  // In-Memory Database
}
else
{
    builder.Services.AddDbContext<QuoteDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // SQL Server
}

// Add controllers for Web API
builder.Services.AddControllers();

// Set up CORS policy
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add Swagger support for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS middleware
app.UseCors();

// Enable authorization middleware
app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Run the app
app.Run();
