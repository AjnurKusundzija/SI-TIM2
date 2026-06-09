# Proof of Testing — Sprint 11

Dokument evidentira dokaze testiranja za Sprint 11 funkcionalnosti: Export izvještaja u CSV formatu (PB-46/US-112), Login via broj telefona (PB-67/US-119) i SLA praćenje i upozorenja (PB-65/US-115, US-116).

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 06.06.2026 | PB-46 Export izvještaja | Frontend unit | Prošlo |
| 08.06.2026 | PB-67 Login via broj telefona | Frontend unit + Backend unit | Prošlo |
| 08.06.2026 | PB-65 SLA praćenje i upozorenja | Frontend unit + Backend unit | Prošlo |

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
| **Frontend unit (PB-46)** | `Sprint9AdminDashboard.test.jsx` | **17** | **Prošlo** |
| **Frontend unit (PB-67)** | `Login.test.jsx` | **8** | **Prošlo** |
| **Backend unit (PB-67)** | `AuthServiceTests.cs` | **3 nova** (ukupno 21) | **Prošlo** |
| **Backend unit (PB-67)** | `EmailOrBiHPhoneAttributeTests.cs` | **9** | **Prošlo** |
| **Backend Auth suite** | `TelecomSupportSystem.Tests` (filter: Auth) | **103** | **103 prošlo** |
| **Frontend unit (PB-65)** | `SlaIndicator.test.jsx` | **8** | **Prošlo** |
| **Backend unit (PB-65)** | `SlaServiceTests.cs` | **12** | **Prošlo** |

---

## PB-46 Export izvještaja — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Frontend unit | US-112 | Export dugme je aktivno (nije disabled) | `Sprint9AdminDashboard.test.jsx — Export dugme je aktivno i pokreće preuzimanje CSV-a (US-85)` | Prošlo |
| Frontend unit | US-112 | Klik na Export poziva generateReport | `Sprint9AdminDashboard.test.jsx — Export dugme je aktivno i pokreće preuzimanje CSV-a (US-85)` | Prošlo |
| Frontend unit | US-112 | Reports mod prikazuje sve report type chipove | `Sprint9AdminDashboard.test.jsx — podržani svi report tipovi prikazani kao chip dugmad` | Prošlo |
| Frontend unit | US-112 | Reports mod ne poziva dashboard endpoint | `Sprint9AdminDashboard.test.jsx — reports mod NE prikazuje KPI kartice i NE poziva dashboard endpoint` | Prošlo |
| Frontend unit | US-112 | Export dugme nije vidljivo u metrics modu | `Sprint9AdminDashboard.test.jsx — NE prikazuje generisanje izvještaja u metrics modu` | Prošlo |

### Test fajlovi

- Frontend: `frontend/src/test/Sprint9AdminDashboard.test.jsx`

### Izvršene komande

```
npx vitest run src/test/Sprint9AdminDashboard.test.jsx
```

### Rezultat izvršavanja

```
 ✓ src/test/Sprint9AdminDashboard.test.jsx  (17 tests) 1897ms

 Test Files  1 passed (1)
      Tests  17 passed (17)
   Start at  15:45:41
   Duration  8.54s
```

### Izmjene u test fajlu

| Izmjena | Razlog |
|---|---|
| Test `Export dugme postoji ali je disabled (US-85)` preimenovan u `Export dugme je aktivno i pokreće preuzimanje CSV-a (US-85)` | Reflektuje novo ponašanje dugmeta (aktivan umjesto disabled) |
| `expect(exportBtn).toBeDisabled()` zamijenjeno sa `expect(exportBtn).not.toBeDisabled()` | Dugme je sada uvijek aktivno |
| Dodan `fireEvent.click(exportBtn)` i `await waitFor(() => expect(mocks.generateReport).toHaveBeenCalled())` | Verificira da klik trigeruje API poziv |
| Dodan `global.URL.createObjectURL = vi.fn(() => 'blob:mock')` u `beforeEach` | jsdom ne implementira URL.createObjectURL; mock je neophodan da se izbjegne `TypeError` tokom testa |
| Dodan `global.URL.revokeObjectURL = vi.fn()` u `beforeEach` | Komplementarni mock uz createObjectURL za čišćenje blob URL-a |

