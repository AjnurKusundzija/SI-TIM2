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

DotNetEnv.Env.Load(".env", new DotNetEnv.LoadOptions(clobberExistingVars: false));
DotNetEnv.Env.Load("../../.env", new DotNetEnv.LoadOptions(clobberExistingVars: false));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
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
            },
            new User
            {
                FirstName = "Amir",
                LastName = "Hodžić",
                Email = "amir@test.com",
                Username = "amirh",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Amir123!"),
                Phone = "",
                Address = "",
                Role = Role.CLIENT,
                AccountStatus = AccountStatus.ACTIVE
            }
        );
        db.SaveChanges();
    }

    if (!db.Tickets.Any())
    {
        var clientId = db.Users.First(u => u.Email == "client@test.com").UserId;
        var amirId = db.Users.First(u => u.Email == "amir@test.com").UserId;
        db.Tickets.AddRange(
            new Ticket { Title = "Internet ne radi", Description = "Nemam internet konekciju od jutros.", CreatedDate = DateTime.UtcNow.AddDays(-3), Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatorId = clientId },
            new Ticket { Title = "Spor internet", Description = "Brzina je puno manja od ugovorene.", CreatedDate = DateTime.UtcNow.AddDays(-5), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.INTERNET, CreatorId = clientId },
            new Ticket { Title = "TV signal isčezava", Description = "Slika se gubi svakih par minuta.", CreatedDate = DateTime.UtcNow.AddDays(-7), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-2), Priority = Priority.LOW, ProblemCategory = ProblemCategory.TV, CreatorId = clientId },
            new Ticket { Title = "Pogrešan iznos na računu", Description = "Naplaćena mi je usluga koju nisam naručio.", CreatedDate = DateTime.UtcNow.AddDays(-1), Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.BILLING, CreatorId = clientId },
            new Ticket { Title = "Mobilna mreža bez signala", Description = "Nema signala u mom kvartu već 2 dana.", CreatedDate = DateTime.UtcNow.AddDays(-2), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = clientId },
            new Ticket { Title = "Ne mogu uputiti poziv", Description = "Pozivi ne prolaze, javlja se greška.", CreatedDate = DateTime.UtcNow.AddDays(-4), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-1), Priority = Priority.HIGH, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = clientId },
            new Ticket { Title = "Tehnička podrška za ruter", Description = "Trebam pomoć sa konfiguracijom rutera.", CreatedDate = DateTime.UtcNow.AddDays(-6), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-3), Priority = Priority.LOW, ProblemCategory = ProblemCategory.TECHNICAL_SUPPORT, CreatorId = clientId },
            new Ticket { Title = "TV aplikacija ne radi", Description = "Ne mogu pristupiti TV aplikaciji.", CreatedDate = DateTime.UtcNow.AddDays(-8), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.TV, CreatorId = clientId },
            new Ticket { Title = "Prekid usluge bez obavijesti", Description = "Usluga je prekinuta bez ikakve najave.", CreatedDate = DateTime.UtcNow.AddDays(-9), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-5), Priority = Priority.HIGH, ProblemCategory = ProblemCategory.TECHNICAL_SUPPORT, CreatorId = clientId },

            // Tiketi za drugog klijenta (Amir Hodžić)
            new Ticket { Title = "Nestabilan Wi-Fi signal", Description = "Wi-Fi se prekida svakih nekoliko minuta u stanu.", CreatedDate = DateTime.UtcNow.AddDays(-2), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.INTERNET, CreatorId = amirId },
            new Ticket { Title = "Račun veći nego prošli mjesec", Description = "Mjesečni račun mi je dvostruko veći nego ranije, molim provjeru stavki.", CreatedDate = DateTime.UtcNow.AddDays(-4), Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.BILLING, CreatorId = amirId },
            new Ticket { Title = "Ne mogu se prijaviti na korisnički portal", Description = "Prilikom prijave na portal javlja se greška o neispravnoj lozinki iako je tačna.", CreatedDate = DateTime.UtcNow.AddDays(-6), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-4), Priority = Priority.LOW, ProblemCategory = ProblemCategory.TECHNICAL_SUPPORT, CreatorId = amirId },
            new Ticket { Title = "Roaming ne radi u inostranstvu", Description = "Putovao sam u Njemačku i nisam mogao koristiti mobilne podatke.", CreatedDate = DateTime.UtcNow.AddDays(-10), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = amirId },
            new Ticket { Title = "Nedostaju kanali na TV-u", Description = "Nakon zadnjeg ažuriranja nestala su mi tri sportska kanala iz paketa.", CreatedDate = DateTime.UtcNow.AddDays(-1), Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.TV, CreatorId = amirId },
            new Ticket { Title = "Telefon prekida poziv nakon minute", Description = "Svi pozivi prema fiksnoj mreži se automatski prekidaju nakon otprilike jednog minuta.", CreatedDate = DateTime.UtcNow.AddDays(-7), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-2), Priority = Priority.HIGH, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = amirId },
            new Ticket { Title = "Sporo učitavanje stranica", Description = "Stranice se učitavaju sporo iako sam plaćam najbrži paket.", CreatedDate = DateTime.UtcNow.AddDays(-3), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.INTERNET, CreatorId = amirId }
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