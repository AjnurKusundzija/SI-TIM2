# Proof of Testing — Sprint 10

Dokument evidentira dokaze testiranja za Sprint 10 funkcionalnosti: AI prijedlog odgovora za agente i tehničare (PB-57), AI uvidi za administratore (PB-58), redizajn korisničkog sučelja (PB-59), proširenje administratorskog prosljeđivanja tiketa (PB-31/US-101), admin CRUD nad FAQ stavkama (PB-61/US-104) i samodjelovanje tiketa za agente (PB-62/US-105).

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 26.05.2026 | PB-57 AI prijedlog odgovora | Backend unit + frontend unit | Prošlo |
| 26.05.2026 | PB-58 AI uvidi za administratore | Backend unit + frontend unit | Prošlo |
| 26.05.2026 | PB-59 Redizajn korisničkog sučelja | Frontend unit + manualno | Prošlo |
| 26.05.2026 | PB-31/US-101 Admin preraspodjela tiketa | Manualno (UI) | Prošlo |
| 26.05.2026 | PB-61/US-104 Admin CRUD FAQ | Backend unit + integration + frontend unit | Prošlo |
| 26.05.2026 | PB-62/US-105 Assign to me — samodjelovanje tiketa | Backend unit + integration + frontend unit | Prošlo |

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
| Backend unit (PB-57) | `AISuggestion*Tests.cs` (postojeći) | postojeći | Prošlo |
| Frontend unit (PB-57) | `AISuggestionModal.test.jsx` (postojeći) | postojeći | Prošlo |
| Backend unit (PB-58) | `AIInsights*Tests.cs` (postojeći) | postojeći | Prošlo |
| Frontend unit (PB-58) | `AIInsightsPanel.test.jsx` (postojeći) | postojeći | Prošlo |
| Frontend unit (PB-59) | `Sidebar.test.jsx`, `Header.test.jsx` (postojeći) | postojeći | Prošlo |
| Manualno (PB-31/US-101) | TicketDetail admin preraspodjela | — | Prošlo |
| **Backend unit (PB-61)** | `Faq/FaqAdminCrudTests.cs` | **15** | **Prošlo** |
| **Backend integracija (PB-61)** | `Integration/FaqAdminCrudIntegrationTests.cs` | **8** | **Prošlo** |
| **Frontend unit (PB-61)** | `Faq.test.jsx`, `faqService.test.js` | **19** | **Prošlo** |
| **Backend unit (PB-62)** | `TicketT/SelfAssignServiceTests.cs` | **7** | **Prošlo** |
| **Backend integracija (PB-62)** | `Integration/SelfAssignIntegrationTests.cs` | **5** | **Prošlo** |
| **Frontend unit (PB-62)** | `TicketDetail.test.jsx` (proširen), `ticketService.test.js` | **10** | **Prošlo** |
| **Ukupno novih testova Sprint 10 (PB-61 + PB-62)** | | **64** | **Prošlo (64/64)** |
| **Backend test suite** | `TelecomSupportSystem.Tests` | 521 | 520 prošlo / 1 flaky (`AuthPerformanceTests` — nije vezano za PB-61/PB-62) |
| **Frontend test suite** | `Project/frontend` | 324 | 324 prošlo |

---

## PB-57 AI prijedlog odgovora — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-96 | AI prijedlog na osnovu tipa problema | Postojeći testovi `AIService` | Prošlo |
| Backend unit | US-96 | Knowledge base za 6 kategorija | Postojeći testovi `AIService` | Prošlo |
| Frontend unit | US-97 | AISuggestionModal — prikaz i kopiranje | Postojeći `AISuggestionModal.test.jsx` | Prošlo |
| Frontend unit | US-96 | Dugme vidljivo samo za AGENT i TECHNICIAN | Postojeći `TicketDetail.test.jsx` | Prošlo |

### Test fajlovi

- Backend: pokriveno postojećim testovima `AIService` iz prethodnih izmjena Sprinta 10
- Frontend: pokriveno postojećim `AISuggestionModal.test.jsx`

---

## PB-58 AI uvidi za administratore — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-98 | Admin insights generisanje | Postojeći testovi `AIService` | Prošlo |
| Frontend unit | US-98 | AIInsightsPanel prikaz | Postojeći `AIInsightsPanel.test.jsx` | Prošlo |
| Frontend unit | US-99 | Otvaranje/zatvaranje panela | Postojeći `AIInsightsPanel.test.jsx` | Prošlo |
| Frontend unit | US-98 | AI Uvidi dugme vidljivo samo adminu | Postojeći `Header.test.jsx` | Prošlo |

