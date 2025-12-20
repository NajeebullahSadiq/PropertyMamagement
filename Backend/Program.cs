
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Models;
using WebAPIBackend.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Get Connection String
builder.Services.AddDbContext<AppDbContext>(opts =>
               opts.UseNpgsql(builder.Configuration["connection:connectionString"]));
//Get data from appsettings.json and store to ApplicationSettings model than we use in our classes and Controller like Login Controller
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddMvc();

//use buit in Idenntity from .net core and create all Identity Tables
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//Policy Base Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyTypes.Users.View, policy => { policy.RequireClaim(CustomClaimTypes.Permission, UserPermissions.View); });
    options.AddPolicy(PolicyTypes.Users.EditRole, policy => { policy.RequireClaim(CustomClaimTypes.Permission, UserPermissions.ViewUserTest); });
});
//End Policy Authorization
builder.Services.Configure<IdentityOptions>(options =>
options.User.RequireUniqueEmail = true);
//Password Configuration
builder.Services.Configure<IdentityOptions>(options =>
options.Password.RequireDigit = false);
builder.Services.Configure<IdentityOptions>(options =>
options.Password.RequireNonAlphanumeric = false);
builder.Services.Configure<IdentityOptions>(options =>
options.Password.RequireLowercase = false);
builder.Services.Configure<IdentityOptions>(options =>
options.Password.RequireUppercase = false);
builder.Services.Configure<IdentityOptions>(options =>
options.Password.RequiredLength = 4);
//JWT Token Setup
var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"] ?? throw new InvalidOperationException("JWT_Secret is not configured"));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

//Should Use Authorization when we use JWT 
builder.Services.AddAuthorization();

builder.Services.AddControllers();

//CorsPolicy Settings - Define allowed origins
string[] allowedOrigins = new string[] 
{
    "http://103.132.98.92",  // Your server IP
    "https://yourdomain.com", // Your domain if you have one
    "http://localhost:2400",   // Local development  
    "http://localhost:4200"    // Angular development server
};

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

//Memory Managment for File Upload
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});




var app = builder.Build();

// Seed database and create admin user
await DatabaseSeeder.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
// Use CORS (must be before UseAuthentication and UseAuthorization)
app.UseCors();

app.UseStaticFiles();
var resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
Directory.CreateDirectory(resourcesPath);
Directory.CreateDirectory(Path.Combine(resourcesPath, "Images"));
var documentsPath = Path.Combine(resourcesPath, "Documents");
Directory.CreateDirectory(documentsPath);
Directory.CreateDirectory(Path.Combine(documentsPath, "Identity"));
Directory.CreateDirectory(Path.Combine(documentsPath, "Profile"));
Directory.CreateDirectory(Path.Combine(documentsPath, "Property"));
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(resourcesPath),
    RequestPath = new PathString("/Resources")
});

//allow Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();