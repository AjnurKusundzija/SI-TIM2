# Proof of Testing — Sprint 6
---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30 | xUnit + Moq + EF InMemory | 28 novih testova (Sprint 6) | PASS |
| Unit — Frontend | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | Vitest + Testing Library | 17 novih testova (Sprint 6) | PASS |
| Integracijsko — Backend | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | xUnit + EF InMemory | 22 nova testa (Sprint 6) | PASS |
| Performansno — Backend | US-1, US-2, US-8, US-11, US-14, US-19, US-29, US-30, US-31, US-32 | xUnit + Stopwatch | 6 novih testova (Sprint 6) | PASS |
| Sistemsko — Frontend | US-1, US-2, US-3, US-8, US-11, US-12, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | Vitest + Testing Library | 8 testova (Sprint 6) | PASS |
| Prihvatno — Frontend | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | Vitest + Testing Library | 9 testova (Sprint 6) | PASS |
| **Ukupno Sprint 6** | **US-1 do US-3, US-8 do US-15, US-19, US-20, US-29 do US-32** | | **90 novih testova** | **PASS** |
| **Ukupno projekat** | **US-1 do US-3, US-8 do US-15, US-19, US-20, US-29 do US-32** | | **119 backend + 96 frontend = 215** | **PASS** |

---

## PB-19 — Prijava i upravljanje sesijama

### Pokriveni AC (US-1, US-2, US-3)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-1 | Ispravna validacija emaila i lozinke | `AuthServiceTests` — provjera toka prijave | PASS |
| Unit | US-3 | Generička poruka greške bez otkrivanja detalja | `AuthControllerTests` — 401 odgovor | PASS |
| UI | US-1, US-2, US-3 | Forma prijave, poruka greške, stanje sesije | `Login.test.jsx` — 4 testa | PASS |
| Integracijsko | US-1 | Login s ispravnim kredencijalima vraća JWT kroz sve slojeve | `AuthIntegrationTests.Login_WithValidCredentials_ReturnsJwtTokenThroughFullStack` | PASS |
| Integracijsko | US-3 | Pogrešna lozinka vraća 401 generičkim odgovorom | `AuthIntegrationTests.Login_WithWrongPassword_ReturnsUnauthorizedWithGenericMessage` | PASS |
| Integracijsko | US-3 | Nepostojeci email vraća 401 (isti odgovor kao pogrešna lozinka) | `AuthIntegrationTests.Login_WithUnknownEmail_ReturnsUnauthorized` | PASS |
| Integracijsko | US-2 | Odjava revokuje refresh token u bazi | `AuthIntegrationTests.Logout_AfterSuccessfulLogin_RevokesRefreshTokenInDatabase` | PASS |
| Integracijsko | US-1 | Refresh token daje novi access token | `AuthIntegrationTests.Refresh_WithValidToken_ReturnsNewAccessToken` | PASS |
| Performansno | US-1 | Tok prijave (Controller → Service + BCrypt → Repository) < 2 sekunde | `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` | PASS |
| Sistemsko | US-1 | Login pozvan sa ispravnim podacima; nema console.error | `AuthSystem.test.jsx > korisnik unosi kredencijale...` | PASS |
| Sistemsko | US-3 | Greška prikazana bez otkrivanja koji podatak je pogrešan | `AuthSystem.test.jsx > pogresan unos prikazuje poruku greske...` | PASS |
| Prihvatno | US-1 | Klijent se uspješno prijavljuje i sistem ga preusmjerava | `AuthAcceptance.test.jsx > klijent se uspjesno prijavljuje...` | PASS |
| Prihvatno | US-3 | Poruka greške ne otkriva koji podatak je pogrešan — generički tekst vidljiv | `AuthAcceptance.test.jsx > poruka greske ne otkriva koji podatak...` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/Auth/AuthServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Auth/AuthServiceTests.cs)
- [TelecomSupportSystem.Tests/Auth/AuthControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Auth/AuthControllerTests.cs)
- [TelecomSupportSystem.Tests/Integration/AuthIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AuthIntegrationTests.cs) — 5 testova
- [TelecomSupportSystem.Tests/Performance/AuthPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/AuthPerformanceTests.cs) — 1 test
- [frontend/src/test/Login.test.jsx](../Project/frontend/src/test/Login.test.jsx) — 4 testa
- [frontend/src/test/system/AuthSystem.test.jsx](../Project/frontend/src/test/system/AuthSystem.test.jsx) — 2 testa
- [frontend/src/test/acceptance/AuthAcceptance.test.jsx](../Project/frontend/src/test/acceptance/AuthAcceptance.test.jsx) — 2 testa

