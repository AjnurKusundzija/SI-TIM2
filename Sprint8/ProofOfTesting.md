# Proof of Testing - Sprint 8

Dokument evidentira dokaze testiranja za Sprint 8 funkcionalnosti. Pokrivenost je mapirana na sve stavke Sprint 8 backloga: US-58, US-59, US-60, US-61, US-62, US-63, US-64, US-65, US-66, US-67, US-68, US-69 i US-70.

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 19.05.2026 | PB-49 Notifikacije | Backend unit tests + SignalR/chat tests | PASS |
| 19.05.2026 | PB-36 Ažuriranje statusa tiketa | Backend unit/integration-style tests + frontend unit/UI tests | PASS |
| 19.05.2026 | PB-26 Ocjenjivanje tiketa | Backend service/controller tests + frontend UI tests | PASS |
| 19.05.2026 | PB-42 Statistika agenta i tehničara | Backend repository/service tests + frontend dashboard tests | PASS |
| 19.05.2026 | PB-20 Upravljanje korisničkim profilom | Backend service/security tests + frontend auth/profile tests | PASS |
| 19.05.2026 | PB-34 Pregled korisničkih profila | Backend profile/security tests + ticket detail/list tests | PASS |
| 19.05.2026 | PB-21 Prikaz paketa i pretplata | Backend package/profile wiring + migration/DTO checks | PASS |
| 19.05.2026 | Sistemske poruke u chatu pri prosljeđivanju tiketa | Backend comment/chat tests + frontend communication tests | PASS |
| 19.05.2026 | Zajednička dodjela agentu i tehničaru | Backend repository tests + frontend assigned-ticket/detail tests | PASS |

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

### Backend - rating, profili, chat i SignalR

```bash
dotnet test TelecomSupportSystem.slnx --no-restore --filter "FullyQualifiedName~RatingServiceTests|FullyQualifiedName~RatingControllerTests|FullyQualifiedName~AdminUserProfileServiceTests|FullyQualifiedName~RoleAccessSecurityTests|FullyQualifiedName~CommentServiceTests|FullyQualifiedName~CommentControllerTests|FullyQualifiedName~ChatHubTests"
```

Rezultat: PASS  
Pokriva: US-59, US-61, US-64, US-65, US-66, US-67, US-69

### Frontend - rating, dashboard i communication workflow

```bash
npm test -- --run src/test/TicketRating.test.jsx src/test/Dashboard.test.jsx src/test/TechnicianDashboard.test.jsx src/test/AssignedTickets.test.jsx src/test/acceptance/CommunicationAcceptance.test.jsx src/test/system/CommunicationSystem.test.jsx
```

Rezultat: PASS  
Pokriva: US-61, US-62, US-63, US-69

---

## Pokrivenost po User Storyju

### US-58 - Slanje notifikacija

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Klijent prihvati zatvaranje tiketa | Aktivno dodijeljeni agent i tehničar dobijaju TICKET_CLOSED notifikaciju | TicketClosureServiceTests.AcceptClosureAsync_ShouldNotifyAllActiveAssignedStaff_WhenClientAcceptsClosure | PASS |
| Klijent odbije zatvaranje tiketa | Aktivno dodijeljeni agent i tehničar dobijaju STATUS_CHANGED notifikaciju | TicketClosureServiceTests.RejectClosureAsync_ShouldNotifyAllActiveAssignedStaff_WhenClientRejectsClosure | PASS |
| Assigned staff prisilno zatvori tiket | Klijent dobija TICKET_CLOSED notifikaciju | TicketClosureServiceTests.ForceCloseAsync_ShouldNotifyClient_WhenAssignedStaffForceClosesTicket | PASS |
| Staff koji nije aktivno dodijeljen pokuša force close | Sistem odbija akciju i ne šalje notifikaciju | TicketClosureServiceTests.ForceCloseAsync_ShouldThrowUnauthorized_WhenStaffIsNotAssigned | PASS |

---

### US-59 - Prikaz i real-time tok notifikacija

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Promjena statusa šalje notifikaciju korisniku | NotificationService se poziva sa ispravnim tipom, porukom i ticketId vrijednošću | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldUpdateStatusAndNotifyCreator_WhenAssignedTechnicianRequestsClosure | PASS |
| SignalR grupa se koristi za real-time tok po tiketu | Konekcija se dodaje u ticket grupu | ChatHubTests.JoinTicketGroup_ShouldAddConnectionToGroup | PASS |
| Korisnik može napustiti SignalR ticket grupu | Konekcija se uklanja iz ticket grupe | ChatHubTests.LeaveTicketGroup_ShouldRemoveConnectionFromGroup | PASS |
| Različiti tiketi imaju odvojene grupe | Konekcija se pridružuje različitim ticket grupama | ChatHubTests.JoinTicketGroup_WithDifferentTickets_ShouldJoinDifferentGroups | PASS |