---

---

## PB-67 Login via broj telefona — detalji testiranja

### Pokriveni Acceptance Criteria (US-119)

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Frontend unit | US-119 | Polje naznačava email ili telefon (label) | `Login.test.jsx — label indicates email or phone number can be entered` | Prošlo |
| Frontend unit | US-119 | Placeholder prikazuje +387 format | `Login.test.jsx — placeholder shows +387 phone number format` | Prošlo |
| Frontend unit | US-119 | Prijava sa +387 brojem poziva login | `Login.test.jsx — calls login with phone number when +387 format is submitted` | Prošlo |
| Frontend unit | US-119 | Email login i dalje radi | `Login.test.jsx — still allows login with email address` | Prošlo |
| Backend unit | US-119 | AuthService ruta telefon → GetByPhoneAsync | `AuthServiceTests — LoginAsync_ValidPhoneNumber_UsesGetByPhoneAsync` | Prošlo |
| Backend unit | US-119 | Nepostojeći broj telefona vraća null | `AuthServiceTests — LoginAsync_PhoneNumberNotFound_ReturnsNull` | Prošlo |
| Backend unit | US-119 | Email identifikator koristi GetByEmailAsync | `AuthServiceTests — LoginAsync_EmailIdentifier_UsesGetByEmailAsync_NotPhone` | Prošlo |
| Backend unit | US-119 | Ispravni email formati prolaze | `EmailOrBiHPhoneAttributeTests — IsValid_ValidEmail_ReturnsSuccess` | Prošlo |
| Backend unit | US-119 | Ispravni +387 telefoni prolaze | `EmailOrBiHPhoneAttributeTests — IsValid_ValidBiHPhone_ReturnsSuccess` | Prošlo |
| Backend unit | US-119 | Telefon bez +387 prefiksa odbijen | `EmailOrBiHPhoneAttributeTests — IsValid_PhoneWithoutPlus387Prefix_ReturnsError` | Prošlo |
| Backend unit | US-119 | Prekratak broj odbijen | `EmailOrBiHPhoneAttributeTests — IsValid_TooShortPhoneNumber_ReturnsError` | Prošlo |
| Backend unit | US-119 | Neispravan email format odbijen | `EmailOrBiHPhoneAttributeTests — IsValid_InvalidEmailFormat_ReturnsError` | Prošlo |

### Test fajlovi

- Frontend: `frontend/src/test/Login.test.jsx`
- Backend: `TelecomSupportSystem.Tests/Auth/AuthServiceTests.cs`
- Backend (novi): `TelecomSupportSystem.Tests/Auth/EmailOrBiHPhoneAttributeTests.cs`

### Izvršene komande

```
npx vitest run src/test/Login.test.jsx
dotnet test TelecomSupportSystem.Tests --filter "FullyQualifiedName~Auth"
```

### Rezultat izvršavanja

```
 ✓ src/test/Login.test.jsx  (8 tests) 915ms

 Test Files  1 passed (1)
      Tests  8 passed (8)
   Start at  14:33:54
   Duration  8.33s

Passed!  - Failed: 0, Passed: 103, Skipped: 0, Total: 103, Duration: 11 s
```

---

---

## PB-65 SLA praćenje i upozorenja — detalji testiranja

