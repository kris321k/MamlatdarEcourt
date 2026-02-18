using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MamlatdarEcourt.Data;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.Services;
using MamlatdarEcourt.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Enum JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

// SQL Server Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// CORS (Allow frontend connection)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<LitigantAuthService>();
builder.Services.AddScoped<LitigantRepository>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddMemoryCache();

var app = builder.Build();
app.UseCors("AllowFrontend");





app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "litigant" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();