---

## PB-22 — Kreiranje tiketa

### Pokriveni AC (US-8, US-9, US-10)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-8, US-9 | Odbijanje nevažećeg modela (400); novi tiket uvijek OPEN | `TicketControllerTests`, `TicketServiceTests` | PASS |
| Unit | US-9, US-10 | Svi prioriteti i tipovi problema se ispravno čuvaju i mapiraju | `TicketRepositoryTests` | PASS |
| UI | US-8, US-9, US-10 | Validacija forme, poruka potvrde, reset forme, greška pri 401 | `CreateTicket.test.jsx` — 11 testova | PASS |
| Integracijsko | US-8 | Kreiran tiket persistira u bazi i vraća 201 CreatedAtAction | `TicketIntegrationTests.CreateTicket_PersistsToDatabase_AndReturnsCreated` | PASS |
| Integracijsko | US-8 | Novi tiket uvijek dobija status OPEN kroz sve slojeve | `TicketIntegrationTests.CreateTicket_NewTicketAlwaysHasStatusOpen` | PASS |
| Performansno | US-8 | Kreiranje tiketa kroz sve slojeve < 3 sekunde (NFR-04) | `TicketPerformanceTests.CreateTicket_ShouldCompleteWithinThreeSeconds_InTestEnvironment` | PASS |
| Sistemsko | US-8, US-9, US-10 | Korisnik popunjava formu i uspješno kreira tiket — forma se resetuje | `TicketCreateSystem.test.jsx > korisnik popunjava formu...` | PASS |
| Sistemsko | US-8 | Forma odbija submit bez naslova — createTicket se ne poziva | `TicketCreateSystem.test.jsx > forma odbija submit bez naslova...` | PASS |
| Prihvatno | US-8, US-9, US-10 | Forma ima sva potrebna polja; createTicket pozvan s ispravnim PascalCase podacima | `CreateTicketAcceptance.test.jsx > klijent popunjava formu...` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketControllerTests.cs)
- [TelecomSupportSystem.Tests/TicketT/TicketServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketServiceTests.cs)
- [TelecomSupportSystem.Tests/Integration/TicketIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/TicketIntegrationTests.cs) — 5 testova (PB-22 i PB-23)
- [TelecomSupportSystem.Tests/Performance/TicketPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/TicketPerformanceTests.cs) — 2 testa (PB-22 i PB-23)
- [frontend/src/test/CreateTicket.test.jsx](../Project/frontend/src/test/CreateTicket.test.jsx) — 11 testova
- [frontend/src/test/system/TicketCreateSystem.test.jsx](../Project/frontend/src/test/system/TicketCreateSystem.test.jsx) — 2 testa
- [frontend/src/test/acceptance/CreateTicketAcceptance.test.jsx](../Project/frontend/src/test/acceptance/CreateTicketAcceptance.test.jsx) — 1 test

---

## PB-23 — Pregled vlastitih tiketa

