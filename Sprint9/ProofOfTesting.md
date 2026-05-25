# Proof of Testing — Sprint 9

Dokument evidentira dokaze testiranja za Sprint 9 funkcionalnosti: admin dashboard s ključnim metrikama (PB-45), prosječno vrijeme prvog odgovora (PB-50), modul izvještaja (PB-38, PB-39, PB-40, PB-41, PB-43, PB-44), upravljanje korisničkim nalozima (PB-51), upravljanje katalogom paketa i pretplata (PB-52), audit log aktivnosti (PB-53), prilozi na tiketima (PB-56) i preraspodjelu agenata po timovima (PB-29).

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 25.05.2026 | PB-45 admin dashboard, PB-50 FIRST_RESPONSE, PB-38–44 svi tipovi izvještaja | Backend unit + integration + frontend unit | PASS |
| 25.05.2026 | PB-51 upravljanje korisničkim nalozima (CRUD, deaktivacija, reaktivacija, sigurnost) | Backend unit + integration + security + sistemsko + frontend unit | PASS |
| 25.05.2026 | PB-53 audit log aktivnosti (filtriranje, pristup, evidencija) | Backend unit + integration + frontend unit | PASS |
| 25.05.2026 | PB-56 prilozi na tiketima (upload, preuzimanje, validacija formata i veličine) | Backend unit + frontend unit | PASS |
| 25.05.2026 | PB-29 preraspodjela agenata po timovima | Backend unit + integration | PASS |
| 25.05.2026 | PB-52 katalog paketa i pretplata | Manualno (UI) — bez automatizovanih testova | PASS (manualno) |

---

## Test okruženje

| Stavka | Vrijednost |
|---|---|
| Backend | .NET 10, xUnit, Moq, FluentAssertions, EF Core InMemory, Microsoft.AspNetCore.Mvc.Testing |
| Frontend | React, Vitest, Testing Library, jsdom |
| Projekat | `Project/TelecomSupportSystem/TelecomSupportSystem.slnx` i `Project/frontend` |

---

## Ukupni rezultati

| Nivo | Fajl / Klasa | Broj testova | Rezultat |
|---|---|---|---|
| Backend unit | `AdminReportServiceTests.cs` | 5 | PASS |
| Backend unit | `AgentStatisticsServiceTests` (ReportServiceTests.cs) | 7 | PASS |
| Backend integration | `AdminDashboardIntegrationTests.cs` | 2 | PASS |
| Backend integration | `ReportIntegrationTests.cs` | 5 | PASS |
| Frontend unit | `AdminDashboard.test.jsx` | 4 | PASS |
| Frontend unit | `Reports.test.jsx` | 3 | PASS |
| Frontend unit | `ReportsDashboard.test.jsx` | 10 | PASS |
| Backend unit + integration + security | `UserAccountManagementServiceTests`, `UserAccountManagementControllerTests`, `UserAccountManagementIntegrationTests`, `UserAccountManagementSecurityTests`, `AdminUserProfileServiceTests`, `Sprint9UserStoriesSystemTests` | 61 (UAM scope) + 7 (AdminUserProfile) | PASS |
| Frontend unit (PB-51) | `Sprint9UsersList.test.jsx` | 10 | PASS |
| Frontend unit (PB-45/PB-50 dodatno) | `Sprint9AdminDashboard.test.jsx`, `Sprint9FirstResponse.test.jsx` | 21 | PASS |
| Backend unit + integration (PB-53) | `AuditLogServiceTests.cs`, `AuditLogControllerIntegrationTests.cs` | 10 | PASS |
| Frontend unit (PB-53) | `AuditLogPage.test.jsx`, `AuditLogTable.test.jsx`, `AuditLogFilters.test.jsx`, `AuditLogDetailModal.test.jsx`, `auditLog.service.test.js` | 52 | PASS |
| Backend unit (PB-56) | `AttachmentTests.cs` | 27 | PASS |
| Frontend unit (PB-56) | `FileUpload.test.jsx`, `AttachmentList.test.jsx`, `ticketServiceAttachments.test.js` | 16 | PASS |
| Backend unit + integration (PB-29) | `TeamManagementServiceTests.cs`, `TeamManagementIntegrationTests.cs` | 11 | PASS |
| **Ukupno Sprint 9** | | **251** | **PASS** |