---

### US-60 - Ažuriranje statusa tiketa od strane tehničara

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Dodijeljeni tehničar postavi status CLOSURE_REQUESTED | Status se čuva, closure request postaje PENDING, klijent dobija STATUS_CHANGED | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldUpdateStatusAndNotifyCreator_WhenAssignedTechnicianRequestsClosure | PASS |
| Tehničar koji nije dodijeljen pokuša promijeniti status | Sistem vraća UnauthorizedAccessException | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldThrowUnauthorized_WhenTicketNotAssignedToTechnician | PASS |
| Zatvorenom tiketu se pokuša promijeniti status | Sistem odbija promjenu | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldThrowInvalidOperation_WhenTicketIsClosed | PASS |
| Tehničar vrati tiket sa CLOSURE_REQUESTED na OPEN | Closure request status prelazi u REJECTED | TicketStatusUpdateServiceTests.UpdateTicketStatusAsync_ShouldMarkClosureAsRejected_WhenMovingFromClosureRequestedBackToOpen | PASS |

---

### US-61 - Ocjenjivanje tiketa

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Klijent ocijeni vlastiti zatvoreni tiket | Rating se kreira i vraća Created odgovor | RatingServiceTests.CreateRatingAsync_ShouldSucceed_WhenClientRatesOwnClosedTicket, RatingControllerTests.CreateRating_ShouldReturnCreated_WhenClientRatesClosedTicket | PASS |
| Klijent pokuša ocijeniti tiket koji nije zatvoren | Sistem odbija ocjenu | RatingServiceTests.CreateRatingAsync_ShouldThrow_WhenTicketIsNotClosed, RatingControllerTests.CreateRating_ShouldReturnConflict_WhenTicketIsNotClosed | PASS |
| Isti tiket se pokuša ocijeniti više puta | Sistem vraća grešku za duplikat | RatingServiceTests.CreateRatingAsync_ShouldThrow_WhenTicketAlreadyRated, RatingControllerTests.CreateRating_ShouldReturnConflict_WhenTicketAlreadyRated | PASS |
| Korisnik koji nije klijent pokuša ocijeniti tiket | Sistem odbija akciju | RatingServiceTests.CreateRatingAsync_ShouldThrow_WhenRoleIsNotClient, RatingControllerTests.CreateRating_ShouldReturnForbid_WhenAgentTriesToRate | PASS |
| Frontend prikazuje tok ocjenjivanja | UI test pokriva komponentu ocjenjivanja | TicketRating.test.jsx | PASS |

---

### US-62 - Statistika agenta

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Agent ostaje aktivno dodijeljen nakon prosljeđivanja tehničaru | Tiket ulazi u agentove assigned/statistics upite | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldKeepAgentAndTechnicianAssigned_WhenForwardedToTechnician | PASS |
| Forward na drugog agenta | Prethodni agent više nije aktivni assignee | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldNotKeepPreviousAgentAssigned_WhenForwardedToAnotherAgent | PASS |
| Agent vidi dashboard/statistiku | Frontend dashboard se renderuje sa podacima korisnika | Dashboard.test.jsx | PASS |

---

### US-63 - Statistika tehničara

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Tehničar je aktivno dodijeljen nakon prosljeđivanja | Tiket ulazi u tehničareve assigned upite | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldKeepAgentAndTechnicianAssigned_WhenForwardedToTechnician | PASS |
| Tehničar vidi svoje dodijeljene zadatke/statistiku | Tehničarski dashboard i assigned list se renderuju | TechnicianDashboard.test.jsx, AssignedTickets.test.jsx | PASS |

---

### US-64 - Promjena emaila korisničkog profila

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Korisnik mijenja email na slobodnu adresu | Servis ažurira email i poziva repozitorij | AdminUserProfileServiceTests.UpdateEmailAsync_ShouldUpdateAndCallRepo_WhenNewEmailIsUnique | PASS |
| Korisnik pokuša koristiti zauzet email | Sistem vraća InvalidOperationException | AdminUserProfileServiceTests.UpdateEmailAsync_ShouldThrowInvalidOperation_WhenEmailAlreadyTaken | PASS |
| Promjena emaila za nepostojećeg korisnika | Sistem vraća KeyNotFoundException | AdminUserProfileServiceTests.UpdateEmailAsync_ShouldThrowKeyNotFound_WhenUserDoesNotExist | PASS |

---

