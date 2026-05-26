using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.API.Hubs;
using TelecomSupportSystem.API.Workers;

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
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

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
        // SignalR sends JWT via query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationhub"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
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
              .AllowAnyHeader()
              .AllowCredentials());
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
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();
builder.Services.AddScoped<IPackageFeatureRepository, PackageFeatureRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IFaqRepository, FaqRepository>();
// PB-52 / US-76, US-77
builder.Services.AddScoped<ICatalogPackageRepository, CatalogPackageRepository>();
builder.Services.AddScoped<IClientSubscriptionRepository, ClientSubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionAuditLogRepository, SubscriptionAuditLogRepository>();

// Services
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationPusher, NotificationPusher>();
builder.Services.AddScoped<IChatPusher, ChatPusher>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IPackageService, PackageService>();
// PB-52 / US-76, US-77
builder.Services.AddScoped<ICatalogPackageService, CatalogPackageService>();
builder.Services.AddScoped<IClientSubscriptionService, ClientSubscriptionService>();
// Audit Log
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
// PB-57, PB-58: AI suggestions (Gemini)
builder.Services.AddHttpClient<IAIService, AIService>();

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

    var retries = 10;
    var delay = TimeSpan.FromSeconds(3);
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            retries--;
            if (retries == 0) throw;
            Thread.Sleep(delay);
        }
    }

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
                Location = Location.SARAJEVO,
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
                Location = Location.SARAJEVO,
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
                Location = Location.SARAJEVO,
                Role = Role.CLIENT,
                AccountStatus = AccountStatus.ACTIVE
            },
            new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Username = "Joohnyy",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Johny123!"),
                Phone = "",
                Location = Location.MOSTAR,
                Role = Role.CLIENT,
                AccountStatus = AccountStatus.ACTIVE
            }
        );
        db.SaveChanges();
    }

    var developmentUsers = new[]
    {
        new { FirstName = "Admin", LastName = "User", Email = "admin@test.com", Username = "admin", Password = "Admin123!", Location = Location.SARAJEVO, Role = Role.ADMINISTRATOR },
        new { FirstName = "Agent", LastName = "User", Email = "agent@test.com", Username = "agent", Password = "Agent123!", Location = Location.SARAJEVO, Role = Role.AGENT },
        new { FirstName = "Client", LastName = "User", Email = "client@test.com", Username = "client", Password = "Client123!", Location = Location.SARAJEVO, Role = Role.CLIENT },
        new { FirstName = "John", LastName = "Doe", Email = "john@test.com", Username = "Joohnyy", Password = "Johny123!", Location = Location.MOSTAR, Role = Role.CLIENT }
    };

    foreach (var seedUser in developmentUsers)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == seedUser.Email);

        if (user is null)
        {
            db.Users.Add(new User
            {
                FirstName = seedUser.FirstName,
                LastName = seedUser.LastName,
                Email = seedUser.Email,
                Username = seedUser.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedUser.Password),
                Phone = "",
                Location = seedUser.Location,
                Role = seedUser.Role,
                AccountStatus = AccountStatus.ACTIVE
            });
            continue;
        }

        user.FirstName = seedUser.FirstName;
        user.LastName = seedUser.LastName;
        user.Username = seedUser.Username;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedUser.Password);
        user.Location = seedUser.Location;
        user.Role = seedUser.Role;
        user.AccountStatus = AccountStatus.ACTIVE;
    }

    db.SaveChanges();

    if (!db.Tickets.Any())
    {
        var clientId = db.Users.First(u => u.Email == "client@test.com").UserId;
        var johnId = db.Users.First(u => u.Email == "john@test.com").UserId;
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
            
            // John Doe's tickets
            new Ticket { Title = "Problem sa optikom", Description = "Optički kabal je oštećen ispred kuće.", CreatedDate = DateTime.UtcNow.AddHours(-2), Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatorId = johnId },
            new Ticket { Title = "Dupli račun za maj", Description = "Dobio sam dva računa za isti mjesec.", CreatedDate = DateTime.UtcNow.AddDays(-1), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.BILLING, CreatorId = johnId },
            new Ticket { Title = "Nema Arena Sport kanala", Description = "Od jutros ne vidim Arena Sport kanale u listi.", CreatedDate = DateTime.UtcNow.AddHours(-5), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.TV, CreatorId = johnId },
            new Ticket { Title = "Spor mobilni internet", Description = "4G brzina je izuzetno mala u centru grada.", CreatedDate = DateTime.UtcNow.AddDays(-2), Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = johnId },
            new Ticket { Title = "Konfiguracija WiFi ekstendera", Description = "Trebam pomoć pri uparivanju novog WiFi ekstendera.", CreatedDate = DateTime.UtcNow.AddDays(-3), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.TECHNICAL_SUPPORT, CreatorId = johnId }
        );
        db.SaveChanges();
    }

    if (!db.Teams.Any())
    {
        db.Teams.AddRange(
            new Team { TeamName = "Internet Tim", Description = "Agenti specijalizovani za probleme s internetom.", TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.INTERNET },
            new Team { TeamName = "TV Tim", Description = "Agenti specijalizovani za TV probleme.", TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.TV },
            new Team { TeamName = "Mobilni Tim", Description = "Agenti specijalizovani za mobilnu mrežu.", TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.MOBILE_NETWORK },
            new Team { TeamName = "Naplata Tim", Description = "Agenti specijalizovani za račune i naplatu.", TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.BILLING },
            new Team { TeamName = "Tehnička Podrška Tim", Description = "Agenti specijalizovani za tehničku podršku.", TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.TECHNICAL_SUPPORT },
            new Team { TeamName = "Tehničari Tim", Description = "Terenski tehničari.", TeamType = TeamType.TECHNICIANS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = null }
        );
        db.SaveChanges();
    }

    if (!db.Users.Any(u => u.Role == Role.AGENT && u.TeamId != null))
    {
        var internetTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.INTERNET).TeamId;
        var tvTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.TV).TeamId;
        var mobileTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.MOBILE_NETWORK).TeamId;
        var billingTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.BILLING).TeamId;
        var techSupportTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.TECHNICAL_SUPPORT).TeamId;

        db.Users.AddRange(
            // Internet Tim
            new User { FirstName = "Amina", LastName = "Hodžić", Email = "amina.hodzic@telecom.ba", Username = "amina.hodzic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = internetTeamId },
            new User { FirstName = "Emir", LastName = "Kovač", Email = "emir.kovac@telecom.ba", Username = "emir.kovac", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = internetTeamId },
            new User { FirstName = "Lejla", LastName = "Softić", Email = "lejla.softic@telecom.ba", Username = "lejla.softic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = internetTeamId },
            // TV Tim
            new User { FirstName = "Dino", LastName = "Muratović", Email = "dino.muratovic@telecom.ba", Username = "dino.muratovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = tvTeamId },
            new User { FirstName = "Sara", LastName = "Begić", Email = "sara.begic@telecom.ba", Username = "sara.begic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = tvTeamId },
            new User { FirstName = "Haris", LastName = "Čolić", Email = "haris.colic@telecom.ba", Username = "haris.colic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = tvTeamId },
            // Mobilni Tim
            new User { FirstName = "Maja", LastName = "Halilović", Email = "maja.halilovic@telecom.ba", Username = "maja.halilovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.MOSTAR, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = mobileTeamId },
            new User { FirstName = "Tarik", LastName = "Džanić", Email = "tarik.dzanic@telecom.ba", Username = "tarik.dzanic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.MOSTAR, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = mobileTeamId },
            new User { FirstName = "Nela", LastName = "Selimović", Email = "nela.selimovic@telecom.ba", Username = "nela.selimovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.MOSTAR, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = mobileTeamId },
            // Naplata Tim
            new User { FirstName = "Kenan", LastName = "Imamović", Email = "kenan.imamovic@telecom.ba", Username = "kenan.imamovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.BANJA_LUKA, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = billingTeamId },
            new User { FirstName = "Alma", LastName = "Karić", Email = "alma.karic@telecom.ba", Username = "alma.karic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.BANJA_LUKA, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = billingTeamId },
            new User { FirstName = "Nermin", LastName = "Zukić", Email = "nermin.zukic@telecom.ba", Username = "nermin.zukic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.BANJA_LUKA, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = billingTeamId },
            // Tehnička Podrška Tim
            new User { FirstName = "Irma", LastName = "Spahić", Email = "irma.spahic@telecom.ba", Username = "irma.spahic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.TUZLA, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techSupportTeamId },
            new User { FirstName = "Adnan", LastName = "Mešić", Email = "adnan.mesic@telecom.ba", Username = "adnan.mesic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.TUZLA, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techSupportTeamId },
            new User { FirstName = "Belma", LastName = "Fočo", Email = "belma.foco@telecom.ba", Username = "belma.foco", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"), Phone = "", Location = Location.TUZLA, Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techSupportTeamId }
        );
        db.SaveChanges();
    }

    if (!db.Users.Any(u => u.Role == Role.TECHNICIAN))
    {
        var techTeamId = db.Teams.First(t => t.TeamType == TeamType.TECHNICIANS).TeamId;

        db.Users.AddRange(
            new User { FirstName = "Mirza", LastName = "Omerović", Email = "mirza.omerovic@telecom.ba", Username = "mirza.omerovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Damir", LastName = "Čaušević", Email = "damir.causevic@telecom.ba", Username = "damir.causevic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.SARAJEVO, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Vedran", LastName = "Bajrić", Email = "vedran.bajric@telecom.ba", Username = "vedran.bajric", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.MOSTAR, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Jasmina", LastName = "Hadžić", Email = "jasmina.hadzic@telecom.ba", Username = "jasmina.hadzic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.MOSTAR, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Sanel", LastName = "Petrović", Email = "sanel.petrovic@telecom.ba", Username = "sanel.petrovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.BANJA_LUKA, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Azra", LastName = "Numanović", Email = "azra.numanovic@telecom.ba", Username = "azra.numanovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.TUZLA, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Elmir", LastName = "Hadžić", Email = "elmir.hadzic@telecom.ba", Username = "elmir.hadzic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.ZENICA, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Amra", LastName = "Babić", Email = "amra.babic@telecom.ba", Username = "amra.babic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.BIJELJINA, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Igor", LastName = "Lukić", Email = "igor.lukic@telecom.ba", Username = "igor.lukic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.BRCKO, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Edin", LastName = "Džeko", Email = "edin.dzeko@telecom.ba", Username = "edin.dzeko", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.BIHAC, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Milan", LastName = "Stanković", Email = "milan.stankovic@telecom.ba", Username = "milan.stankovic", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.PRIJEDOR, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId },
            new User { FirstName = "Zoran", LastName = "Petrić", Email = "zoran.petric@telecom.ba", Username = "zoran.petric", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"), Phone = "", Location = Location.DOBOJ, Role = Role.TECHNICIAN, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, TeamId = techTeamId }
        );
        db.SaveChanges();
    }

    var seededFaqs = new[]
    {
        new Faq { Question = "Kako resetovati ruter?", Answer = "Isključite ruter 30 sekundi, uključite i sačekajte da se LED indikatori stabilizuju.", Category = "Internet", SortOrder = 1, IsActive = true, CreatedDate = DateTime.UtcNow },
        new Faq { Question = "Internet je spor", Answer = "Provjerite kablove, restartujte ruter i testirajte brzinu; ako problem ostaje, prijavite tiket.", Category = "Internet", SortOrder = 2, IsActive = true, CreatedDate = DateTime.UtcNow },
        new Faq { Question = "TV signal nestaje", Answer = "Provjerite HDMI/koaksijalni kabl i ponovo pokrenite STB uređaj.", Category = "TV", SortOrder = 3, IsActive = true, CreatedDate = DateTime.UtcNow },
        new Faq { Question = "Nema signala na mobilnoj mreži", Answer = "Uključite/isključite avion režim i probajte SIM u drugom telefonu.", Category = "Mobilna mreža", SortOrder = 4, IsActive = true, CreatedDate = DateTime.UtcNow },
        new Faq { Question = "Pogrešan iznos na računu", Answer = "Provjerite detalje računa; ako stavka nije jasna, otvorite tiket.", Category = "Računi", SortOrder = 5, IsActive = true, CreatedDate = DateTime.UtcNow },
        new Faq { Question = "Kako otvoriti novi tiket?", Answer = "Izaberite Kreiraj tiket, popunite obavezna polja i pošaljite zahtjev.", Category = "Tiketi", SortOrder = 6, IsActive = true, CreatedDate = DateTime.UtcNow }
    };

    foreach (var seededFaq in seededFaqs)
    {
        var existingFaq = db.Faqs.FirstOrDefault(faq => faq.SortOrder == seededFaq.SortOrder);

        if (existingFaq is null)
        {
            db.Faqs.Add(seededFaq);
            continue;
        }

        existingFaq.Question = seededFaq.Question;
        existingFaq.Answer = seededFaq.Answer;
        existingFaq.Category = seededFaq.Category;
        existingFaq.IsActive = seededFaq.IsActive;
    }

    db.SaveChanges();

    // US-25, US-53, US-54: Seed TicketUser assignments (automatska dodjela tiketa agentima)
    if (!db.Set<TicketUser>().Any())
    {
        var internetTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.INTERNET).TeamId;
        var tvTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.TV).TeamId;
        var mobileTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.MOBILE_NETWORK).TeamId;
        var billingTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.BILLING).TeamId;
        var techSupportTeamId = db.Teams.First(t => t.SpecializedCategory == ProblemCategory.TECHNICAL_SUPPORT).TeamId;

        var aminaId = db.Users.First(u => u.Email == "amina.hodzic@telecom.ba").UserId;
        var emerId = db.Users.First(u => u.Email == "emir.kovac@telecom.ba").UserId;
        var dinoId = db.Users.First(u => u.Email == "dino.muratovic@telecom.ba").UserId;
        var saraId = db.Users.First(u => u.Email == "sara.begic@telecom.ba").UserId;
        var majaId = db.Users.First(u => u.Email == "maja.halilovic@telecom.ba").UserId;
        var irmaId = db.Users.First(u => u.Email == "irma.spahic@telecom.ba").UserId;
        var kenanId = db.Users.First(u => u.Email == "kenan.imamovic@telecom.ba").UserId;

        var allTickets = db.Tickets.ToList();

        // Dodjeljivanje tiketa agentima prema kategoriji
        var assignments = new List<TicketUser>();

        if (allTickets.Count > 0)
        {
            var internetTickets = allTickets.Where(t => t.ProblemCategory == ProblemCategory.INTERNET).ToList();
            var tvTickets = allTickets.Where(t => t.ProblemCategory == ProblemCategory.TV).ToList();
            var mobileTickets = allTickets.Where(t => t.ProblemCategory == ProblemCategory.MOBILE_NETWORK).ToList();
            var billingTickets = allTickets.Where(t => t.ProblemCategory == ProblemCategory.BILLING).ToList();
            var techSupportTickets = allTickets.Where(t => t.ProblemCategory == ProblemCategory.TECHNICAL_SUPPORT).ToList();

            // Distribuiraj Internet tikete agentima
            for (int i = 0; i < internetTickets.Count; i++)
            {
                var agentId = i % 2 == 0 ? aminaId : emerId;
                assignments.Add(new TicketUser
                {
                    TicketId = internetTickets[i].TicketId,
                    UserId = agentId,
                    TeamId = internetTeamId,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.AUTOMATIC,
                    Note = "Automatska dodjela prema kategoriji tiketa"
                });
            }

            // Distribuiraj TV tikete agentima
            for (int i = 0; i < tvTickets.Count; i++)
            {
                var agentId = i % 2 == 0 ? dinoId : saraId;
                assignments.Add(new TicketUser
                {
                    TicketId = tvTickets[i].TicketId,
                    UserId = agentId,
                    TeamId = tvTeamId,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.AUTOMATIC,
                    Note = "Automatska dodjela prema kategoriji tiketa"
                });
            }

            // Distribuiraj Mobile tikete agentima
            for (int i = 0; i < mobileTickets.Count; i++)
            {
                var agentId = majaId;
                assignments.Add(new TicketUser
                {
                    TicketId = mobileTickets[i].TicketId,
                    UserId = agentId,
                    TeamId = mobileTeamId,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.AUTOMATIC,
                    Note = "Automatska dodjela prema kategoriji tiketa"
                });
            }

            // Distribuiraj Billing tikete agentima
            for (int i = 0; i < billingTickets.Count; i++)
            {
                var agentId = kenanId;
                assignments.Add(new TicketUser
                {
                    TicketId = billingTickets[i].TicketId,
                    UserId = agentId,
                    TeamId = billingTeamId,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.AUTOMATIC,
                    Note = "Automatska dodjela prema kategoriji tiketa"
                });
            }

            // Distribuiraj Tech Support tikete agentima
            for (int i = 0; i < techSupportTickets.Count; i++)
            {
                var agentId = irmaId;
                assignments.Add(new TicketUser
                {
                    TicketId = techSupportTickets[i].TicketId,
                    UserId = agentId,
                    TeamId = techSupportTeamId,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.AUTOMATIC,
                    Note = "Automatska dodjela prema kategoriji tiketa"
                });
            }

            if (assignments.Count > 0)
            {
                db.Set<TicketUser>().AddRange(assignments);
                db.SaveChanges();
            }
        }
    }

    // PB-52 / US-76: Seed kataloga paketa (admin-managed)
    if (!db.CatalogPackages.Any())
    {
        db.CatalogPackages.AddRange(
            new CatalogPackage
            {
                Name = "Internet Start 100 Mbps",
                Type = PackageType.INTERNET,
                Description = "Optički internet za svakodnevne potrebe — streaming, video pozivi, lagano gaming.",
                Price = 29.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "Internet Premium 1 Gbps",
                Type = PackageType.INTERNET,
                Description = "Gigabitni optički internet za najzahtjevnije korisnike i timski rad od kuće.",
                Price = 59.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "TV Basic",
                Type = PackageType.TV,
                Description = "Standardna ponuda kanala uključujući domaće i informativne kanale.",
                Price = 14.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "TV Premium",
                Type = PackageType.TV,
                Description = "Bogata ponuda HD kanala — sport, film, dječji i strani sadržaj.",
                Price = 24.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "Mobilni M",
                Type = PackageType.MOBILE,
                Description = "Mjesečni paket sa minutama, SMS porukama i mobilnim podacima.",
                Price = 19.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "Mobilni L Unlimited",
                Type = PackageType.MOBILE,
                Description = "Neograničeni razgovori, SMS i mobilni podaci na cijeloj BH mreži.",
                Price = 34.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "Duo paket Internet + TV",
                Type = PackageType.BUNDLE,
                Description = "Kombinacija brzog interneta i kvalitetne TV ponude — povoljnije nego zasebno.",
                Price = 49.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "All-in-One paket",
                Type = PackageType.BUNDLE,
                Description = "Sve usluge u jednom — najbrži internet, kompletna TV ponuda i neograničena mobilna.",
                Price = 79.90m,
                Status = PackageStatus.ACTIVE,
            },
            new CatalogPackage
            {
                Name = "Internet Legacy ADSL",
                Type = PackageType.INTERNET,
                Description = "Stari ADSL paket — više nije dostupan za nove klijente.",
                Price = 19.90m,
                Status = PackageStatus.INACTIVE,
            }
        );
        db.SaveChanges();
    }

    // PB-52: PB-21 legacy seed uklonjen. Pretplate klijenata se sada dodjeljuju
    // isključivo preko admin sekcije "Pretplate" (US-77).
    // Stare SubscriptionPackages rows (ako postoje iz prethodnih runova) — obriši ih
    // da klijentski view bude prazan po default-u, kako bismo izbjegli zbunjujuće
    // duple stavke iz dvaju izvora.
    var legacyRows = db.SubscriptionPackages.ToList();
    if (legacyRows.Count > 0)
    {
        db.SubscriptionPackages.RemoveRange(legacyRows);
        db.SaveChanges();
    }

    TelecomSupportSystem.API.AuditLogSeed.Seed(db);
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<NotificationHub>("/notificationhub");

app.Run();

public partial class Program { }
