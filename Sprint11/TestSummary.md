# Test Summary / QA izvještaj

## Pregled

Ovaj dokument je završni izvještaj o testiranju TelecomSupportSystem projekta. Pokriva sve sprintove od Sprint 5 do Sprint 11.

| Kategorija | Ukupno |
|---|---|
| Backend test metode ([Fact]/[Theory]) | **507** |
| Frontend test slučajevi (it()) | **348** |
| **Ukupno automatizovanih testova** | **855** |
| Backend test fajlova | 75 |
| Frontend test fajlova | 49 |

---

## 1. Vrste testova

### 1.1 Backend unit testovi (xUnit)

Pišu se korištenjem **xUnit**, **Moq** i **FluentAssertions**.

Pokrivaju: servisnu logiku, validaciju poslovnih pravila, kontrolere (HTTP kodovi, autorizacija), repository operacije i rubne slučajeve.

**Lokacija:** `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/`

### 1.2 Backend integracijski testovi (xUnit + EF InMemory / WebApplicationFactory)

Koriste **EF Core InMemory** provider i **Microsoft.AspNetCore.Mvc.Testing** (`WebApplicationFactory`) za testiranje kompletnog HTTP toka (request → controller → service → in-memory DB → response).

Pokrivaju: cijele use case tokove, role-based pristup, 401/403 scenarije, sekvencijalne operacije (npr. kreiranje tiketa pa zatvaranje).

### 1.3 Backend sigurnosni testovi

Posebni test fajlovi koji provjeravaju da neautorizovani korisnici ne mogu izvršiti zabranjene operacije.

**Lokacija:** `Tests/Security/`, `Tests/Sprint9/UserAccountManagementSecurityTests.cs`

### 1.4 Backend performansni testovi

Provjeravaju da kritični endpointi (login, pregled tiketa, auto-assign) odgovaraju unutar definisanog vremenskog limita.

**Napomena:** `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` je dokumentovan kao flaky u CI okruženju i ne blokira build.

**Lokacija:** `Tests/Performance/`

### 1.5 Backend sistemski testovi

Testiraju kompletne korisničke scenarije kroz više koraka (npr. kreiranje tiketa → dodjela → prosljeđivanje → zatvaranje).

**Lokacija:** `Tests/System/Sprint7UserStoriesSystemTests.cs`, `Tests/Sprint9/Sprint9UserStoriesSystemTests.cs`

### 1.6 Frontend unit testovi (Vitest + Testing Library)

Pišu se korištenjem **Vitest**, **@testing-library/react** i **jsdom** okruženja.

Pokrivaju: renderovanje komponenti, korisničke interakcije (klik, submit, filtriranje), API pozive (mock Axios), role-based prikaz, edge case ponašanje.

**Lokacija:** `Project/frontend/src/test/`

### 1.7 Frontend servisni testovi

Testiraju API sloj (`authService.js`, `ticketService.js`, `auditLog.service.js`, itd.) izolovano od komponenti — mockani Axios, provjera ispravnih endpoint poziva.

**Lokacija:** `Project/frontend/src/test/*.test.js`

### 1.8 Frontend acceptance testovi

Testiraju acceptance kriterije iz user storija kroz simulirane UI interakcije.

**Lokacija:** `Project/frontend/src/test/acceptance/`

### 1.9 Frontend sistemski testovi

Simuliraju kompletne višekoračne korisničke tokove na frontend nivou.

**Lokacija:** `Project/frontend/src/test/system/`

### 1.10 Manualni testovi (UI/UX)

Ručna provjera vizualnog izgleda, real-time ponašanja, MCP Copilot interakcija i funkcionalnosti koje ne mogu biti pouzdano automatizovane (npr. SignalR notifikacije, CSV preuzimanje u browseru).

---

## 2. Kako se testovi pokreću

### Backend — svi testovi

```bash
cd Project/TelecomSupportSystem
dotnet test TelecomSupportSystem.Tests
```