---

## Izvršene test komande

### Backend — admin dashboard i izvještaji

```bash
dotnet test TelecomSupportSystem.slnx --no-restore --filter "FullyQualifiedName~AdminReportServiceTests|FullyQualifiedName~AdminDashboardIntegrationTests"
```

Rezultat: PASS  
Ukupno: 7 passed, 0 failed, 0 skipped

### Backend — statistike agenata i tehničara

```bash
dotnet test TelecomSupportSystem.slnx --no-restore --filter "FullyQualifiedName~AgentStatisticsServiceTests|FullyQualifiedName~ReportIntegrationTests"
```

Rezultat: PASS  
Ukupno: 12 passed, 0 failed, 0 skipped

### Frontend — admin dashboard i izvještaji

```bash
npm test -- --run src/test/AdminDashboard.test.jsx src/test/Reports.test.jsx src/test/ReportsDashboard.test.jsx
```

Rezultat: PASS  
Ukupno: 17 passed, 0 failed

---

## Lokalno pokretanje testova

Iz root direktorija:

### Backend (samo Sprint 9 testovi):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --filter "FullyQualifiedName~Sprint9" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-51 — upravljanje korisničkim nalozima):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --filter "FullyQualifiedName~UserAccountManagement" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-50 — prosječno vrijeme prvog odgovora):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --filter "FullyQualifiedName~FirstResponseReport" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-45 — admin dashboard):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --filter "FullyQualifiedName~AdminDashboard|FullyQualifiedName~Sprint9UserStoriesSystemTests" --logger "console;verbosity=normal" 2>&1
```

### Backend (kompletan test suite):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --logger "console;verbosity=normal" 2>&1
```

### Frontend (samo Sprint 9 testovi):
```bash
cd Project/frontend && npx vitest run src/test/Sprint9CreateUser.test.jsx src/test/Sprint9UsersList.test.jsx src/test/Sprint9AdminDashboard.test.jsx src/test/Sprint9FirstResponse.test.jsx 2>&1
```

### Frontend (kompletan test suite):
```bash
cd Project/frontend && npx vitest run 2>&1
```

---

## PB-45 — Admin Dashboard sa ključnim metrikama

### Pokriveni AC (US-71, US-72, US-82, US-83, US-85, US-86)

