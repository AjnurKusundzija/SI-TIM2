#CLAUDE.md

This file provides guidance to Claude code when working in this repository.

## Project Overview

Helpdesk and ticketing system The system should enable reporting problems, categorization of requests, assignment of tickets to responsible persons, status monitoring, comments, recording solutions, and basic reporting.

## Tech Stack

### Backend

- Framework: ASP.NET Core 10 — RESTful Web API
- Language: C# (.NET 10)
- Architecture: 3-layer (API → BLL → DAL)
- ORM: Entity Framework Core 10 — pristup bazi podataka 
- Database: SQL Server / MySQL 8.0+
- Authentication: JWT Bearer tokens (+ refresh and session) + BCrypt.Net-Next za hashovanje lozinki
- Real-time: SignalR
- API documentation: Swagger / OpenAPI (Swashbuckle)

### Frontend

- Framework: React 19
- Language: JavaScript (JSX) and TypeScript
- Build tool: Vite 8
- Routing: React Router DOM 7
- Stilizacija: TailwindCSS 4 + custom CSS in index.css
- HTTP client: Axios
- State management: Zustand
- Forms: React Hook Form
- Real-time: Socket.io-client / @microsoft/signalr
- Package manager: npm
- Testing: Vitest + Testing Library

### DevOps

- Containerization: Docker + Docker Compose
- CI/CD: GitHub Actions
- Version control: Git (GitFlow — main / develop / feature grane)

## Development Commands
 
### Backend
```bash
# Run the API (from solution root)
dotnet run --project TelecomSupportSystem.API
 
# Build entire solution
dotnet build
 
# Restore NuGet packages
dotnet restore
 
# Run tests
dotnet test
 
# EF Core migrations
dotnet ef migrations add <MigrationName> --project TelecomSupportSystem.DAL --startup-project TelecomSupportSystem.API
dotnet ef database update --project TelecomSupportSystem.DAL --startup-project TelecomSupportSystem.API
```
 
### Frontend
```bash
# Install dependencies
npm install
 
# Start dev server (http://localhost:5173)
npm run dev
 
# Production build
npm run build
 
# Run tests
npm run test
 
# Preview production build
npm run preview
```
 
### Docker
```bash
# Start all services
docker-compose up
 
# Build and start
docker-compose up --build
 
# Stop
docker-compose down
```
 
---
 
## Project Structure
 
```
Project/
├── TelecomSupportSystem/               # Backend solution
│   ├── TelecomSupportSystem.API/       # Controllers, Program.cs, Middleware
│   │   ├── Controllers/                # One controller per domain (AuthController, TicketController…)
│   │   ├── Middleware/
│   │   ├── Program.cs                  # DI registration, middleware pipeline
│   │   └── appsettings.json            # DB connection string, JWT config
│   │
│   ├── TelecomSupportSystem.BLL/       # Business Logic Layer
│   │   ├── DTOs/
│   │   │   ├── Auth/                   # LoginRequestDto, LoginResponseDto
│   │   │   └── Tickets/                # CreateTicketDto, GetTicketDto…
│   │   └── Services/
│   │       ├── Interfaces/             # IAuthService, ITicketService…
│   │       └── *.cs                    # AuthService, TicketService…
│   │
│   └── TelecomSupportSystem.DAL/       # Data Access Layer
│       ├── Entities/
│       │   ├── Enums/                  # Role, TicketStatus, Priority, ProblemCategory…
│       │   └── *.cs                    # User, Ticket, Team, Comment…
│       ├── Repositories/
│       │   ├── Interfaces/             # IUserRepository, ITicketRepository…
│       │   └── *.cs                    # UserRepository, TicketRepository…
│       └── ApplicationDbContext.cs
│
└── frontend/                           # React app
    ├── src/
    │   ├── services/
    │   │   ├── api.js                  # Axios instance + JWT interceptor
    │   │   ├── authService.js          # login(), logout(), getUser(), isAuthenticated()
    │   │   └── tiketService.js         # createTiket(), getUserTiketi()
    │   ├── context/
    │   │   └── AuthContext.jsx         # AuthProvider + useAuth() hook
    │   ├── components/
    │   │   ├── ProtectedRoute.jsx      # Redirects to /login if not authenticated
    │   │   └── Navbar.jsx
    │   ├── pages/
    │   │   ├── Login.jsx
    │   │   ├── Dashboard.jsx
    │   │   └── NoviTiket.jsx
    │   ├── App.jsx                     # Router + AuthProvider wrapping
    │   ├── main.jsx
    │   └── index.css                   # Global styles + CSS variables
    ├── vite.config.js                  # Proxy: /api → http://localhost:5000
    └── package.json
```
 
---
 
## Architecture Rules
 
### Backend — strict 3-layer flow
```
Request → Controller (API) → Service (BLL) → Repository (DAL) → Database
```
- **Controllers** — only handle HTTP: parse input, call service, return response. No business logic.
- **Services (BLL)** — all business logic lives here. Never access DbContext directly.
- **Repositories (DAL)** — only EF Core queries. No business logic.
- **DTOs** — always use DTOs for API input/output. Never expose entity classes directly.
### Frontend — API communication
- All HTTP calls go through `src/services/api.js` (Axios instance with base URL `/api`)
- Vite proxies `/api` → `http://localhost:5000` in dev
- JWT token stored in `sessionStorage` (cleared on browser close)
- All protected routes wrapped in `<ProtectedRoute>`
- Auth state managed via `AuthContext` — use `useAuth()` hook in components
---
 