### Backend — filtriranje po domenu

```bash
# Samo Auth testovi
dotnet test TelecomSupportSystem.Tests --filter "FullyQualifiedName~Auth"

# Samo ticket testovi
dotnet test TelecomSupportSystem.Tests --filter "FullyQualifiedName~Ticket"

# Samo Sprint 9 testovi
dotnet test TelecomSupportSystem.Tests --filter "FullyQualifiedName~Sprint9"

# Bez performansnih testova
dotnet test TelecomSupportSystem.Tests --filter "Category!=Performance"
```

### Backend — s code coverage

```bash
dotnet test TelecomSupportSystem.Tests --collect:"XPlat Code Coverage"
# Coverage izvještaj: coverage.cobertura.xml
```

### Frontend — svi testovi

```bash
cd Project/frontend
npx vitest run
```

### Frontend — specifičan fajl

```bash
npx vitest run src/test/Login.test.jsx
npx vitest run src/test/TicketDetail.test.jsx
```

### Frontend — s coverage izvještajem

```bash
npx vitest run --coverage
```

### CI pipeline (automatski)

Svaki push na `main`, `develop`, `feature/**`, `bugfix/**`, `hotfix/**` i `release/**` automatski pokreće sve testove u GitHub Actions CI pipeline-u:

```
.github/workflows/ci.yml
```

Backend koraci: `dotnet restore` → `dotnet build -c Release` → `dotnet test`

Frontend koraci: `npm install --legacy-peer-deps` → `eslint src/` → `vitest run` → `vite build`

---

## 3. Ukupan broj testova po domenima

### Backend (507 test metoda u 75 fajlova)

| Domen | Test fajlovi | Metode |
|---|---|---|
| Autentifikacija i autorizacija | `AuthServiceTests`, `AuthControllerTests`, `EmailOrBiHPhoneAttributeTests`, `AuthIntegrationTests` | **40** |
| Ticket workflow | `TicketServiceTests`, `TicketControllerTests`, `TicketRepositoryTests`, `TicketDetailServiceTests`, `TicketDetailControllerTests`, `TicketDetailIntegrationTests`, `TicketIntegrationTests`, `TicketStatusUpdateTests`, `TicketClosureServiceTests`, `TicketClosureWorkflowTests`, `TicketControllerClosureTests`, `TicketClosureIntegrationTests`, `TicketPriorityServiceTests`, `TicketPriorityIntegrationTests`, `TicketControllerForwardingTests` | **~118** |
| Komunikacija (komentari) | `CommentServiceTests`, `CommentControllerTests`, `CommentIntegrationTests` | **14** |
| Automatska i ručna dodjela tiketa | `AutoAssignServiceTests`, `AutoAssignRepositoryTests`, `SelfAssignServiceTests`, `AutoAssignIntegrationTests`, `SelfAssignIntegrationTests`, `AutoAssignSecurityTests`, `AutoAssignDataIntegrityTests` | **36** |
| Ocjenjivanje tiketa | `RatingServiceTests`, `RatingControllerTests`, `TicketRatingIntegrationTests` | **24** |
| FAQ | `FaqServiceTests`, `FaqControllerTests`, `FaqRepositoryTests`, `FaqAdminCrudTests`, `FaqIntegrationTests`, `FaqAdminCrudIntegrationTests` | **31** |
| Upravljanje korisnicima | `UserRepositoryTests`, `UserAccountManagementServiceTests`, `UserAccountManagementControllerTests`, `UserAccountManagementIntegrationTests`, `UserAccountManagementSecurityTests`, `AdminUserProfileServiceTests` | **66** |
| Izvještaji i admin dashboard | `AdminReportServiceTests`, `ReportServiceTests`, `AdminDashboardServiceTests`, `AdminDashboardIntegrationTests`, `FirstResponseReportTests`, `FirstResponseReportIntegrationTests`, `ReportIntegrationTests` | **47** |
| Audit log | `AuditLogServiceTests`, `AuditLogControllerIntegrationTests` | **10** |
| Prilozi | `AttachmentTests` | **19** |
| Timovi | `TeamManagementServiceTests`, `TeamManagementIntegrationTests` | **11** |
| AI / MCP Copilot | `AdminCopilotServiceTests`, `AdminCopilotControllerTests` | **13** |
| SLA praćenje | `SlaServiceTests` | **12** |
| Sigurnost | `RoleAccessSecurityTests` | **10** |
| Sistemski testovi | `Sprint7UserStoriesSystemTests`, `Sprint9UserStoriesSystemTests` | **11** |
| Performansni testovi | `AuthPerformanceTests`, `TicketPerformanceTests`, `CommentPerformanceTests`, `FaqPerformanceTests`, `AllTicketsPerformanceTests`, `AutoAssignPerformanceTests`, `AutoAssignScalePerformanceTests`, `TicketDetailPerformanceTests`, `AdminDashboardPerformanceTests` | **~9** |
| Ostalo (ChatHub, EmptyServices, AllTickets) | `ChatHubTests`, `EmptyServicesTests`, `AllTickets*Tests`, `AllTicketsControllerTests` | **~16** |
| **Ukupno** | **75 fajlova** | **507** |

