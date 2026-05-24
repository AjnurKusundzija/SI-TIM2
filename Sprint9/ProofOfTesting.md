# Proof of Testing — Sprint 9

Dokument evidentira dokaze testiranja za Sprint 9 funkcionalnosti: admin dashboard s ključnim metrikama (PB-45), prosječno vrijeme prvog odgovora (PB-50) i modul izvještaja (PB-38, PB-39, PB-40, PB-41, PB-43, PB-44).

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 25.05.2026 | PB-45 admin dashboard, PB-50 FIRST_RESPONSE, PB-38–44 svi tipovi izvještaja | Backend unit + integration + frontend unit | PASS |

---

## Test okruženje

| Stavka | Vrijednost |
|---|---|
| Backend | .NET 10, xUnit, Moq, FluentAssertions, EF Core InMemory |
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
| **Ukupno Sprint 9 (reports scope)** | | **36** | **PASS** |

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

### Backend:
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --logger "console;verbosity=normal" 2>&1
```

### Frontend:
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