## Code Conventions
 
### C# (Backend)
- **Classes, methods, properties:** `PascalCase` — `AuthService`, `LoginAsync`, `UserId`
- **Private fields:** `_camelCase` — `_userRepository`, `_configuration`
- **Local variables, parameters:** `camelCase` — `loginDto`, `userId`
- **Async methods:** always suffix with `Async` — `GetByEmailAsync`, `CreateTicketAsync`
- **Interfaces:** prefix with `I` — `IAuthService`, `IUserRepository`
- **Enums:** `SCREAMING_SNAKE_CASE` values — `OPEN`, `HIGH`, `MOBILE_NETWORK` (matches existing enums)
- **Namespaces:** match folder structure exactly — `TelecomSupportSystem.BLL.Services`
- One class per file. Filename matches class name.
- Use `async/await` throughout — never `.Result` or `.Wait()`
- Nullable reference types enabled — handle nulls explicitly
### JavaScript/JSX (Frontend)
- **Components:** `PascalCase` files and function names — `NoviTiket.jsx`, `AuthContext.jsx`
- **Services, hooks, utilities:** `camelCase` files — `authService.js`, `api.js`
- **Variables, functions:** `camelCase`
- **Constants:** `SCREAMING_SNAKE_CASE` for static lookup arrays — `KATEGORIJE`, `PRIORITETI`
- One component per file
- Use functional components + hooks only — no class components
- Prefer named exports for utilities; default export for page/component files

### Branch Naming (GitFlow)
```
feature/ticket-creation        # PB-22
feature/auth-login             # PB-19
bugfix/jwt-refresh-loop
hotfix/security-patch-jwt
release/v1.0.0
```
- Always branch from `develop`, not `main`
- Open a PR — never push directly to `develop` or `main`
- Minimum 1 reviewer approval required before merge
- Merge strategy: "Create a merge commit"
---
 
## What NOT to Do
 
### Backend
- ❌ Do not put business logic in Controllers — move it to BLL services
- ❌ Do not inject `ApplicationDbContext` into Controllers or BLL services — use repositories
- ❌ Do not return entity classes from API endpoints — always map to DTOs
- ❌ Do not use `.Result` or `.Wait()` on async calls — causes deadlocks
- ❌ Do not commit `appsettings.json` with real credentials — use environment variables or secrets
- ❌ Do not skip `[Authorize]` on protected endpoints
- ❌ Do not reveal whether email or password is wrong in auth error messages (US-3 requirement)
### Frontend
- ❌ Do not call `fetch()` directly — use the Axios instance from `src/services/api.js`
- ❌ Do not store JWT in `localStorage` — use `sessionStorage` (clears on browser close)
- ❌ Do not access protected pages without `<ProtectedRoute>` wrapping
- ❌ Do not import from `../../../` — reorganize into services/context/components instead
- ❌ Do not use `<form>` HTML elements — use `onSubmit` on a `<form>` tag with `e.preventDefault()` or manage state manually
- ❌ Do not use class components — hooks only
### General
- ❌ Do not push to `main` or `develop` directly — always use PRs
- ❌ Do not use `git flow feature finish` — open a PR on GitHub instead
- ❌ Do not add new packages without team discussion — keep dependencies minimal
- ❌ Do not leave console.log statements in committed code
- ❌ Do not bypass the layered architecture for "quick" fixes

---

## Key Domain Enums
 
```csharp
// DAL/Entities/Enums/
enum Role           { CLIENT = 1, AGENT = 2, TECHNICIAN = 3, ADMINISTRATOR = 4 }
enum TicketStatus   { OPEN = 1, CLOSED = 2 }
enum Priority       { LOW = 1, MEDIUM = 2, HIGH = 3 }
enum ProblemCategory{ INTERNET = 1, TV = 2, MOBILE_NETWORK = 3, BILLING = 4, TECHNICAL_SUPPORT = 5 }
enum NotificationType { TICKET_ASSIGNED = 1, TICKET_FORWARDED = 2, STATUS_CHANGED = 3, TICKET_RESPONSE = 4, TICKET_CLOSED = 5 }
```

---

## Auth Flow
 
```
POST /api/auth/login  { email, password }
  → AuthService.LoginAsync()
  → UserRepository.GetByEmailAsync()
  → BCrypt.Verify(password, hash)
  → Generate JWT (claims: NameIdentifier, Email, Role, GivenName, Surname)
  → Return { token, userId, firstName, lastName, email, role }
 
Frontend stores token in sessionStorage.
Axios interceptor attaches: Authorization: Bearer <token>
401 response → auto logout + redirect to /login
```
---

## Preferences
- Ask before committing to git
- Prefer editing existing files over creating new ones
- Run tests after making changes
- Keep code simple — no over-engineering
- Keep components focused and single-purpose
- No unnecessary comments or docstrings

## Workflow
- When something goes sideways, stop and re-plan — don't keep pushing
- After finishing a task: run typecheck, tests, and lint before calling it done

## Style
- Prefer small, focused functions
- Use early returns over nested conditionals