### Frontend (348 test slučajeva u 49 fajlova)

| Fajl | Test slučajeva |
|---|---|
| `TicketDetail.test.jsx` | 17 |
| `Sprint9AdminDashboard.test.jsx` | 17 |
| `Faq.test.jsx` | 13 |
| `CreateTicket.test.jsx` | 13 |
| `AuditLogDetailModal.test.jsx` | 13 |
| `ticketService.test.js` | 16 |
| `ticketServiceAdvanced.test.js` | 13 |
| `authService.test.js` | 12 |
| `AuditLogFilters.test.jsx` | 12 |
| `MyTickets.test.jsx` | 11 |
| `auditLog.service.test.js` | 10 |
| `Tickets.test.jsx` | 10 |
| `Sprint9UsersList.test.jsx` | 10 |
| `ReportsDashboard.test.jsx` | 10 |
| `AuditLogPage.test.jsx` | 9 |
| `TicketRating.test.jsx` | 8 |
| `SlaIndicator.test.jsx` | 8 |
| `Login.test.jsx` | 8 |
| `Home.test.jsx` | 8 |
| `Badge.test.jsx` | 8 |
| `AuditLogTable.test.jsx` | 8 |
| `TechnicianDashboard.test.jsx` | 7 |
| `Dashboard.test.jsx` | 7 |
| `ConfirmDialog.test.jsx` | 7 |
| `AttachmentList.test.jsx` | 7 |
| `AdminCopilotPanel.test.jsx` | 7 |
| `StarRating.test.jsx` | 6 |
| `Modal.test.jsx` | 6 |
| `FileUpload.test.jsx` | 6 |
| `faqService.test.js` | 6 |
| `EmptyState.test.jsx` | 5 |
| `Sprint9FirstResponse.test.jsx` | 4 |
| `ProtectedRoute.test.jsx` | 4 |
| `AdminDashboard.test.jsx` | 4 |
| `Reports.test.jsx` | 3 |
| `AuthContext.test.jsx` | 3 |
| `aiService.test.js` | 3 |
| `ticketServiceAttachments.test.js` | 3 |
| `TicketCreateSystem.test.jsx` | 2 |
| `HeaderCopilot.test.jsx` | 2 |
| `FaqUi.test.jsx` | 2 |
| `CommunicationAcceptance.test.jsx` | 2 |
| `AuthSystem.test.jsx` | 2 |
| `AuthAcceptance.test.jsx` | 2 |
| `AssignedTickets.test.jsx` | 2 |
| Acceptance testovi (7 fajlova) | 7 |
| System testovi (6 fajlova) | 6 |
| UI testovi (2 fajla) | 3 |
| **Ukupno** | **348** |

