# Architecture / Technical Overview

## Naziv aplikacije

TelecomSupportSystem — Helpdesk i Ticketing sistem za telekom okruženje

---

## Komponente sistema

Sistem se sastoji od četiri servisa koji rade zajedno unutar Docker Compose okruženja:

| Servis | Tehnologija | Port |
|---|---|---|
| Frontend | React 19 + Vite + Nginx | 80 |
| Backend API | ASP.NET Core 10.0 | 8080 (interni), 5000 (lokalni razvoj) |
| MCP Server | Node.js 20 + TypeScript | 3001 |
| Baza podataka | Microsoft SQL Server 2019 | 1433 |

---

## Dijagram komunikacije

```
Browser
  │
  ▼ HTTP:80
Frontend (Nginx)
  │
  ├─► /api, /chathub, /notificationhub  → Backend API (api:8080)
  │
  └─► Statički fajlovi (React SPA)

Backend API (api:8080)
  │
  ├─► SQL Server (sqlserver:1433)   — Entity Framework Core (ORM)
  └─► MCP Server (mcp-server:3001)  — AI Copilot funkcionalnosti

MCP Server (mcp-server:3001)
  └─► SQL Server (sqlserver:1433)   — Read-only SQL pristup
```

Svi servisi komuniciraju unutar interne Docker Compose mreže. SQL Server nije izložen prema van.

---

## Frontend

**Tehnologije:** React 19, Vite, Tailwind CSS, Axios, Zustand, Recharts, SignalR JS klijent

**Lokacija koda:** `Project/frontend/src/`

**Struktura:**

| Direktorij | Sadržaj |
|---|---|
| `src/components/` | Zajedničke UI komponente (Sidebar, Header, AppLayout) |
| `src/pages/` | Stranice po rolama i funkcionalnostima |
| `src/services/` | API servisni sloj (axios pozivi prema backendu) |
| `src/context/` | AuthContext — globalno stanje autentifikacije |
| `src/store/` | Zustand store (uiStore za AI panel i alert stanje) |
| `src/test/` | Vitest unit testovi |

**Autentifikacija:** JWT token pohranjen u localStorage; Axios interceptor automatski dodaje `Authorization: Bearer` header na svaki zahtjev i osvježava token putem refresh token mehanizma.

**Real-time:** SignalR klijent se spaja na `/notificationhub` i `/chathub` za primanje notifikacija u realnom vremenu.

---

## Backend

**Tehnologije:** ASP.NET Core 10.0, Entity Framework Core, SQL Server, SignalR, xUnit

**Lokacija koda:** `Project/TelecomSupportSystem/`

**Arhitektura:** Troslojna Service-Repository arhitektura

| Sloj | Opis |
|---|---|
| Controllers | REST API endpointi; role-based authorization atributi |
| Services | Poslovna logika; jedino mjesto gdje se donose odluke |
| Repositories | Pristup bazi putem EF Core; bez direktnog DbContext-a u servisima |
| DTOs | Modeli za razmjenu podataka (ulaz/izlaz API-ja) |

**Ključni moduli:**

| Modul | Kontroler | Opis |
|---|---|---|
| Auth | `AuthController` | Login, refresh token, rate limiting |
| Tickets | `TicketController` | CRUD tiketa, dodjela, prosljeđivanje, zatvaranje |
| Users | `UserController` | Upravljanje korisnicima, deaktivacija, statistika |
| Teams | `TeamController` | Upravljanje timovima, availability status |
| Packages | `PackageCatalogController` | Katalog paketa i pretplate |
| Reports | `ReportController` | Administrativni izvještaji (7 tipova) |
| AI | `AIController` | AI prijedlozi odgovora, AI Insights |
| MCP Copilot | `AdminCopilotController` | Posreduje između frontenda i MCP servera |
| Audit | `AuditLogController` | Read-only pregled audit loga |
| Attachments | `AttachmentController` | Upload i download fajlova |
| SLA | `SlaService` | Izračun SLA rokova i statusa po prioritetu |
| Notifications | `NotificationHub` (SignalR) | Real-time notifikacije korisnicima |

**Baza podataka:** EF Core Code-First s migracijama. Migracije se automatski primjenjuju pri startu aplikacije uz retry mehanizam (10 pokušaja × 3 sec pauze).

**Seed podaci:** Automatski se upisuju pri prvom pokretanju u Development okruženju (testni korisnici, timovi, paketi, FAQ).

---

## MCP Server

**Tehnologije:** Node.js 20, TypeScript, `@modelcontextprotocol/sdk`, Groq API

