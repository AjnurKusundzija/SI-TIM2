# Proof of Testing — Sprint 7
---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |
| Unit — Backend (service) | US-25 | xUnit + Moq | 12 novih testova (7 metoda, 12 slučajeva uključujući Theory) | PASS |
| Unit — Backend (repository, EF InMemory) | US-25 | xUnit + EF InMemory | 7 novih testova | PASS |
| Integracijsko — Backend | US-25 | xUnit + EF InMemory | 5 novih testova | PASS |
| Performansno — Backend | US-25 (NFR-04) | xUnit + Stopwatch | 1 novi test | PASS |
| **Ukupno Sprint 7** | **US-25** | | **25 novih backend testova** | **PASS** |
| **Ukupno projekat** | **US-1 do US-3, US-8 do US-15, US-18 do US-20, US-25, US-29 do US-32** | | **145 backend + 154 frontend = 299** | **PASS** |

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

## Veza sa Test Strategijom

| Test strategija nivo | US | PB | Dokaz | Status |
| --- | --- | --- | --- | --- |
| Unit — backend servis | US-25 | PB-30 | `AutoAssignServiceTests` (logika izbora agenta, mapiranje kategorija na tim, fallback poruke) | PASS |
| Unit — backend repository (EF InMemory) | US-25 | PB-30 | `AutoAssignRepositoryTests` (`GetBySpecializedCategoryAsync`, `GetAvailableAgentsByTeamIdAsync`, `AddAssignmentAsync`) | PASS |
| Integracijsko — backend | US-25 | PB-30 | `AutoAssignIntegrationTests` (kreiranje → auto-dodjela → vidljivost agentu kroz pun stack) | PASS |
| Performansno — backend (NFR-04) | US-25 | PB-30 | `AutoAssignPerformanceTests` (<3s pri realističnom broju agenata) | PASS |
| Sigurnosno (rola, dostupnost) | US-25 | PB-30 | `AutoAssignRepositoryTests.GetAvailableAgentsByTeamIdAsync_DoesNotReturnNonAgents` (samo `Role.AGENT`); `AutoAssignServiceTests`/`AutoAssignIntegrationTests` provjeravaju da `BUSY`/`UNAVAILABLE` agenti nisu kandidati | PASS |
| UI — stranice (regresija) | US-25 | PB-30 | `Tickets.test.jsx` — agent sidebar i odvojeni linkovi `/tickets` vs `/assigned` | PASS |

---

## Napomena o pristupu

- **Mapiranje kategorija → tim** je implementirano kroz `Team.SpecializedCategory` (a ne kroz zasebnu `AssignmentRules` tabelu); ova arhitekturna odluka je donesena u Sprintu 7 i objašnjena u [DecisionLog.md](DecisionLog.md). „Pravila dodjele“ (AC5) su time implicitno definisana strukturom timova. AC6 („Nema definisanih pravila“) se okida kada za odabranu `ProblemCategory` ne postoji tim sa odgovarajućim `SpecializedCategory`.
- **„Nedodijeljen“ stanje** (AC4) nije zaseban `TicketStatus` enum, već se kombinuje status `OPEN` sa popunjenom `AssignmentMessage` porukom u DTO-u — što frontend prepoznaje i prikazuje korisniku. Testovi verifikuju i poruku i odsustvo `TicketUser` zapisa u bazi.
- **Agent vidi auto-dodijeljen tiket** (AC3) ne zahtijeva poseban endpoint; postojeći `GET /api/tickets?assignedOnly=true` (PB-32) ga već prikazuje jer auto-dodjela kreira `TicketUser` zapis. Test `AutoAssignIntegrationTests.CreateTicket_AutoAssignsAndAgentSeesTicket_EndToEnd` to verifikuje end-to-end.

---

## Lokalno pokretanje testova:

Iz root direktorija:

### Backend (svi US-25 testovi):
```bash
cd Project/TelecomSupportSystem && dotnet test --filter "FullyQualifiedName~AutoAssign" --logger "console;verbosity=normal" 2>&1
```

### Backend (kompletan test suite):
```bash
cd Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --logger "console;verbosity=normal" 2>&1
```

### Frontend:
```bash
cd Project/frontend && npx vitest run 2>&1
```