### Pokriveni AC (US-11, US-12, US-13)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-11, US-12 | Korisnik vidi samo vlastite tikete; OPEN/CLOSED mapiranje; prazna lista | `TicketServiceTests`, `TicketRepositoryTests` | PASS |
| UI | US-11, US-12, US-13 | Lista tiketa, statusi, filteri, pretraga, prazno stanje, greška | `MyTickets.test.jsx` — 11 testova | PASS |
| Integracijsko | US-11 | Korisnik vidi samo vlastite tikete — tiketi drugog korisnika se ne vraćaju | `TicketIntegrationTests.GetMyTickets_ReturnsOnlyOwnTickets_NotOtherUsersTickets` | PASS |
| Integracijsko | US-12 | OPEN i CLOSED statusi se ispravno mapiraju u string | `TicketIntegrationTests.GetMyTickets_MapsStatusCorrectlyToString` | PASS |
| Integracijsko | US-11 | Prazna lista kada korisnik nema tiketa | `TicketIntegrationTests.GetMyTickets_ReturnsEmptyList_WhenUserHasNoTickets` | PASS |
| Performansno | US-11 | Lista od 200 tiketa ucitava se u manje od 2 sekunde | `TicketPerformanceTests.GetMyTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` | PASS |
| Sistemsko | US-11, US-12 | Korisnik vidi listu vlastitih tiketa bez console.error greske | `MyTicketsSystem.test.jsx > korisnik vidi listu...` | PASS |
| Prihvatno | US-11, US-12, US-13 | Klijent vidi vlastite tikete; pretraga i filter rade ispravno | `MyTicketsAcceptance.test.jsx > klijent vidi vlastite tikete...` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketServiceTests.cs)
- [TelecomSupportSystem.Tests/TicketT/TicketRepositoryTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketRepositoryTests.cs)
- [TelecomSupportSystem.Tests/Integration/TicketIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/TicketIntegrationTests.cs) — 5 testova (PB-22 i PB-23)
- [TelecomSupportSystem.Tests/Performance/TicketPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/TicketPerformanceTests.cs) — 2 testa (PB-22 i PB-23)
- [frontend/src/test/MyTickets.test.jsx](../Project/frontend/src/test/MyTickets.test.jsx) — 11 testova
- [frontend/src/test/system/MyTicketsSystem.test.jsx](../Project/frontend/src/test/system/MyTicketsSystem.test.jsx) — 1 test
- [frontend/src/test/acceptance/MyTicketsAcceptance.test.jsx](../Project/frontend/src/test/acceptance/MyTicketsAcceptance.test.jsx) — 1 test

---

## PB-24 — Detaljan prikaz tiketa

### Pokriveni AC (US-14, US-15)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-14 | Klijent moze vidjeti vlastiti tiket | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenClientIsOwner` | PASS |
| Unit | US-14 | Tiket koji ne postoji baca KeyNotFoundException | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowKeyNotFound_WhenTicketMissing` | PASS |
| Unit — Sigurnosno | US-14 | Klijent ne moze vidjeti tudji tiket | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherTicket` | PASS |
| Unit — Sigurnosno | US-14 | Agent moze vidjeti svaki tiket | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenAgentAccessesAnyTicket` | PASS |
| Unit | US-14 | Sva polja tiketa se ispravno mapiraju u DTO | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldMapAllFieldsToDto` | PASS |
| Unit | US-14 | Controller vraca 200 OK za vlasnika | `TicketDetailControllerTests.GetTicketById_ReturnsOk_WhenTicketFound` | PASS |
| Unit | US-14 | Controller vraca 404 za nepostojeci tiket | `TicketDetailControllerTests.GetTicketById_ReturnsNotFound_WhenTicketMissing` | PASS |
| Unit — Sigurnosno | US-14 | Controller vraca 403 za neovlasteni pristup | `TicketDetailControllerTests.GetTicketById_ReturnsForbid_WhenClientAccessesForeignTicket` | PASS |
| Unit — Sigurnosno | US-14 | Controller vraca 401 bez JWT claimova | `TicketDetailControllerTests.GetTicketById_ReturnsUnauthorized_WhenNoUserClaim` | PASS |
| UI | US-14 | UI prikazuje detalje tiketa | `TicketDetail.test.jsx > renders ticket details after successful load` | PASS |
| UI | US-14 | UI prikazuje ime klijenta i agenta | `TicketDetail.test.jsx > shows client name and assigned agent name` | PASS |
| UI | US-15 | UI prikazuje historiju komentara | `TicketDetail.test.jsx > displays comment history when comments exist` | PASS |
| UI | US-14 | UI prikazuje error state pri API gresci | `TicketDetail.test.jsx > shows error empty state when API call fails` | PASS |
| Integracijsko | US-14 | CLIENT owner → 200 sa svim ispravno mapiranim poljima (Title, Description, Status, ClientName) | `TicketDetailIntegrationTests.GetTicketById_ClientOwner_ReturnsDetailDtoWithAllFields` | PASS |
| Integracijsko | US-14 | AGENT vidi svaki tiket bez obzira na vlasnistvo | `TicketDetailIntegrationTests.GetTicketById_AgentRole_CanAccessAnyTicket` | PASS |
| Integracijsko — Sigurnosno | US-14 | CLIENT na tudem tiketu → 403 Forbid | `TicketDetailIntegrationTests.GetTicketById_ClientAccessingOtherTicket_ReturnsForbid` | PASS |
| Integracijsko | US-14 | Nepostojeci tiket → 404 NotFound | `TicketDetailIntegrationTests.GetTicketById_TicketDoesNotExist_ReturnsNotFound` | PASS |
| Performansno | US-14 | Prikaz detalja tiketa (s Creator i Assignments include) < 2 sekunde | `TicketDetailPerformanceTests.GetTicketById_ShouldLoadWithinTimeLimit_InTestEnvironment` | PASS |
| Sistemsko | US-14, US-15 | Korisnik otvara tiket i vidi naslov, opis i historiju komentara bez console.error | `TicketDetailSystem.test.jsx > korisnik otvara tiket i vidi sve detalje...` | PASS |
| Prihvatno | US-14, US-15 | Klijent vidi naslov, opis, ime agenta i historiju komunikacije bez poruke o gresci | `TicketDetailAcceptance.test.jsx > klijent vidi kompletan prikaz tiketa...` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketDetailServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketDetailServiceTests.cs) — 5 testova
- [TelecomSupportSystem.Tests/TicketT/TicketDetailControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketDetailControllerTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Integration/TicketDetailIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/TicketDetailIntegrationTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Performance/TicketDetailPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/TicketDetailPerformanceTests.cs) — 1 test
- [frontend/src/test/TicketDetail.test.jsx](../Project/frontend/src/test/TicketDetail.test.jsx) — 8 testova (pokriva PB-24 i PB-27)
- [frontend/src/test/system/TicketDetailSystem.test.jsx](../Project/frontend/src/test/system/TicketDetailSystem.test.jsx) — 1 test
- [frontend/src/test/acceptance/TicketDetailAcceptance.test.jsx](../Project/frontend/src/test/acceptance/TicketDetailAcceptance.test.jsx) — 1 test

