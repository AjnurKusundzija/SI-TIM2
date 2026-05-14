# Proof of Testing — Sprint 7
---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend (service) | US-25 | xUnit + Moq | 12 novih testova (7 metoda, 12 slučajeva uključujući Theory) | PASS |
| Unit — Backend (repository, EF InMemory) | US-25 | xUnit + EF InMemory | 7 novih testova | PASS |
| Integracijsko — Backend | US-25 | xUnit + EF InMemory | 5 novih testova | PASS |
| Performansno — Backend | US-25 (NFR-04) | xUnit + Stopwatch | 1 novi test | PASS |
| Unit — Backend (controller) | US-16, US-17 | xUnit + Moq | 11 novih testova | PASS |
| Unit — Backend (controller) | US-21 | xUnit + Moq | 2 nova testa | PASS |
| Unit — Backend (service) | US-14, US-30, US-39 | xUnit + Moq | 5 novih testova | PASS |
| Unit — Backend (controller) | US-14, US-30, US-39 | xUnit + Moq | 4 nova testa | PASS |
| Integracijsko — Backend | US-14, US-30, US-39 | xUnit + EF InMemory | 4 nova testa | PASS |
| Performansno — Backend | US-14 (NFR) | xUnit + Stopwatch | 1 novi test | PASS |
| Unit — Backend (service) | US-55, US-56 | xUnit + Moq | 8 novih testova (6 metoda, 8 slučajeva uključujući Theory) — branch coverage `GetWeightsByPriority` | PASS |
| Unit — Backend (controller) | US-55, US-56 | xUnit + Moq | 13 novih testova | PASS |
| Unit — Backend (service) | US-53, US-54 | xUnit + Moq | 4 nova testa | PASS |
| Unit — Backend (controller) | US-53, US-54 | xUnit + Moq | 3 nova testa | PASS |
| Unit — Backend (repository, EF InMemory) | US-53, US-54 | xUnit + EF InMemory | 4 nova testa | PASS |
| Integracijsko — Backend | US-53, US-54 | xUnit + EF InMemory | 4 nova testa | PASS |
| Performansno — Backend | US-53, US-54 (NFR) | xUnit + Stopwatch | 1 novi test | PASS |
| Sistemski — Backend (end-to-end kroz sve slojeve) | US-16, US-17, US-21, US-25, US-39, US-55, US-53, US-54 | xUnit + EF InMemory | 7 novih testova | PASS |
| **Ukupno Sprint 7** | **US-16, US-17, US-21, US-25, US-39, US-53, US-54, US-55, US-56** | | **96 novih backend testova** | **PASS** |
| **Ukupno projekat** | **US-1 do US-3, US-8 do US-17, US-18 do US-21, US-25, US-29 do US-32, US-39, US-53 do US-56** | | **216 backend + 154 frontend = 370** | **PASS** |

---

## PB-25 — Zatvaranje tiketa

