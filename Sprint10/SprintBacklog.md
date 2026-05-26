# Sprint Backlog – Sprint 10

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati AI-potpomognute funkcionalnosti sistema kroz modul prijedloga odgovora za agente i tehničare te modul AI uvida za administratore, izvršiti kompletni redizajn korisničkog sučelja, proširiti administratorske ovlasti nad tiketima, dovršiti pregled rasporeda timova za administratore, te implementirati interne komentare na tiketima, upravljanje FAQ sadržajem od strane administratora, samodjelovanje tiketa za agente i status dostupnosti agenata.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-57 AI prijedlog odgovora za agente i tehničare | US-96, US-97 | Uma | Done | `AISuggestionModal` u `TicketDetail`; backend `AIService.GetAgentSuggestionAsync` s knowledge base |
| SB-02 | PB-58 AI uvidi za administratore | US-98, US-99 | Uma | Done | `AIInsightsPanel`, `AIInsightsCard` na admin dashboardu; backend `AIService.GetAdminInsightsAsync`; AI Uvidi dugme u Headeru |
| SB-03 | PB-59 Redizajn korisničkog sučelja | US-100 | Uma | Done | Kompletni revamp `Sidebar`, `Header`, `AppLayout`, `AdminDashboardSection`; navy color palette; `uiStore.js`; stat kartice s trend indikatorima |
| SB-04 | PB-31 Proširenje prosljeđivanja tiketa — admin preraspodjela | US-101 | Uma | Done | Administrator može otvoriti forward modal iz `TicketDetail`; admin ne može slati poruke u chat |
| SB-05 | PB-29 Dovršetak — pregled rasporeda timova za administratora | US-24 | — | In Progress | Admin pristupa sekciji Timovi i vidi sve timove s članovima, filterima i preraspodjelu agenata |
| SB-06 | PB-60 Interni komentari na tiketima | US-102, US-103 | — | In Progress | Interne bilješke vidljive samo osoblju, skrivene od klijenta; vizualno razlikovane od regularnih poruka |
| SB-07 | PB-61 Admin CRUD FAQ | US-104 | — | In Progress | Administrator može kreirati, uređivati i brisati FAQ stavke direktno iz sučelja |
| SB-08 | PB-62 Assign to me — samodjelovanje tiketa | US-105 | — | In Progress | Agent jednim klikom preuzima nedodijeljeni tiket na sebe |
| SB-09 | PB-63 Agent availability status | US-106, US-107 | — | In Progress | Agent postavlja vlastiti status dostupnosti; admin i agenti vide statuse u timskom pregledu |

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

## PB-29 Dovršetak — pregled rasporeda timova za administratora

### US-24
*Kao administrator, želim da vidim raspored agenata po timovima u sekciji Timovi, kako bih imao pregledan uvid u organizaciju tima i mogao vršiti preraspodjele direktno iz tog prikaza.*

**Acceptance Criteria:**
- Kada administrator pristupi sekciji Timovi, sistem mora prikazati sve timove i njihove trenutne članove
- Za svakog agenta u timu sistem mora prikazati ime, prezime i kategoriju stručnosti
- Kada se prikazuju članovi tima, sistem ne smije prikazivati deaktivirane ili obrisane agente kao aktivne članove
- Kada administrator primijeni filter ili sortiranje na pregledu, sistem mora ažurirati prikaz prema odabranim kriterijima
- Sistem mora prikazati aktivno stanje svakog tima (broj aktivnih agenata, broj otvorenih tiketa dodjeljenih timu)
- Administrator može vršiti preraspodjelu agenta između timova direktno iz sekcije Timovi, a ne samo iz detalja tiketa
- Kada administrator izvrši preraspodjelu, sistem mora prikazati potvrdu i evidentirati promjenu u audit log sa timestamp-om i imenom administratora
- Sistem ne smije dozvoliti preraspodjelu ako je agent neaktivan

---

## PB-60 Interni komentari na tiketima

### US-102
*Kao agent ili tehničar, želim da dodam interni komentar na tiket koji neće biti vidljiv klijentu, kako bih mogao bilježiti interne napomene, eskalacijske detalje ili tehničke informacije relevantne samo za osoblje.*