| Nivo | US | AC | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-71 | Dashboard vraća statusBreakdown i avgFirstResponseMinutes za tikete u periodu | `AdminReportServiceTests.GetAdminDashboardAsync_ShouldReturnStatusBreakdownAndFirstResponse_WhenTicketsExist` | PASS |
| Backend unit | US-72 | Custom raspon s krajem prije početka baca ArgumentException | `AdminReportServiceTests.GetAdminDashboardAsync_ShouldThrow_WhenCustomRangeInvalid` | PASS |
| Backend integration | US-71 | GET /api/admin/dashboard vraća 200 s tačnim totalTicketsInPeriod | `AdminDashboardIntegrationTests.GetDashboard_ShouldReturn200_WithTicketCounts` | PASS |
| Backend integration | US-72 | GET /api/admin/dashboard vraća 400 za nevalidan custom raspon | `AdminDashboardIntegrationTests.GetDashboard_ShouldReturn400_WhenCustomRangeInvalid` | PASS |
| Frontend unit | US-71, US-86 | KPI kartice se prikazuju s tačnim vrijednostima nakon učitavanja | `AdminDashboard.test.jsx — prikazuje KPI kartice nakon učitavanja` | PASS |
| Frontend unit | US-83 | Sekcija generisanja izvještaja nije vidljiva u metrics modu | `AdminDashboard.test.jsx — ne prikazuje generisanje izvještaja u metrics modu` | PASS |
| Frontend unit | US-87 | KPI kartica "Prosj. 1. odgovor" prikazana bez posebnog trend grafa | `AdminDashboard.test.jsx — prikazuje KPI prosj. prvog odgovora bez grafa trenda` | PASS |
| Frontend unit | US-72 | Vremenski filter s preset dugmadima prikazan | `AdminDashboard.test.jsx — prikazuje vremenski filter` | PASS |
| Frontend unit | US-83, US-72 | Reports stranica sadrži period filter i sekciju izvještaja (bez KPI kartica) | `Reports.test.jsx — prikazuje vremenski period i generisanje izvještaja` | PASS |
| Frontend unit | US-85 | Disabled Export dugme prikazano na /reports | `Reports.test.jsx — prikazuje disabled Export dugme` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Services/AdminReportServiceTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AdminDashboardIntegrationTests.cs`
- `Project/frontend/src/test/AdminDashboard.test.jsx`
- `Project/frontend/src/test/Reports.test.jsx`

---

## PB-50 — Prosječno vrijeme prvog odgovora (admin izvještaj)

### Pokriveni AC (US-87, US-88)

| Nivo | US | AC | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-88 | FIRST_RESPONSE izvještaj vraća buckets s granularnošću "Po danu" za sedmični period | `AdminReportServiceTests.GenerateReportAsync_ShouldReturnFirstResponseBuckets_WhenRequested` | PASS |
| Backend unit | US-83 | Kada nema tiketa, izvještaj vraća HasData=false s porukom | `AdminReportServiceTests.GenerateReportAsync_ShouldReturnNoDataMessage_WhenEmpty` | PASS |
| Frontend unit | US-87 | KPI kartica "Prosj. 1. odgovor" prikazana na dashboardu | `AdminDashboard.test.jsx — prikazuje KPI prosj. prvog odgovora bez grafa trenda` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Services/AdminReportServiceTests.cs`
- `Project/frontend/src/test/AdminDashboard.test.jsx`

---

## PB-38 / PB-39 / PB-40 / PB-41 / PB-43 / PB-44 — Modul izvještaja

### Pokriveni AC (US-41, US-43, US-45, US-47, US-94, US-95)

| Nivo | US | AC | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-43 | TICKET_STATUS izvještaj sa ShowLargePeriodWarning=true za period > 90 dana | `AdminReportServiceTests.GenerateReportAsync_ShouldWarnOnLargePeriod_ForStatusReport` | PASS |
| Backend unit | US-41 | TICKET_COUNT bez podataka vraća HasData=false s porukom "Nema podataka" | `AdminReportServiceTests.GenerateReportAsync_ShouldReturnNoDataMessage_WhenEmpty` | PASS |
| Frontend unit | US-83 | Stranica /reports prikazuje sekciju izvještaja s chip selectorom (vidljiv "Broj tiketa") | `Reports.test.jsx — prikazuje vremenski period i generisanje izvještaja` | PASS |
| Frontend unit | US-85 | Disabled Export dugme vidljivo i onemogućeno | `Reports.test.jsx — prikazuje disabled Export dugme` | PASS |

### Napomena o pokrivenosti