### Pokriveni AC (US-16, US-17)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Controller | US-17 | Agent šalje zahtjev za zatvaranje tiketa — controller vraća 200 OK | `TicketControllerClosureTests.RequestClosure_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-17 | Zahtjev za zatvaranje nepostojećeg tiketa — 404 NotFound | `TicketControllerClosureTests.RequestClosure_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller | US-17 | Neovlašteni korisnik šalje zahtjev za zatvaranje — 403 Forbid | `TicketControllerClosureTests.RequestClosure_ShouldReturnForbid_WhenUserUnauthorized` | PASS |
| Unit — Controller | US-17 | Zahtjev za zatvaranje tiketa koji nije OPEN — 400 BadRequest | `TicketControllerClosureTests.RequestClosure_ShouldReturnBadRequest_WhenInvalidOperation` | PASS |
| Unit — Controller | US-17 | Klijent prihvata zahtjev za zatvaranje — controller vraća 200 OK i poziva `AcceptClosureAsync` | `TicketControllerClosureTests.AcceptClosure_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-17 | Prihvatanje zatvaranja nepostojećeg tiketa — 404 NotFound | `TicketControllerClosureTests.AcceptClosure_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller | US-17 | Klijent odbija zahtjev za zatvaranje — controller vraća 200 OK i poziva `RejectClosureAsync` | `TicketControllerClosureTests.RejectClosure_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-17 | Neovlašteni korisnik odbija zahtjev za zatvaranje — 403 Forbid | `TicketControllerClosureTests.RejectClosure_ShouldReturnForbid_WhenUserUnauthorized` | PASS |
| Unit — Controller | US-17 | Agent prisilno zatvara tiket — controller vraća 200 OK i poziva `ForceCloseAsync` | `TicketControllerClosureTests.ForceClose_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-17 | Prisilno zatvaranje nepostojećeg tiketa — 404 NotFound | `TicketControllerClosureTests.ForceClose_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller | US-16 | Klijent ili agent zatvara tiket — controller vraća 200 OK i poziva `CloseTicketAsync` | `TicketControllerClosureTests.CloseTicket_ShouldReturnOk_WhenRequestIsValid` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketControllerClosureTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketControllerClosureTests.cs) — 11 testova zatvaranja + 2 testa internog prioriteta (PB-28)

---

## PB-28 — Upravljanje prioritetima tiketa

### Pokriveni AC (US-21)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Controller | US-21 | Agent postavlja interni prioritet — controller vraća 200 OK i poziva `UpdateInternalPriorityAsync` | `TicketControllerClosureTests.UpdateInternalPriority_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-21 | Postavljanje internog prioriteta za nepostojeći tiket — 404 NotFound | `TicketControllerClosureTests.UpdateInternalPriority_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketControllerClosureTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketControllerClosureTests.cs) — testovi na linijama 246–282 (dijele fajl s PB-25 testovima)

---

## PB-30 — Automatska dodjela tiketa

### Pokriveni AC (US-25)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-25 | AC1 — kada postoji tim za kategoriju i dostupan agent, sistem auto-dodjeljuje tiket | `AutoAssignServiceTests.CreateTicketAsync_AutoAssignsTicket_WhenTeamAndAvailableAgentExist` | PASS |
| Unit — Service | US-25 | AC5 — bira agenta s najmanjim brojem trenutno dodijeljenih tiketa | `AutoAssignServiceTests.CreateTicketAsync_PicksAgentWithFewestAssignments` | PASS |
| Unit — Service | US-25 | AC5 — tiebreaker: kod jednakog broja tiketa bira agenta s nižim prosječnim prioritetom | `AutoAssignServiceTests.CreateTicketAsync_BreaksTieByLowerMeanPriority` | PASS |
| Unit — Service | US-25 | AC1 — dodjela koristi `AssignmentType.AUTOMATIC` i sadrži objašnjenje u Note polju | `AutoAssignServiceTests.CreateTicketAsync_RecordsAssignmentAsAutomaticWithNote` | PASS |
| Unit — Service | US-25 | AC2, AC4 — kada nema dostupnih agenata, ne dolazi do dodjele i vraća se poruka „Nema dostupnih agenata. Tiket je označen kao Nedodijeljen.“ | `AutoAssignServiceTests.CreateTicketAsync_DoesNotAssign_WhenNoAgentsAreAvailable` | PASS |
| Unit — Service | US-25 | AC6 — kada ne postoji tim za kategoriju (nema pravila), vraća se poruka „Nema definisanih pravila dodjele za odabranu kategoriju.“ | `AutoAssignServiceTests.CreateTicketAsync_ReturnsNoRulesMessage_WhenNoTeamMatchesCategory` | PASS |
| Unit — Service | US-25 | Tiket persistira u bazi prije pokušaja dodjele (Create dolazi prije AddAssignment) | `AutoAssignServiceTests.CreateTicketAsync_PersistsTicketBeforeAttemptingAssignment` | PASS |
| Unit — Service (Theory, 5 slučajeva) | US-25 | AC5 — mapiranje svih kategorija (`INTERNET`, `TV`, `MOBILE_NETWORK`, `BILLING`, `TECHNICAL_SUPPORT`) na pripadajući tim | `AutoAssignServiceTests.CreateTicketAsync_AssignsCorrectTeam_BasedOnCategory` | PASS |
| Unit — Repository | US-25 | AC5 — `GetBySpecializedCategoryAsync` vraća tim za zadanu kategoriju | `AutoAssignRepositoryTests.GetBySpecializedCategoryAsync_ReturnsMatchingTeam` | PASS |
| Unit — Repository | US-25 | AC6 — `GetBySpecializedCategoryAsync` vraća `null` kada nema tima za kategoriju (signal „nema pravila“) | `AutoAssignRepositoryTests.GetBySpecializedCategoryAsync_ReturnsNull_WhenNoTeamHasCategory` | PASS |
| Unit — Repository | US-25 | AC2 — vraća samo agente s `AvailabilityStatus.AVAILABLE` iz traženog tima | `AutoAssignRepositoryTests.GetAvailableAgentsByTeamIdAsync_ReturnsOnlyAvailableAgents_InTeam` | PASS |
| Unit — Repository — Sigurnosno | US-25 | AC2 — tehničar/administrator/klijent se ne mogu vratiti kao kandidati za dodjelu (samo `Role.AGENT`) | `AutoAssignRepositoryTests.GetAvailableAgentsByTeamIdAsync_DoesNotReturnNonAgents` | PASS |
| Unit — Repository | US-25 | AC5 — učitava `TicketAssignments` + `Ticket` (`Include`/`ThenInclude`) kako bi servis mogao sortirati po opterećenju | `AutoAssignRepositoryTests.GetAvailableAgentsByTeamIdAsync_IncludesAssignmentsAndTickets_ForLoadSorting` | PASS |
| Unit — Repository | US-25 | AC1 — `AddAssignmentAsync` perzistira novi `TicketUser` zapis u bazi | `AutoAssignRepositoryTests.AddAssignmentAsync_PersistsAssignment` | PASS |
| Unit — Repository | US-25 | AC3 — nakon `AddAssignmentAsync`, `GetByAssigneeIdAsync` vraća tiket za tog agenta | `AutoAssignRepositoryTests.AddAssignmentAsync_MakesTicketVisibleToAssignedAgent` | PASS |
| Integracijsko | US-25 | AC1, AC3 — end-to-end: klijent kreira tiket → sistem auto-dodjeljuje → agent vidi tiket u svojoj listi (`?assignedOnly=true`) | `AutoAssignIntegrationTests.CreateTicket_AutoAssignsAndAgentSeesTicket_EndToEnd` | PASS |
| Integracijsko | US-25 | AC2 — `BUSY` i `UNAVAILABLE` agenti se preskaču, sistem izabere prvog `AVAILABLE` | `AutoAssignIntegrationTests.CreateTicket_SkipsUnavailableAgent_AndPicksAvailableOne` | PASS |
| Integracijsko | US-25 | AC4 — kada tim postoji ali nijedan agent nije dostupan, vraća se poruka i ne kreira se dodjela u bazi | `AutoAssignIntegrationTests.CreateTicket_WhenTeamExistsButNoAvailableAgents_ReturnsMessageAndNoAssignment` | PASS |
| Integracijsko | US-25 | AC6 — kada nema tima za kategoriju, vraća se poruka „Nema definisanih pravila dodjele…“ i nema dodjele u bazi | `AutoAssignIntegrationTests.CreateTicket_WhenNoTeamMatchesCategory_ReturnsNoRulesMessage` | PASS |
| Integracijsko | US-25 | AC5 — bira agenta s najmanjim brojem postojećih dodjela; novi zapis dobija `AssignmentType.AUTOMATIC` | `AutoAssignIntegrationTests.CreateTicket_PicksAgentWithFewestExistingAssignments_EndToEnd` | PASS |
| Performansno | US-25, NFR-04 | Cijeli auto-assign tok (create + lookup tima + lookup agenata sa opterećenjem + sort + assign) < 3 sekunde pri 20 dostupnih agenata sa varirajućim brojem postojećih dodjela | `AutoAssignPerformanceTests.CreateTicketWithAutoAssign_ShouldCompleteWithinThreeSeconds_AtScale` | PASS |
| Unit — Frontend (regresija) | US-25 | Sidebar agenta prikazuje odvojene linkove „Svi tiketi“ (`/tickets`) i „Dodijeljeni meni“ (`/assigned`) | `Tickets.test.jsx` — ažurirani u istom US-25 commit-u | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/AutoAssignServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AutoAssignServiceTests.cs) — 7 metoda, 12 test slučajeva (1 Theory s 5 inline data)
- [TelecomSupportSystem.Tests/TicketT/AutoAssignRepositoryTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AutoAssignRepositoryTests.cs) — 7 testova
- [TelecomSupportSystem.Tests/Integration/AutoAssignIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AutoAssignIntegrationTests.cs) — 5 testova
- [TelecomSupportSystem.Tests/Performance/AutoAssignPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/AutoAssignPerformanceTests.cs) — 1 test
- [frontend/src/test/Tickets.test.jsx](../Project/frontend/src/test/Tickets.test.jsx) — frontend pokrivenost (sidebar i odvojeni linkovi za AGENT-a) ažurirana u US-25 commit-u

---

## PB-31 — Prosljeđivanje tiketa (scoring algoritam)

### Pokriveni AC (US-55, US-56, US-57)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service (Theory, 3 slučaja) | US-55, US-56 | Težine se dinamički prilagođavaju prioritetu tiketa — grana `Priority.LOW` u `GetWeightsByPriority` | `TicketServiceTests.GetAgentScoresAsync_ShouldReturnScores_ForEachPriority[LOW]` | PASS |
| Unit — Service (Theory, 3 slučaja) | US-55, US-56 | Težine se dinamički prilagođavaju prioritetu tiketa — grana `Priority.MEDIUM` u `GetWeightsByPriority` | `TicketServiceTests.GetAgentScoresAsync_ShouldReturnScores_ForEachPriority[MEDIUM]` | PASS |
| Unit — Service (Theory, 3 slučaja) | US-55, US-56 | Težine se dinamički prilagođavaju prioritetu tiketa — grana `Priority.HIGH` u `GetWeightsByPriority` | `TicketServiceTests.GetAgentScoresAsync_ShouldReturnScores_ForEachPriority[HIGH]` | PASS |
| Unit — Service | US-55, US-56 | LOW prioritet (wExperience=0.6) — agent s više riješenih tiketa iste kategorije dobija najviši score | `TicketServiceTests.GetAgentScoresAsync_LowPriority_FavorsExperiencedAgent` | PASS |
| Unit — Service | US-55, US-56 | HIGH prioritet (wAvailability=0.5) — agent s manje otvorenih tiketa dobija najviši score | `TicketServiceTests.GetAgentScoresAsync_HighPriority_FavorsAvailableAgent` | PASS |
| Unit — Service | US-55, US-56 | MEDIUM prioritet (wExperience=0.5, wRating=0.3, wAvailability=0.2) — lista agenata je sortirana silazno po score-u | `TicketServiceTests.GetAgentScoresAsync_MediumPriority_RanksAgentsCorrectly` | PASS |
| Unit — Service | US-56 | Kada nema dostupnih agenata, `GetAgentScoresAsync` vraća praznu listu bez greške | `TicketServiceTests.GetAgentScoresAsync_NoAgents_ReturnsEmpty` | PASS |
| Unit — Service | US-56 | Kada tiket ne postoji, `GetAgentScoresAsync` baca `KeyNotFoundException` | `TicketServiceTests.GetAgentScoresAsync_TicketNotFound_ThrowsKeyNotFoundException` | PASS |

### Pokriveni AC — Controller (US-55, US-56)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Controller | US-56 | Agent dohvata listu score-ova dostupnih agenata — 200 OK | `TicketControllerForwardingTests.GetAgentScores_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-56 | Dohvat score-ova za nepostojeći tiket — 404 NotFound | `TicketControllerForwardingTests.GetAgentScores_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller (sigurnosno) | US-56 | Klijent ne može dohvatiti score-ove — 403 Forbid | `TicketControllerForwardingTests.GetAgentScores_ShouldReturnForbid_WhenUserIsNotAgent` | PASS |
| Unit — Controller | US-55 | Agent automatski prosljeđuje tiket — 200 OK s podacima o novom agentu | `TicketControllerForwardingTests.AutoForwardTicket_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-55 | Automatsko prosljeđivanje nepostojećeg tiketa — 404 NotFound | `TicketControllerForwardingTests.AutoForwardTicket_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller | US-55 | Agent koji nije vlasnik tiketa pokušava proslijediti — 403 Forbid | `TicketControllerForwardingTests.AutoForwardTicket_ShouldReturnForbid_WhenUserUnauthorized` | PASS |
| Unit — Controller | US-55 | Nema dostupnih agenata za prosljeđivanje — 400 BadRequest | `TicketControllerForwardingTests.AutoForwardTicket_ShouldReturnBadRequest_WhenInvalidOperation` | PASS |
| Unit — Controller | US-56 | Agent ručno prosljeđuje tiket odabranom agentu — 200 OK | `TicketControllerForwardingTests.ForwardTicketToAgent_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-56 | Ručno prosljeđivanje nepostojećeg tiketa — 404 NotFound | `TicketControllerForwardingTests.ForwardTicketToAgent_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller (sigurnosno) | US-56 | Klijent ne može ručno proslijediti tiket — 403 Forbid | `TicketControllerForwardingTests.ForwardTicketToAgent_ShouldReturnForbid_WhenUserIsNotAgent` | PASS |
| Unit — Controller | US-55, US-56 | Agent prosljeđuje tiket tehničaru na lokaciji — 200 OK | `TicketControllerForwardingTests.ForwardTicketToTechnician_ShouldReturnOk_WhenRequestIsValid` | PASS |
| Unit — Controller | US-55, US-56 | Prosljeđivanje tehničaru za nepostojeći tiket — 404 NotFound | `TicketControllerForwardingTests.ForwardTicketToTechnician_ShouldReturnNotFound_WhenTicketDoesNotExist` | PASS |
| Unit — Controller | US-55, US-56 | Nema tehničara na lokaciji kreatora — 400 BadRequest | `TicketControllerForwardingTests.ForwardTicketToTechnician_ShouldReturnBadRequest_WhenLocationInvalid` | PASS |
| Unit — Controller (sigurnosno) | US-55, US-56 | Klijent ne može proslijediti tiket tehničaru — 403 Forbid | `TicketControllerForwardingTests.ForwardTicketToTechnician_ShouldReturnForbid_WhenUserIsNotAgent` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketServiceTests.cs) — 6 novih metoda, 8 test slučajeva (1 Theory s 3 inline data + 5 Facts)
- [TelecomSupportSystem.Tests/TicketT/TicketControllerForwardingTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketControllerForwardingTests.cs) — 13 testova (GetAgentScores × 3, AutoForward × 4, ForwardToAgent × 3, ForwardToTechnician × 4)

---

## PB-37 — Tehničar vidi osnovne informacije o tiketu

### Pokriveni AC (US-14, US-30, US-39)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-14 | Klijent dohvata vlastiti tiket — vraća TicketDetailDto | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenClientIsOwner` | PASS |
| Unit — Service | US-14 | Tiket ne postoji — baca KeyNotFoundException | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowKeyNotFound_WhenTicketMissing` | PASS |
| Unit — Service (sigurnosno) | US-14 | Klijent ne može dohvatiti tuđi tiket — baca UnauthorizedAccessException | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherTicket` | PASS |
| Unit — Service | US-30 | Agent može dohvatiti svaki tiket bez obzira na vlasništvo | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenAgentAccessesAnyTicket` | PASS |
| Unit — Service | US-14 | Sva polja tiketa (Title, Status, Priority, ClientName, AssignedAgentName) ispravno mapirana u DTO | `TicketDetailServiceTests.GetTicketByIdAsync_ShouldMapAllFieldsToDto` | PASS |
| Unit — Controller | US-14 | Tiket postoji i korisnik ima pristup — 200 OK s TicketDetailDto | `TicketDetailControllerTests.GetTicketById_ShouldReturnOk_WhenTicketFound` | PASS |
| Unit — Controller | US-14 | Tiket ne postoji — 404 NotFound | `TicketDetailControllerTests.GetTicketById_ShouldReturnNotFound_WhenTicketMissing` | PASS |
| Unit — Controller (sigurnosno) | US-14 | Korisnik nema prava pristupa tiketu — 403 Forbid | `TicketDetailControllerTests.GetTicketById_ShouldReturnForbid_WhenAccessDenied` | PASS |
| Unit — Controller (sigurnosno) | US-14 | JWT claim nije prisutan — 401 Unauthorized | `TicketDetailControllerTests.GetTicketById_ShouldReturnUnauthorized_WhenNoUserClaim` | PASS |
| Integracijsko | US-14 | Klijent vidi vlastiti tiket end-to-end — sva polja ispravno mapirana u response | `TicketDetailIntegrationTests.GetTicketById_ClientOwner_ReturnsDetailDtoWithAllFields` | PASS |
| Integracijsko | US-30 | Agent može vidjeti bilo koji tiket end-to-end | `TicketDetailIntegrationTests.GetTicketById_AgentRole_CanAccessAnyTicket` | PASS |
| Integracijsko (sigurnosno) | US-14 | Klijent ne može vidjeti tuđi tiket — 403 Forbid end-to-end | `TicketDetailIntegrationTests.GetTicketById_ClientAccessingOtherTicket_ReturnsForbid` | PASS |
| Integracijsko | US-14 | Nepostojeći tiket — 404 NotFound end-to-end | `TicketDetailIntegrationTests.GetTicketById_TicketDoesNotExist_ReturnsNotFound` | PASS |
| Performansno (NFR) | US-14 | Detalji tiketa (s Creator i Assignments include-om) učitavaju se u < 2s | `TicketDetailPerformanceTests.GetTicketById_ShouldLoadWithinTimeLimit_InTestEnvironment` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/TicketDetailServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketDetailServiceTests.cs) — 5 testova
- [TelecomSupportSystem.Tests/TicketT/TicketDetailControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/TicketDetailControllerTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Integration/TicketDetailIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/TicketDetailIntegrationTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Performance/TicketDetailPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/TicketDetailPerformanceTests.cs) — 1 test

---

## PB-48 — Pregled i historija dodijeljenih tiketa za agente

### Pokriveni AC (US-53, US-54)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |
| Unit — Service | US-53 | Agent bez `assignedOnly` dohvata sve tikete u sistemu | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldReturnAllTickets_WhenAgentCallsWithoutAssignedOnly` | PASS |
| Unit — Service | US-53 | Agent sa `assignedOnly=true` dohvata samo tikete koji su mu dodijeljeni | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldReturnAssignedOnly_WhenAgentSetsAssignedOnlyTrue` | PASS |
| Unit — Service | US-39 | Tehničar uvijek dohvata samo dodijeljene tikete | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldReturnAssignedOnly_WhenTechnicianCalls` | PASS |
| Unit — Service (sigurnosno) | US-53 | Klijent ne može pozvati `GetAllTickets` — baca UnauthorizedAccessException | `AllTicketsServiceTests.GetAllTicketsAsync_ShouldThrowUnauthorized_WhenClientCalls` | PASS |
| Unit — Controller | US-53 | Agent dohvata sve tikete — 200 OK s listom MyTicketDto | `AllTicketsControllerTests.GetAllTickets_ShouldReturnOk_WhenAgentCalls` | PASS |
| Unit — Controller (sigurnosno) | US-53 | Klijent ne može pristupiti listi svih tiketa — 403 Forbid | `AllTicketsControllerTests.GetAllTickets_ShouldReturnForbid_WhenClientCalls` | PASS |
| Unit — Controller (sigurnosno) | US-53 | JWT claim nije prisutan — 401 Unauthorized | `AllTicketsControllerTests.GetAllTickets_ShouldReturnUnauthorized_WhenNoUserClaim` | PASS |
| Unit — Repository | US-53 | `GetAllAsync` vraća sve tikete bez filtera | `AllTicketsRepositoryTests.GetAllAsync_ShouldReturnAllTickets` | PASS |
| Unit — Repository | US-53 | `GetAllAsync` vraća praznu listu kada nema tiketa u bazi | `AllTicketsRepositoryTests.GetAllAsync_ShouldReturnEmpty_WhenNoTicketsExist` | PASS |
| Unit — Repository | US-53 | `GetAllAsync` vraća tikete sortirane od najnovijeg ka najstarijem | `AllTicketsRepositoryTests.GetAllAsync_ShouldReturnTicketsOrderedByCreatedDateDescending` | PASS |
| Unit — Repository | US-53 | `GetByAssigneeIdAsync` vraća samo tikete na kojima je zadani agent dodijeljen | `AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldReturnOnlyAssignedTickets` | PASS |
| Integracijsko | US-53 | Agent vidi sve tikete u sistemu end-to-end | `AllTicketsIntegrationTests.GetAllTickets_AgentRole_ReturnsAllTickets` | PASS |
| Integracijsko (sigurnosno) | US-53 | Klijent ne može pristupiti listi svih tiketa end-to-end — 403 Forbid | `AllTicketsIntegrationTests.GetAllTickets_ClientRole_ReturnsForbid` | PASS |
| Integracijsko | US-53 | Agent s `assignedOnly=true` vidi samo vlastite tikete end-to-end | `AllTicketsIntegrationTests.GetAllTickets_AgentWithAssignedOnlyTrue_ReturnsOnlyAssignedTickets` | PASS |
| Integracijsko | US-53 | Tiketi sortirani od najnovijeg ka najstarijem end-to-end | `AllTicketsIntegrationTests.GetAllTickets_ReturnsTicketsOrderedByDateDescending` | PASS |
| Performansno (NFR) | US-53 | Lista od 500 tiketa učitava se u < 2s | `AllTicketsPerformanceTests.GetAllTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment` | PASS |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/TicketT/AllTicketsServiceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AllTicketsServiceTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/TicketT/AllTicketsControllerTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AllTicketsControllerTests.cs) — 3 testa
- [TelecomSupportSystem.Tests/TicketT/AllTicketsRepositoryTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/TicketT/AllTicketsRepositoryTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Integration/AllTicketsIntegrationTests.cs) — 4 testa
- [TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Performance/AllTicketsPerformanceTests.cs) — 1 test

