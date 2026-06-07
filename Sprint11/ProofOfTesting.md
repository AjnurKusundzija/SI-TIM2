# Proof of Testing — Sprint 11

Dokument evidentira dokaze testiranja za Sprint 11 funkcionalnosti: Export izvještaja u CSV formatu (PB-46/US-112).

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| 06.06.2026 | PB-46 Export izvještaja | Frontend unit | Prošlo |

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
| **Frontend test suite** | `Project/frontend` | 17 (relevantni za PB-46) | 17 prošlo |

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

## Veza sa Test Strategijom

| Kategorija | Status Sprint 11 |
|---|---|
| Unit testovi (backend) | N/A za PB-46 — nema backend promjena |
| Integration testovi (backend) | N/A za PB-46 — nema backend promjena |
| Unit testovi (frontend) | Pokriveno — `Sprint9AdminDashboard.test.jsx` (17 testova, svi prolaze) |
| Sistemski testovi | N/A za PB-46 |
| Manualni testovi (UI/UX) | Export dugme aktivno, CSV preuzimanje verificirano |

---