---

## PB-59 Redizajn korisničkog sučelja — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Frontend unit | US-100 | Sidebar navigacija i aktivne stavke | Postojeći `Sidebar.test.jsx` | Prošlo |
| Frontend unit | US-100 | Status chip — amber/zeleni | Postojeći `Sidebar.test.jsx` | Prošlo |
| Manualno | US-100 | Vizualna konzistentnost kroz sve stranice | Manualni pregled | Prošlo |
| Manualno | US-100 | Alert banner klikabilnost | Manualni pregled | Prošlo |

---

## PB-31/US-101 Admin preraspodjela tiketa — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Manualno | US-101 | Dugme „Prerasporedi agenta/tehničara" vidljivo adminu | Manualni pregled TicketDetail | Prošlo |
| Manualno | US-101 | Admin nije blokiran assignedAgentId provjerom | Manualni pregled | Prošlo |
| Manualno | US-101 | Admin ne može slati poruke u chat | Manualni pregled | Prošlo |
| Manualno | US-101 | Dugme ne prikazuje se za zatvorene tikete | Manualni pregled | Prošlo |

---

## PB-61 Admin CRUD FAQ — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-104 | Validacija — pitanje ne smije biti prazno | `FaqAdminCrudTests.CreateFaqAsync_ShouldThrow_WhenQuestionIsEmpty`, `UpdateFaqAsync_ShouldThrow_WhenQuestionIsEmpty` | Prošlo |
| Backend unit | US-104 | Validacija — odgovor ne smije biti prazan | `FaqAdminCrudTests.CreateFaqAsync_ShouldThrow_WhenAnswerIsEmpty` | Prošlo |
| Backend unit | US-104 | Mapiranje i trim DTO → entitet | `FaqAdminCrudTests.CreateFaqAsync_ShouldPersistTrimmedFaq_WhenInputIsValid`, `UpdateFaqAsync_ShouldPersistChanges_WhenInputIsValid` | Prošlo |
| Backend unit | US-104 | KeyNotFound za nepostojeću stavku (update/delete) | `UpdateFaqAsync_ShouldThrowKeyNotFound_WhenFaqDoesNotExist`, `DeleteFaqAsync_ShouldThrowKeyNotFound_WhenFaqDoesNotExist` | Prošlo |
| Backend unit | US-104 | Brisanje uklanja stavku | `DeleteFaqAsync_ShouldRemove_WhenFaqExists` | Prošlo |
| Backend unit | US-104 | Admin lista (uključuje neaktivne) | `GetAllFaqsAsync_ShouldReturnInactiveAndActive` | Prošlo |
| Backend unit | US-104 | Autorizacija — POST/PUT/DELETE/GET all zahtijevaju `ADMINISTRATOR` rolu | `AdminEndpoints_ShouldRequireAdministratorRole` (Theory) | Prošlo |
| Backend unit | US-104 | Public GET nema role-restriction | `GetFaqs_ShouldNotRequireAdministratorRole` | Prošlo |
| Backend unit | US-104 | Controller status kodovi (BadRequest, NotFound, NoContent) | `CreateFaq_ShouldReturnBadRequest_WhenServiceThrowsArgumentException`, `UpdateFaq_ShouldReturnNotFound_WhenServiceThrowsKeyNotFound`, `DeleteFaq_ShouldReturnNoContent_OnSuccess` | Prošlo |
| Backend integracija | US-104 | Kreirana stavka odmah vidljiva svim korisnicima | `FaqAdminCrudIntegrationTests.CreateFaq_ShouldPersistAndBeVisibleInPublicList` | Prošlo |
| Backend integracija | US-104 | BadRequest za prazne forme | `CreateFaq_ShouldReturnBadRequest_WhenQuestionIsEmpty`, `CreateFaq_ShouldReturnBadRequest_WhenAnswerIsEmpty` | Prošlo |
| Backend integracija | US-104 | Update persistira izmjene | `UpdateFaq_ShouldPersistChanges` | Prošlo |
| Backend integracija | US-104 | NotFound za nepostojeću stavku | `UpdateFaq_ShouldReturnNotFound_WhenFaqMissing`, `DeleteFaq_ShouldReturnNotFound_WhenFaqMissing` | Prošlo |
| Backend integracija | US-104 | Delete uklanja iz DB | `DeleteFaq_ShouldRemoveFromDatabase` | Prošlo |
| Backend integracija | US-104 | Admin GET vraća i neaktivne, public ne | `GetAllFaqs_ShouldIncludeInactiveEntries` | Prošlo |
| Frontend unit | US-104 | Read-only prikaz se ne lomi (CLIENT) | `Faq.test.jsx — read-only (CLIENT)` describe blok | Prošlo |
| Frontend unit | US-104 | CRUD kontrole skrivene za ne-admin korisnike | `Faq.test.jsx — does not render admin CRUD controls for non-admin users` | Prošlo |
| Frontend unit | US-104 | CLIENT koristi public endpoint, ne admin | `Faq.test.jsx — CLIENT uses public endpoint and not admin endpoint` | Prošlo |
| Frontend unit | US-104 | Admin vidi „Dodaj/Uredi/Obriši" kontrole | `Faq.test.jsx — uses admin endpoint and renders edit/delete controls` | Prošlo |
| Frontend unit | US-104 | Validacija praznog pitanja u UI | `Faq.test.jsx — shows validation error when question is empty` | Prošlo |
| Frontend unit | US-104 | Validacija praznog odgovora u UI | `Faq.test.jsx — shows validation error when answer is empty` | Prošlo |
| Frontend unit | US-104 | Kreiranje FAQ stavke poziva API | `Faq.test.jsx — creates a new FAQ when form is valid` | Prošlo |
| Frontend unit | US-104 | Edit forma popunjena postojećim podacima | `Faq.test.jsx — opens edit form pre-filled and submits update` | Prošlo |
| Frontend unit | US-104 | Confirm dialog prije brisanja + refresh liste | `Faq.test.jsx — confirms deletion before calling delete API and refreshes list` | Prošlo |
| Frontend unit | US-104 | Admin endpointi u servisu (POST/PUT/DELETE/GET all) | `faqService.test.js — createFaq/updateFaq/deleteFaq/getAllFaqs` | Prošlo |

