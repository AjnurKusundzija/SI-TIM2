# Sprint Backlog – Sprint 10

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati AI-potpomognute funkcionalnosti sistema kroz modul prijedloga odgovora za agente i tehničare te modul AI uvida za administratore, izvršiti kompletni redizajn korisničkog sučelja, proširiti administratorske ovlasti nad tiketima, dovršiti pregled rasporeda timova za administratore, te implementirati interne komentare na tiketima, upravljanje FAQ sadržajem od strane administratora, samodjelovanje tiketa za agente, status dostupnosti agenata i MCP Admin Copilot chat za administratorska pitanja nad živim podacima.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-57 AI prijedlog odgovora za agente i tehničare | US-96, US-97 | Uma | Done | `AISuggestionModal` u `TicketDetail`; backend `AIService.GetAgentSuggestionAsync` s knowledge base |
| SB-02 | PB-58 AI uvidi za administratore | US-98, US-99 | Uma | Done | `AIInsightsPanel`, `AIInsightsCard` na admin dashboardu; backend `AIService.GetAdminInsightsAsync`; AI Uvidi dugme u Headeru |
| SB-03 | PB-59 Redizajn korisničkog sučelja | US-100 | Uma | Done | Kompletni revamp `Sidebar`, `Header`, `AppLayout`, `AdminDashboardSection`; navy color palette; `uiStore.js`; stat kartice s trend indikatorima |
| SB-04 | PB-31 Proširenje prosljeđivanja tiketa — admin preraspodjela | US-101 | Uma | Done | Administrator može otvoriti forward modal iz `TicketDetail`; admin ne može slati poruke u chat |
| SB-05 | PB-29 Dovršetak — pregled rasporeda timova za administratora | US-24 | Ajdin | In Progress | Admin pristupa sekciji Timovi i vidi sve timove s članovima, filterima i preraspodjelu agenata |
| SB-06 | PB-60 Interni komentari na tiketima | US-102, US-103 | Eldar | Done | Interne bilješke vidljive samo osoblju, skrivene od klijenta; vizualno razlikovane od regularnih poruka |
| SB-07 | PB-61 Admin CRUD FAQ | US-104 | Ajnur Kušundžija | Done | Administrator može kreirati, uređivati i brisati FAQ stavke direktno iz UI; backend `FaqController`/`FaqService`/`FaqRepository` prošireni sa CRUD endpointima uz `[Authorize(Roles = "ADMINISTRATOR")]`; frontend `Faq.jsx` proširen admin UX-om (forma, validacija, confirm delete) uz čuvanje read-only prikaza za klijente |
| SB-08 | PB-62 Assign to me — samodjelovanje tiketa | US-105 | Ajnur Kušundžija | Done | Agent jednim klikom preuzima nedodijeljeni tiket na sebe; backend `SelfAssignTicketAsync` + endpoint `POST /api/tickets/{id}/self-assign` (samo AGENT) sa race condition zaštitom; frontend „Preuzmi tiket" dugme u `TicketDetail` vidljivo samo agentu za otvoren i nedodijeljen tiket |
| SB-09 | PB-63 Agent availability status | US-106, US-107 | Merisa | Done | Agent postavlja vlastiti status dostupnosti; admin i agenti vide statuse u timskom pregledu |
| SB-10 | PB-70 MCP Admin Copilot | US-108, US-109, US-110, US-111 | Ajnur Kušundžija | Done | Novi MCP server (`Project/mcp-server`, TypeScript + zvanični `@modelcontextprotocol/sdk`, Streamable HTTP) izlaže read-only alate `ticket.search`/`ticket.analytics`/`team.workload`/`faq.search` nad živom bazom; backend `AdminCopilotController` (`POST /api/ai/admin-copilot/query`, samo ADMINISTRATOR) + `AdminCopilotService` prepoznaje intent, poziva MCP alate preko `IMcpClient` i Groq modelom (`GROQ_API_KEY_2`) formatira odgovor; frontend `AdminCopilotPanel`/`AdminCopilotMessage` chat panel (dugme „MCP Copilot" u Headeru, inline u `AdminDashboardSection`); novi `mcp-server` servis u `docker-compose.yml` |

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

## PB-70 MCP Admin Copilot

### US-108
*Kao administrator, želim imati chat interfejs u admin panelu u kojem mogu postavljati pitanja o stanju sistema, kako bih brzo dobio operativne uvide bez ručnog pretraživanja više stranica.*

**Acceptance Criteria:**
- Kada je korisnik administrator, sistem mora prikazati pristup MCP Admin Copilot panelu iz admin dashboarda ili headera
- Sistem ne smije prikazati MCP Admin Copilot korisnicima koji nisu administratori
- Chat interfejs mora omogućiti unos pitanja slobodnim tekstom
- Administrator mora moći poslati pitanja poput: „Koji tim je najopterećeniji?", „Prikaži tikete bez odgovora duže od 2 sata" i „Koji problemi se ponavljaju, a nisu pokriveni FAQ-om?"
- Nakon slanja pitanja, sistem mora prikazati loading stanje dok backend i MCP alati obrađuju zahtjev
- Odgovor mora biti prikazan u chat formatu, sa jasno odvojenim pitanjem administratora i odgovorom sistema
- Odgovor mora sadržavati kratak sažetak, ključne metrike i preporučenu administratorsku akciju kada je moguće
- Ako sistem ne može razumjeti pitanje, mora prikazati poruku koja traži preciziranje umjesto generisanja nepouzdanog odgovora
- Chat panel ne smije resetovati postojeće dashboard filtere, KPI kartice niti trenutno stanje admin dashboarda
- Frontend servis mora pozivati poseban backend endpoint za MCP Admin Copilot, npr. `POST /api/ai/admin-copilot/query`

---

### US-109
*Kao administrator, želim da MCP Admin Copilot koristi žive podatke iz sistema preko Model Context Protocol alata, kako bih dobio trenutno stanje tiketa, timova i FAQ pokrivenosti.*

**Acceptance Criteria:**
- Sistem mora koristiti MCP server kao posrednički sloj između AI/admin-copilot logike i internih podataka sistema
- MCP server mora biti implementiran u TypeScriptu koristeći zvanični Model Context Protocol TypeScript SDK i prateću TypeScript dokumentaciju
- MCP server mora izložiti read-only alate za dohvat podataka; prva verzija ne smije automatski mijenjati tikete, korisnike, timove ili FAQ stavke
- Minimalni MCP alati za ovaj scope moraju uključivati `ticket.search`, `ticket.analytics`, `team.workload` i `faq.search`
- `ticket.search` mora podržati filtriranje po statusu, prioritetu, timu, agentu, kategoriji problema i vremenskom periodu kada su ti podaci dostupni
- `ticket.analytics` mora vratiti agregirane podatke o broju otvorenih tiketa, starosti tiketa, kategorijama problema i ponavljanim problemima
- `team.workload` mora vratiti opterećenje timova kroz broj otvorenih tiketa, broj dodijeljenih agenata i relevantne metrike odgovora kada su dostupne
- `faq.search` mora omogućiti pretragu FAQ sadržaja po ključnim riječima, kategoriji problema ili sličnosti sa opisom problema
- AI sloj ne smije direktno čitati bazu podataka; smije koristiti samo rezultate koje vrate MCP alati ili postojeći backend servisi koji su povezani kao MCP alati
- Svaki odgovor mora prikazati korištene izvore, npr. tiketi, timovi, report metrika ili FAQ
- Ako MCP alat ne vrati podatke, sistem mora jasno prikazati da nema dostupnih rezultata za postavljeno pitanje
- Ako MCP server nije dostupan, chat mora prikazati kontrolisanu grešku bez rušenja admin dashboarda
- Backend mora evidentirati pitanje administratora, korištene MCP alate i vrijeme izvršavanja u AI usage log ili audit zapis

---

### US-110
*Kao administrator, želim pitati koji tim je trenutno najopterećeniji, kako bih mogao donijeti odluku o preraspodjeli tiketa ili uključivanju dodatnih agenata.*

**Acceptance Criteria:**
- Kada administrator pita „Koji tim je najopterećeniji?", sistem mora prepoznati namjeru pitanja kao analizu opterećenja tima
- Sistem mora pozvati MCP alat `team.workload`, a po potrebi i `ticket.analytics`
- Odgovor mora prikazati broj otvorenih tiketa po timu
- Odgovor mora prikazati broj tiketa starijih od definisanog praga, npr. duže od 2 sata bez odgovora, ako sistem može izračunati tu metriku iz postojećih podataka
- Odgovor mora prikazati broj agenata ili članova tima koji trenutno nose opterećenje, ako su ti podaci dostupni
- Odgovor mora prikazati prosječno vrijeme prvog odgovora po timu ako je metrika dostupna kroz postojeći report/admin modul
- Sistem mora jasno označiti tim koji je trenutno najopterećeniji i objasniti po kojem kriteriju je odabran
- Sistem mora dati preporuku, npr. preraspodijeliti dio tiketa, uključiti dodatnog agenta ili provjeriti tikete s najdužim čekanjem
- Preporuka mora biti zasnovana na prikazanim metrikama, a ne na generičkom tekstu
- Rezultat mora sadržavati link ili akciju koja vodi administratora na filtriranu listu relevantnih tiketa kada takav UI put postoji
- Ako nema dovoljno podataka za poređenje timova, sistem mora to jasno navesti i prikazati dostupne parcijalne podatke

---

### US-111
*Kao administrator, želim da MCP Admin Copilot provjeri da li se česti problemi već pokrivaju kroz FAQ, kako bih znao kada treba dopuniti bazu znanja.*

**Acceptance Criteria:**
- Administrator mora moći pitati „Koji problemi se najčešće ponavljaju i da li FAQ pokriva te slučajeve?"
- Sistem mora prepoznati namjeru pitanja kao analizu ponavljanih problema i FAQ pokrivenosti
- Sistem mora pozvati MCP alat `ticket.analytics` za identifikaciju najčešćih kategorija, naslova ili obrazaca u tiketima
- Sistem mora pozvati MCP alat `faq.search` za provjeru postojećih FAQ stavki
- Odgovor mora prikazati najčešće ponavljane probleme u odabranom ili podrazumijevanom vremenskom periodu
- Za svaki ponavljani problem sistem mora prikazati da li postoji odgovarajuća FAQ stavka
- Ako FAQ ne pokriva problem, sistem mora predložiti kreiranje nove FAQ stavke
- Sistem može predložiti nacrt pitanja i odgovora, ali mora jasno označiti da administrator treba pregledati i potvrditi sadržaj
- Sistem ne smije automatski kreirati FAQ stavku bez eksplicitne administratorske akcije
- Ako postoji relevantna FAQ stavka, sistem mora prikazati naziv, kratki sažetak ili link na tu stavku
- Ako se problem ponavlja, ali nema dovoljno tekstualnog konteksta za FAQ prijedlog, sistem mora prikazati da je potrebna ručna analiza uz listu relevantnih tiketa

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
