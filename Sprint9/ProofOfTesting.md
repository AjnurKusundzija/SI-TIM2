# Proof of Testing — Sprint 9

---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend (service) | US-73, US-74, US-75, US-89 | xUnit + Moq | 19 metoda (uključujući Theory) — `UserAccountManagementServiceTests` | NIJE POKRENUTO LOKALNO (vidi napomenu) |
| Unit — Backend (controller) | US-73, US-74, US-75, US-89 | xUnit + Moq | 17 metoda — `UserAccountManagementControllerTests` | NIJE POKRENUTO LOKALNO |
| Integracijsko — Backend | US-73, US-74, US-75, US-89 | xUnit + EF InMemory + AuthController smoke | 11 metoda — `UserAccountManagementIntegrationTests` | NIJE POKRENUTO LOKALNO |
| Sigurnosno — Backend | US-73, US-75, US-89 | xUnit + EF InMemory + Theory | 5 metoda — `UserAccountManagementSecurityTests` | NIJE POKRENUTO LOKALNO |
| Unit — Backend (helper) | US-87, US-88 | xUnit + Theory | 8 metoda — `FirstResponseReportTests` | NIJE POKRENUTO LOKALNO |
| Integracijsko — Backend | US-87, US-88 | xUnit + EF InMemory | 5 metoda — `FirstResponseReportIntegrationTests` | NIJE POKRENUTO LOKALNO |
| Unit — Backend (service) | US-71, US-72, US-82, US-83, US-86 | xUnit + Moq + Theory | 10 metoda — `AdminDashboardServiceTests` | NIJE POKRENUTO LOKALNO |
| Integracijsko — Backend | US-71, US-72, US-83, US-85, US-86 | xUnit + EF InMemory + Reflection | 7 metoda — `Sprint9AdminDashboardIntegrationTests` | NIJE POKRENUTO LOKALNO |
| Performansno — Backend | US-71 (< 5 s NFR) | xUnit + Stopwatch | 1 metoda — `AdminDashboardPerformanceTests` | NIJE POKRENUTO LOKALNO |
| Sistemski — Backend (end-to-end) | PB-45, PB-50, PB-51 | xUnit + EF InMemory + AuthController | 4 metode — `Sprint9UserStoriesSystemTests` | NIJE POKRENUTO LOKALNO |
| **Ukupno novi backend testovi (Sprint 9)** | | | **87 test metoda** (sa Theory inline-data ukupno > 100 slučajeva) | **NIJE POKRENUTO LOKALNO** |
| UI / Acceptance — Frontend | US-73 | Vitest + RTL | 9 testova — `Sprint9CreateUser.test.jsx` | PASS |
| UI / Acceptance — Frontend | US-74, US-75, US-89 | Vitest + RTL | 10 testova — `Sprint9UsersList.test.jsx` | PASS |
| UI / Acceptance — Frontend | US-71, US-72, US-82, US-83, US-84, US-85, US-86 | Vitest + RTL | 17 testova — `Sprint9AdminDashboard.test.jsx` | PASS |
| UI / Acceptance — Frontend | US-87, US-88 | Vitest + RTL | 4 testa — `Sprint9FirstResponse.test.jsx` | PASS |
| **Ukupno novi frontend testovi (Sprint 9)** | | | **40 novih testova** | **PASS** |
| **Ukupno Sprint 9** | **US-71, US-72, US-73, US-74, US-75, US-82, US-83, US-84, US-85, US-86, US-87, US-88, US-89** | | **127 novih test metoda (40 PASS na frontendu + 87 nepokrenutih backend testova)** | **PASS frontend / NIJE POKRENUTO backend** |
| **Ukupno projekat (kumulativno)** | uključuje sve prethodne sprintove + Sprint 9 | | **303 backend test metoda + 243 frontend = 546+** | **vidi prethodne ProofOfTesting fajlove** |

> **Backend testovi nisu lokalno pokrenuti** zato što .NET SDK nije dostupan u trenutnom okruženju (`dotnet --version` vraća „command not found“). Test fajlovi su napisani po istom uzoru kao postojeći Sprint 7/Sprint 8 testovi i koriste isti API. Komande za pokretanje navedene su u sekciji „Lokalno pokretanje testova“; lokalni run očekuje se kao zelen prije merge-a u `main`.

---

## PB-51 — Upravljanje korisničkim nalozima