---

## Sistemski testovi — Sprint 7

Fajl `Sprint7UserStoriesSystemTests.cs` sadrži end-to-end testove koji prolaze kroz Controller → Service → Repository → EF InMemory za sve Sprint 7 PB stavke, verifikujući konzistentnost između slojeva.

| Test | PB | Šta verifikuje |
| --- | --- | --- |
| `PB25_ClosureWorkflow_RequestAcceptAndAlreadyClosedValidation_WorksThroughControllerServiceRepo` | PB-25 | Cijeli closure workflow: agent šalje zahtjev → tiket `CLOSURE_REQUESTED` + `PENDING` → klijent prihvata → tiket `CLOSED` + `ACCEPTED` → ponovni pokušaj zatvaranja vraća 400 |
| `PB28_InternalPriority_IsVisibleToStaffAndHiddenFromClient` | PB-28 | Agent postavlja interni prioritet `CRITICAL` → agent ga vidi u TicketDetailDto → klijent ga ne vidi (null) |
| `PB30_AutoAssignedTicket_IsVisibleInAssignedListAndTicketDetail` | PB-30 | Klijent kreira tiket → auto-dodjela agentu → agent vidi tiket u `assignedOnly=true` listi i u detalju tiketa |
| `PB31_ManualForward_UsesSortedAvailableAgentScoresAndPersistsNewOwner` | PB-31 | Agent dohvata score-ove (samo AVAILABLE, isključuje vlasnika) → ručno prosljeđuje → u bazi kreira TicketUser s `FORWARDED_TO_AGENT` |
| `PB37_TechnicianCanOpenOnlyAssignedTicketDetails` | PB-37 | Tehničar vidi detalje dodijeljenog tiketa → ne može vidjeti nedodijeljeni tiket — 403 Forbid |
| `PB37_TechnicianCanCommentOnAssignedTicketButNotUnassignedOrEmptyMessage` | PB-37 | Tehničar komentariše na dodijeljenom tiketu → prazna poruka vraća 400 → komentar na nedodijeljenom tiketu vraća 403 |
| `PB48_AgentOpenAndClosedAssignedEndpointsReturnOnlyCurrentLatestAssignments` | PB-48 | Agent vidi samo vlastite otvorene tikete i samo vlastite zatvorene tikete — tuđi se ne pojavljuju |