Tipovi izvještaja TICKET_COUNT, PROBLEM_TYPE, TEAM_WORKLOAD, USER_RATINGS i AVG_RESOLUTION pokriveni su strukturnim testovima stranice (chip selector renderovan, period filter prisutan). Detaljna validacija podataka po tipu oslanja se na backend unit testove u `AdminReportServiceTests` (no-data handling, large period warning) i manualnu verifikaciju kroz UI pri razvoju.

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Services/AdminReportServiceTests.cs`
- `Project/frontend/src/test/Reports.test.jsx`

---

## Statistike agenata i tehničara (US-41–US-48, agent/tehničar strana)

### Pokriveni AC

| Nivo | US | AC | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-41 | Tačan broj tiketa po statusu (OPEN, CLOSED, CLOSURE_REQUESTED) | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldReturnCorrectStatusCounts_WhenMixedStatuses` | PASS |
| Backend unit | US-41 | Svi brojevi su 0 kada nema tiketa | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldReturnAllZeros_WhenNoTickets` | PASS |
| Backend unit | US-44 | Prosječno rješavanje izračunato samo za CLOSED tikete | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldCalculateAvgResolution_ForClosedTicketsOnly` | PASS |
| Backend unit | US-44 | avgResolutionHours je null kada nema zatvorenih tiketa | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldReturnNullAvgResolution_WhenNoClosedTickets` | PASS |
| Backend unit | US-48 | Broj otvorenih tiketa kao proxy za workload agenta | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldCountOpenTickets_AsAgentWorkload` | PASS |
| Backend unit | US-45 | Prosječna ocjena izračunata za AGENT rolu | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldCalculateAvgRating_WhenAgentHasRatedTickets` | PASS |
| Backend unit | US-45 | avgRating je null za TECHNICIAN rolu | `AgentStatisticsServiceTests.GetMyStatisticsAsync_ShouldReturnNullAvgRating_ForTechnicianRole` | PASS |
| Backend integration | US-41, US-44 | GET /api/users/me/statistics vraća 200 s tačnim aggregatima za agenta | `ReportIntegrationTests.GetMyStatistics_ShouldReturn200WithCounts_WhenAgentRequestsStats` | PASS |
| Backend integration | US-41 | GET /api/users/me/statistics vraća 200 za tehničara | `ReportIntegrationTests.GetMyStatistics_ShouldReturn200_WhenTechnicianRequestsStats` | PASS |
| Backend integration | US-41 | Klijent dobija 403 za /api/users/me/statistics | `ReportIntegrationTests.GetMyStatistics_ShouldReturn403_WhenClientRequestsStats` | PASS |
| Backend integration | US-41 | Administrator dobija 403 za /api/users/me/statistics | `ReportIntegrationTests.GetMyStatistics_ShouldReturn403_WhenAdminRequestsStats` | PASS |
| Backend integration | US-44 | Prosječno rješavanje izračunato iz ClosedDate−CreatedDate | `ReportIntegrationTests.GetMyStatistics_ShouldCalculateAvgResolutionTime_FromClosedTicketDates` | PASS |
| Frontend unit | US-41 | StatCard "Otvoreni tiketi" prikazan s tačnom vrijednošću | `ReportsDashboard.test.jsx — shows open ticket count stat card` | PASS |
| Frontend unit | US-41 | StatCard "Zatvoreni tiketi" prikazan s tačnom vrijednošću | `ReportsDashboard.test.jsx — shows closed ticket count stat card` | PASS |
| Frontend unit | US-41 | StatCard "Čeka se" prikazan s tačnom vrijednošću | `ReportsDashboard.test.jsx — shows pending closure count stat card` | PASS |
| Frontend unit | US-44 | StatCard "Prosj. rješavanje" prikazan | `ReportsDashboard.test.jsx — shows average resolution time card` | PASS |
| Frontend unit | US-45 | StatCard "Prosječna ocjena" prikazan za AGENT rolu | `ReportsDashboard.test.jsx — shows average rating card for AGENT role` | PASS |
| Frontend unit | US-45 | StatCard "Prosječna ocjena" nije prikazan za TECHNICIAN rolu | `ReportsDashboard.test.jsx — does not show average rating for TECHNICIAN role` | PASS |
| Frontend unit | US-41 | Null vrijednosti prikazuju "—" placeholder | `ReportsDashboard.test.jsx — renders "—" placeholder for null metric values` | PASS |
| Frontend unit | US-41 | Error poruka prikazana kada API padne | `ReportsDashboard.test.jsx — shows error message when statistics API fails` | PASS |
| Frontend unit | US-41 | Loading indikator prikazan dok se statistike učitavaju | `ReportsDashboard.test.jsx — shows loading indicator while statistics are loading` | PASS |
| Frontend unit | US-41 | Render bez console.error grešaka | `ReportsDashboard.test.jsx — renders without console errors when data loads successfully` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Services/ReportServiceTests.cs` (klasa `AgentStatisticsServiceTests`)
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/ReportIntegrationTests.cs`
- `Project/frontend/src/test/ReportsDashboard.test.jsx`

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PB | Dokaz | Status |
|---|---|---|---|---|
| Backend unit | US-71, US-72 | PB-45 | `AdminReportServiceTests` | PASS |
| Backend integration | US-71, US-72 | PB-45 | `AdminDashboardIntegrationTests` | PASS |
| Backend unit | US-88 | PB-50 | `AdminReportServiceTests` | PASS |
| Backend unit | US-43 | PB-39 | `AdminReportServiceTests` | PASS |
| Backend unit | US-41, US-44, US-45, US-48 | agent/tech stats | `AgentStatisticsServiceTests` | PASS |
| Backend integration | US-41, US-44, US-45 | agent/tech stats | `ReportIntegrationTests` | PASS |
| Frontend unit | US-71, US-72, US-82, US-83, US-85 | PB-45 | `AdminDashboard.test.jsx`, `Reports.test.jsx` | PASS |
| Frontend unit | US-41–US-48 | agent/tech stats | `ReportsDashboard.test.jsx` | PASS |

---

## Napomena o pristupu

- Testovi za admin izvještajne tipove (TICKET_COUNT, PROBLEM_TYPE, TEAM_WORKLOAD, USER_RATINGS, AVG_RESOLUTION) pokrivaju rubne slučajeve (no-data, large period warning) na service nivou; kompletan E2E rendering svakog tipa izvještaja verifikovan je manualnim testiranjem tokom razvoja
- `ReportsDashboard.test.jsx` testira Statistics stranicu za agente i tehničare, ne admin reports stranicu — US numeracija u tom fajlu (US-41–US-48) odnosi se na agent/tech statistiku, ne admin izvještaje
- Recharts komponente mockirane u svim frontend testovima radi kompatibilnosti s jsdom okruženjem

---

## PB-51 — Upravljanje korisničkim nalozima

### Pokriveni AC (US-73, US-74, US-75, US-89, US-90, US-91, US-92, US-93)

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-73 | Kreiranje agenta sa kategorijom stručnosti, validacija jedinstvenosti emaila i lozinke | `UserAccountManagementServiceTests` | PASS |
| Backend unit | US-74 | Izmjena podataka (ime, prezime, telefon, lokacija) i zabrana izmjene role | `UserAccountManagementServiceTests` | PASS |
| Backend unit | US-75 | Deaktivacija klijentskog naloga (status → INACTIVE) i zabrana prijave deaktiviranom korisniku | `UserAccountManagementServiceTests`, `UserAccountManagementSecurityTests` | PASS |
| Backend unit | US-89 | Lista samo aktivnih agenata; admin može deaktivirati agenta; nemoguća deaktivacija vlastitog naloga | `UserAccountManagementServiceTests` | PASS |
| Backend unit | US-90 | Lista samo aktivnih tehničara; samo admin može deaktivirati tehničara | `UserAccountManagementServiceTests` | PASS |
| Backend unit | US-91 | Pregled deaktiviranih i reaktivacija (status → ACTIVE); evidencija u audit log | `UserAccountManagementServiceTests`, `AdminUserProfileServiceTests` | PASS |
| Backend unit | US-92 | Validacija prilikom uređivanja ista kao kod kreiranja | `AdminUserProfileServiceTests` | PASS |
| Backend integration | US-73, US-74, US-89, US-90 | POST/PUT/GET kroz `Controller → Service → Repository` (EF InMemory) | `UserAccountManagementIntegrationTests`, `UserAccountManagementControllerTests` | PASS |
| Backend security | US-75, US-89, US-91 | Klijent/agent dobija 403 za admin sekcije; deaktivirani korisnik dobija 401 pri prijavi; zabrana prikaza lozinki | `UserAccountManagementSecurityTests` | PASS |
| Backend sistemsko | US-73, US-75 | E2E: kreiranje → prijava → deaktivacija → blokirana ponovna prijava | `Sprint9UserStoriesSystemTests.PB51_CreateLoginDeactivateBlockedLogin_WorksThroughControllerServiceRepo` | PASS |
| Frontend unit | US-74, US-89, US-90 | Lista korisnika s pretragom, filtriranjem po roli i lokaciji; otvaranje detalja; "Dodaj korisnika"; deaktivacija | `Sprint9UsersList.test.jsx` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementServiceTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementControllerTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementIntegrationTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementSecurityTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/Sprint9UserStoriesSystemTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Services/AdminUserProfileServiceTests.cs`
- `Project/frontend/src/test/Sprint9UsersList.test.jsx`