### Pokriveni AC (US-73 — Kreiranje korisničkih naloga)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service (Theory, 3 slučaja) | US-73 | Admin može kreirati CLIENT, AGENT i TECHNICIAN nalog | `UserAccountManagementServiceTests.CreateUserAsync_ShouldCreate_WhenAdminCreatesAllowedRoles` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-73 | Lozinka se uvijek hash-uje (BCrypt) — plaintext nikad ne završi u bazi | `UserAccountManagementServiceTests.CreateUserAsync_ShouldHashPassword_NeverStoringPlaintext` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-73 | Duplikat emaila se odbija sa `InvalidOperationException` (→ HTTP 409) | `UserAccountManagementServiceTests.CreateUserAsync_ShouldThrowInvalidOperation_WhenEmailAlreadyExists` | NIJE POKRENUTO LOKALNO |
| Unit — Service (Theory, 3 slučaja) | US-73 | Klijent/agent/tehničar ne mogu kreirati korisnika preko forme — `UnauthorizedAccessException` | `UserAccountManagementServiceTests.CreateUserAsync_ShouldThrowUnauthorized_WhenNonAdminCallsService` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-73 | Agent i tehničar automatski dobivaju `AvailabilityStatus.AVAILABLE`; agentu se postavlja TeamId | `UserAccountManagementServiceTests.CreateUserAsync_ShouldAssignAvailableAvailability_ForAgentAndTechnician` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-73 | Validan DTO → 200 OK | `UserAccountManagementControllerTests.CreateUser_ShouldReturnOk_WhenAdminAndValidDto` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-73 | Bez Role claima → 401 Unauthorized | `UserAccountManagementControllerTests.CreateUser_ShouldReturnUnauthorized_WhenNoRoleClaim` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-73 | Agent pokušava kreirati → 403 Forbid | `UserAccountManagementControllerTests.CreateUser_ShouldReturnForbid_WhenAgentTriesToCreate` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-73 | Duplikat emaila → 409 Conflict | `UserAccountManagementControllerTests.CreateUser_ShouldReturnConflict_WhenEmailAlreadyTaken` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-73 | ModelState nevalidan → 400 BadRequest | `UserAccountManagementControllerTests.CreateUser_ShouldReturnBadRequest_WhenModelStateInvalid` | NIJE POKRENUTO LOKALNO |
| Integracijsko (Controller→Service→Repo→DB) | US-73 | End-to-end: admin kreira klijenta → klijent se uspješno prijavljuje istim emailom/lozinkom | `UserAccountManagementIntegrationTests.CreateUser_EndToEnd_ShouldPersistAndAllowLogin_WhenAdminCreatesClient` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-73 | Duplikat emaila kroz puni stack → 409 Conflict | `UserAccountManagementIntegrationTests.CreateUser_EndToEnd_ShouldReturnConflict_WhenEmailAlreadyExists` | NIJE POKRENUTO LOKALNO |
| Sigurnosno (Theory, 3 slučaja) | US-73 | Klijent/agent/tehničar kroz controller ne mogu kreirati i ništa se ne perzistira u bazi | `UserAccountManagementSecurityTests.CreateUser_ShouldReturnForbid_WhenNonAdminAttempts` | NIJE POKRENUTO LOKALNO |
| UI — Acceptance | US-73 | Forma sadrži obavezna polja (ime, prezime, email, telefon, lozinka, rola, lokacija) | `Sprint9CreateUser.test.jsx` „forma ima obavezna polja…“ | PASS |
| UI — Acceptance | US-73 | Admin rola se NE pojavljuje u opcijama (ne može se kreirati admin nalog kroz formu) | `Sprint9CreateUser.test.jsx` „rola admin ne postoji u opcijama“ | PASS |
| UI — Acceptance | US-73 | AGENT rola otkriva polje „Ekspertiza (Tim)“ | `Sprint9CreateUser.test.jsx` „za AGENT rolu prikazuje polje za ekspertizu“ | PASS |
| UI — Validacija | US-73 | Nevalidan email format prikazuje grešku na blur | `Sprint9CreateUser.test.jsx` „odbija nevalidan email format na blur“ | PASS |
| UI — Validacija | US-73 | Prekratka lozinka (< 8 znakova) prikazuje grešku | `Sprint9CreateUser.test.jsx` „odbija prekratku lozinku“ | PASS |
| UI — Validacija | US-73 | Submit prazne forme prikazuje sve greške i ne poziva API | `Sprint9CreateUser.test.jsx` „prikazuje sve greške…“ | PASS |
| UI — Happy path | US-73 | Uspješan submit prikazuje toast „Korisnik kreiran!“ i poziva createUser sa ispravnim payload-om | `Sprint9CreateUser.test.jsx` „uspješan submit prikazuje toast…“ | PASS |
| UI — Error handling | US-73 | Server konflikt prikazuje poruku „Email adresa je već zauzeta“ | `Sprint9CreateUser.test.jsx` „konflikt email prikazuje server grešku“ | PASS |
| UI — Pristup | US-73 | Non-admin korisnik biva preusmjeren na `/dashboard` | `Sprint9CreateUser.test.jsx` „preusmjerava non-admin korisnika na /dashboard“ | PASS |