### Fajlovi sa testovima

- [TelecomSupportSystem.Tests/System/Sprint7UserStoriesSystemTests.cs](../Project/TelecomSupportSystem/TelecomSupportSystem.Tests/System/Sprint7UserStoriesSystemTests.cs) — 7 sistemskih testova

---

## Veza sa Test Strategijom

| Test strategija nivo | US | PB | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — backend servis | US-25 | PB-30 | `AutoAssignServiceTests` (logika izbora agenta, mapiranje kategorija na tim, fallback poruke) | PASS |
| Unit — backend repository (EF InMemory) | US-25 | PB-30 | `AutoAssignRepositoryTests` (`GetBySpecializedCategoryAsync`, `GetAvailableAgentsByTeamIdAsync`, `AddAssignmentAsync`) | PASS |
| Integracijsko — backend | US-25 | PB-30 | `AutoAssignIntegrationTests` (kreiranje → auto-dodjela → vidljivost agentu kroz pun stack) | PASS |
| Performansno — backend (NFR-04) | US-25 | PB-30 | `AutoAssignPerformanceTests` (<3s pri realističnom broju agenata) | PASS |
| Sigurnosno (rola, dostupnost) | US-25 | PB-30 | `AutoAssignRepositoryTests.GetAvailableAgentsByTeamIdAsync_DoesNotReturnNonAgents` (samo `Role.AGENT`); `AutoAssignServiceTests`/`AutoAssignIntegrationTests` provjeravaju da `BUSY`/`UNAVAILABLE` agenti nisu kandidati | PASS |
| UI — stranice (regresija) | US-25 | PB-30 | `Tickets.test.jsx` — agent sidebar i odvojeni linkovi `/tickets` vs `/assigned` | PASS |
| Unit — backend controller | US-16, US-17 | PB-25 | `TicketControllerClosureTests` (RequestClosure, AcceptClosure, RejectClosure, ForceClose, CloseTicket) | PASS |
| Unit — backend controller | US-21 | PB-28 | `TicketControllerClosureTests` (UpdateInternalPriority — OK i NotFound) | PASS |
| Unit — backend servis | US-14, US-30 | PB-37 | `TicketDetailServiceTests` (pristup po roli, mapiranje polja, sigurnosno odvajanje klijenta) | PASS |
| Unit — backend controller | US-14, US-30 | PB-37 | `TicketDetailControllerTests` (200/404/403/401) | PASS |
| Integracijsko — backend | US-14, US-30 | PB-37 | `TicketDetailIntegrationTests` (klijent-vlasnik, agent, tuđi tiket, nepostojeći) | PASS |
| Performansno — backend (NFR) | US-14 | PB-37 | `TicketDetailPerformanceTests` (detalji tiketa < 2s) | PASS |
| Unit — backend servis (scoring) | US-55, US-56 | PB-31 | `TicketServiceTests` (GetWeightsByPriority grane, scoring logika po prioritetu) | PASS |
| Unit — backend controller | US-55, US-56 | PB-31 | `TicketControllerForwardingTests` (GetAgentScores, AutoForward, ForwardToAgent, ForwardToTechnician) | PASS |
| Unit — backend servis | US-53, US-54 | PB-48 | `AllTicketsServiceTests` (AGENT sve/dodijeljene, TECHNICIAN, sigurnosno CLIENT) | PASS |
| Unit — backend controller | US-53, US-54 | PB-48 | `AllTicketsControllerTests` (200/403/401) | PASS |
| Unit — backend repository (EF InMemory) | US-53, US-54 | PB-48 | `AllTicketsRepositoryTests` (GetAllAsync, GetByAssigneeIdAsync, sortiranje) | PASS |
| Integracijsko — backend | US-53, US-54 | PB-48 | `AllTicketsIntegrationTests` (agent, klijent 403, assignedOnly, sortiranje) | PASS |
| Performansno — backend (NFR) | US-53, US-54 | PB-48 | `AllTicketsPerformanceTests` (500 tiketa < 2s) | PASS |
| Sistemski — end-to-end svi slojevi | svi Sprint 7 US | PB-25 do PB-48 | `Sprint7UserStoriesSystemTests` (7 testova: closure workflow, interni prioritet, auto-dodjela, prosljeđivanje, tehničar, PB-48) | PASS |