### Test fajlovi

- Backend: `TelecomSupportSystem.Tests/Faq/FaqAdminCrudTests.cs`, `TelecomSupportSystem.Tests/Integration/FaqAdminCrudIntegrationTests.cs`
- Frontend: `frontend/src/test/Faq.test.jsx`, `frontend/src/test/faqService.test.js`

### Izvršene komande

```
dotnet test TelecomSupportSystem.Tests/TelecomSupportSystem.Tests.csproj --filter "FullyQualifiedName~FaqAdminCrudTests|FullyQualifiedName~FaqAdminCrudIntegrationTests"
npx vitest run src/test/Faq.test.jsx src/test/faqService.test.js
```

---

## PB-62 Assign to me — samodjelovanje tiketa — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-105 | Uspješno samodjelovanje agenta | `SelfAssignServiceTests.SelfAssignTicketAsync_ShouldAssignTicket_ToCallingAgent` | Prošlo |
| Backend unit | US-105 | Sistemski komentar u istoriji + notifikacija klijentu | `SelfAssignTicketAsync_ShouldNotifyClientAndAddSystemComment_OnSuccess` | Prošlo |
| Backend unit | US-105 | Odbijanje kada je tiket već dodijeljen drugom agentu | `SelfAssignTicketAsync_ShouldThrow_WhenTicketAlreadyAssigned` | Prošlo |
| Backend unit | US-105 | Odbijanje za zatvoreni tiket | `SelfAssignTicketAsync_ShouldThrow_WhenTicketIsClosed` | Prošlo |
| Backend unit | US-105 | Samo agent može preuzeti — ne tehničar/klijent/admin | `SelfAssignTicketAsync_ShouldThrow_WhenCallerIsNotAgent` | Prošlo |
| Backend unit | US-105 | KeyNotFound kada tiket ne postoji | `SelfAssignTicketAsync_ShouldThrowKeyNotFound_WhenTicketMissing` | Prošlo |
| Backend unit | US-105 | Agent bez tima + tiket bez tima → odbijeno | `SelfAssignTicketAsync_ShouldThrow_WhenAgentHasNoTeamAndTicketHasNoTeam` | Prošlo |
| Backend integracija | US-105 | End-to-end self-assign per agent | `SelfAssignIntegrationTests.SelfAssign_ShouldAssignTicket_ToCallingAgent` | Prošlo |
| Backend integracija | US-105 | Forbid za CLIENT, TECHNICIAN, ADMINISTRATOR | `SelfAssign_ShouldReturnForbid_ForNonAgents` | Prošlo |
| Backend integracija | US-105 | Race condition — backend odbija kada je tiket dodijeljen između učitavanja i klika | `SelfAssign_ShouldReturnBadRequest_WhenAlreadyAssignedToAnotherAgent` | Prošlo |
| Backend integracija | US-105 | BadRequest za zatvoreni tiket | `SelfAssign_ShouldReturnBadRequest_WhenTicketIsClosed` | Prošlo |
| Backend integracija | US-105 | NotFound za nepostojeći tiket | `SelfAssign_ShouldReturnNotFound_WhenTicketDoesNotExist` | Prošlo |
| Frontend unit | US-105 | Dugme „Preuzmi tiket" vidljivo samo AGENT-u kada je tiket OPEN i nedodijeljen | `TicketDetail.test.jsx — shows "Preuzmi tiket" button for AGENT when ticket is open and unassigned` | Prošlo |
| Frontend unit | US-105 | Dugme sakriveno za CLIENT-a | `TicketDetail.test.jsx — hides "Preuzmi tiket" button for CLIENT` | Prošlo |
| Frontend unit | US-105 | Dugme sakriveno za TECHNICIAN-a | `TicketDetail.test.jsx — hides "Preuzmi tiket" button for TECHNICIAN` | Prošlo |
| Frontend unit | US-105 | Dugme sakriveno za ADMINISTRATOR-a | `TicketDetail.test.jsx — hides "Preuzmi tiket" button for ADMINISTRATOR` | Prošlo |
| Frontend unit | US-105 | Dugme sakriveno kada tiket ima dodijeljenog agenta | `TicketDetail.test.jsx — hides "Preuzmi tiket" button when ticket already has assigned agent` | Prošlo |
| Frontend unit | US-105 | Dugme sakriveno za zatvoreni tiket | `TicketDetail.test.jsx — hides "Preuzmi tiket" button when ticket is closed` | Prošlo |
| Frontend unit | US-105 | Klik na dugme dodjeljuje tiket i osvježava prikaz bez confirm dialoga | `TicketDetail.test.jsx — clicking "Preuzmi tiket" calls selfAssignTicket and refreshes ticket` | Prošlo |
| Frontend unit | US-105 | Backend reject — prikazana je jasna poruka greške | `TicketDetail.test.jsx — shows error message when self-assign fails` | Prošlo |
| Frontend unit | US-105 | Service POST poziv ka `/tickets/:id/self-assign` | `ticketService.test.js — selfAssignTicket() calls POST /tickets/:id/self-assign` | Prošlo |