### Pokriveni AC (US-74 — Uređivanje postojećih korisnika)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-74 | Admin može mijenjati ime, prezime, telefon, lokaciju agenta | `UserAccountManagementServiceTests.UpdateUserDetailsAsync_ShouldUpdateNamePhoneLocation_WhenAdminEditsAgent` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-74 | Rola se NIKAD ne mijenja kroz `UpdateUserDetailsAsync` | `UserAccountManagementServiceTests.UpdateUserDetailsAsync_ShouldNotChangeRole_EvenIfTargetWasAdmin` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-74 | Nepostojeći korisnik → `KeyNotFoundException` (→ 404) | `UserAccountManagementServiceTests.UpdateUserDetailsAsync_ShouldThrowKeyNotFound_WhenUserMissing` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-74 | Agent NE može mijenjati podatke drugog agenta — `UnauthorizedAccessException` | `UserAccountManagementServiceTests.UpdateUserDetailsAsync_ShouldThrowUnauthorized_WhenAgentEditsAgent` | NIJE POKRENUTO LOKALNO |
| Unit — Service (Theory) | US-74 | Klijent/tehničar ne mogu editovati druge korisnike | `UserAccountManagementServiceTests.UpdateUserDetailsAsync_ShouldThrowUnauthorized_WhenNonStaffCalls` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-74 | Admin update → 200 OK | `UserAccountManagementControllerTests.UpdateUserDetails_ShouldReturnOk_WhenAdminEdits` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-74 | Nepostojeći korisnik → 404 NotFound | `UserAccountManagementControllerTests.UpdateUserDetails_ShouldReturnNotFound_WhenUserDoesNotExist` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-74 | Service baca Unauthorized → 403 Forbid | `UserAccountManagementControllerTests.UpdateUserDetails_ShouldReturnForbid_WhenServiceThrowsUnauthorized` | NIJE POKRENUTO LOKALNO |
| Unit — Controller (lista) | US-74, US-89 | Admin → 200 OK; Klijent → 403 | `UserAccountManagementControllerTests.GetUsersList_ShouldReturnOk_WhenAdminCalls` i `GetUsersList_ShouldReturnForbid_WhenClientCalls` | NIJE POKRENUTO LOKALNO |
| Unit — Service (lista) | US-74 | Service vraća items + paginaciju (Page, PageSize, TotalCount) | `UserAccountManagementServiceTests.GetUsersPaginatedAsync_ShouldReturnItemsAndPagination_WhenAdminCalls` | NIJE POKRENUTO LOKALNO |
| Unit — Service (lista) | US-74 | Enum filteri (role, status, location) se prosljeđuju repozitoriju | `UserAccountManagementServiceTests.GetUsersPaginatedAsync_ShouldForwardEnumFilters_WhenProvidedAsStrings` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-74 | Lista vraća samo aktivne korisnike kad je status=ACTIVE | `UserAccountManagementIntegrationTests.GetUsersList_EndToEnd_ShouldReturnPagedActiveUsers_WhenAdminCallsWithoutFilters` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-74 | Filter po lokaciji i pretraga po telefonu funkcionišu kroz puni stack | `UserAccountManagementIntegrationTests.GetUsersList_EndToEnd_ShouldFilterByLocationAndSearch` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-74 | Update kroz puni stack mijenja ime/telefon/lokaciju, ali NE rolu | `UserAccountManagementIntegrationTests.UpdateUserDetails_EndToEnd_ShouldUpdate_WithoutChangingRole` | NIJE POKRENUTO LOKALNO |
| Sigurnosno | US-74 | `UserListItemDto` ne sadrži PasswordHash niti Password property | `UserAccountManagementSecurityTests.UserListItemDto_ShouldNeverContainPasswordOrHash` | NIJE POKRENUTO LOKALNO |
| Sigurnosno | US-74 | `GET /api/users/list` za klijenta → 403 Forbid | `UserAccountManagementSecurityTests.GetUsersList_ShouldReturnForbid_WhenClientAttempts` | NIJE POKRENUTO LOKALNO |
| UI | US-74 | Lista sadrži pretragu po imenu/emailu/telefonu | `Sprint9UsersList.test.jsx` „prikazuje pretraga input po imenu/emailu/telefonu“ | PASS |
| UI | US-74 | Pretraga šalje `search` parametar na API | `Sprint9UsersList.test.jsx` „pretraga prosljeđuje search parametar API-ju“ | PASS |
| UI | US-74 | Detalji button vodi na korisnika | `Sprint9UsersList.test.jsx` „prikazuje detalji button…“ | PASS |
| UI | US-74 | Filter po lokaciji prosljeđuje location parametar | `Sprint9UsersList.test.jsx` „filter po lokaciji prosljeđuje location parametar“ | PASS |
| UI | US-74 | Prazna lista prikazuje informativnu poruku | `Sprint9UsersList.test.jsx` „prazna lista prikazuje informativnu poruku“ | PASS |
| **GAP** | US-74 | „Sistem mora evidentirati izmjenu u audit log“ — audit log za `Users` table NIJE implementiran u trenutnom produkcijskom kodu (samo `SubscriptionAuditLog` postoji). Nije pokriveno testom; ne ispravljamo produkcijski kod. | — | GAP |

### Pokriveni AC (US-75 — Pregled i deaktivacija klijenata)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-75 | Admin deaktivira klijenta → `AccountStatus.INACTIVE` | `UserAccountManagementServiceTests.ChangeUserStatusAsync_ShouldSetInactive_WhenAdminDeactivatesClient` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-75 | Admin ne može deaktivirati vlastiti nalog → `InvalidOperationException` (→ 400) | `UserAccountManagementServiceTests.ChangeUserStatusAsync_ShouldThrow_WhenAdminDeactivatesOwnAccount` | NIJE POKRENUTO LOKALNO |
| Unit — Service (Theory, 3 slučaja) | US-75 | Agent ne može deaktivirati admin/agent/tehničar — samo klijenta | `UserAccountManagementServiceTests.ChangeUserStatusAsync_ShouldThrowUnauthorized_WhenAgentDeactivatesNonClientRoles` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-75 (i US-89) | Reaktivacija inactive klijenta postavlja `AccountStatus.ACTIVE` | `UserAccountManagementServiceTests.ChangeUserStatusAsync_ShouldReactivate_WhenAdminReactivates` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-75 | DeactivateUser admin → 200; klijent → 400 ako vlastiti, 403 ako neauthorized, 404 ako missing | `UserAccountManagementControllerTests.DeactivateUser_*` (4 testa) | NIJE POKRENUTO LOKALNO |
| Integracijsko (sa AuthController smoke) | US-75 | Deaktiviran klijent ne može se prijaviti — `POST /api/auth/login` vraća 401 | `UserAccountManagementIntegrationTests.DeactivateClient_EndToEnd_ShouldSetInactiveAndBlockLogin` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-75 | Historijski tiketi deaktiviranog klijenta se NE brišu | `UserAccountManagementIntegrationTests.DeactivateClient_EndToEnd_ShouldPreserveHistoricalTickets` | NIJE POKRENUTO LOKALNO |
| Integracijsko (sigurnosno) | US-75 | Agent deaktivira agenta kroz API → 403 Forbid, status u bazi ostaje ACTIVE | `UserAccountManagementIntegrationTests.DeactivateUser_EndToEnd_ShouldReturnForbid_WhenAgentDeactivatesAgent` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-75 | Admin pokušava deaktivirati vlastiti nalog → 400 BadRequest, status u bazi ostaje ACTIVE | `UserAccountManagementIntegrationTests.DeactivateUser_EndToEnd_ShouldReturnBadRequest_WhenAdminDeactivatesOwnAccount` | NIJE POKRENUTO LOKALNO |
| Sigurnosno | US-75 | Login za INACTIVE klijenta vraća 401 Unauthorized (i ne otkriva razlog) | `UserAccountManagementSecurityTests.DeactivatedUser_CannotLogin` | NIJE POKRENUTO LOKALNO |
| UI | US-75 | Stranica `/users/clients` poziva API sa `role=CLIENT` i `status=ACTIVE` (samo aktivni) | `Sprint9UsersList.test.jsx` „zove API sa role=CLIENT i status=ACTIVE“ | PASS |
| Sistemski (end-to-end kroz sve slojeve) | US-75 (i US-89) | Lifecycle: kreiranje → login → deaktivacija (login blokiran) → reaktivacija (login radi) | `Sprint9UserStoriesSystemTests.PB51_CreateLoginDeactivateBlockedLogin_WorksThroughControllerServiceRepo` | NIJE POKRENUTO LOKALNO |

