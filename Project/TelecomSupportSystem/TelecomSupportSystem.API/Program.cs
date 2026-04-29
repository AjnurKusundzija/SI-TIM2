using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.BLL.Services;

DotNetEnv.Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/*
// JWT_KEY: env var (production/CI) takes priority; User Secrets / IConfiguration used in development
if (Environment.GetEnvironmentVariable("JWT_KEY") is null)
{
    var fromConfig = builder.Configuration["JWT_KEY"];
    if (fromConfig is not null)
        Environment.SetEnvironmentVariable("JWT_KEY", fromConfig);
}

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT_KEY is not set. Use 'dotnet user-secrets set' locally or an environment variable in production.");
*/
/*
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing in configuration.");
*/

var jwtKey = builder.Configuration["JWT_KEY"]
    ?? throw new InvalidOperationException("JWT_KEY is missing");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

//for REACT frontent port: 5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            "http://localhost:5173", //React frontedn
            "http://localhost:5122",
            "https://localhost:7148"//Swagger UI
            )
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueLimit = 0;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    options.RejectionStatusCode = 429;
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();
builder.Services.AddScoped<IPackageFeatureRepository, PackageFeatureRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed test users in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // DODANO: Kreira bazu i tabele ako ne postoje
    db.Database.EnsureCreated();

    // add users if not existant
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@test.com",
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Phone = "",
                Address = "",
                Role = Role.ADMINISTRATOR,
                AccountStatus = AccountStatus.ACTIVE
            },
            new User
            {
                FirstName = "Agent",
                LastName = "User",
                Email = "agent@test.com",
                Username = "agent",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Phone = "",
                Address = "",
                Role = Role.AGENT,
                AccountStatus = AccountStatus.ACTIVE
            },
            new User
            {
                FirstName = "Client",
                LastName = "User",
                Email = "client@test.com",
                Username = "client",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Client123!"),
                Phone = "",
                Address = "",
                Role = Role.CLIENT,
                AccountStatus = AccountStatus.ACTIVE
            }
        );
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();