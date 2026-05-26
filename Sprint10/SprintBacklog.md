# Sprint Backlog – Sprint 10

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati AI-potpomognute funkcionalnosti sistema kroz modul prijedloga odgovora za agente i tehničare te modul AI uvida za administratore, izvršiti kompletni redizajn korisničkog sučelja radi poboljšanja korisničkog iskustva, te proširiti administratorske ovlasti nad tiketima omogućavanjem preraspodjele agenata i tehničara iz prikaza detalja tiketa.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-57 AI prijedlog odgovora za agente i tehničare | US-96, US-97 | Uma | Done | `AISuggestionModal` u `TicketDetail`; backend `AIService.GetAgentSuggestionAsync` s knowledge base |
| SB-02 | PB-58 AI uvidi za administratore | US-98, US-99 | Uma | Done | `AIInsightsPanel`, `AIInsightsCard` na admin dashboardu; backend `AIService.GetAdminInsightsAsync`; AI Uvidi dugme u Headeru |
| SB-03 | PB-59 Redizajn korisničkog sučelja | US-100 | Uma | Done | Kompletni revamp `Sidebar`, `Header`, `AppLayout`, `AdminDashboardSection`; navy color palette; `uiStore.js` za dijeljeno stanje; stat kartice s trend indikatorima; key highlights sekcija |
| SB-04 | PB-31 Proširenje prosljeđivanja tiketa — admin preraspodjela | US-101 | Uma | Done | Administrator može otvoriti forward modal iz `TicketDetail` za preraspodjelu agenta/tehničara; admin ne može slati poruke u chat |

---

# Detaljni User Stories (US)

---

## PB-57 AI prijedlog odgovora za agente i tehničare

### US-96
*Kao agent ili tehničar, želim da mi AI predloži odgovor na tiket na osnovu opisa problema i historije komunikacije, kako bih brže i preciznije odgovorio korisniku.*

**Acceptance Criteria:**
- Kada je korisnik agent ili tehničar i tiket nije zatvoren, sistem mora prikazati dugme „AI Prijedlog" u sekciji za slanje poruke
- Kada agent ili tehničar klikne na „AI Prijedlog", sistem mora prikazati modal s generisanim prijedlogom odgovora
- Prijedlog mora biti generisan na osnovu tipa problema tiketa i opisa iz historije komunikacije
- Sistem mora koristiti internu knowledge base za telekomunikacijske probleme (Internet, TV, mobilna mreža, naplata, tehnička podrška, opće)
- Sistem ne smije prikazati dugme „AI Prijedlog" klijentima niti administratorima
- Kada AI servis nije dostupan, sistem mora prikazati odgovarajuću poruku greške i ne smije srušiti stranicu

---

### US-97
*Kao agent ili tehničar, mogu prihvatiti, urediti ili odbaciti AI prijedlog odgovora, kako bih imao potpunu kontrolu nad sadržajem koji šaljem korisniku.*

**Acceptance Criteria:**
- Modal s AI prijedlogom mora sadržavati: generisani tekst prijedloga, dugme „Kopiraj u poruku" i dugme za zatvaranje
- Kada agent klikne „Kopiraj u poruku", sistem mora prenijeti prijedlog u textarea za pisanje poruke
- Agent ili tehničar mora moći ručno izmijeniti tekst nakon kopiranja prije slanja
- Agent ili tehničar može zatvoriti modal i ignorisati prijedlog bez ikakvih posljedica
- Prijedlog se ne smije automatski slati — agent uvijek mora eksplicitno kliknuti „Pošalji"

---

## PB-58 AI uvidi za administratore

### US-98
*Kao administrator, želim da vidim AI-generisane uvide o stanju sistema na admin dashboardu, kako bih dobio inteligentnu analizu trendova i preporuke bez manualnog čitanja svih metrika.*

**Acceptance Criteria:**
- Kada administrator klikne dugme „AI Uvidi" u headeru, sistem mora prikazati inline panel s AI uvidima ispod KPI kartica na dashboardu
- Panel mora sadržavati: sažetak stanja sistema, identifikovane trendove, ključne nalaze i preporuke za akciju
- AI uvidi moraju biti generisani na osnovu trenutnih dashboard metrika (broj tiketa, statusi, prosječna rješavanja, ocjene, opterećenje agenata)
- Sistem ne smije prikazati dugme „AI Uvidi" korisnicima koji nisu administratori
- Kada AI servis nije dostupan, sistem mora prikazati poruku greške unutar panela

---

### US-99
*Kao administrator, želim da mogu zatvoriti panel s AI uvidima, kako bih vratio pun pregled dashboard metrika.*

**Acceptance Criteria:**
- Panel s AI uvidima mora imati dugme za zatvaranje
- Kada administrator klikne dugme „AI Uvidi" dok je panel otvoren, panel se mora zatvoriti
- Stanje otvorenosti/zatvorenosti panela mora biti dijeljeno između Headera i AdminDashboardSection komponente
- Zatvaranje panela ne smije resetovati dashboard filter niti ponovo dohvatati podatke

---

## PB-59 Redizajn korisničkog sučelja

### US-100
*Kao korisnik sistema, želim da interfejs bude moderan, pregledan i konzistentan kroz sve stranice, kako bih imao bolji UX i lakšu navigaciju.*

**Acceptance Criteria:**
- Sidebar mora imati svijetlu pozadinu (`#f0f2f5`), navy-800 logo i avatar, te aktivne stavke s `bg-navy-50 text-navy-800`
- Header mora biti vizualno konzistentan s main content područjem (`#f4f6f9`), sadržavati search bar i bell notifikacije
- Admin dashboard mora prikazivati stat kartice s ikonom, trend indikatorom (ArrowUpRight/ArrowDownRight), velikom numeričkom vrijednosti i labelom
- Sidebar mora prikazivati status chip na dnu za administratore: amber upozorenje s brojem tiketa koji zahtijevaju pažnju ili zeleni „Sistem aktivan"
- Klik na status chip u sidebaru mora navigirati na filtriranu listu tiketa
- Alert banner na dashboardu mora biti u cijelosti klikabilan (ne samo dugme)
- AI panel dugme mora biti smješteno u headeru (ne na dashboardu)
- Inline AI panel mora se prikazivati ispod KPI kartica kada je aktivan
- Navy color palette mora koristiti tamne navy vrijednosti (navy-800: `#162d58`) umjesto aliasa za plave boje

---

## PB-31 Proširenje prosljeđivanja tiketa — admin preraspodjela

### US-101
*Kao administrator, želim da mogu preraspodijeliti agenta ili tehničara na tiket direktno iz prikaza detalja tiketa, kako bih ispravio nepravilnu dodjelu ili uravnotežio opterećenje tima bez manualnih intervencija u sistemu.*

**Acceptance Criteria:**
- Kada je korisnik administrator i tiket je u statusu `OPEN`, sistem mora prikazati dugme „Prerasporedi agenta/tehničara" u sekciji akcija
- Klik na dugme mora otvoriti postojeći forward modal s opcijama za izbor agenta i tehničara
- Administrator ne smije biti blokiran `assignedAgentId` provjerom koja vrijedi za agente
- Kada administrator potvrdi preraspodjelu, tiket mora biti dodijeljen odabranom agentu ili tehničaru
- Administrator ne smije moći slati poruke u chat tiketa — umjesto textarea treba prikazati informativnu poruku „Administrator može pratiti razgovor, ali ne može slati poruke."
- Dugme za preraspodjelu ne smije biti prikazano za zatvorene tikete

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
