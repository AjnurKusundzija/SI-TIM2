# Proof of Testing - Sprint 8

Dokument evidentira dokaze testiranja za Sprint 8 funkcionalnosti, uključujući dodatne izmjene uvedene tokom refiniranja assignment i closure workflow logike.

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 19.05.2026 | Zajednička dodjela agentu i tehničaru, assigned list, detalji tiketa, status "Čeka se" | Backend unit/integration-style tests + frontend unit/UI tests | PASS |
| 19.05.2026 | Closure workflow notifikacije: accept, reject, force close | Backend unit tests | PASS |

---

## Test okruženje

| Stavka | Vrijednost |
|---|---|
| Backend | .NET 10, xUnit, Moq, FluentAssertions, EF Core InMemory |
| Frontend | React, Vitest, Testing Library, jsdom |
| Projekat | Project/TelecomSupportSystem/TelecomSupportSystem.slnx i Project/frontend |
| Napomena | Prvi pokušaj dotnet test unutar sandboxa pao je zbog MSBuild named-pipe ograničenja (SocketException: Permission denied). Test je ponovljen uz dozvoljeno izvršavanje izvan sandboxa i prošao. |

---

## Izvršene test komande

### Backend - assignment, status i detalji tiketa

```bash
dotnet test TelecomSupportSystem.slnx --no-restore --filter "FullyQualifiedName~AllTicketsRepositoryTests|FullyQualifiedName~TicketDetailServiceTests|FullyQualifiedName~TicketStatusUpdate"
```

Rezultat: PASS  
Ukupno: 24 passed, 0 failed, 0 skipped

### Frontend - assigned tickets i detalji tiketa

```bash
npm test -- --run src/test/Tickets.test.jsx src/test/TicketDetail.test.jsx src/test/ui/TicketsAssignmentStatusUi.test.jsx
```

Rezultat: PASS  
Ukupno: 20 passed, 0 failed

### Backend - closure workflow notifikacije

```bash
dotnet test TelecomSupportSystem.slnx --no-restore --filter "FullyQualifiedName~TicketClosureServiceTests|FullyQualifiedName~TicketControllerClosureTests|FullyQualifiedName~TicketStatusUpdate"
```

Rezultat: PASS  
Ukupno: 29 passed, 0 failed, 0 skipped

### Backend - odbijanje zatvaranja tiketa

```bash
dotnet test TelecomSupportSystem.slnx --no-restore --filter "FullyQualifiedName~TicketClosureServiceTests|FullyQualifiedName~TicketControllerClosureTests"
```

Rezultat: PASS  
Ukupno: 17 passed, 0 failed, 0 skipped

### Frontend - detalji tiketa nakon closure/assignment izmjena

```bash
npm test -- --run src/test/TicketDetail.test.jsx
```

Rezultat: PASS  
Ukupno: 9 passed, 0 failed

---

## Pokrivenost po User Storyju

### US-58 - Notifikacije

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Klijent prihvati zatvaranje tiketa | Aktivno dodijeljeni agent i tehničar dobijaju TICKET_CLOSED notifikaciju | TicketClosureServiceTests.AcceptClosureAsync_ShouldNotifyAllActiveAssignedStaff_WhenClientAcceptsClosure | PASS |
| Klijent odbije zatvaranje tiketa | Aktivno dodijeljeni agent i tehničar dobijaju STATUS_CHANGED notifikaciju | TicketClosureServiceTests.RejectClosureAsync_ShouldNotifyAllActiveAssignedStaff_WhenClientRejectsClosure | PASS |
| Assigned staff prisilno zatvori tiket | Klijent dobija TICKET_CLOSED notifikaciju | TicketClosureServiceTests.ForceCloseAsync_ShouldNotifyClient_WhenAssignedStaffForceClosesTicket | PASS |
| Staff koji nije aktivno dodijeljen pokuša force close | Sistem odbija akciju i ne šalje notifikaciju | TicketClosureServiceTests.ForceCloseAsync_ShouldThrowUnauthorized_WhenStaffIsNotAssigned | PASS |

---

### US-60 - Ažuriranje statusa tiketa od strane tehničara

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Dodijeljeni tehničar postavi status CLOSURE_REQUESTED | Status se čuva, closure request postaje PENDING, klijent dobija STATUS_CHANGED | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldUpdateStatusAndNotifyCreator_WhenAssignedTechnicianRequestsClosure | PASS |
| Tehničar koji nije dodijeljen pokuša promijeniti status | Sistem vraća UnauthorizedAccessException | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldThrowUnauthorized_WhenTicketNotAssignedToTechnician | PASS |
| Zatvorenom tiketu se pokuša promijeniti status | Sistem odbija promjenu | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldThrowInvalidOperation_WhenTicketIsClosed | PASS |
| Tehničar vrati tiket sa CLOSURE_REQUESTED na OPEN | Closure request status prelazi u REJECTED | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldMarkClosureAsRejected_WhenMovingFromClosureRequestedBackToOpen | PASS |

---

### US-62 / US-63 - Statistika agenta i tehničara

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Agent ostaje aktivno dodijeljen nakon prosljeđivanja tehničaru | Tiket ulazi u agentove assigned/statistics upite | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldKeepAgentAndTechnicianAssigned_WhenForwardedToTechnician | PASS |
| Tehničar je aktivno dodijeljen nakon prosljeđivanja | Tiket ulazi u tehničareve assigned upite | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldKeepAgentAndTechnicianAssigned_WhenForwardedToTechnician | PASS |
| Forward na drugog agenta | Prethodni agent više nije aktivni assignee | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldNotKeepPreviousAgentAssigned_WhenForwardedToAnotherAgent | PASS |

---

### US-70 - Zajednička dodjela agentu i tehničaru

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Agent proslijedi tiket tehničaru | Sistem tretira i agenta i tehničara kao aktivne assigneeje | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldKeepAgentAndTechnicianAssigned_WhenForwardedToTechnician | PASS |
| Agent otvori dodijeljene tikete nakon što je status "Čeka se" | Tiket se prikazuje u listi dodijeljenih tiketa | TicketsAssignmentStatusUi.test.jsx | PASS |
| Detalji tiketa nakon prosljeđivanja tehničaru | UI prikazuje Agent: ... i ispod Tehničar: ... | TicketDetail.test.jsx i TicketDetailServiceTests.GetTicketByIdAsync_ShouldMapAllFieldsToDto | PASS |
| Lista dodijeljenih tiketa | Default filter je Svi statusi, ne samo OPEN | Tickets.test.jsx, TicketsAssignmentStatusUi.test.jsx | PASS |
