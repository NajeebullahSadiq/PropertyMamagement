using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Middleware;
using WebAPIBackend.Services.Verification;

var builder = WebApplication.CreateBuilder(args);

// Configure Npgsql to use timestamp without time zone
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.

//Get Connection String
builder.Services.AddDbContext<AppDbContext>(opts =>
               opts.UseNpgsql(builder.Configuration["connection:connectionString"], npgsqlOpts =>
                   npgsqlOpts.MaxBatchSize(100))
               .ConfigureWarnings(warnings =>
                   warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
//Get data from appsettings.json and store to ApplicationSettings model than we use in our classes and Controller like Login Controller
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddMvc();

//use buit in Idenntity from .net core and create all Identity Tables
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//Policy Base Authorization
builder.Services.AddSingleton<IAuthorizationPolicyProvider, WebAPIBackend.Authorization.PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, WebAPIBackend.Authorization.PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyTypes.Users.View, policy => { policy.RequireClaim(CustomClaimTypes.Permission, UserPermissions.View); });
    options.AddPolicy(PolicyTypes.Users.EditRole, policy => { policy.RequireClaim(CustomClaimTypes.Permission, UserPermissions.ViewUserTest); });
    // Audit Log Policies
    options.AddPolicy("AuditLogViewPolicy", policy => { policy.RequireClaim(CustomClaimTypes.Permission, Permissions.AuditLogView); });
    options.AddPolicy("AuditLogExportPolicy", policy => { policy.RequireClaim(CustomClaimTypes.Permission, Permissions.AuditLogExport); });
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // TODO: Remove ReferenceHandler once all controllers use DTOs/projections instead of returning entities directly
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Fix Persian/Dari text encoding - prevent Unicode escaping
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        
        // GLOBAL DATE CONVERSION: Hijri Shamsi (Frontend) ↔ Gregorian (Database)
        // All dates automatically converted between Hijri Shamsi (UI) and Gregorian (DB)
        options.JsonSerializerOptions.Converters.Add(new WebAPIBackend.Converters.HijriShamsiDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new WebAPIBackend.Converters.HijriShamsiNullableDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new WebAPIBackend.Converters.HijriShamsiDateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new WebAPIBackend.Converters.HijriShamsiNullableDateOnlyConverter());
    });

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
    o.ValueLengthLimit = 100 * 1024 * 1024;           // 100MB max form value
    o.MultipartBodyLengthLimit = 100 * 1024 * 1024;   // 100MB max multipart body
    o.MemoryBufferThreshold = 50 * 1024 * 1024;       // 50MB - buffer to disk above this
});

// Register Verification Services
builder.Services.AddScoped<IVerificationCodeGenerator, VerificationCodeGenerator>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();

// Register License Number Generator Service
builder.Services.AddScoped<WebAPIBackend.Services.ILicenseNumberGenerator, WebAPIBackend.Services.LicenseNumberGenerator>();

// Register Province Filter Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<WebAPIBackend.Services.IProvinceFilterService, WebAPIBackend.Services.ProvinceFilterService>();

// Register Memory Cache for lookup/static data
builder.Services.AddMemoryCache();
builder.Services.AddScoped<WebAPIBackend.Services.ILookupCacheService, WebAPIBackend.Services.LookupCacheService>();

// Register Company and License Services
builder.Services.AddScoped<WebAPIBackend.Services.ICompanyService, WebAPIBackend.Services.CompanyService>();
builder.Services.AddScoped<WebAPIBackend.Services.ILicenseService, WebAPIBackend.Services.LicenseService>();

builder.Services.AddScoped<WebAPIBackend.Services.ISecurityAuditLogger, WebAPIBackend.Services.SecurityAuditLogger>();

// Register Comprehensive Audit Service
builder.Services.AddScoped<WebAPIBackend.Services.IComprehensiveAuditService, WebAPIBackend.Services.ComprehensiveAuditService>();

// Response Compression (Brotli + Gzip) for API responses
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/json" });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// Register Exception Handler
builder.Services.AddExceptionHandler<WebAPIBackend.Middleware.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// NOTE: Schema migrations have been moved to a one-time SQL script.
// Run: Backend/Scripts/schema_migrations.sql against the database once.
// This removes 2-5 seconds of startup time previously spent on 15+ ALTER/CREATE INDEX statements.

// Seed database and create admin user
try
{
    await DatabaseSeeder.SeedDatabase(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database seeding/migration failed. The application will continue starting, but some features may not work until the database is fixed.");
}

// Configure the HTTP request pipeline.
// Use exception handler
app.UseExceptionHandler();

// NOTE: Response compression is handled by Nginx (brotli + gzip) at the edge.
// Do NOT enable app.UseResponseCompression() here - it breaks Angular HttpClient
// deserialization because the Kestrel-compressed body isn't decompressed by the
// browser before Angular reads it.

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

// Province-based access control middleware (must be after authentication)
app.UseProvinceAuthorization();

app.MapControllers();
app.Run();