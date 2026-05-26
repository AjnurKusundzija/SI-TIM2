# Proof of Testing — Sprint 10

Dokument evidentira dokaze testiranja za Sprint 10 funkcionalnosti: AI prijedlog odgovora za agente i tehničare (PB-57), AI uvidi za administratore (PB-58), redizajn korisničkog sučelja (PB-59) i proširenje administratorskog prosljeđivanja tiketa (PB-31/US-101).

---

## Sažetak testiranja

| Datum | Opseg | Tip testiranja | Rezultat |
|---|---|---|---|
| [PLACEHOLDER] | PB-57 AI prijedlog odgovora | Backend unit + frontend unit | [PLACEHOLDER] |
| [PLACEHOLDER] | PB-58 AI uvidi za administratore | Backend unit + frontend unit | [PLACEHOLDER] |
| [PLACEHOLDER] | PB-59 Redizajn korisničkog sučelja | Frontend unit + manualno | [PLACEHOLDER] |
| [PLACEHOLDER] | PB-31/US-101 Admin preraspodjela tiketa | Manualno (UI) | [PLACEHOLDER] |

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
| Backend unit (PB-57) | [PLACEHOLDER] | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit (PB-57) | [PLACEHOLDER] | [PLACEHOLDER] | [PLACEHOLDER] |
| Backend unit (PB-58) | [PLACEHOLDER] | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit (PB-58) | [PLACEHOLDER] | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit (PB-59) | [PLACEHOLDER] | [PLACEHOLDER] | [PLACEHOLDER] |
| Manualno (PB-31/US-101) | TicketDetail admin preraspodjela | — | [PLACEHOLDER] |
| **Ukupno Sprint 10** | | **[PLACEHOLDER]** | **[PLACEHOLDER]** |

---

## PB-57 AI prijedlog odgovora — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-96 | AI prijedlog na osnovu tipa problema | [PLACEHOLDER] | [PLACEHOLDER] |
| Backend unit | US-96 | Knowledge base za 6 kategorija | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit | US-97 | AISuggestionModal — prikaz i kopiranje | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit | US-96 | Dugme vidljivo samo za AGENT i TECHNICIAN | [PLACEHOLDER] | [PLACEHOLDER] |

### Test fajlovi

- Backend: [PLACEHOLDER]
- Frontend: [PLACEHOLDER]

### Izvršene komande

```
[PLACEHOLDER]
```

---

## PB-58 AI uvidi za administratore — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Backend unit | US-98 | Admin insights generisanje | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit | US-98 | AIInsightsPanel prikaz | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit | US-99 | Otvaranje/zatvaranje panela | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit | US-98 | AI Uvidi dugme vidljivo samo adminu | [PLACEHOLDER] | [PLACEHOLDER] |

### Test fajlovi

- Backend: [PLACEHOLDER]
- Frontend: [PLACEHOLDER]

### Izvršene komande

```
[PLACEHOLDER]
```

---

## PB-59 Redizajn korisničkog sučelja — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Frontend unit | US-100 | Sidebar navigacija i aktivne stavke | [PLACEHOLDER] | [PLACEHOLDER] |
| Frontend unit | US-100 | Status chip — amber/zeleni | [PLACEHOLDER] | [PLACEHOLDER] |
| Manualno | US-100 | Vizualna konzistentnost kroz sve stranice | Manualni pregled | [PLACEHOLDER] |
| Manualno | US-100 | Alert banner klikabilnost | Manualni pregled | [PLACEHOLDER] |

### Test fajlovi

- Frontend: [PLACEHOLDER]

### Izvršene komande

```
[PLACEHOLDER]
```

---

## PB-31/US-101 Admin preraspodjela tiketa — detalji testiranja

### Pokriveni Acceptance Criteria

| Nivo | US | AC fokus | Test koji pokriva | Status |
|---|---|---|---|---|
| Manualno | US-101 | Dugme „Prerasporedi agenta/tehničara" vidljivo adminu | Manualni pregled TicketDetail | [PLACEHOLDER] |
| Manualno | US-101 | Admin nije blokiran assignedAgentId provjerom | Manualni pregled | [PLACEHOLDER] |
| Manualno | US-101 | Admin ne može slati poruke u chat | Manualni pregled | [PLACEHOLDER] |
| Manualno | US-101 | Dugme ne prikazuje se za zatvorene tikete | Manualni pregled | [PLACEHOLDER] |

---

## Veza sa Test Strategijom

| Kategorija | Status Sprint 10 |
|---|---|
| Unit testovi (backend) | [PLACEHOLDER] |
| Integration testovi (backend) | [PLACEHOLDER] |
| Unit testovi (frontend) | [PLACEHOLDER] |
| Sistemski testovi | [PLACEHOLDER] |
| Manualni testovi (UI/UX) | [PLACEHOLDER] |

---

**Napomena:** Ovaj dokument se popunjava po završetku testiranja. Placeholderi trebaju biti zamijenjeni stvarnim podacima (datumi, nazivi test fajlova, broj testova, rezultati) prije finalnog sprint review-a.