---

## PB-27 — Komunikacija kroz tiket

### Pokriveni AC (US-19, US-20)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit | US-19 | Uspjesno slanje komentara | `CommentServiceTests.AddCommentAsync_ShouldSucceed_WhenValidContentAndOwner` | PASS |
| Unit | US-19 | Odbijanje sadrzaja duzeg od 1000 znakova | `CommentServiceTests.AddCommentAsync_ShouldThrowArgumentException_WhenContentTooLong` | PASS |
| Unit | US-19 | KeyNotFound za nepostojeci tiket | `CommentServiceTests.AddCommentAsync_ShouldThrowKeyNotFound_WhenTicketMissing` | PASS |
| Unit — Sigurnosno | US-19 | UnauthorizedAccess za klijenta koji nije vlasnik | `CommentServiceTests.AddCommentAsync_ShouldThrowUnauthorized_WhenClientNotOwner` | PASS |
| Unit | US-19, US-20 | GetComments vraca listu za vlasnika tiketa | `CommentServiceTests.GetCommentsForTicketAsync_ShouldReturnList_WhenCallerIsOwner` | PASS |
| Unit — Sigurnosno | US-19, US-20 | GetComments baca UnauthorizedAccess za tredju stranu | `CommentServiceTests.GetCommentsForTicketAsync_ShouldThrowUnauthorized_WhenCallerIsNotOwner` | PASS |
| Unit | US-19, US-20 | Controller vraca 200 OK sa komentarima | `CommentControllerTests.GetComments_ReturnsOk_WhenAuthorized` | PASS |
| Unit — Sigurnosno | US-19, US-20 | Controller vraca 401 bez JWT claimova (GET) | `CommentControllerTests.GetComments_ReturnsUnauthorized_WhenNoUserClaim` | PASS |
| Unit | US-19 | Controller vraca 400 za prazan sadrzaj | `CommentControllerTests.AddComment_ReturnsBadRequest_WhenContentEmpty` | PASS |
| Unit — Sigurnosno | US-19 | Controller vraca 401 bez JWT claimova (POST) | `CommentControllerTests.AddComment_ReturnsUnauthorized_WhenNoUserClaim` | PASS |
| UI | US-19 | UI input vidljiv za otvoreni tiket | `TicketDetail.test.jsx > shows message input for open ticket` | PASS |
| UI | US-19 | UI input skriven za zatvoreni tiket | `TicketDetail.test.jsx > hides message input for closed ticket` | PASS |
| UI | US-19 | Dugme Pošalji onemoguceno za prazan unos | `TicketDetail.test.jsx > disables send button when message is empty` | PASS |
| UI | US-19 | Slanje poruke poziva addComment sa ispravnim parametrima | `TicketDetail.test.jsx > submits a message by calling addComment` | PASS |
| Integracijsko | US-19, US-20 | Vlasnik dohvata komentare → lista s autorom (AuthorName = "Merjem Omerovic") | `CommentIntegrationTests.GetComments_OwnerOfTicket_ReturnsCommentList` | PASS |
| Integracijsko | US-19 | Slanje komentara persistira u bazi i vraca DTO s autorom | `CommentIntegrationTests.AddComment_ValidContent_PersistsAndReturnsDto` | PASS |
| Integracijsko | US-19 | Sadrzaj >1000 znakova → 400 BadRequest | `CommentIntegrationTests.AddComment_ContentTooLong_ReturnsBadRequest` | PASS |
| Integracijsko — Sigurnosno | US-19, US-20 | Klijent na tudem tiketu → 403 Forbid | `CommentIntegrationTests.GetComments_ClientAccessingOtherTicket_ReturnsForbid` | PASS |
| Performansno | US-19, US-20 | Lista od 100 komentara ucitava se u manje od 2 sekunde | `CommentPerformanceTests.GetComments_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` | PASS |
| Sistemsko | US-19 | Korisnik unosi poruku i šalje je — addComment pozvan s ispravnim ticketId i sadrzajem, nema console.error | `CommunicationSystem.test.jsx > korisnik otvara tiket, unosi poruku i šalje je...` | PASS |
| Prihvatno | US-19 | Klijent šalje poruku na otvorenom tiketu (addComment pozvan s ispravnim podacima) | `CommunicationAcceptance.test.jsx > klijent moze poslati poruku agentu...` | PASS |
| Prihvatno | US-19 | Input za poruku je skriven kada je tiket zatvoren — komunikacija nije moguca | `CommunicationAcceptance.test.jsx > input za poruku je skriven kada je tiket zatvoren...` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/Communication/CommentServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Communication/CommentServiceTests.cs) — 6 testova
- [TelecomSupportSystem.Tests/Communication/CommentControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Communication/CommentControllerTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Integration/CommentIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/CommentIntegrationTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Performance/CommentPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/CommentPerformanceTests.cs) — 1 test
- [frontend/src/test/TicketDetail.test.jsx](../Project/frontend/src/test/TicketDetail.test.jsx) — 8 testova (pokriva PB-24 i PB-27)
- [frontend/src/test/system/CommunicationSystem.test.jsx](../Project/frontend/src/test/system/CommunicationSystem.test.jsx) — 1 test
- [frontend/src/test/acceptance/CommunicationAcceptance.test.jsx](../Project/frontend/src/test/acceptance/CommunicationAcceptance.test.jsx) — 2 testa

