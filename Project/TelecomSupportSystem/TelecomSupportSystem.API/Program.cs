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
builder.Services.AddScoped<IFaqRepository, FaqRepository>();

// Services
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFaqService, FaqService>();

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

    if (!db.Tickets.Any())
    {
        var clientId = db.Users.First(u => u.Email == "client@test.com").UserId;
        db.Tickets.AddRange(
            new Ticket { Title = "Internet ne radi", Description = "Nemam internet konekciju od jutros.", CreatedDate = DateTime.UtcNow.AddDays(-3), Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatorId = clientId },
            new Ticket { Title = "Spor internet", Description = "Brzina je puno manja od ugovorene.", CreatedDate = DateTime.UtcNow.AddDays(-5), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.INTERNET, CreatorId = clientId },
            new Ticket { Title = "TV signal isčezava", Description = "Slika se gubi svakih par minuta.", CreatedDate = DateTime.UtcNow.AddDays(-7), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-2), Priority = Priority.LOW, ProblemCategory = ProblemCategory.TV, CreatorId = clientId },
            new Ticket { Title = "Pogrešan iznos na računu", Description = "Naplaćena mi je usluga koju nisam naručio.", CreatedDate = DateTime.UtcNow.AddDays(-1), Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.BILLING, CreatorId = clientId },
            new Ticket { Title = "Mobilna mreža bez signala", Description = "Nema signala u mom kvartu već 2 dana.", CreatedDate = DateTime.UtcNow.AddDays(-2), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = clientId },
            new Ticket { Title = "Ne mogu uputiti poziv", Description = "Pozivi ne prolaze, javlja se greška.", CreatedDate = DateTime.UtcNow.AddDays(-4), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-1), Priority = Priority.HIGH, ProblemCategory = ProblemCategory.MOBILE_NETWORK, CreatorId = clientId },
            new Ticket { Title = "Tehnička podrška za ruter", Description = "Trebam pomoć sa konfiguracijom rutera.", CreatedDate = DateTime.UtcNow.AddDays(-6), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-3), Priority = Priority.LOW, ProblemCategory = ProblemCategory.TECHNICAL_SUPPORT, CreatorId = clientId },
            new Ticket { Title = "TV aplikacija ne radi", Description = "Ne mogu pristupiti TV aplikaciji.", CreatedDate = DateTime.UtcNow.AddDays(-8), Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.TV, CreatorId = clientId },
            new Ticket { Title = "Prekid usluge bez obavijesti", Description = "Usluga je prekinuta bez ikakve najave.", CreatedDate = DateTime.UtcNow.AddDays(-9), Status = TicketStatus.CLOSED, ClosedDate = DateTime.UtcNow.AddDays(-5), Priority = Priority.HIGH, ProblemCategory = ProblemCategory.TECHNICAL_SUPPORT, CreatorId = clientId }
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
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