### Pokriveni AC (US-89 — Upravljanje agentskim nalozima)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-89 | Admin može deaktivirati agenta bez aktivnih tiketa | `UserAccountManagementServiceTests.ChangeUserStatusAsync_ShouldDeactivateAgent_WhenAdminAndNoOpenTickets` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-89 | Deaktivacija agenta s OPEN tiketima → `InvalidOperationException` (upozorenje korisniku) | `UserAccountManagementServiceTests.ChangeUserStatusAsync_ShouldThrowInvalidOperation_WhenDeactivatingAgentWithOpenTickets` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-89 | `GetAgentTeams` za agenta → 403 Forbid (admin-only) | `UserAccountManagementControllerTests.GetAgentTeams_ShouldReturnForbid_WhenAgentCalls` | NIJE POKRENUTO LOKALNO |
| Unit — Controller | US-89 | `GetAgentTeams` za admina → 200 OK | `UserAccountManagementControllerTests.GetAgentTeams_ShouldReturnOk_WhenAdminCalls` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-89 | Pokušaj deaktivacije agenta s aktivnim OPEN tiketom kroz puni stack → 400 BadRequest, status u bazi ostaje ACTIVE | `UserAccountManagementIntegrationTests.DeactivateAgent_EndToEnd_ShouldRejectWhenAgentHasOpenAssignedTickets` | NIJE POKRENUTO LOKALNO |
| Integracijsko (repository) | US-89 | Deaktivirani agent (`AccountStatus.INACTIVE`) NE pojavljuje se u listi kandidata za prosljeđivanje | `UserAccountManagementIntegrationTests.DeactivatedAgent_ShouldNotAppear_InForwardingCandidates` | NIJE POKRENUTO LOKALNO |
| Sigurnosno | US-89 | `GetAgentTeams` (admin endpoint) ne dozvoljava AGENT roli | `UserAccountManagementSecurityTests.GetAgentTeams_ShouldReturnForbid_WhenAgentRoleAttempts` | NIJE POKRENUTO LOKALNO |
| UI | US-89 | `/users/agents` zove API sa `role=AGENT` i `status=ACTIVE` | `Sprint9UsersList.test.jsx` „zove API sa role=AGENT i status=ACTIVE“ | PASS |
| UI | US-89 | Lista agenata prikazuje kolonu Ekspertiza i vrijednost | `Sprint9UsersList.test.jsx` „prikazuje kolonu Ekspertiza…“ | PASS |
| UI / Sigurnosno | US-89 | AGENT korisnik koji posjeti `/users/agents` ili `/users/deactivated` preusmjerava se na `/dashboard` | `Sprint9UsersList.test.jsx` „agent koji posjeti /users/agents biva preusmjeren…“ i „/users/deactivated…“ | PASS |
| Sistemski (end-to-end) | US-89 | Admin lifecycle s reaktivacijom — provjereno kroz isti `PB51_CreateLoginDeactivateBlockedLogin_WorksThroughControllerServiceRepo` test | `Sprint9UserStoriesSystemTests.PB51_CreateLoginDeactivateBlockedLogin_WorksThroughControllerServiceRepo` | NIJE POKRENUTO LOKALNO |

### Fajlovi sa testovima — PB-51

- [TelecomSupportSystem.Tests/Sprint9/UserAccountManagementServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementServiceTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/UserAccountManagementControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementControllerTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/UserAccountManagementIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementIntegrationTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/UserAccountManagementSecurityTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/UserAccountManagementSecurityTests.cs)
- [frontend/src/test/Sprint9CreateUser.test.jsx](../Project/frontend/src/test/Sprint9CreateUser.test.jsx)
- [frontend/src/test/Sprint9UsersList.test.jsx](../Project/frontend/src/test/Sprint9UsersList.test.jsx)

---

## PB-50 — Prosječno vrijeme prvog odgovora

### Pokriveni AC (US-87 — KPI „Prosj. 1. odgovor“ na dashboardu)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Helper | US-87 | Prvi odgovor se računa od kreiranja tiketa do prvog komentara STAFFA (Agent/Technician), ne klijenta | `FirstResponseReportTests.GetFirstResponseMinutes_ShouldUseFirstStaffComment_NotClient` | NIJE POKRENUTO LOKALNO |
| Unit — Helper | US-87 | Tiket sa samo klijent komentarima → null (nije lažna nula) | `FirstResponseReportTests.GetFirstResponseMinutes_ShouldReturnNull_WhenOnlyClientComments` | NIJE POKRENUTO LOKALNO |
| Unit — Helper | US-87 | Average ignoriše tikete bez staff odgovora | `FirstResponseReportTests.CalculateAvgFirstResponseMinutes_ShouldIgnoreTicketsWithoutStaffComments` | NIJE POKRENUTO LOKALNO |
| Unit — Helper | US-87 | Kada nijedan tiket nema staff odgovor → average je null (nije 0) | `FirstResponseReportTests.CalculateAvgFirstResponseMinutes_ShouldReturnNull_WhenNoTicketHasStaffResponse` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-87 | `GET /api/admin/dashboard` vraća tačan `avgFirstResponseMinutes` (∼40 min za 30+50 min) | `FirstResponseReportIntegrationTests.GetDashboard_ShouldReturnAvgFirstResponse_OverCreatedTicketsInPeriod` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-87 | Bez staff odgovora endpoint vraća `avgFirstResponseMinutes = null` (ne lažnu nulu) | `FirstResponseReportIntegrationTests.GetDashboard_ShouldReturnNullAvgFirstResponse_WhenNoStaffReplies` | NIJE POKRENUTO LOKALNO |
| UI | US-87 | KPI „Prosj. 1. odgovor“ prikazuje empty poruku kada nema odgovora (ne „0 min“) | `Sprint9FirstResponse.test.jsx` „prikazuje empty poruku kada nema staff odgovora“ | PASS |
| UI | US-87 | Vrijednost se formatira u h/min (90 → „1 h 30 min“) | `Sprint9FirstResponse.test.jsx` „prikazuje formatiranu vrijednost…“ | PASS |