---

## 4. Dokaz rezultata testiranja (CI logovi)

> Detaljan dokaz testiranja za Sprint 11 funkcionalnosti (PB-46, PB-65, PB-67) sa log izlazima i pokrićem acceptance kriterija dostupan je u [ProofOfTesting.md](ProofOfTesting.md).


### Sprint 10 — cjelokupni CI run (kraj Sprint 10)

```
Backend test suite: 
Run dotnet test --no-build -c Release
Test run for /home/runner/work/SI-TIM2/SI-TIM2/Project/TelecomSupportSystem/TelecomSupportSystem.Tests/bin/Release/net10.0/TelecomSupportSystem.Tests.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:   571, Skipped:     0, Total:   571, Duration: 8 s - TelecomSupportSystem.Tests.dll (net10.0)

Frontend test suite:
Run npx vitest run

RUN  v1.6.1 /home/runner/work/SI-TIM2/SI-TIM2/Project/frontend
 Test Files  57 passed (57)
      Tests  348 passed (348)
   Start at  13:29:04
   Duration  38.06s (transform 1.71s, setup 10.64s, collect 11.71s, tests 19.92s, environment 52.40s, prepare 7.67s)

```

---

## 5. Ručno testirani tokovi

| Funkcionalnost | Razlog ručnog testiranja | Verificirano u |
|---|---|---|
| PB-52 Upravljanje katalogom paketa i pretplata | Nema automatizovanih testova — tehnički dug | Sprint 9 Review demo |
| MCP Admin Copilot — chat tok s pitanjima | Groq API vanjska zavisnost; ne može se pouzdano mockati u CI | Sprint 10 Review demo |
| Real-time notifikacije (SignalR) | SignalR konekcija zahtijeva živi WebSocket; jsdom ne podržava | Sprint 8, Sprint 10 |
| File upload — drag & drop, thumbnail prikaz, progress bar | Vizualno ponašanje browsera | Sprint 9 |
| CSV preuzimanje u browseru — svih 7 tipova, otvaranje u Excelu | `URL.createObjectURL` i browser download API | Sprint 11 |
| SLA indikatori na listi tiketa — vizualna boja-kodiranja | Vizualni prikaz u stvarnom okruženju | Sprint 11 |
| Login putem broja telefona — unos +387 formata kroz UI | Provjera forme u produkcijskom okruženju | Sprint 11 |
| AI prijedlog odgovora — kopiranje u poruku, sadržaj prijedloga | LLM output nije deterministički | Sprint 10 |
| AI Insights na dashboardu | LLM output nije deterministički | Sprint 10 |
| Redizajn UI — Sidebar, Header, Dashboard na svim ulogama | Vizualni izgled i konzistentnost dizajna | Sprint 10 |
| Notifikacija putem bell ikone — badge, dropdown, klik na tiket | Interakcija u stvarnom browseru | Sprint 8 |
| Deaktivacija korisnika — zabrana prijave | End-to-end provjera u živom okruženju | Sprint 9 |
| Agent availability status — automatska preraspodjela tiketa | Kompleksno višekorisničko scenarijo | Sprint 10 |

---

## 6. Ključni korisnički tokovi pokriveni testovima

### Tok 1 — Prijava i autorizacija

| Korak | Automatski | Manualno |
|---|---|---|
| Prijava emailom ili brojem telefona | ✓ `AuthServiceTests`, `Login.test.jsx`, `authService.test.js` | ✓ |
| Refresh token mehanizam | ✓ `AuthServiceTests`, `AuthIntegrationTests` | ✓ |
| Odbijanje deaktiviranog korisnika (401) | ✓ `UserAccountManagementSecurityTests` | ✓ |
| Role-based redirect nakon prijave | ✓ `ProtectedRoute.test.jsx`, `AuthAcceptance.test.jsx` | ✓ |

### Tok 2 — Klijent kreira tiket i komunicira