### Izvršene komande

```bash
dotnet test TelecomSupportSystem.slnx --no-build --filter "FullyQualifiedName~UserAccountManagement"
```
Rezultat: PASS — 61 passed, 0 failed, 0 skipped

```bash
dotnet test TelecomSupportSystem.slnx --no-build --filter "FullyQualifiedName~AdminUserProfile"
```
Rezultat: PASS — 7 passed, 0 failed, 0 skipped

```bash
npx vitest run src/test/Sprint9UsersList.test.jsx
```
Rezultat: PASS — 10 passed, 0 failed

---

## PB-53 — Pregled audit log-a aktivnosti

### Pokriveni AC (US-78, US-79)

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-78 | Evidentiranje akcija (timestamp, user, type, entity, opis); nema osjetljivih podataka (lozinke) | `AuditLogServiceTests.cs` | PASS |
| Backend unit | US-79 | Filtriranje po tipu akcije, korisniku, vremenskom periodu; pretraga po opisu; kombinovani filteri | `AuditLogServiceTests.cs` | PASS |
| Backend integration | US-78 | Klijent/Agent dobija 403 za GET /api/audit-logs; admin 200 sa paginiranom listom | `AuditLogControllerIntegrationTests.cs` | PASS |
| Backend integration | US-78 | Sistem ne dozvoljava izmjenu ili brisanje zapisa (samo READ endpoint) | `AuditLogControllerIntegrationTests.cs` | PASS |
| Frontend unit | US-78 | Tabela audit log-a prikazuje vrijeme, korisnika, akciju, entitet; sortiranje od najnovijih | `AuditLogTable.test.jsx` | PASS |
| Frontend unit | US-79 | Filteri po tipu akcije, korisniku, periodu; reset filtera; kombinovani filteri | `AuditLogFilters.test.jsx` | PASS |
| Frontend unit | US-78 | Detalji audit zapisa kroz modal (timestamp, akcija, entitet, opis) | `AuditLogDetailModal.test.jsx` | PASS |
| Frontend unit | US-78, US-79 | Glavna stranica audit log-a (loading, error, prazno stanje, paginacija) | `AuditLogPage.test.jsx` | PASS |
| Frontend unit | US-78 | Service sloj za audit log (API pozivi, query parametri za filtere) | `auditLog.service.test.js` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/AuditLog/AuditLogServiceTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/AuditLog/AuditLogControllerIntegrationTests.cs`
- `Project/frontend/src/test/AuditLogPage.test.jsx`
- `Project/frontend/src/test/AuditLogTable.test.jsx`
- `Project/frontend/src/test/AuditLogFilters.test.jsx`
- `Project/frontend/src/test/AuditLogDetailModal.test.jsx`
- `Project/frontend/src/test/auditLog.service.test.js`

### Izvršene komande

```bash
dotnet test TelecomSupportSystem.slnx --no-build --filter "FullyQualifiedName~AuditLog"
```
Rezultat: PASS — 10 passed, 0 failed, 0 skipped

```bash
npx vitest run src/test/AuditLogPage.test.jsx src/test/AuditLogTable.test.jsx src/test/AuditLogFilters.test.jsx src/test/AuditLogDetailModal.test.jsx src/test/auditLog.service.test.js
```
Rezultat: PASS — 52 passed, 0 failed

---

## PB-56 — Prilozi na tiketima

### Pokriveni AC (US-80, US-81)

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-80 | Validacija formata (PNG, JPG, JPEG, PDF, DOCX, TXT); zabrana izvršnih fajlova (.exe, .bat, .sh) | `AttachmentTests.cs` | PASS |
| Backend unit | US-80 | Maksimalna veličina pojedinačnog priloga 5 MB i odbacivanje većih | `AttachmentTests.cs` | PASS |
| Backend unit | US-80 | Maksimalan broj priloga po tiketu/poruci = 5 | `AttachmentTests.cs` | PASS |
| Backend unit | US-80 | Sanitizacija naziva fajla (specijalni karakteri uklonjeni) | `AttachmentTests.cs` | PASS |
| Backend unit | US-81 | Lista priloga po tiketu (naziv, veličina, vrijeme uploada, korisnik) | `AttachmentTests.cs` | PASS |
| Backend unit | US-81 | Zabrana pristupa prilozima za korisnika koji nije vlasnik/agent tiketa | `AttachmentTests.cs` | PASS |
| Backend unit | US-81 | Zabrana brisanja priloga nakon što je priložen | `AttachmentTests.cs` | PASS |
| Frontend unit | US-80 | FileUpload komponenta — odabir fajla, indikator napretka, prikaz greške za nedozvoljen format i preveliki fajl | `FileUpload.test.jsx` | PASS |
| Frontend unit | US-81 | AttachmentList — prikaz thumbnail-a slika, lightbox pregled, link za preuzimanje dokumenata, metadata (veličina, korisnik) | `AttachmentList.test.jsx` | PASS |
| Frontend unit | US-80, US-81 | TicketService prilozi — upload via FormData, preuzimanje blob-a, lista priloga | `ticketServiceAttachments.test.js` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Attachments/AttachmentTests.cs`
- `Project/frontend/src/test/FileUpload.test.jsx`
- `Project/frontend/src/test/AttachmentList.test.jsx`
- `Project/frontend/src/test/ticketServiceAttachments.test.js`