**Lokacija koda:** `Project/mcp-server/`

**Uloga:** Zaseban read-only posrednički servis koji backendu daje strukturirane podatke iz baze i prosljeđuje ih LLM modelu (Groq).

**Dostupni MCP alati:**

| Alat | Opis |
|---|---|
| `ticket.search` | Pretraga tiketa po ključnim riječima, statusu, prioritetu |
| `ticket.analytics` | Agregatne statistike tiketa |
| `team.workload` | Opterećenje timova i agenata |
| `faq.search` | Pretraga FAQ sadržaja |

MCP server je ograničen na read-only operacije i ne smije mijenjati podatke.

---

## Baza podataka

**Tehnologija:** Microsoft SQL Server 2019

**ORM:** Entity Framework Core (Code-First, migracije)

**Ključne tabele:**

| Tabela | Opis |
|---|---|
| `Users` | Korisnici svih uloga |
| `Tickets` | Tiketi s punim životnim ciklusom |
| `Comments` | Poruke i interni komentari na tiketima |
| `Notifications` | Notifikacije vezane za tikete |
| `Teams` | Timovi agenata i tehničara |
| `CatalogPackages` | Katalog telekom paketa |
| `ClientSubscriptions` | Pretplate klijenata |
| `AuditLogs` | Neizmjenjiv log aktivnosti |
| `Attachments` | Metapodaci o uploadovanim fajlovima |
| `FAQItems` | FAQ sadržaj |

**Napomena:** Legacy tabela `SubscriptionPackages` ostala je u šemi i preporučeno je njeno uklanjanje u budućim migracijama.

---

## Gdje se nalazi ključni kod

| Funkcionalnost | Lokacija |
|---|---|
| JWT autentifikacija | `TelecomSupportSystem.API/Services/AuthService.cs` |
| Ticket workflow | `TelecomSupportSystem.API/Services/TicketService.cs` |
| SLA logika | `TelecomSupportSystem.API/Services/SlaService.cs` |
| AI prijedlozi | `TelecomSupportSystem.API/Services/AIService.cs` |
| MCP Copilot | `TelecomSupportSystem.API/Services/AdminCopilotService.cs` |
| SignalR hub | `TelecomSupportSystem.API/Hubs/NotificationHub.cs` |
| EF migracije | `TelecomSupportSystem.Infrastructure/Migrations/` |
| MCP server alati | `Project/mcp-server/src/tools/` |
| Frontend stranice | `Project/frontend/src/pages/` |
| Frontend testovi | `Project/frontend/src/test/` |
| Backend testovi | `Project/TelecomSupportSystem.Tests/` |
| GitHub Actions CI | `.github/workflows/ci.yml` |
| GitHub Actions CD | `.github/workflows/deploy.yml` |

---

## Sigurnosne odluke

| Odluka | Detalji |
|---|---|
| JWT + Refresh Token | Kratki pristupni token (access) + dugi refresh token za osvježavanje bez ponovne prijave |
| JWT ključ u environment varijabli | `JWT_KEY` nije u konfiguracijskom fajlu nego u `.env` / GitHub Secrets |
| Role-based authorization | `[Authorize(Roles = "...")]` na svakom endpointu koji zahtijeva specifičnu ulogu |
| Rate limiting | Login endpoint je zaštićen rate limitingom radi sprečavanja brute-force napada |
| Generička poruka greške pri loginu | Sistem ne otkriva da li je email ili lozinka pogrešna |
| Whitelist validacija fajlova | Upload dozvoljava samo PNG, JPG, JPEG, PDF, DOCX, TXT do 5 MB; izvršne datoteke su zabranjene |
| Path traversal zaštita | Nazivi fajlova se sanitiziraju pri uploadu |
| MCP Server read-only | MCP server nema write privilegije na bazi; ne može izvršavati administratorske akcije |
| Deaktivirani korisnici | Dobijaju 401 pri svakom pokušaju prijave |
| Audit log neizmjenjiv | Nema API endpointa za brisanje ili izmjenu audit log zapisa |

---

## Vanjske zavisnosti

| Servis | Uloga |
|---|---|
| Groq API (`GROQ_API_KEY`) | LLM za AI prijedloge odgovora (agenti/tehničari) i AI Insights (admin) |
| Groq API (`GROQ_API_KEY_2`) | Zaseban ključ za MCP Admin Copilot |
| Docker Hub | Pohrana Docker image-a za produkcijski deployment |
| GitHub Actions | CI/CD pipeline |