| Korak | Automatski | Manualno |
|---|---|---|
| Kreiranje tiketa | ✓ `TicketServiceTests`, `CreateTicket.test.jsx`, `CreateTicketAcceptance.test.jsx` | ✓ |
| Pregled vlastitih tiketa | ✓ `MyTickets.test.jsx`, `MyTicketsAcceptance.test.jsx` | ✓ |
| Komunikacija kroz tiket | ✓ `CommentServiceTests`, `CommunicationAcceptance.test.jsx` | ✓ |
| Ocjenjivanje zatvorenog tiketa | ✓ `RatingServiceTests`, `RatingControllerTests`, `TicketRating.test.jsx` | ✓ |
| Pregled paketa i pretplata | — | ✓ |

### Tok 3 — Agent obrađuje tiket

| Korak | Automatski | Manualno |
|---|---|---|
| Pregled svih tiketa s filterima | ✓ `AllTickets*Tests`, `Tickets.test.jsx`, `SearchFilterAcceptance.test.jsx` | ✓ |
| Preuzimanje tiketa (Assign to me) | ✓ `SelfAssignServiceTests`, `SelfAssignIntegrationTests` | ✓ |
| Automatska dodjela tiketa | ✓ `AutoAssignServiceTests`, `AutoAssignRepositoryTests`, `AutoAssignIntegrationTests` | ✓ |
| Prosljeđivanje tiketa | ✓ `TicketControllerForwardingTests` | ✓ |
| Upravljanje prioritetima | ✓ `TicketPriorityServiceTests`, `TicketPriorityIntegrationTests` | ✓ |
| SLA provjera po prioritetu | ✓ `SlaServiceTests`, `SlaIndicator.test.jsx` | ✓ |
| Zatvaranje tiketa | ✓ `TicketClosureWorkflowTests`, `TicketClosureIntegrationTests`, `TicketControllerClosureTests` | ✓ |

### Tok 4 — Administrator upravlja sistemom

| Korak | Automatski | Manualno |
|---|---|---|
| Upravljanje korisnicima (CRUD, deaktivacija) | ✓ `UserAccountManagement*Tests` (66 backend + 10 frontend) | ✓ |
| Pregled audit loga s filterima | ✓ `AuditLogServiceTests`, `AuditLog*.test.jsx` (10+52) | ✓ |
| Generisanje izvještaja | ✓ `AdminReportServiceTests`, `ReportsDashboard.test.jsx`, `Reports.test.jsx` | ✓ |
| CSV export izvještaja | ✓ `Sprint9AdminDashboard.test.jsx` | ✓ |
| FAQ CRUD | ✓ `FaqAdminCrudTests`, `FaqAdminCrudIntegrationTests`, `Faq.test.jsx` | ✓ |
| Upravljanje paketima i pretplatama | — | ✓ (PB-52 manualno) |
| AI Insights | — | ✓ |
| MCP Admin Copilot | ✓ `AdminCopilotServiceTests`, `AdminCopilotPanel.test.jsx` | ✓ |

### Tok 5 — Upload i preuzimanje priloga

| Korak | Automatski | Manualno |
|---|---|---|
| Upload PNG/JPG/PDF — whitelist validacija | ✓ `AttachmentTests` (19), `FileUpload.test.jsx` | ✓ |
| Odbijanje .exe i prevelikih fajlova | ✓ `AttachmentTests` | ✓ |
| Preuzimanje od strane autorizovanog korisnika | ✓ `AttachmentTests`, `AttachmentList.test.jsx` | ✓ |

---

## 7. Pokrivenost po funkcionalnostima (PB stavkama)

