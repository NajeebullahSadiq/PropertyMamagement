
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
using WebAPIBackend.Services.Verification;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Get Connection String
builder.Services.AddDbContext<AppDbContext>(opts =>
               opts.UseNpgsql(builder.Configuration["connection:connectionString"], 
                   npgsqlOpts => npgsqlOpts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
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

// Register Verification Services
builder.Services.AddScoped<IVerificationCodeGenerator, VerificationCodeGenerator>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();




var app = builder.Build();

// Seed database and create admin user
try
{
    await DatabaseSeeder.SeedDatabase(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database seeding/migration failed. The application will continue starting, but some features may not work until the database is fixed.");
}

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.ExecuteSqlRawAsync(@"
        DO $$
        BEGIN
            IF NOT EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'ElectronicNationalIdNumber'
            ) THEN
                ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""ElectronicNationalIdNumber"" VARCHAR(50) NULL;
            END IF;

            IF NOT EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'org' AND table_name = 'Guarantors' AND column_name = 'ElectronicNationalIdNumber'
            ) THEN
                ALTER TABLE org.""Guarantors"" ADD COLUMN ""ElectronicNationalIdNumber"" VARCHAR(50) NULL;
            END IF;

            IF NOT EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RenewalRound'
            ) THEN
                ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RenewalRound"" INTEGER NULL;
            END IF;
        END $$;
    ");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Schema guard failed. The application will continue starting, but some Company module queries may fail until the database schema is updated.");
}

// Configure the HTTP request pipeline.
// Use CORS (must be before UseAuthentication and UseAuthorization)
app.UseCors();

app.UseStaticFiles();
var storageRoot = builder.Configuration["FileStorage:RootPath"] ?? AppContext.BaseDirectory;
var resourcesPath = Path.Combine(storageRoot, "Resources");
Directory.CreateDirectory(resourcesPath);
Directory.CreateDirectory(Path.Combine(resourcesPath, "Images"));
var documentsPath = Path.Combine(resourcesPath, "Documents");
Directory.CreateDirectory(documentsPath);
Directory.CreateDirectory(Path.Combine(documentsPath, "Identity"));
Directory.CreateDirectory(Path.Combine(documentsPath, "Profile"));
Directory.CreateDirectory(Path.Combine(documentsPath, "Property"));
Directory.CreateDirectory(Path.Combine(documentsPath, "Vehicle"));
Directory.CreateDirectory(Path.Combine(documentsPath, "Company"));
Directory.CreateDirectory(Path.Combine(documentsPath, "License"));
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(resourcesPath),
    RequestPath = new PathString("/Resources")
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(resourcesPath),
    RequestPath = new PathString("/api/Resources")
});

//allow Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();