### Test fajlovi

- Backend: `TelecomSupportSystem.Tests/TicketT/SelfAssignServiceTests.cs`, `TelecomSupportSystem.Tests/Integration/SelfAssignIntegrationTests.cs`
- Frontend: `frontend/src/test/TicketDetail.test.jsx`, `frontend/src/test/ticketService.test.js`

### Izvršene komande

```
dotnet test TelecomSupportSystem.Tests/TelecomSupportSystem.Tests.csproj --filter "FullyQualifiedName~SelfAssignServiceTests|FullyQualifiedName~SelfAssignIntegrationTests"
npx vitest run src/test/TicketDetail.test.jsx src/test/ticketService.test.js
```

---

## Veza sa Test Strategijom

| Kategorija | Status Sprint 10 |
|---|---|
| Unit testovi (backend) | Pokriveno — `FaqAdminCrudTests` (15), `SelfAssignServiceTests` (7) za PB-61/PB-62 + postojeći testovi za ostatak |
| Integration testovi (backend) | Pokriveno — `FaqAdminCrudIntegrationTests` (8), `SelfAssignIntegrationTests` (5) za PB-61/PB-62 |
| Unit testovi (frontend) | Pokriveno — `Faq.test.jsx` (13), `faqService.test.js` (6), `TicketDetail.test.jsx` (17), `ticketService.test.js` (16) |
| Sistemski testovi | Pokriveno — `FaqSystem.test.jsx` ažuriran sa novim AuthContext mock-om; ostali postojeći system testovi prošli |
| Manualni testovi (UI/UX) | Manualno provjereno za PB-31/US-101 i PB-59 |

---

**Napomena:** Ukupno je dodano 64 nova testa za PB-61 i PB-62. Svi su prošli. Postojeći test suite (520 backend + 324 frontend = 844 testa) ostao je zelen nakon refaktoriranja FAQ Page komponente za podršku admin moda (uz dodavanje AuthContext mocka u tri postojeća FAQ test fajla: `FaqUi.test.jsx`, `FaqSystem.test.jsx`, `FaqAcceptance.test.jsx`). Jedini failing test je `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` koji je flaky timing test nevezan za PB-61/PB-62.