### Pokriveni AC (US-88 — `FIRST_RESPONSE` izvještaj na `/reports`)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Helper (Theory, 3 slučaja) | US-88 | Standardni periodi: `week → Po danu`, `month → Po sedmici`, `year → Po mjesecu` | `FirstResponseReportTests.ResolveGranularity_ShouldMapStandardPeriods` | NIJE POKRENUTO LOKALNO |
| Unit — Helper (Theory, 3 slučaja) | US-88 | Custom raspon automatski bira granularnost: ≤14 dana → Day, ≤90 → Week, > 90 → Month | `FirstResponseReportTests.ResolveGranularity_Custom_ShouldPickByRangeWidth` | NIJE POKRENUTO LOKALNO |
| Unit — Helper | US-88 | Prazan period vraća `TotalTicketsCount=0`, `TicketsWithResponseCount=0`, average `null` | `FirstResponseReportTests.Build_ShouldReturnZero_WhenNoTicketsForPeriod` | NIJE POKRENUTO LOKALNO |
| Unit — Helper | US-88 | Tiketi su grupirani po bucket-ima, average se računa po bucket-u | `FirstResponseReportTests.Build_ShouldGroupTicketsIntoBuckets_AndComputeAvgPerBucket` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-88 | `POST /api/reports/generate` sa `FIRST_RESPONSE` vraća DTO sa avg, total, withResponse, bucket-ima | `FirstResponseReportIntegrationTests.GenerateReport_FirstResponse_ShouldReturnAvgAndBuckets` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-88 | Prazna baza → `HasData=false`, poruka „Nema podataka za odabrani period.“ | `FirstResponseReportIntegrationTests.GenerateReport_FirstResponse_ShouldReturnNoDataMessage_WhenEmpty` | NIJE POKRENUTO LOKALNO |
| Integracijsko (Theory, 3 slučaja) | US-88 | `BucketGranularityLabel` ovisi o `period` paramentru | `FirstResponseReportIntegrationTests.GenerateReport_FirstResponse_ShouldUseExpectedGranularityLabel` | NIJE POKRENUTO LOKALNO |
| Sigurnosno (deklarativno) | US-88 | `ReportsController` je `[Authorize(Roles="ADMINISTRATOR")]` | `Sprint9AdminDashboardIntegrationTests.ReportsController_GenerateReport_ShouldHaveAuthorizeAdminAttribute` | NIJE POKRENUTO LOKALNO |
| UI | US-88 | FIRST_RESPONSE izvještaj prikazuje prosjek, broj s odgovorom / ukupno i bucket tabelu | `Sprint9FirstResponse.test.jsx` „generiše FIRST_RESPONSE i prikazuje prosjek + bucket tabelu“ | PASS |
| UI | US-88 | Bez odgovora prikazuje poruku „Nema tiketa s prvim odgovorom…“ | `Sprint9FirstResponse.test.jsx` „FIRST_RESPONSE bez odgovora prikazuje informativnu poruku“ | PASS |
| Sistemski (end-to-end) | US-88 | Cijeli tok kroz `ReportsController → ReportService → ReportRepository → InMemory DB` | `Sprint9UserStoriesSystemTests.PB50_GenerateFirstResponseReport_HasBucketsAndAvgConsistentWithDb` | NIJE POKRENUTO LOKALNO |

### Fajlovi sa testovima — PB-50

- [TelecomSupportSystem.Tests/Sprint9/FirstResponseReportTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/FirstResponseReportTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/FirstResponseReportIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/FirstResponseReportIntegrationTests.cs)
- [frontend/src/test/Sprint9FirstResponse.test.jsx](../Project/frontend/src/test/Sprint9FirstResponse.test.jsx)

---

## PB-45 — Admin Dashboard sa ključnim metrikama