### Pokriveni Acceptance Criteria (US-115, US-116)

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-115 | CRITICAL rok = 2h | `SlaServiceTests — GetSlaInfo_CriticalPriority_DeadlineIs2HoursFromCreation` | Prošlo |
| Backend unit | US-115 | HIGH rok = 8h | `SlaServiceTests — GetSlaInfo_HighPriority_DeadlineIs8HoursFromCreation` | Prošlo |
| Backend unit | US-115 | MEDIUM rok = 24h | `SlaServiceTests — GetSlaInfo_MediumPriority_DeadlineIs24HoursFromCreation` | Prošlo |
| Backend unit | US-115 | LOW rok = 72h | `SlaServiceTests — GetSlaInfo_LowPriority_DeadlineIs72HoursFromCreation` | Prošlo |
| Backend unit | US-115 | > 50% preostalo → GREEN | `SlaServiceTests — GetSlaInfo_MoreThan50PercentRemaining_ReturnsGreen` | Prošlo |
| Backend unit | US-115 | 20–50% → YELLOW | `SlaServiceTests — GetSlaInfo_Between20And50PercentRemaining_ReturnsYellow` | Prošlo |
| Backend unit | US-115 | < 20% → RED | `SlaServiceTests — GetSlaInfo_LessThan20PercentRemaining_ReturnsRed` | Prošlo |
| Backend unit | US-115 | Prekoračenje → RED + IsBreached | `SlaServiceTests — GetSlaInfo_DeadlinePassed_ReturnsRedAndIsBreachedTrue` | Prošlo |
| Backend unit | US-116 | Zatvoreni tiket s breachom se ne broji | `SlaServiceTests — CountBreaches_ClosedTicketWithBreachedSla_NotCounted` | Prošlo |
| Backend unit | US-116 | Otvoreni tiket s breachom se broji | `SlaServiceTests — CountBreaches_OpenTicketWithBreachedSla_CountedOnce` | Prošlo |
| Backend unit | US-116 | Mješavina tiketa — broji samo otvorene s breachom | `SlaServiceTests — CountBreaches_MixedTickets_CountsOnlyOpenBreached` | Prošlo |
| Frontend unit | US-115 | Bez statusa → tihi placeholder (zatvoreni tiketi) | `SlaIndicator.test.jsx — renders dash placeholder when slaStatus is not provided` | Prošlo |
| Frontend unit | US-115 | GREEN status prikazuje preostalo vrijeme | `SlaIndicator.test.jsx — shows remaining time when SLA is not breached` | Prošlo |
| Frontend unit | US-115 | RED + breach → "SLA prekoračen" | `SlaIndicator.test.jsx — shows breach label when slaIsBreached is true` | Prošlo |
| Frontend unit | US-115 | Boja-kodiranje YELLOW | `SlaIndicator.test.jsx — shows remaining time for YELLOW status` | Prošlo |

### Test fajlovi

- Frontend (novi): `frontend/src/test/SlaIndicator.test.jsx`
- Backend (novi): `TelecomSupportSystem.Tests/Services/SlaServiceTests.cs`

### Izvršene komande

```
npx vitest run src/test/SlaIndicator.test.jsx
dotnet test TelecomSupportSystem.Tests --filter "FullyQualifiedName~SlaServiceTests"
```

### Rezultat izvršavanja

```
 ✓ src/test/SlaIndicator.test.jsx  (8 tests) 74ms

 Test Files  1 passed (1)
      Tests  8 passed (8)
   Start at  14:38:09
   Duration  8.92s

Passed!  - Failed: 0, Passed: 12, Skipped: 0, Total: 12, Duration: 65 ms
```

---

## Veza sa Test Strategijom

| Kategorija | Status Sprint 11 |
|---|---|
| Unit testovi (backend) | Pokriveno — `AuthServiceTests.cs` (3 nova, US-119) + `EmailOrBiHPhoneAttributeTests.cs` (9, US-119) + `SlaServiceTests.cs` (12, US-115/116) |
| Integration testovi (backend) | N/A za PB-46, PB-67, PB-65 |
| Unit testovi (frontend) | Pokriveno — `Sprint9AdminDashboard.test.jsx` (17, PB-46) + `Login.test.jsx` (8, PB-67) + `SlaIndicator.test.jsx` (8, US-115) |
| Sistemski testovi | N/A za PB-46, PB-67, PB-65 |
| Manualni testovi (UI/UX) | Export dugme aktivno, CSV preuzimanje verificirano; Login forma prihvata email i +387 broj; SLA indikatori vidljivi na tiketima |

---
