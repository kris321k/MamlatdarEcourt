using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MamlatdarEcourt.Data;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.Services;
using MamlatdarEcourt.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ================= Controllers =================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

// ================= Database =================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ================= Identity (JWT-Friendly) =================
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// ================= JWT Authentication =================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Read JWT from cookie
            var token = context.Request.Cookies["SecureSessionToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }


            Console.WriteLine("Janhavi ");
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),

        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// ================= CORS (DEV MODE OPEN) =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ================= Services =================
builder.Services.AddScoped<LitigantAuthService>();
builder.Services.AddScoped<LitigantRepository>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();   // MUST be before Authorization
app.UseAuthorization();

app.MapControllers();

// ================= Create Roles =================
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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