### Pokriveni AC (US-71 — Admin dashboard)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-71 | Dashboard vraća sve must-have sekcije: period, status breakdown, KPI, top problemi, role counts, AvgRating | `AdminDashboardServiceTests.GetAdminDashboardAsync_ShouldReturnAllMustHaveSections` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-71 | Status agregati uključuju `OPEN`, `CLOSED`, `CLOSURE_REQUESTED`; ne pojavljuje se `CANCELLED` | `AdminDashboardServiceTests.GetAdminDashboardAsync_StatusBreakdown_ShouldIncludeAllValidStatuses_AndExcludeNonexistentCancelled` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-71 | Prazan period → `TotalTicketsInPeriod=0`, KPI vrijednosti `null` | `AdminDashboardServiceTests.GetAdminDashboardAsync_ShouldReturnZeroCounts_AndNullKpi_WhenNoTickets` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-71 | `GET /api/admin/dashboard` vraća 200 sa svim sekcijama i tačnim brojem aktivnih korisnika po rolama | `Sprint9AdminDashboardIntegrationTests.GetDashboard_EndToEnd_ShouldReturn200_WithMustHaveSections` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-71 | StatusBreakdown na endpoint-u ne uvodi nepostojeće statuse | `Sprint9AdminDashboardIntegrationTests.GetDashboard_EndToEnd_StatusBreakdown_DoesNotIntroduceUnknownStatuses` | NIJE POKRENUTO LOKALNO |
| Sigurnosno (deklarativno) | US-71 | `AdminController` je `[Authorize(Roles="ADMINISTRATOR")]` — non-admin ne može doći do GET /api/admin/dashboard | `Sprint9AdminDashboardIntegrationTests.AdminController_GetDashboard_ShouldHaveAuthorizeAdminAttribute` | NIJE POKRENUTO LOKALNO |
| Performansno (NFR) | US-71 | Dashboard < 5 s za 200 tiketa, 5 agenata, 44 klijenta | `AdminDashboardPerformanceTests.GetDashboard_ShouldCompleteWithinFiveSeconds_ForTypicalDataset` | NIJE POKRENUTO LOKALNO |
| UI | US-71, US-86 | Sve must-have KPI kartice se prikazuju | `Sprint9AdminDashboard.test.jsx` „prikazuje sve must-have KPI kartice“ | PASS |
| UI | US-71 | Prikazuju se aktivni korisnici po rolama (Klijenti / Agenti / Tehničari / Admini) | `Sprint9AdminDashboard.test.jsx` „prikazuje aktivne korisnike po rolama“ | PASS |
| UI | US-71 | Prazno stanje: KPI „Prosj. 1. odgovor“ prikazuje empty poruku umjesto „0 min“ | `Sprint9AdminDashboard.test.jsx` (empty section) i `Sprint9FirstResponse.test.jsx` | PASS |
| UI | US-71 | Reports mod NE poziva `GET /api/admin/dashboard` (admin metrike su odvojene od /reports) | `Sprint9AdminDashboard.test.jsx` „reports mod NE prikazuje KPI…“ | PASS |
| Sistemski (end-to-end) | US-71 | Dashboard agregat odražava stvarne tikete u bazi | `Sprint9UserStoriesSystemTests.PB45_DashboardAggregates_ReflectActualTicketsInDb` | NIJE POKRENUTO LOKALNO |

### Pokriveni AC (US-72 — Globalni vremenski filter)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service (Theory, 4 slučaja) | US-72 | Backend prima `week`, `month`, `year`, `alltime` bez greške | `AdminDashboardServiceTests.GetAdminDashboardAsync_ShouldAcceptQuickPeriods` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-72 | Nevalidan custom raspon (kraj prije početka) → `ArgumentException` (→ 400) | `AdminDashboardServiceTests.GetAdminDashboardAsync_ShouldThrow_WhenCustomRangeReversed` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-72 | Nepoznat period (npr. „nonsense“) → `ArgumentException` | `AdminDashboardServiceTests.GetAdminDashboardAsync_ShouldThrow_WhenPeriodUnknown` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-72 | Endpoint vraća 400 BadRequest na obrnut custom raspon | `Sprint9AdminDashboardIntegrationTests.GetDashboard_EndToEnd_ShouldReturn400_WhenCustomRangeReversed` | NIJE POKRENUTO LOKALNO |
| UI | US-72 | Brzi periodi (Sedmica/Mjesec/Godina) i Prilagođeno su prisutni | `Sprint9AdminDashboard.test.jsx` „prikazuje brze periode…“ | PASS |
| UI | US-72 | Promjena perioda + Primijeni → osvježi dashboard | `Sprint9AdminDashboard.test.jsx` „promjena perioda i Primijeni dugme osvježava dashboard“ | PASS |
| UI | US-72 | Nevalidan custom raspon na Primijeni prikazuje poruku greške | `Sprint9AdminDashboard.test.jsx` „nevalidan custom raspon prikazuje grešku na Primijeni…“ | PASS |
| Sistemski (end-to-end) | US-72 | Custom range odbijen kroz puni stack — ne dohvataju se podaci za nevalidan raspon | `Sprint9UserStoriesSystemTests.PB45_GlobalFilter_RejectsInvalidCustomRange_WithoutData` | NIJE POKRENUTO LOKALNO |
| **GAP (frontend)** | US-72 | „Ne smije pozvati API kada je raspon nevalidan“ — frontend `AdminDashboardSection` ima auto-reload preko useEffect na promjenu filtera, pa `GET /api/admin/dashboard` može biti pozvan i prije nego korisnik klikne Primijeni. Backend ipak vraća 400 BadRequest pa nema upisa, ali poziv API-ja se događa. Test eksplicitno dokumentuje ovo (vidi komentar u testu). Nismo mijenjali produkcijski kod. | — | GAP |

### Pokriveni AC (US-82 — Grafovi)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| UI | US-82 | Grafovi (Po statusu, Top tipovi problema, Opterećenje agenata) prikazuju se na metrics modu | `Sprint9AdminDashboard.test.jsx` „prikazuje grafove na metrics modu“ | PASS |
| UI | US-82 | Grafovi NISU na reports modu | `Sprint9AdminDashboard.test.jsx` (kroz „reports mod NE prikazuje KPI…“) i postojeći `Reports.test.jsx` | PASS |
| UI | US-82 | Kada nema podataka — graf prikazuje poruku umjesto praznog grafikona | `Sprint9AdminDashboard.test.jsx` „grafovi prikazuju poruku umjesto praznog grafikona“ | PASS |
| UI | US-82 | Grafovi poštuju globalni vremenski filter (preko buildQuery koji se prosljeđuje API-ju) | dokazano kroz „promjena perioda i Primijeni…“ | PASS |