| PB | Naziv | Backend auto | Frontend auto | Manualno | Ukupno testova |
|---|---|---|---|---|---|
| PB-19 | Login / Autentifikacija | ✓ 40+ | ✓ 12+8 | ✓ | ~60 |
| PB-22/23 | Kreiranje i pregled tiketa | ✓ | ✓ 13+11 | ✓ | ~30 |
| PB-24 | Detaljan prikaz tiketa | ✓ | ✓ 17 | ✓ | ~25 |
| PB-25 | Zatvaranje tiketa | ✓ 16+13+5 | ✓ | ✓ | ~40 |
| PB-26 | Ocjenjivanje tiketa | ✓ 24 | ✓ 8 | ✓ | 32 |
| PB-27 | Komunikacija kroz tiket | ✓ 14 | ✓ | ✓ | ~20 |
| PB-28 | Prioriteti tiketa | ✓ 11 | ✓ | ✓ | ~15 |
| PB-29 | Preraspodjela agenata | ✓ 11 | — | ✓ | 11 |
| PB-30 | Automatska dodjela | ✓ 36 | — | ✓ | 36 |
| PB-31 | Prosljeđivanje tiketa | ✓ 14 | — | ✓ | 14 |
| PB-32/33 | Pregled i filtriranje tiketa | ✓ | ✓ 10 | ✓ | ~20 |
| PB-38–44 | Izvještaji i admin dashboard | ✓ 47 | ✓ 17+10+4+3 | ✓ | ~81 |
| PB-46 | CSV export izvještaja | — | ✓ 17 | ✓ | 17 |
| PB-47/61 | FAQ + Admin CRUD FAQ | ✓ 31 | ✓ 13+6 | ✓ | ~50 |
| PB-51 | Upravljanje korisnicima | ✓ 66 | ✓ 10 | ✓ | 76 |
| PB-52 | Katalog paketa i pretplata | — | — | ✓ | manualno |
| PB-53 | Audit log | ✓ 10 | ✓ 52 | ✓ | 62 |
| PB-56 | Prilozi na tiketima | ✓ 19 | ✓ 16+3 | ✓ | 38 |
| PB-57 | AI prijedlog odgovora | ✓ (via Copilot) | ✓ | ✓ | ~10 |
| PB-58 | AI Insights | — | — | ✓ | manualno |
| PB-62 | Assign to me | ✓ 12 | ✓ | ✓ | ~15 |
| PB-63 | Agent availability status | — | — | ✓ | manualno |
| PB-65 | SLA praćenje | ✓ 12 | ✓ 8 | ✓ | 20 |
| PB-67 | Login via broj telefona | ✓ 12 | ✓ 8 | ✓ | 20 |
| PB-70 | MCP Admin Copilot | ✓ 13 | ✓ 7 | ✓ | 20 |

---

## 8. Poznati testni propusti

| Propust | Opis | Prioritet |
|---|---|---|
| PB-52 nema automatizovanih testova | Upravljanje katalogom paketa verificirano isključivo manualno; automatizovani testovi su dokumentovani kao tehnički dug | Visok |
| Nema E2E / browser testova | Ne postoje Playwright, Cypress ni Selenium testovi koji simuliraju korisnika u pravom browseru | Visok |
| SignalR nije automatski testiran | Real-time notifikacije provjerene manualno; jsdom ne podržava WebSocket konekcije | Srednji |
| AI servisni izlaz nije testiran | `AIService` poziva internu knowledge base, ali sadržaj LLM odgovora nije deterministički i nije pokriven unit testovima | Srednji |
| Nema integracijskih testova prema SQL Serveru | Svi backend testovi koriste EF Core InMemory; razlike u SQL Server ponašanju (npr. case-sensitivity, constraint handling) nisu pokrivene | Srednji |
| Flaky performansni test | `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` povremeno pada u CI zbog varijabilnosti izvršnog okruženja; nije uvjet za prolaz | Nizak |
| CSV sadržaj nije testiran | Frontend testovi verificiraju da se `generateReport` pozove, ali ne provjeravaju strukturu generisanog CSV fajla | Nizak |
| PB-63 Agent availability nema auto testova | Automatska preraspodjela tiketa pri promjeni availability statusa nije pokrivena unit testovima | Srednji |
