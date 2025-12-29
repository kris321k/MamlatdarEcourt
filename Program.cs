using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MamlatdarEcourt.Data;
using MamlatdarEcourt.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Allows enums like: "Bardez" instead of 0
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add Authentication
builder.Services.AddAuthentication();

// Add Authorization
builder.Services.AddAuthorization();


var app = builder.Build();

// Middlewares


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

app.Run();