### Pokriveni AC (US-83 — Generisanje izvještaja)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service (Theory, 5 slučajeva) | US-83 | Svi tipovi (`TICKET_COUNT`, `TICKET_STATUS`, `PROBLEM_TYPE`, `USER_RATINGS`, `FIRST_RESPONSE`) vraćaju DTO | `AdminDashboardServiceTests.GenerateReportAsync_ShouldReturnDto_ForEachSupportedType` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-83 | `TEAM_WORKLOAD` čita agent rows iz repository-ja | `AdminDashboardServiceTests.GenerateReportAsync_TeamWorkload_ShouldFetchAgentRows` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-83 | `TICKET_STATUS` za custom period > 90 dana postavlja `ShowLargePeriodWarning=true` | `AdminDashboardServiceTests.GenerateReportAsync_TicketStatus_ShouldSetLargePeriodWarning_ForCustomLargeRange` | NIJE POKRENUTO LOKALNO |
| Unit — Service | US-83 | Prazan period → `HasData=false`, poruka „Nema podataka…“ | `AdminDashboardServiceTests.GenerateReportAsync_ShouldReturnNoDataMessage_WhenEmptyTickets` | NIJE POKRENUTO LOKALNO |
| Integracijsko | US-83 | `TICKET_COUNT` izvještaj vraća tačan broj iz baze | `Sprint9AdminDashboardIntegrationTests.GenerateReport_EndToEnd_TicketCount_ShouldReturnRealCount` | NIJE POKRENUTO LOKALNO |
| UI | US-83 | `/reports` sadrži vremenski period i sekciju generisanja izvještaja (bez KPI/grafova) | `Sprint9AdminDashboard.test.jsx` „reports mod NE prikazuje KPI kartice…“ | PASS |
| UI | US-83 | Sve report tipove podržava select | `Sprint9AdminDashboard.test.jsx` „podržani svi report tipovi u select-u“ | PASS |
| UI | US-83 | „Generiši izvještaj“ poziva `POST /api/reports/generate` s odabranim tipom i periodom | `Sprint9AdminDashboard.test.jsx` „Generiši izvještaj poziva POST /api/reports/generate…“ | PASS |
| UI | US-83 | `TICKET_STATUS` veliki period prikazuje upozorenje | `Sprint9AdminDashboard.test.jsx` „warning za veliki period…“ | PASS |
| UI | US-83 | Prazan rezultat prikazuje informativnu poruku | `Sprint9AdminDashboard.test.jsx` „prazan rezultat prikazuje informativnu poruku“ | PASS |

### Pokriveni AC (US-84 — Drill-down)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| UI | US-84 | Klik na KPI „Kreirani tiketi“ navigira na `/tickets?...` (sa period query) | `Sprint9AdminDashboard.test.jsx` „klik na Kreirani tiketi KPI navigira na /tickets“ | PASS |
| UI | US-84 | Klik na „Otvoreni“ KPI dodaje `status=OPEN` u URL | `Sprint9AdminDashboard.test.jsx` „klik na Otvoreni KPI navigira na /tickets sa status=OPEN“ | PASS |
| UI | US-84 | Drill-down poštuje globalni filter (kroz `buildQuery()` → URLSearchParams) | dokazano u istom testu (period parametar je dio URL-a) | PASS |

### Pokriveni AC (US-85 — Export placeholder)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| UI | US-85 | „Export“ dugme postoji u sekciji izvještaja | `Sprint9AdminDashboard.test.jsx` „Export dugme postoji ali je disabled“ | PASS |
| UI | US-85 | Dugme je `disabled` (nije funkcionalno) | isti test (`expect(exportBtn).toBeDisabled()`) | PASS |
| UI | US-85 | Napomena „CSV export planiran (PB-46)“ je vidljiva | isti test (`getByText(/CSV export planiran/i)`) | PASS |
| Integracijsko (negativan dokaz) | US-85 | Backend `ReportResultDto` ne sadrži `File`/`FileBytes`/`Csv` polje | `Sprint9AdminDashboardIntegrationTests.GenerateReport_EndToEnd_ShouldNotReturnExportArtifact` | NIJE POKRENUTO LOKALNO |

### Pokriveni AC (US-86 — KPI layout/placeholder)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| UI | US-86 | Layout svih must-have KPI kartica je prisutan | `Sprint9AdminDashboard.test.jsx` „prikazuje sve must-have KPI kartice“ | PASS |
| UI | US-86 | Dashboard zove `GET /api/admin/dashboard` na mount-u | dokazano kroz `mocks.getAdminDashboard.mock.calls` u svim metrics testovima | PASS |
| UI | US-86 | Reports povezuje sa `POST /api/reports/generate` | `Sprint9AdminDashboard.test.jsx` „Generiši izvještaj poziva POST /api/reports/generate…“ | PASS |
| UI | US-86 | Placeholder/prazna stanja postoje kada backend nema podataka | `Sprint9AdminDashboard.test.jsx` „grafovi prikazuju poruku umjesto praznog grafikona“ i `FirstResponse.test.jsx` empty | PASS |

### Fajlovi sa testovima — PB-45

- [TelecomSupportSystem.Tests/Sprint9/AdminDashboardServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/AdminDashboardServiceTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/AdminDashboardIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/AdminDashboardIntegrationTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/AdminDashboardPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/AdminDashboardPerformanceTests.cs)
- [TelecomSupportSystem.Tests/Sprint9/Sprint9UserStoriesSystemTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/Sprint9UserStoriesSystemTests.cs)
- [frontend/src/test/Sprint9AdminDashboard.test.jsx](../Project/frontend/src/test/Sprint9AdminDashboard.test.jsx)

---

## Sistemski testovi — Sprint 9

`Sprint9UserStoriesSystemTests.cs` sadrži 4 end-to-end testa koja prolaze kroz Controller → Service → Repository → EF InMemory za PB-45, PB-50 i PB-51.

| Test | PB | Šta verifikuje |
| --- | --- | --- |
| `PB51_CreateLoginDeactivateBlockedLogin_WorksThroughControllerServiceRepo` | PB-51 (US-73/75/89) | Admin kreira klijenta → klijent se prijavi → admin deaktivira → login odbijen (401) → reaktivacija → login ponovo radi |
| `PB45_DashboardAggregates_ReflectActualTicketsInDb` | PB-45 / PB-50 | Dashboard agregat (TotalTicketsInPeriod, AvgFirstResponseMinutes, ActiveUsersByRole) odgovara stvarnom stanju baze |
| `PB45_GlobalFilter_RejectsInvalidCustomRange_WithoutData` | PB-45 (US-72) | Backend odbija obrnut custom raspon s 400 BadRequest |
| `PB50_GenerateFirstResponseReport_HasBucketsAndAvgConsistentWithDb` | PB-50 (US-88) | `POST /api/reports/generate` sa FIRST_RESPONSE vraća konsistentne bucket-e, average, i broj odgovorenih tiketa |