**Acceptance Criteria:**
- Kada je korisnik agent ili tehničar i tiket nije zatvoren, sistem mora prikazati opciju za dodavanje internog komentara odvojenu od opcije za slanje poruke klijentu
- Kada agent ili tehničar kreira interni komentar, sistem mora ga pohraniti s oznakom da je interni
- Interni komentari moraju biti vidljivi agentima, tehničarima i administratorima
- Sistem ne smije prikazati interne komentare klijentima ni u jednom prikazu tiketa
- Interni komentari moraju biti vizualno razlikovani od regularnih poruka u toku razgovora (drugačija boja pozadine, ikona ili labela „Interno")
- Sistem mora prikazati informaciju o tome ko je i kada ostavio interni komentar
- Kada klijent otvori tiket, interni komentari ne smiju biti vidljivi niti naznačeni

---

### US-103
*Kao agent, tehničar ili administrator, želim da vidim sve interne komentare na tiketu u hronološkom toku zajedno s regularnim porukama, kako bih imao potpuni kontekst slučaja.*

**Acceptance Criteria:**
- Sistem mora prikazivati interne komentare u hronološkom slijedu zajedno s regularnim porukama, ali s jasnom vizualnom razlikom
- Interni komentar mora sadržavati: ime autora, vremensku oznaku i sadržaj komentara
- Kada osoblje pregledava tikete, interni komentari moraju uvijek biti vidljivi bez potrebe za posebnom akcijom
- Sistem mora prikazati interni komentar odmah nakon kreiranja bez potrebe za ponovnim učitavanjem stranice (real-time via SignalR ili refresh)
- Interni komentari moraju biti prikazani i u historiji zatvorenog tiketa za osoblje

---

## PB-61 Admin CRUD FAQ

### US-104
*Kao administrator, želim da mogu kreirati, uređivati i brisati FAQ stavke direktno iz sučelja sistema, kako bih održavao ažurnost i tačnost FAQ sadržaja bez intervencija u kodu ili bazi podataka.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju FAQ, sistem mora prikazati listu svih FAQ stavki s opcijama za uređivanje i brisanje uz svaku stavku, te dugme za kreiranje nove stavke
- Kada administrator klikne „Dodaj pitanje", sistem mora prikazati formu s poljima: pitanje i odgovor
- Sistem mora validirati da ni pitanje ni odgovor nisu prazni
- Kada administrator sačuva novu FAQ stavku, ona mora biti odmah vidljiva u FAQ sekciji svim korisnicima
- Kada administrator klikne „Uredi" na postojećoj stavci, sistem mora prikazati formu s unaprijed popunjenim trenutnim sadržajem
- Kada administrator sačuva izmjenu, sistem mora ažurirati stavku i prikazati potvrdu
- Kada administrator klikne „Obriši", sistem mora tražiti potvrdu prije brisanja
- Nakon potvrde brisanja, stavka mora biti uklonjena iz FAQ sekcije za sve korisnike
- Sistem ne smije dozvoliti pristup CRUD operacijama nad FAQ stavkama korisnicima koji nisu administratori
- FAQ stavke kojima upravljaju agenti/klijenti (read-only) ostaju nepromijenjene u svom prikazu

---

## PB-62 Assign to me — samodjelovanje tiketa

### US-105
*Kao agent, želim da jednim klikom dodijelim nedodijeljeni tiket sebi, kako bih brzo preuzeo tiket bez potrebe da čekam manualnu dodjelu od administratora ili drugog agenta.*

**Acceptance Criteria:**
- Kada agent pregleda tiket koji nije dodijeljen nijednom agentu, sistem mora prikazati dugme „Preuzmi tiket" ili „Dodijeli meni"
- Klik na dugme mora odmah dodijeliti tiket prijavljenom agentu bez dodatnih potvrda
- Nakon samodjelovnja, sistem mora ažurirati prikaz tiketa i prikazati agenta kao dodjeljenog
- Sistem mora evidentirati dodjelu u historiji tiketa i poslati notifikaciju relevantnim stranama
- Sistem ne smije prikazati dugme „Dodijeli meni" agentu koji je već dodijeljen na taj tiket
- Sistem ne smije prikazati dugme „Dodijeli meni" ako je tiket zatvoren
- Sistem ne smije dozvoliti agentima da preuzmu tiket koji je već dodijeljen drugom agentu putem ovog dugmeta
- Klijenti i tehničari ne smiju imati mogućnost samodjelovnja tiketa

---

## PB-63 Agent availability status

### US-106
*Kao agent, želim da postavim vlastiti status dostupnosti, kako bi sistem i tim znali da li sam dostupan za primanje novih tiketa.*

**Acceptance Criteria:**
- Sistem mora omogućiti agentu postavljanje jednog od sljedećih statusa: Dostupan (`AVAILABLE`), Zauzet (`BUSY`), Nedostupan (`UNAVAILABLE`)
- Agent može promijeniti vlastiti status dostupnosti u bilo kojem trenutku dok je prijavljen
- Status mora biti vidljiv u profilu agenta i u timskom pregledu
- Kada agent postavi status „Nedostupan" (`UNAVAILABLE`), sistem mora automatski preraspodijeliti sve njegove trenutno otvorene i dodijeljene tikete — svaki tiket zasebno, po postojećem algoritmu automatske dodjele (najboljim dostupnim agentom po stručnosti i opterećenju)
- Sistem ne smije dodjeljivati tikete agentima u statusu `UNAVAILABLE` ni pri preraspodjeli ni pri kreiranju novih tiketa
- Kada je agent u statusu „Zauzet" (`BUSY`), sistem može nastaviti dodjeljivati tikete ali uz vizualno upozorenje u UI-u
- Status se mora automatski resetovati na `AVAILABLE` pri ponovnoj prijavi na sistem
- Sistem mora prikazati vizualnu oznaku statusa (boja/ikona) uz ime agenta u svim relevantnim listama

---

### US-107
*Kao administrator, želim da vidim statuse dostupnosti svih agenata, kako bih mogao optimalno rasporediti tikete i prepoznati kada je tim preopterećen ili nedostupan.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju Timovi ili listu agenata, sistem mora prikazati trenutni status dostupnosti svakog agenta
- Statusi moraju biti vizualno razlikovani bojama: Dostupan (zelena), Zauzet (žuta), Nedostupan (siva/crvena)
- Kada se status agenta promijeni, prikaz u administratorskom sučelju mora se ažurirati bez potrebe za ručnim osvježenjem stranice
- Sistem mora prikazati i broj tiketa koji su trenutno dodijeljeni svakom agentu uz njegov status
- Administrator može filtrirati listu agenata po statusu dostupnosti

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