### Izvršene komande

```bash
dotnet test TelecomSupportSystem.slnx --no-build --filter "FullyQualifiedName~Attachment"
```
Rezultat: PASS — 27 passed, 0 failed, 0 skipped

```bash
npx vitest run src/test/FileUpload.test.jsx src/test/AttachmentList.test.jsx src/test/ticketServiceAttachments.test.js
```
Rezultat: PASS — 16 passed, 0 failed

---

## PB-29 — Preraspodjela agenata po timovima

### Pokriveni AC (US-23)

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-23 | Administrator premješta agenta iz tima A u tim B; promjena se evidentira sa timestamp-om | `TeamManagementServiceTests.cs` | PASS |
| Backend unit | US-23 | Bez potvrde akcije nema promjene podataka | `TeamManagementServiceTests.cs` | PASS |
| Backend unit | US-23 | Greška/poruka kada preraspodjela nije moguća (npr. tim ne postoji) | `TeamManagementServiceTests.cs` | PASS |
| Backend integration | US-23 | PUT /api/teams/{id}/agents kroz Controller → Service → Repository; samo admin može | `TeamManagementIntegrationTests.cs` | PASS |
| Backend integration | US-23 | Pregled raspodjele po timovima s filtriranjem; promjena vidljiva nakon save | `TeamManagementIntegrationTests.cs` | PASS |