---

## Napomena o pristupu

- **Mapiranje kategorija → tim** je implementirano kroz `Team.SpecializedCategory` (a ne kroz zasebnu `AssignmentRules` tabelu); ova arhitekturna odluka je donesena u Sprintu 7 i objašnjena u [DecisionLog.md](DecisionLog.md). „Pravila dodjele“ (AC5) su time implicitno definisana strukturom timova. AC6 („Nema definisanih pravila“) se okida kada za odabranu `ProblemCategory` ne postoji tim sa odgovarajućim `SpecializedCategory`.
- **„Nedodijeljen“ stanje** (AC4) nije zaseban `TicketStatus` enum, već se kombinuje status `OPEN` sa popunjenom `AssignmentMessage` porukom u DTO-u — što frontend prepoznaje i prikazuje korisniku. Testovi verifikuju i poruku i odsustvo `TicketUser` zapisa u bazi.
- **Agent vidi auto-dodijeljen tiket** (AC3) ne zahtijeva poseban endpoint; postojeći `GET /api/tickets?assignedOnly=true` (PB-32) ga već prikazuje jer auto-dodjela kreira `TicketUser` zapis. Test `AutoAssignIntegrationTests.CreateTicket_AutoAssignsAndAgentSeesTicket_EndToEnd` to verifikuje end-to-end.

---

## Lokalno pokretanje testova:

Iz root direktorija:

### Backend (samo US-25 / PB-30 testovi):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~AutoAssign" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-25 i PB-28 — closure i prioritet):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~TicketControllerClosureTests" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-37 — detalji tiketa):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~TicketDetail" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-31 — prosljeđivanje i scoring):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~Forwarding|FullyQualifiedName~TicketServiceTests" --logger "console;verbosity=normal" 2>&1
```

### Backend (PB-48 — pregled dodijeljenih tiketa):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~AllTickets" --logger "console;verbosity=normal" 2>&1
```

### Backend (sistemski testovi Sprint 7):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~Sprint7UserStoriesSystemTests" --logger "console;verbosity=normal" 2>&1
```

### Backend (kompletan test suite):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --logger "console;verbosity=normal" 2>&1
```

### Frontend:
```bash
cd Project/frontend && npx vitest run 2>&1
```