---

## Veza sa Test Strategijom

Test Strategy iz Sprint 3 definiše 7 osnovnih nivoa testiranja (unit / integracijsko / sistemsko / UI / sigurnosno / performansno / prihvatno) plus regression/smoke. Sprint 9 ih pokriva kako slijedi:

| Test strategija nivo | US | PB | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — backend servis | US-73, US-74, US-75, US-89 | PB-51 | `UserAccountManagementServiceTests` (kreiranje, lozinka hash, update, deaktivacija, pretraga) | NIJE POKRENUTO LOKALNO |
| Unit — backend controller | US-73, US-74, US-75, US-89 | PB-51 | `UserAccountManagementControllerTests` (HTTP kodovi 200/400/401/403/404/409) | NIJE POKRENUTO LOKALNO |
| Integracijsko — backend | US-73 do US-89 (PB-51) | PB-51 | `UserAccountManagementIntegrationTests` (sa AuthController smoke) | NIJE POKRENUTO LOKALNO |
| Sigurnosno (rola + zaštita podataka) | US-73, US-74, US-75, US-89 | PB-51 | `UserAccountManagementSecurityTests` (RBAC, blokada login-a za INACTIVE, DTO sanitizacija) | NIJE POKRENUTO LOKALNO |
| Unit — backend helper | US-87, US-88 | PB-50 | `FirstResponseReportTests` (TicketMetricsHelper, FirstResponseReportHelper) | NIJE POKRENUTO LOKALNO |
| Integracijsko — backend | US-87, US-88 | PB-50 | `FirstResponseReportIntegrationTests` (AdminController + ReportsController) | NIJE POKRENUTO LOKALNO |
| Unit — backend servis | US-71, US-72, US-82, US-83, US-86 | PB-45 | `AdminDashboardServiceTests` (sve sekcije, status statusi bez CANCELLED, period validacija, sve report tipove) | NIJE POKRENUTO LOKALNO |
| Integracijsko — backend | US-71, US-72, US-83, US-85 | PB-45 | `Sprint9AdminDashboardIntegrationTests` (dashboard endpoint + reports endpoint + RBAC attribute provjera) | NIJE POKRENUTO LOKALNO |
| Performansno — backend (NFR) | US-71 | PB-45 | `AdminDashboardPerformanceTests` (< 5 s za 200 tiketa) | NIJE POKRENUTO LOKALNO |
| Sistemski — end-to-end svi slojevi | PB-45, PB-50, PB-51 | sve tri | `Sprint9UserStoriesSystemTests` (4 testa) | NIJE POKRENUTO LOKALNO |
| UI — stranice (React forme, KPI, filteri, drill-down) | US-71, US-72, US-73, US-74, US-75, US-82, US-83, US-84, US-85, US-86, US-87, US-88, US-89 | PB-45, PB-50, PB-51 | `Sprint9CreateUser.test.jsx`, `Sprint9UsersList.test.jsx`, `Sprint9AdminDashboard.test.jsx`, `Sprint9FirstResponse.test.jsx` | **PASS (40/40)** |
| Acceptance / smoke | sve gore | sve gore | UI testovi iznad funkcionišu kao smoke acceptance jer prolaze ključne korisničke tokove s mock servisima | **PASS** |
| Regression | postojeći testovi | sve | Postojeća 203 frontend testa nisu mijenjana i prošla su (vidi „Run backend and frontend test suites“) | **PASS (203/203)** |

---

## Napomena o pristupu

- **Produkcijski kod nije mijenjan.** Sve nove provjere implementirane su samo kao test fajlovi u `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/` i `Project/frontend/src/test/Sprint9*.test.jsx`.
- **Postojeći testovi nisu mijenjani.** Niti jedna postojeća .cs/.jsx test datoteka nije modificirana — samo su dodani novi fajlovi, što potvrđuje i `git status`.
- **Backend testovi nisu lokalno pokrenuti** jer `.NET SDK` nije dostupan u trenutnom okruženju. Test fajlovi su napisani po istom obrascu kao postojeći Sprint 7/Sprint 8 testovi (xUnit + Moq + EF InMemory + FluentAssertions), s istim referencama, isti pattern claims setup, identičan stil DTO inicijalizacije. Očekuje se da kompajliraju i prolaze kada se izvrše komande iz „Lokalno pokretanje testova“.
- **Frontend testovi prolaze (40/40 novih + 203/203 postojećih = 243/243)** u Vitest okruženju.
- **Gap-ovi:**
  - **US-74 audit log** — produkcijski kod nema audit log table za `Users` izmjene (samo `SubscriptionAuditLog` postoji). Ne ispravljamo produkcijski kod; gap je evidentiran iznad.
  - **US-72 auto-reload na filter change** — `AdminDashboardSection.jsx` poziva `loadDashboard` kroz `useEffect` čim se period ili custom datumi promijene, pa `GET /api/admin/dashboard` može biti pozvan i prije nego korisnik klikne „Primijeni“ (s nevalidnim datumima). Backend ipak odbija (400), ali frontend AC kaže „ne smije pozvati API“. Ne ispravljamo produkcijski kod; gap je evidentiran iznad.
  - **US-89 upozorenje za agenta sa aktivnim tiketima** — backend trenutno baca `InvalidOperationException` koji se mapira u 400 BadRequest s porukom („Korisnik ima otvorene tikete. Potrebno ih je prvo preusmjeriti.“). To je pokriveno testovima. Frontend UI ne prikazuje eksplicitnu confirm modal (samo prikazuje server poruku) — moglo bi se proširiti u kasnijem sprintu.
- **„Nedodijeljen“ stanje, mapiranje statusa, scoring algoritam, auto-assign** — naslijeđeno iz prethodnih sprintova, postojeći testovi i dalje važe.

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
