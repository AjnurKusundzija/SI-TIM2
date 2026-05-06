# Proof of Testing — Sprint 6
---

## Ukupni rezultati

| Nivo | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- |
| Unit — Backend | xUnit + Moq + EF InMemory | 28 novih testova (Sprint 6) | PASS |
| Unit — Frontend | Vitest + Testing Library | 17 novih testova (Sprint 6) | PASS |
| Integracijsko — Backend | xUnit + EF InMemory | 22 nova testa (Sprint 6) | PASS |
| Performansno — Backend | xUnit + Stopwatch | 6 novih testova (Sprint 6) | PASS |
| Sistemsko — Frontend | Vitest + Testing Library | 8 testova (Sprint 6) | PASS |
| Prihvatno — Frontend | Vitest + Testing Library | 9 testova (Sprint 6) | PASS |
| **Ukupno Sprint 6** | | **90 novih testova** | **PASS** |
| **Ukupno projekat** | | **119 backend + 96 frontend = 215** | **PASS** |

---

## PB-19 — Prijava i upravljanje sesijama

### Pokriveni AC (US-1, US-2, US-3)

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| Unit | Ispravna validacija emaila i lozinke | `AuthServiceTests` — provjera toka prijave |
| Unit | Generička poruka greške bez otkrivanja detalja | `AuthControllerTests` — 401 odgovor |
| UI | Forma prijave, poruka greške, stanje sesije | `Login.test.jsx` — 4 testa |
| Integracijsko | Login s ispravnim kredencijalima vraća JWT kroz sve slojeve | `AuthIntegrationTests.Login_WithValidCredentials_ReturnsJwtTokenThroughFullStack` |
| Integracijsko | Pogrešna lozinka vraća 401 generičkim odgovorom | `AuthIntegrationTests.Login_WithWrongPassword_ReturnsUnauthorizedWithGenericMessage` |
| Integracijsko | Nepostojeci email vraća 401 (isti odgovor kao pogrešna lozinka) | `AuthIntegrationTests.Login_WithUnknownEmail_ReturnsUnauthorized` |
| Integracijsko | Odjava revokuje refresh token u bazi | `AuthIntegrationTests.Logout_AfterSuccessfulLogin_RevokesRefreshTokenInDatabase` |
| Integracijsko | Refresh token daje novi access token | `AuthIntegrationTests.Refresh_WithValidToken_ReturnsNewAccessToken` |
| Performansno | Tok prijave (Controller → Service + BCrypt → Repository) < 2 sekunde | `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` |
| Sistemsko | Login pozvan sa ispravnim podacima; nema console.error | `AuthSystem.test.jsx > korisnik unosi kredencijale...` |
| Sistemsko | Greška prikazana bez otkrivanja koji podatak je pogrešan | `AuthSystem.test.jsx > pogresan unos prikazuje poruku greske...` |
| Prihvatno | Klijent se uspješno prijavljuje i sistem ga preusmjerava | `AuthAcceptance.test.jsx > klijent se uspjesno prijavljuje...` |
| Prihvatno | Poruka greške ne otkriva koji podatak je pogrešan — generički tekst vidljiv | `AuthAcceptance.test.jsx > poruka greske ne otkriva koji podatak...` |

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

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| Unit | Odbijanje nevažećeg modela (400); novi tiket uvijek OPEN | `TicketControllerTests`, `TicketServiceTests` |
| Unit | Svi prioriteti i tipovi problema se ispravno čuvaju i mapiraju | `TicketRepositoryTests` |
| UI | Validacija forme, poruka potvrde, reset forme, greška pri 401 | `CreateTicket.test.jsx` — 11 testova |
| Integracijsko | Kreiran tiket persistira u bazi i vraća 201 CreatedAtAction | `TicketIntegrationTests.CreateTicket_PersistsToDatabase_AndReturnsCreated` |
| Integracijsko | Novi tiket uvijek dobija status OPEN kroz sve slojeve | `TicketIntegrationTests.CreateTicket_NewTicketAlwaysHasStatusOpen` |
| Performansno | Kreiranje tiketa kroz sve slojeve < 3 sekunde (NFR-04) | `TicketPerformanceTests.CreateTicket_ShouldCompleteWithinThreeSeconds_InTestEnvironment` |
| Sistemsko | Korisnik popunjava formu i uspješno kreira tiket — forma se resetuje | `TicketCreateSystem.test.jsx > korisnik popunjava formu...` |
| Sistemsko | Forma odbija submit bez naslova — createTicket se ne poziva | `TicketCreateSystem.test.jsx > forma odbija submit bez naslova...` |
| Prihvatno | Forma ima sva potrebna polja; createTicket pozvan s ispravnim PascalCase podacima | `CreateTicketAcceptance.test.jsx > klijent popunjava formu...` |

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

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| Unit | Korisnik vidi samo vlastite tikete; OPEN/CLOSED mapiranje; prazna lista | `TicketServiceTests`, `TicketRepositoryTests` |
| UI | Lista tiketa, statusi, filteri, pretraga, prazno stanje, greška | `MyTickets.test.jsx` — 11 testova |
| Integracijsko | Korisnik vidi samo vlastite tikete — tiketi drugog korisnika se ne vraćaju | `TicketIntegrationTests.GetMyTickets_ReturnsOnlyOwnTickets_NotOtherUsersTickets` |
| Integracijsko | OPEN i CLOSED statusi se ispravno mapiraju u string | `TicketIntegrationTests.GetMyTickets_MapsStatusCorrectlyToString` |
| Integracijsko | Prazna lista kada korisnik nema tiketa | `TicketIntegrationTests.GetMyTickets_ReturnsEmptyList_WhenUserHasNoTickets` |
| Performansno | Lista od 200 tiketa ucitava se u manje od 2 sekunde | `TicketPerformanceTests.GetMyTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` |
| Sistemsko | Korisnik vidi listu vlastitih tiketa bez console.error greske | `MyTicketsSystem.test.jsx > korisnik vidi listu...` |
| Prihvatno | Klijent vidi vlastite tikete; pretraga i filter rade ispravno | `MyTicketsAcceptance.test.jsx > klijent vidi vlastite tikete...` |

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

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| Unit | Klijent moze vidjeti vlastiti tiket | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenClientIsOwner` |
| Unit | Tiket koji ne postoji baca KeyNotFoundException | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowKeyNotFound_WhenTicketMissing` |
| Unit — Sigurnosno | Klijent ne moze vidjeti tudji tiket | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherTicket` |
| Unit — Sigurnosno | Agent moze vidjeti svaki tiket | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenAgentAccessesAnyTicket` |
| Unit | Sva polja tiketa se ispravno mapiraju u DTO | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldMapAllFieldsToDto` |
| Unit | Controller vraca 200 OK za vlasnika | `TicketDetailControllerTests.GetTicketById_ReturnsOk_WhenTicketFound` |
| Unit | Controller vraca 404 za nepostojeci tiket | `TicketDetailControllerTests.GetTicketById_ReturnsNotFound_WhenTicketMissing` |
| Unit — Sigurnosno | Controller vraca 403 za neovlasteni pristup | `TicketDetailControllerTests.GetTicketById_ReturnsForbid_WhenClientAccessesForeignTicket` |
| Unit — Sigurnosno | Controller vraca 401 bez JWT claimova | `TicketDetailControllerTests.GetTicketById_ReturnsUnauthorized_WhenNoUserClaim` |
| UI | UI prikazuje detalje tiketa | `TicketDetail.test.jsx > renders ticket details after successful load` |
| UI | UI prikazuje ime klijenta i agenta | `TicketDetail.test.jsx > shows client name and assigned agent name` |
| UI | UI prikazuje historiju komentara | `TicketDetail.test.jsx > displays comment history when comments exist` |
| UI | UI prikazuje error state pri API gresci | `TicketDetail.test.jsx > shows error empty state when API call fails` |
| Integracijsko | CLIENT owner → 200 sa svim ispravno mapiranim poljima (Title, Description, Status, ClientName) | `TicketDetailIntegrationTests.GetTicketById_ClientOwner_ReturnsDetailDtoWithAllFields` |
| Integracijsko | AGENT vidi svaki tiket bez obzira na vlasnistvo | `TicketDetailIntegrationTests.GetTicketById_AgentRole_CanAccessAnyTicket` |
| Integracijsko — Sigurnosno | CLIENT na tudem tiketu → 403 Forbid | `TicketDetailIntegrationTests.GetTicketById_ClientAccessingOtherTicket_ReturnsForbid` |
| Integracijsko | Nepostojeci tiket → 404 NotFound | `TicketDetailIntegrationTests.GetTicketById_TicketDoesNotExist_ReturnsNotFound` |
| Performansno | Prikaz detalja tiketa (s Creator i Assignments include) < 2 sekunde | `TicketDetailPerformanceTests.GetTicketById_ShouldLoadWithinTimeLimit_InTestEnvironment` |
| Sistemsko | Korisnik otvara tiket i vidi naslov, opis i historiju komentara bez console.error | `TicketDetailSystem.test.jsx > korisnik otvara tiket i vidi sve detalje...` |
| Prihvatno | Klijent vidi naslov, opis, ime agenta i historiju komunikacije bez poruke o gresci | `TicketDetailAcceptance.test.jsx > klijent vidi kompletan prikaz tiketa...` |

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

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| Unit | Uspjesno slanje komentara | `CommentServiceTests.AddCommentAsync_ShouldSucceed_WhenValidContentAndOwner` |
| Unit | Odbijanje sadrzaja duzeg od 1000 znakova | `CommentServiceTests.AddCommentAsync_ShouldThrowArgumentException_WhenContentTooLong` |
| Unit | KeyNotFound za nepostojeci tiket | `CommentServiceTests.AddCommentAsync_ShouldThrowKeyNotFound_WhenTicketMissing` |
| Unit — Sigurnosno | UnauthorizedAccess za klijenta koji nije vlasnik | `CommentServiceTests.AddCommentAsync_ShouldThrowUnauthorized_WhenClientNotOwner` |
| Unit | GetComments vraca listu za vlasnika tiketa | `CommentServiceTests.GetCommentsForTicketAsync_ShouldReturnList_WhenCallerIsOwner` |
| Unit — Sigurnosno | GetComments baca UnauthorizedAccess za tredju stranu | `CommentServiceTests.GetCommentsForTicketAsync_ShouldThrowUnauthorized_WhenCallerIsNotOwner` |
| Unit | Controller vraca 200 OK sa komentarima | `CommentControllerTests.GetComments_ReturnsOk_WhenAuthorized` |
| Unit — Sigurnosno | Controller vraca 401 bez JWT claimova (GET) | `CommentControllerTests.GetComments_ReturnsUnauthorized_WhenNoUserClaim` |
| Unit | Controller vraca 400 za prazan sadrzaj | `CommentControllerTests.AddComment_ReturnsBadRequest_WhenContentEmpty` |
| Unit — Sigurnosno | Controller vraca 401 bez JWT claimova (POST) | `CommentControllerTests.AddComment_ReturnsUnauthorized_WhenNoUserClaim` |
| UI | UI input vidljiv za otvoreni tiket | `TicketDetail.test.jsx > shows message input for open ticket` |
| UI | UI input skriven za zatvoreni tiket | `TicketDetail.test.jsx > hides message input for closed ticket` |
| UI | Dugme Pošalji onemoguceno za prazan unos | `TicketDetail.test.jsx > disables send button when message is empty` |
| UI | Slanje poruke poziva addComment sa ispravnim parametrima | `TicketDetail.test.jsx > submits a message by calling addComment` |
| Integracijsko | Vlasnik dohvata komentare → lista s autorom (AuthorName = "Merjem Omerovic") | `CommentIntegrationTests.GetComments_OwnerOfTicket_ReturnsCommentList` |
| Integracijsko | Slanje komentara persistira u bazi i vraca DTO s autorom | `CommentIntegrationTests.AddComment_ValidContent_PersistsAndReturnsDto` |
| Integracijsko | Sadrzaj >1000 znakova → 400 BadRequest | `CommentIntegrationTests.AddComment_ContentTooLong_ReturnsBadRequest` |
| Integracijsko — Sigurnosno | Klijent na tudem tiketu → 403 Forbid | `CommentIntegrationTests.GetComments_ClientAccessingOtherTicket_ReturnsForbid` |
| Performansno | Lista od 100 komentara ucitava se u manje od 2 sekunde | `CommentPerformanceTests.GetComments_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` |
| Sistemsko | Korisnik unosi poruku i šalje je — addComment pozvan s ispravnim ticketId i sadrzajem, nema console.error | `CommunicationSystem.test.jsx > korisnik otvara tiket, unosi poruku i šalje je...` |
| Prihvatno | Klijent šalje poruku na otvorenom tiketu (addComment pozvan s ispravnim podacima) | `CommunicationAcceptance.test.jsx > klijent moze poslati poruku agentu...` |
| Prihvatno | Input za poruku je skriven kada je tiket zatvoren — komunikacija nije moguca | `CommunicationAcceptance.test.jsx > input za poruku je skriven kada je tiket zatvoren...` |

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

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| Unit — Repozitorij | GetAllAsync vraca sve tikete | `AllTicketsRepositoryTests.GetAllAsync_ReturnsAllTickets` |
| Unit — Repozitorij | Prazna lista kada nema tiketa | `AllTicketsRepositoryTests.GetAllAsync_ReturnsEmpty_WhenNoTickets` |
| Unit — Repozitorij | Tiketi sortirani od najnovijeg | `AllTicketsRepositoryTests.GetAllAsync_ReturnsTickets_OrderedByDateDescending` |
| Unit — Repozitorij | GetByAssigneeIdAsync filtrira po dodijeljenosti | `AllTicketsRepositoryTests.GetByAssigneeIdAsync_ReturnsOnlyAssignedTickets` |
| Unit | Agent bez filtera dobija sve tikete | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldCallGetAllAsync_WhenAgentAndNotAssignedOnly` |
| Unit | Agent sa assignedOnly dobija dodijeljene | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldCallGetByAssigneeIdAsync_WhenAgentAndAssignedOnly` |
| Unit | Tehnicar uvijek dobija samo dodijeljene | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldCallGetByAssigneeIdAsync_WhenTechnicianRole` |
| Unit — Sigurnosno | Klijent dobija UnauthorizedAccessException | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldThrowUnauthorized_WhenClientRole` |
| Unit | Agent dobija 200 OK sa listom | `AllTicketsControllerTests.GetAllTickets_ReturnsOk_WhenAgentRole` |
| Unit — Sigurnosno | Klijent dobija 403 Forbid | `AllTicketsControllerTests.GetAllTickets_ReturnsForbid_WhenClientRole` |
| Unit — Sigurnosno | Zahtjev bez JWT dobija 401 | `AllTicketsControllerTests.GetAllTickets_ReturnsUnauthorized_WhenNoUserClaim` |
| UI | UI prikazuje listu tiketa | `Tickets.test.jsx > renders ticket list when tickets are loaded` |
| UI | UI prikazuje OPEN kao "Otvoren" | `Tickets.test.jsx > displays OPEN status as "Otvoren"` |
| UI | UI prikazuje CLOSED kao "Zatvoren" | `Tickets.test.jsx > displays CLOSED status as "Zatvoren"` |
| UI | UI prikazuje toggle Svi tiketi/Dodijeljeni meni | `Tickets.test.jsx > shows assigned-only toggle for agent role` |
| UI | UI prikazuje prazno stanje | `Tickets.test.jsx > shows empty state when there are no tickets` |
| UI | UI prikazuje grešku pri API pozivu | `Tickets.test.jsx > shows error message when API call fails` |
| Sistemsko | Sistemski tok: agent vidi tikete i moze pretraziti | `TicketsSystem.test.jsx > agent otvara listu svih tiketa...` |
| Prihvatno | Prihvatni tok: agent vidi sve kategorije, vidi toggle | `TicketsAcceptance.test.jsx > agent vidi sve tikete u sistemu...` |
| Integracijsko | Agent vidi sve tikete od razlicitih klijenata (3) — integracijski potvrđeno | `AllTicketsIntegrationTests.GetAllTickets_AgentRole_ReturnsAllTickets` |
| Integracijsko — Sigurnosno | CLIENT ne moze pristupiti listi svih tiketa → 403 Forbid | `AllTicketsIntegrationTests.GetAllTickets_ClientRole_ReturnsForbid` |
| Integracijsko | Agent sa assignedOnly=true dobija samo dodijeljeni tiket (1 od 2) | `AllTicketsIntegrationTests.GetAllTickets_AgentWithAssignedOnlyTrue_ReturnsOnlyAssignedTickets` |
| Integracijsko | Tiketi sortirani od najnovijeg prema najstarijem | `AllTicketsIntegrationTests.GetAllTickets_ReturnsTicketsOrderedByDateDescending` |
| Performansno | Lista od 500 tiketa ucitava se u manje od 2 sekunde | `AllTicketsPerformanceTests.GetAllTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` |

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

| Nivo | AC | Test koji pokriva |
| --- | --- | --- |
| UI | Filter po statusu prikazuje samo odgovarajuce tikete | `Tickets.test.jsx > filters tickets by status` |
| UI | Filter po tipu problema prikazuje samo odgovarajuce tikete | `Tickets.test.jsx > filters tickets by problem type` |
| UI | Filter po prioritetu prikazuje samo odgovarajuce tikete | `Tickets.test.jsx > filters tickets by priority` |
| UI | Pretraga po naslovu filtrira tikete | `Tickets.test.jsx > filters tickets by search term` |
| Sistemsko | Pretraga u sistemskom toku filtrira ispravno | `TicketsSystem.test.jsx > agent otvara listu svih tiketa, vidi tikete od razlicitih klijenata i moze ih pretraziti` |

| Integracijsko | Tiketi sortirani od najnovijeg prema najstarijem | `AllTicketsIntegrationTests.GetAllTickets_ReturnsTicketsOrderedByDateDescending` |
| Performansno | Lista od 500 tiketa (s filtriranjem i sortiranjem) < 2 sekunde | `AllTicketsPerformanceTests.GetAllTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` |
| Prihvatno | Agent suzuje listu kombinacijom pretrage i filtera po statusu; reset filtera vraca sve tikete | `SearchFilterAcceptance.test.jsx > agent moze suziti listu tiketa...` |

### Fajlovi sa testovima

- [frontend/src/test/Tickets.test.jsx](../Project/frontend/src/test/Tickets.test.jsx) — filteri pokriveni unutar 9 testova za PB-32/PB-33
- [frontend/src/test/system/TicketsSystem.test.jsx](../Project/frontend/src/test/system/TicketsSystem.test.jsx) — pretraga pokrivena u sistemskom toku
- [TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs) — sortiranje i filter pokriveni (dijeljeno s PB-32)
- [TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs) — 1 test (dijeljeno s PB-32)
- [frontend/src/test/acceptance/SearchFilterAcceptance.test.jsx](../Project/frontend/src/test/acceptance/SearchFilterAcceptance.test.jsx) — 1 test

---

## Veza sa Test Strategijom

| Test strategija nivo | PB | Dokaz |
| --- | --- | --- |
| Unit — backend servis | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32 | `AuthServiceTests`, `TicketServiceTests`, `TicketDetailServiceTests`, `CommentServiceTests`, `AllTicketsServiceTests` |
| Unit — backend controller | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32 | `AuthControllerTests`, `TicketControllerTests`, `TicketDetailControllerTests`, `CommentControllerTests`, `AllTicketsControllerTests` |
| Unit — repository (EF InMemory) | PB-22, PB-23, PB-32 | `TicketRepositoryTests`, `AllTicketsRepositoryTests` |
| Integracijsko — backend | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthIntegrationTests`, `TicketIntegrationTests`, `TicketDetailIntegrationTests`, `CommentIntegrationTests`, `AllTicketsIntegrationTests` |
| Performansno — backend | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthPerformanceTests`, `TicketPerformanceTests`, `TicketDetailPerformanceTests`, `CommentPerformanceTests`, `AllTicketsPerformanceTests` |
| UI | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `Login.test.jsx`, `CreateTicket.test.jsx`, `MyTickets.test.jsx`, `TicketDetail.test.jsx`, `Tickets.test.jsx` |
| Sistemsko | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthSystem.test.jsx`, `TicketCreateSystem.test.jsx`, `MyTicketsSystem.test.jsx`, `TicketDetailSystem.test.jsx`, `CommunicationSystem.test.jsx`, `TicketsSystem.test.jsx` |
| Prihvatno | PB-19, PB-22, PB-23, PB-24, PB-27, PB-32, PB-33 | `AuthAcceptance.test.jsx`, `CreateTicketAcceptance.test.jsx`, `MyTicketsAcceptance.test.jsx`, `TicketDetailAcceptance.test.jsx`, `CommunicationAcceptance.test.jsx`, `TicketsAcceptance.test.jsx`, `SearchFilterAcceptance.test.jsx` |
| Sigurnosno (rola/vlasnistvo) | PB-19, PB-24, PB-27, PB-32 | `AuthIntegrationTests`, `TicketDetailServiceTests`, `CommentServiceTests`, `AllTicketsServiceTests`, `AllTicketsControllerTests` |