### US-65 - Promjena lozinke i sigurnost profila

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Profil ne izlaže password hash | DTO ne sadrži PasswordHash polje | AdminUserProfileServiceTests.GetUserProfileAsync_ShouldNotExposePasswordHash_WhenProfileReturned | PASS |
| Sigurnosna provjera DTO odgovora | Response DTO ne sadrži PasswordHash | RoleAccessSecurityTests.GetUserProfile_ShouldNotContainPasswordHash_InResponseDto | PASS |
| Neovlašten korisnik ne može pristupiti tuđem profilu | Sistem vraća Unauthorized/403 | AdminUserProfileServiceTests.GetUserProfileAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherProfile, RoleAccessSecurityTests.GetUserProfile_ShouldReturn403_WhenClientAccessesOtherUser | PASS |
| Frontend login forma šalje email i lozinku | Auth servis i forma obrađuju kredencijale | Login.test.jsx, authService.test.js, AuthContext.test.jsx | PASS |

---

### US-66 - Pregled korisničkih profila

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Ovlaštena rola čita profil agenta | Servis vraća UserProfileDto za agenta | AdminUserProfileServiceTests.GetUserProfileAsync_ShouldReturnProfile_WhenAdminReadsAgentProfile | PASS |
| Ovlaštena rola čita profil klijenta | Servis vraća UserProfileDto za klijenta | AdminUserProfileServiceTests.GetUserProfileAsync_ShouldReturnProfile_WhenAdminReadsClientProfile | PASS |
| Neovlašten korisnik pokuša čitati tuđi profil | Sistem odbija pristup | AdminUserProfileServiceTests.GetUserProfileAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherProfile | PASS |

---

### US-67 - Pregled historije tiketa korisnika

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Agent/ovlašteni korisnik pregleda detalje tiketa korisnika | Detalji tiketa mapiraju sva polja u DTO | TicketDetailServiceTests.GetTicketByIdAsync_ShouldMapAllFieldsToDto | PASS |
| Lista tiketa prikazuje relevantne dodijeljene i filtrirane tikete | UI i repozitorij vraćaju odgovarajuće tikete za pregled historije | Tickets.test.jsx, TicketDetail.test.jsx, AllTicketsRepositoryTests | PASS |

---

### US-68 - Prikaz paketa i pretplata

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Sistem ima DTO modele za prikaz paketa | PackageSummaryDto i PackageDetailDto sadrže podatke paketa i pretplate | PackageSummaryDto.cs, PackageDetailDto.cs | PASS |
| Profil korisnika može dohvatiti pakete | User profile servis koristi IPackageService.GetMyPackagesAsync | AdminUserProfileServiceTests setup za IPackageService.GetMyPackagesAsync | PASS |
| Baza podržava datume i status pretplate | Migracija dodaje datumska polja na SubscriptionPackages | 20260519120000_AddPackageDates.cs | PASS |

---

### US-69 - Sistemske poruke u chatu pri prosljeđivanju tiketa

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Sistemska/chat poruka se kreira kroz comment servis | Komentar se dodaje i povezuje s tiketom uz validaciju pristupa | CommentServiceTests.AddCommentAsync_ShouldSucceed_WhenClientIsOwnerAndContentValid | PASS |
| Prazan komentar se odbija | Controller vraća BadRequest za prazan sadržaj | CommentControllerTests.AddComment_ShouldReturnBadRequest_WhenContentIsEmpty | PASS |
| Chat poruke se dohvaćaju samo za ovlaštenog korisnika | Sistem vraća komentare vlasniku, a odbija neovlaštenog korisnika | CommentServiceTests.GetCommentsForTicketAsync_ShouldReturnComments_WhenClientIsOwner, CommentServiceTests.GetCommentsForTicketAsync_ShouldThrowUnauthorized_WhenClientNotOwner | PASS |
| Real-time broadcast koristi ticket grupu | SignalR grupa prima konekciju za odgovarajući tiket | ChatHubTests.JoinTicketGroup_ShouldAddConnectionToGroup | PASS |
| Frontend communication tok pokriva prosljeđivanje i chat | Acceptance/system testovi pokrivaju workflow komunikacije | CommunicationAcceptance.test.jsx, CommunicationSystem.test.jsx | PASS |

---

### US-70 - Zajednička dodjela agentu i tehničaru

| Scenario | Očekivani rezultat | Test dokaz | Status |
|---|---|---|---|
| Agent proslijedi tiket tehničaru | Sistem tretira i agenta i tehničara kao aktivne assigneeje | AllTicketsRepositoryTests.GetByAssigneeIdAsync_ShouldKeepAgentAndTechnicianAssigned_WhenForwardedToTechnician | PASS |
| Agent otvori dodijeljene tikete nakon što je status "Čeka se" | Tiket se prikazuje u listi dodijeljenih tiketa | TicketsAssignmentStatusUi.test.jsx | PASS |
| Detalji tiketa nakon prosljeđivanja tehničaru | UI prikazuje Agent: ... i ispod Tehničar: ... | TicketDetail.test.jsx i TicketDetailServiceTests.GetTicketByIdAsync_ShouldMapAllFieldsToDto | PASS |
| Lista dodijeljenih tiketa | Default filter je Svi statusi, ne samo OPEN | Tickets.test.jsx, TicketsAssignmentStatusUi.test.jsx | PASS |