### Fajlovi sa testovima

- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Services/TeamManagementServiceTests.cs`
- `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/TeamManagementIntegrationTests.cs`

### Izvršene komande

```bash
dotnet test TelecomSupportSystem.slnx --no-build --filter "FullyQualifiedName~TeamManagement"
```
Rezultat: PASS — 11 passed, 0 failed, 0 skipped

---

## PB-52 — Upravljanje katalogom paketa i pretplata

### Pokriveni AC (US-76, US-77)

| Nivo | US | AC fokus | Tip dokaza | Status |
|---|---|---|---|---|
| Manualno (UI) | US-76 | Admin kreira/uređuje paket (naziv, tip, opis, cijena, status); validacija praznog naziva i pozitivne cijene | Sprint Review demo / razvojna manualna verifikacija na `/admin/packages` | PASS |
| Manualno (UI) | US-76 | Deaktivacija paketa s aktivnim pretplatama: paket nije izbrisan, postojeće pretplate ostaju | Sprint Review demo | PASS |
| Manualno (UI) | US-77 | Dodjela paketa klijentu (datum početka), ukidanje pretplate, zabrana duple aktivne pretplate na isti paket | Sprint Review demo | PASS |
| Manualno (UI) | US-77 | Klijent vidi ažurirane pakete na svom profilu nakon promjene | Sprint Review demo | PASS |

### Napomena o pokrivenosti (PB-52)

Za PB-52 nisu pisani automatski (xUnit/Vitest) testovi u Sprintu 9; funkcionalnost je verifikovana **manualno kroz UI** tokom razvoja i tokom Sprint Review demo-a. Komponente koje učestvuju u toku (`PackageCatalogController`, `ClientSubscriptionController`, `PackageService`, `CatalogPackageService`) imaju implementiranu poslovnu logiku, a sigurnost pristupa pokrivena je centralizovano kroz role-based authorization koji je posebno pokriven testovima u `RoleAccessSecurityTests.cs`. Automatska pokrivenost PB-52 funkcionalnosti planirana je u narednom sprintu kao tehnički dug.

---

## Veza sa Test Strategijom (proširenje za nove PBI)

| Test strategija nivo | US | PB | Dokaz | Status |
|---|---|---|---|---|
| Backend unit | US-73, US-74, US-75, US-89, US-90, US-91, US-92 | PB-51 | `UserAccountManagementServiceTests`, `AdminUserProfileServiceTests` | PASS |
| Backend integration | US-73, US-74, US-89, US-90 | PB-51 | `UserAccountManagementIntegrationTests`, `UserAccountManagementControllerTests` | PASS |
| Backend sigurnosno | US-75, US-89, US-91 | PB-51 | `UserAccountManagementSecurityTests` | PASS |
| Backend sistemsko | US-73, US-75 | PB-51 | `Sprint9UserStoriesSystemTests` | PASS |
| Frontend unit | US-74, US-89, US-90 | PB-51 | `Sprint9UsersList.test.jsx` | PASS |
| Backend unit | US-78, US-79 | PB-53 | `AuditLogServiceTests` | PASS |
| Backend integration | US-78 | PB-53 | `AuditLogControllerIntegrationTests` | PASS |
| Frontend unit | US-78, US-79 | PB-53 | `AuditLogPage/Table/Filters/DetailModal.test.jsx`, `auditLog.service.test.js` | PASS |
| Backend unit | US-80, US-81 | PB-56 | `AttachmentTests` | PASS |
| Frontend unit | US-80, US-81 | PB-56 | `FileUpload.test.jsx`, `AttachmentList.test.jsx`, `ticketServiceAttachments.test.js` | PASS |
| Backend unit | US-23 | PB-29 | `TeamManagementServiceTests` | PASS |
| Backend integration | US-23 | PB-29 | `TeamManagementIntegrationTests` | PASS |
| Prihvatno (manualno UI) | US-76, US-77 | PB-52 | Sprint Review demo (bez automatizovanih testova) | PASS |