---

## PB-32 — Pregled svih tiketa

### Pokriveni AC (US-29, US-30)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Repozitorij | US-29 | GetAllAsync vraca sve tikete | `AllTicketsRepositoryTests.GetAllAsync_ReturnsAllTickets` | PASS |
| Unit — Repozitorij | US-29 | Prazna lista kada nema tiketa | `AllTicketsRepositoryTests.GetAllAsync_ReturnsEmpty_WhenNoTickets` | PASS |
| Unit — Repozitorij | US-29 | Tiketi sortirani od najnovijeg | `AllTicketsRepositoryTests.GetAllAsync_ReturnsTickets_OrderedByDateDescending` | PASS |
| Unit — Repozitorij | US-29, US-30 | GetByAssigneeIdAsync filtrira po dodijeljenosti | `AllTicketsRepositoryTests.GetByAssigneeIdAsync_ReturnsOnlyAssignedTickets` | PASS |
| Unit | US-29 | Agent bez filtera dobija sve tikete | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldCallGetAllAsync_WhenAgentAndNotAssignedOnly` | PASS |
| Unit | US-29, US-30 | Agent sa assignedOnly dobija dodijeljene | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldCallGetByAssigneeIdAsync_WhenAgentAndAssignedOnly` | PASS |
| Unit | US-30 | Tehnicar uvijek dobija samo dodijeljene | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldCallGetByAssigneeIdAsync_WhenTechnicianRole` | PASS |
| Unit — Sigurnosno | US-29 | Klijent dobija UnauthorizedAccessException | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldThrowUnauthorized_WhenClientRole` | PASS |
| Unit | US-29 | Agent dobija 200 OK sa listom | `AllTicketsControllerTests.GetAllTickets_ReturnsOk_WhenAgentRole` | PASS |
| Unit — Sigurnosno | US-29 | Klijent dobija 403 Forbid | `AllTicketsControllerTests.GetAllTickets_ReturnsForbid_WhenClientRole` | PASS |
| Unit — Sigurnosno | US-29 | Zahtjev bez JWT dobija 401 | `AllTicketsControllerTests.GetAllTickets_ReturnsUnauthorized_WhenNoUserClaim` | PASS |
| UI | US-29 | UI prikazuje listu tiketa | `Tickets.test.jsx > renders ticket list when tickets are loaded` | PASS |
| UI | US-29 | UI prikazuje OPEN kao "Otvoren" | `Tickets.test.jsx > displays OPEN status as "Otvoren"` | PASS |
| UI | US-29 | UI prikazuje CLOSED kao "Zatvoren" | `Tickets.test.jsx > displays CLOSED status as "Zatvoren"` | PASS |
| UI | US-29, US-30 | UI prikazuje toggle Svi tiketi/Dodijeljeni meni | `Tickets.test.jsx > shows assigned-only toggle for agent role` | PASS |
| UI | US-29 | UI prikazuje prazno stanje | `Tickets.test.jsx > shows empty state when there are no tickets` | PASS |
| UI | US-29 | UI prikazuje grešku pri API pozivu | `Tickets.test.jsx > shows error message when API call fails` | PASS |
| Sistemsko | US-29 | Sistemski tok: agent vidi tikete i moze pretraziti | `TicketsSystem.test.jsx > agent otvara listu svih tiketa...` | PASS |
| Prihvatno | US-29, US-30 | Prihvatni tok: agent vidi sve kategorije, vidi toggle | `TicketsAcceptance.test.jsx > agent vidi sve tikete u sistemu...` | PASS |
| Integracijsko | US-29 | Agent vidi sve tikete od razlicitih klijenata (3) — integracijski potvrđeno | `AllTicketsIntegrationTests.GetAllTickets_AgentRole_ReturnsAllTickets` | PASS |
| Integracijsko — Sigurnosno | US-29 | CLIENT ne moze pristupiti listi svih tiketa → 403 Forbid | `AllTicketsIntegrationTests.GetAllTickets_ClientRole_ReturnsForbid` | PASS |
| Integracijsko | US-29, US-30 | Agent sa assignedOnly=true dobija samo dodijeljeni tiket (1 od 2) | `AllTicketsIntegrationTests.GetAllTickets_AgentWithAssignedOnlyTrue_ReturnsOnlyAssignedTickets` | PASS |
| Integracijsko | US-29 | Tiketi sortirani od najnovijeg prema najstarijem | `AllTicketsIntegrationTests.GetAllTickets_ReturnsTicketsOrderedByDateDescending` | PASS |
| Performansno | US-29, US-30 | Lista od 500 tiketa ucitava se u manje od 2 sekunde | `AllTicketsPerformanceTests.GetAllTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/AllTicketsRepositoryTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AllTicketsRepositoryTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/TicketT/AllTicketsServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AllTicketsServiceTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/TicketT/AllTicketsControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AllTicketsControllerTests.cs) — 3 testa
- [TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs) — 4 testa (pokriva PB-32 i PB-33)
- [TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs) — 1 test (pokriva PB-32 i PB-33)
- [frontend/src/test/Tickets.test.jsx](../Project/frontend/src/test/Tickets.test.jsx) — 9 testova
- [frontend/src/test/system/TicketsSystem.test.jsx](../Project/frontend/src/test/system/TicketsSystem.test.jsx) — 1 test
- [frontend/src/test/acceptance/TicketsAcceptance.test.jsx](../Project/frontend/src/test/acceptance/TicketsAcceptance.test.jsx) — 1 test

---

## PB-33 — Pretraga i filtriranje tiketa

### Pokriveni AC (US-31, US-32)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| UI | US-32 | Filter po statusu prikazuje samo odgovarajuce tikete | `Tickets.test.jsx > filters tickets by status` | PASS |
| UI | US-32 | Filter po tipu problema prikazuje samo odgovarajuce tikete | `Tickets.test.jsx > filters tickets by problem type` | PASS |
| UI | US-32 | Filter po prioritetu prikazuje samo odgovarajuce tikete | `Tickets.test.jsx > filters tickets by priority` | PASS |
| UI | US-31 | Pretraga po naslovu filtrira tikete | `Tickets.test.jsx > filters tickets by search term` | PASS |
| Sistemsko | US-31 | Pretraga u sistemskom toku filtrira ispravno | `TicketsSystem.test.jsx > agent otvara listu svih tiketa, vidi tikete od razlicitih klijenata i moze ih pretraziti` | PASS |
| Integracijsko | US-31, US-32 | Tiketi sortirani od najnovijeg prema najstarijem | `AllTicketsIntegrationTests.GetAllTickets_ReturnsTicketsOrderedByDateDescending` | PASS |
| Performansno | US-31, US-32 | Lista od 500 tiketa (s filtriranjem i sortiranjem) < 2 sekunde | `AllTicketsPerformanceTests.GetAllTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` | PASS |
| Prihvatno | US-31, US-32 | Agent suzuje listu kombinacijom pretrage i filtera po statusu; reset filtera vraca sve tikete | `SearchFilterAcceptance.test.jsx > agent moze suziti listu tiketa...` | PASS |

### Fajlovi sa testovima

- [frontend/src/test/Tickets.test.jsx](../Project/frontend/src/test/Tickets.test.jsx) — filteri pokriveni unutar 9 testova za PB-32/PB-33
- [frontend/src/test/system/TicketsSystem.test.jsx](../Project/frontend/src/test/system/TicketsSystem.test.jsx) — pretraga pokrivena u sistemskom toku
- [TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs) — sortiranje i filter pokriveni (dijeljeno s PB-32)
- [TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs) — 1 test (dijeljeno s PB-32)
- [frontend/src/test/acceptance/SearchFilterAcceptance.test.jsx](../Project/frontend/src/test/acceptance/SearchFilterAcceptance.test.jsx) — 1 test

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PB | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — backend servis | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32 | `AuthServiceTests`, `TicketServiceTests`, `TicketDetailServiceTests`, `CommentServiceTests`, `AllTicketsServiceTests` | PASS |
| Unit — backend controller | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32 | `AuthControllerTests`, `TicketControllerTests`, `TicketDetailControllerTests`, `CommentControllerTests`, `AllTicketsControllerTests` | PASS |
| Unit — repository (EF InMemory) | US-8, US-9, US-10, US-11, US-12, US-13, US-29, US-30 | PB-22, PB-23, PB-32 | `TicketRepositoryTests`, `AllTicketsRepositoryTests` | PASS |
| Integracijsko — backend | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthIntegrationTests`, `TicketIntegrationTests`, `TicketDetailIntegrationTests`, `CommentIntegrationTests`, `AllTicketsIntegrationTests` | PASS |
| Performansno — backend | US-1, US-2, US-8, US-11, US-14, US-19, US-29, US-30, US-31, US-32 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthPerformanceTests`, `TicketPerformanceTests`, `TicketDetailPerformanceTests`, `CommentPerformanceTests`, `AllTicketsPerformanceTests` | PASS |
| UI | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `Login.test.jsx`, `CreateTicket.test.jsx`, `MyTickets.test.jsx`, `TicketDetail.test.jsx`, `Tickets.test.jsx` | PASS |
| Sistemsko | US-1, US-2, US-3, US-8, US-11, US-12, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthSystem.test.jsx`, `TicketCreateSystem.test.jsx`, `MyTicketsSystem.test.jsx`, `TicketDetailSystem.test.jsx`, `CommunicationSystem.test.jsx`, `TicketsSystem.test.jsx` | PASS |
| Prihvatno | US-1, US-2, US-3, US-8, US-9, US-10, US-11, US-12, US-13, US-14, US-15, US-19, US-20, US-29, US-30, US-31, US-32 | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthAcceptance.test.jsx`, `CreateTicketAcceptance.test.jsx`, `MyTicketsAcceptance.test.jsx`, `TicketDetailAcceptance.test.jsx`, `CommunicationAcceptance.test.jsx`, `TicketsAcceptance.test.jsx`, `SearchFilterAcceptance.test.jsx` | PASS |
| Sigurnosno (rola/vlasnistvo) | US-1, US-2, US-3, US-14, US-15, US-19, US-20, US-29, US-30 | PB-19, PB-24, PB-27, PB-32 | `AuthIntegrationTests`, `TicketDetailServiceTests`, `CommentServiceTests`, `AllTicketsServiceTests`, `AllTicketsControllerTests` | PASS |


## Lokalno pokretanje testova:

Iz root direktorija: 

### Frontend:
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --logger "console;verbosity=normal" 2>&1

### Backend:
cd Project/frontend && npx vitest run 2>&1