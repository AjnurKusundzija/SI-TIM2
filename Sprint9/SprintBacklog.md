# Sprint Backlog – Sprint 9

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati administratorski dio sistema kroz kontrolni panel sa ključnim metrikama, upravljanje korisničkim nalozima, upravljanje katalogom paketa i pretplata, te audit log aktivnosti, kao i omogućiti dodavanje priloga na tikete radi efikasnije komunikacije između korisnika, agenata i tehničara.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-45 Admin Dashboard sa ključnim metrikama | US-71, US-72, US-82 – US-86 | Uma | Done | Metrike na `/dashboard`; generisanje na `/reports` |
| SB-00 | PB-50 Prosječno vrijeme prvog odgovora (admin) | US-87, US-88 | Uma | Done | On-demand `FIRST_RESPONSE` + trend po pod-periodima na `/reports` |
| SB-02 | PB-51 Upravljanje korisničkim nalozima | US-73, US-74, US-75 | Ajdin | To-Do | CRUD operacije nad nalozima agenata, tehničara i klijenata |
| SB-03 | PB-52 Upravljanje katalogom paketa i pretplata | US-76, US-77 | Eldar | To-Do | Admin definiše pakete i dodjeljuje pretplate klijentima |
| SB-04 | PB-53 Pregled audit log-a aktivnosti | US-78, US-79 | Hana, Lamija | To-Do | Praćenje ključnih akcija u sistemu sa filtriranjem |
| SB-05 | PB-56 Prilozi na tiketima | US-80, US-81 | Merisa | To-Do | Upload i preuzimanje priloga (slike, dokumenti) na tiketima |

---

# Detaljni User Stories (US)

---

## PB-45 Admin Dashboard sa ključnim metrikama

### US-71
*Kao administrator, želim da na dashboardu vidim centralni pregled ključnih metrika cijelog sistema, kako bih imao operativni uvid u stanje helpdeska.*

**Acceptance Criteria:**
- Kada je prijavljen `ADMINISTRATOR`, sistem mora na `/dashboard` prikazati: vremenski period, ključne metrike (KPI), aktivne korisnike po rolama i grafove
- Kada korisnik nije administrator, sistem ne smije prikazati admin metrike niti pozivati `GET /api/admin/dashboard`
- Sidebar stavka „Izvještaji“ vodi na `/reports` (zasebno od metrika)
- Sistem mora učitati podatke preko `GET /api/admin/dashboard` (live upit na bazu) pri otvaranju ili promjeni filtera
- Sistem mora prikazati sve must-have KPI kartice: ukupan broj tiketa (u odabranom periodu), distribucija po statusu, prosječno vrijeme rješavanja, prosječno vrijeme prvog odgovora (PB-50), prosječna ocjena korisnika, opterećenje agenata (sažetak), top tipovi problema (sažetak)
- Sistem mora prikazati operativne metrike: broj aktivnih korisnika po rolama, broj otvorenih tiketa, tiketi u `CLOSURE_REQUESTED`, nedodijeljeni tiketi, tiketi stariji od definisanog praga (ako je konfigurisan)
- U izračun ulaze tiketi u svim postojećim statusima (`OPEN`, `CLOSED`, `CLOSURE_REQUESTED`); sistem ne uvodi status `CANCELLED` niti isključuje tikete iz agregata na osnovu nepostojećeg statusa
- Kada za karticu nema podataka u odabranom periodu, sistem mora prikazati poruku (ne numeričku vrijednost „0“ osim ako je stvarno nula)
- Dashboard mora koristiti isti vizuelni jezik kao `/statistics` (kartice, Recharts)
- Učitavanje dashboard metrika mora biti završeno u &lt; 5 sekundi za tipičan dataset tima

---

### US-72
*Kao administrator, želim da jednim globalnim vremenskim filterom filtriram metrike na dashboardu i izvještaje na stranici Izvještaji, kako bih analizirao isti period.*

**Acceptance Criteria:**
- Sistem mora prikazati globalni filter „Vremenski period“ na `/dashboard` (s dugmetom Primijeni za metrike)
- Sistem mora prikazati globalni filter „Vremenski period“ na `/reports` (za generisanje izvještaja)
- Sistem mora omogućiti brze periode: sedmica, mjesec, godina
- Sistem mora omogućiti custom raspon (od datuma — do datuma)
- Kada administrator promijeni filter, sve KPI kartice, grafovi i on-demand izvještaji moraju se osvježiti za isti period
- Kada je custom raspon nevalidan (kraj prije početka), sistem mora prikazati poruku greške i ne smije pozvati API

---

### US-82
*Kao administrator, želim da na dashboardu odmah vidim grafove ključnih metrika, kako bih brzo uočio trendove i neravnoteže.*

**Acceptance Criteria:**
- Sistem mora prikazati grafove na `/dashboard` (ne na `/reports`)
- Za svaku metriku sistem mora koristiti kombinaciju prikaza: KPI kartica + grafikon i/ili tabela, prema tipu podatka
- Grafovi moraju poštovati globalni vremenski filter
- Kada nema dovoljno podataka za grafikon, sistem mora prikazati poruku umjesto praznog grafa

---

### US-83
*Kao administrator, želim da na stranici Izvještaji generišem bilo koji dostupni izvještaj za odabrani period, kako bih detaljno analizirao podatke.*

**Acceptance Criteria:**
- Stranica `/reports` sadrži isključivo vremenski period i sekciju generisanja izvještaja (bez KPI kartica i grafova)
- Sistem mora omogućiti on-demand generisanje za sve tipove iz `ReportType`: `TICKET_COUNT`, `TICKET_STATUS`, `PROBLEM_TYPE`, `TEAM_WORKLOAD`, `USER_RATINGS`, `FIRST_RESPONSE` (PB-50)
- Generisanje koristi globalni vremenski filter sa `/reports`
- Za izvještaj po statusu (`TICKET_STATUS`), kada je odabran veliki vremenski opseg, sistem mora prikazati upozorenje administratoru da interpretacija postotaka može biti nepouzdana
- Kada izvještaj nema podataka za period, sistem mora prikazati poruku
- Podaci u izvještaju moraju odgovarati stvarnom stanju u bazi (live upit)

---

### US-84
*Kao administrator, želim da iz metrike ili grafa mogu otvoriti listu povezanih tiketa (drill-down), kako bih od agregata prešao na konkretne slučajeve.*

**Acceptance Criteria:**
- Kada administrator klikne na KPI karticu, segment grafa ili red u tabeli izvještaja, sistem mora navigirati na filtriranu listu tiketa (`/tickets`) s primijenjenim filterima (status, tip problema, period, agent — prema kontekstu klika)
- Drill-down mora poštovati globalni vremenski filter (s dashboarda ili iz generisanog izvještaja)
- Kada drill-down nema rezultata, sistem mora prikazati poruku na listi tiketa

---

### US-85
*Kao administrator, želim da vidim dugme za export izvještaja, kako bih znao da će ta funkcionalnost biti dostupna u narednoj fazi.*

**Acceptance Criteria:**
- Sistem mora prikazati dugme „Export“ na sekciji izvještaja
- Dugme mora biti disabled (bez funkcionalnosti) do implementacije PB-46
- Pored disabled dugmeta sistem može prikazati kratku napomenu da je export planiran (PB-46)

---

### US-86
*Kao administrator, želim da KPI kartice na dashboardu budu pripremljene za prikaz podataka čim backend izvještaji budu spremni.*

**Acceptance Criteria:**
- PB-45 isporučuje layout svih must-have KPI kartica i placeholder stanja na `/dashboard`
- Logika izračuna za svaku karticu/izvještaj implementira se u odgovarajućem PB-u (PB-38, PB-39, PB-40, PB-41, PB-43, PB-44, PB-50) i povezuje se preko `GET /api/admin/dashboard` i `POST /api/reports/generate`
- Kartice moraju biti vizualno konzistentne sa ostatkom aplikacije

---

## PB-50 Prosječno vrijeme prvog odgovora (admin izvještaj)

### US-87
*Kao administrator, želim da na stranici Izvještaji vidim prosječno vrijeme prvog odgovora na tikete za odabrani period, kako bih procijenio responzivnost tima.*

**Acceptance Criteria:**
- Na `/dashboard` KPI kartica „Prosj. 1. odgovor“ prikazuje agregat za cijeli sistem u odabranom periodu
- Izračun koristi vrijeme od kreiranja tiketa do prvog komentara osoblja (agent/tehničar), ne klijenta
- Kada nema tiketa s odgovorom u periodu, sistem prikazuje poruku (ne lažnu nulu)
- Podaci dolaze live iz baze (`GET /api/admin/dashboard`)

---

### US-88
*Kao administrator, želim da generišem izvještaj o prosječnom vremenu prvog odgovora s razbreakom po pod-periodima, kako bih uočio trend kroz sedmicu, mjesec ili godinu.*

**Acceptance Criteria:**
- Administrator može odabrati izvještaj „Prosj. prvi odgovor“ (`FIRST_RESPONSE`) u sekciji generisanja na `/reports`
- Izvještaj prikazuje: prosjek u periodu, broj tiketa s odgovorom / ukupno, tabelu po pod-periodima
- Granularnost bucket-a: sedmica → po danu, mjesec → po sedmici, godina → po mjesecu, custom → automatski
- Trend prvog odgovora po pod-periodima dostupan je kroz on-demand izvještaj `FIRST_RESPONSE` na `/reports` (ne kao graf na dashboardu)
- Generisanje koristi `POST /api/reports/generate` i globalni vremenski filter sa `/reports`
- Samo `ADMINISTRATOR` ima pristup

---

## PB-51 Upravljanje korisničkim nalozima

### US-73
*Kao administrator, želim da kreiram nove korisničke naloge za agente, tehničare i klijente, kako bih mogao širiti tim i dodavati nove korisnike sistema bez direktnog pristupa bazi podataka.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju "Upravljanje korisnicima" i klikne na opciju "Dodaj korisnika", sistem mora prikazati formu za kreiranje novog naloga
- Forma mora sadržavati polja: ime, prezime, email, telefon, lozinka, rola (klijent, agent, tehničar), lokacija
- Kada je rola agent, sistem mora ponuditi dodatno polje za kategoriju stručnosti
- Sistem mora validirati ispravnost email formata i jedinstvenost emaila u sistemu
- Sistem mora validirati da lozinka zadovoljava minimalne sigurnosne zahtjeve (minimalna dužina)
- Kada su podaci validni i administrator potvrdi kreiranje, sistem mora kreirati nalog i prikazati potvrdu
- Kada su podaci nevalidni, sistem mora prikazati odgovarajuću poruku greške i ne smije kreirati nalog
- Sistem ne smije dozvoliti kreiranje naloga ako email već postoji u sistemu
- Sistem ne smije dozvoliti kreiranje admin naloga kroz ovu formu

---

### US-74
*Kao administrator, želim da uredim podatke postojećih korisnika, kako bih održavao tačnost informacija u sistemu.*

**Acceptance Criteria:**
- Kada administrator otvori listu korisnika, sistem mora prikazati paginiranu listu svih korisnika sa osnovnim podacima (ime, prezime, email, rola, status naloga)
- Sistem mora omogućiti filtriranje liste po roli i statusu naloga
- Sistem mora omogućiti pretraživanje korisnika po imenu, prezimenu ili emailu
- Kada administrator klikne na korisnika, sistem mora prikazati detaljnu stranicu sa svim podacima i opcijom za izmjenu
- Administrator može mijenjati ime, prezime, telefon i lokaciju korisnika
- Sistem ne smije dozvoliti administratoru promjenu role postojećeg korisnika
- Kada administrator sačuva izmjene, sistem mora prikazati potvrdu uspješne izmjene
- Sistem mora evidentirati izmjenu u audit log

---

### US-75
*Kao administrator, želim da deaktiviram korisničke naloge koji više nisu aktivni, kako bih onemogućio pristup sistemu bivšim zaposlenicima ili neaktivnim klijentima bez gubitka historije.*

**Acceptance Criteria:**
- Kada administrator otvori detaljnu stranicu korisnika, sistem mora prikazati opciju za deaktivaciju naloga
- Kada administrator deaktivira nalog, status naloga se mijenja u `INACTIVE`
- Sistem ne smije dozvoliti prijavu korisnicima sa statusom `INACTIVE`
- Sistem mora zadržati sve historijske podatke deaktiviranog korisnika (tiketi, poruke, ocjene)
- Sistem ne smije dodjeljivati nove tikete deaktiviranom agentu ili tehničaru
- Sistem mora omogućiti administratoru reaktivaciju naloga (vraćanje statusa u `ACTIVE`)
- Kada administrator deaktivira agenta koji ima aktivno dodijeljene tikete, sistem mora prikazati upozorenje i tražiti potvrdu
- Sistem ne smije dozvoliti administratoru deaktivaciju vlastitog naloga

---

## PB-52 Upravljanje katalogom paketa i pretplata

### US-76
*Kao administrator, želim da definišem i uređujem katalog paketa koje firma nudi, kako bih osigurao da klijenti vide tačne informacije o dostupnim uslugama.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju "Upravljanje paketima", sistem mora prikazati listu svih definisanih paketa
- Sistem mora omogućiti administratoru kreiranje novog paketa sa poljima: naziv, tip (Internet, TV, mobilni), opis, cijena, status (aktivan/neaktivan)
- Sistem mora validirati da naziv paketa nije prazan i da je cijena pozitivan broj
- Sistem mora omogućiti izmjenu postojećeg paketa
- Sistem mora omogućiti deaktivaciju paketa, ali ne i njegovo brisanje ako postoje aktivne pretplate
- Kada administrator pokuša obrisati paket koji ima aktivne pretplate, sistem mora prikazati poruku upozorenja i ne smije izvršiti brisanje
- Deaktiviran paket se ne smije prikazivati klijentima kao opcija, ali postojeće pretplate na taj paket ostaju aktivne
- Sistem mora prikazati broj aktivnih pretplata po paketu

---

### US-77
*Kao administrator, želim da dodjeljujem pakete i pretplate klijentima, kako bih mogao upravljati uslugama koje konkretni klijenti koriste.*

**Acceptance Criteria:**
- Kada administrator otvori detaljnu stranicu klijenta, sistem mora prikazati listu trenutnih paketa i pretplata
- Sistem mora omogućiti administratoru dodjelu novog paketa klijentu odabirom iz aktivnih paketa u katalogu
- Pri dodjeli paketa administrator mora unijeti datum početka pretplate
- Sistem mora omogućiti administratoru ukidanje (deaktivaciju) pretplate klijenta
- Kada administrator ukine pretplatu, status pretplate se mijenja u neaktivan
- Sistem ne smije dozvoliti dodjelu istog aktivnog paketa istom klijentu više puta
- Klijent mora vidjeti ažurirane pakete na svom profilu odmah nakon promjene
- Sistem mora evidentirati svaku promjenu pretplate u audit log
- Agent i tehničar ne smiju imati mogućnost izmjene paketa kroz svoje sekcije

---

## PB-53 Pregled audit log-a aktivnosti

### US-78
*Kao administrator, želim da vidim historiju ključnih akcija u sistemu, kako bih mogao pratiti aktivnosti, otkrivati zloupotrebe i osigurati traceability.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju "Audit log", sistem mora prikazati paginiranu listu svih zabilježenih akcija sortiranih po datumu, od najnovijih
- Svaki zapis mora sadržavati: timestamp, korisnika koji je izvršio akciju, tip akcije, ciljani entitet i opis akcije
- Sistem mora evidentirati sljedeće tipove akcija: prijava korisnika, kreiranje tiketa, zatvaranje tiketa, promjena statusa tiketa, prosljeđivanje tiketa, kreiranje/izmjena/deaktivacija korisničkog naloga, izmjena paketa, dodjela/ukidanje pretplate
- Sistem ne smije evidentirati osjetljive podatke poput lozinki
- Sistem ne smije dozvoliti pristup audit logu korisnicima koji nemaju admin rolu
- Sistem ne smije dozvoliti izmjenu ili brisanje zapisa audit log-a
- Kada audit log nema zapisa za odabrani period, sistem mora prikazati odgovarajuću poruku

---

### US-79
*Kao administrator, želim da filtriram audit log po različitim kriterijima, kako bih brzo pronašao relevantne zapise.*

**Acceptance Criteria:**
- Sistem mora omogućiti filtriranje audit log-a po tipu akcije
- Sistem mora omogućiti filtriranje po korisniku koji je izvršio akciju
- Sistem mora omogućiti filtriranje po vremenskom periodu (od datuma — do datuma)
- Sistem mora omogućiti pretraživanje audit log-a po opisu akcije
- Kombinovani filteri moraju funkcionisati istovremeno (npr. tip akcije + period)
- Kada filteri ne vraćaju rezultate, sistem mora prikazati poruku da nema zapisa za zadane kriterije
- Sistem mora omogućiti reset svih filtera jednim klikom

---

## PB-56 Prilozi na tiketima

### US-80
*Kao korisnik, agent ili tehničar, želim da uz tiket ili poruku mogu priložiti slike i dokumente, kako bih lakše opisao problem ili dokumentovao rješenje.*

**Acceptance Criteria:**
- Kada korisnik kreira novi tiket, sistem mora prikazati opciju za dodavanje priloga uz tiket
- Kada korisnik, agent ili tehničar piše poruku na tiketu, sistem mora prikazati opciju za dodavanje priloga uz poruku
- Sistem mora podržavati upload sljedećih formata: slike (PNG, JPG, JPEG) i dokumenti (PDF, DOCX, TXT)
- Sistem mora ograničiti maksimalnu veličinu pojedinačnog priloga na 5 MB
- Sistem mora ograničiti maksimalan broj priloga po tiketu/poruci na 5
- Kada korisnik pokuša uploadati nedozvoljen format ili prevelik fajl, sistem mora prikazati odgovarajuću poruku greške i ne smije izvršiti upload
- Sistem mora prikazati indikator napretka tokom uploada većih fajlova
- Sistem ne smije dozvoliti upload izvršnih fajlova (`.exe`, `.bat`, `.sh` i sl.) iz sigurnosnih razloga
- Sistem mora skenirati nazive fajlova i sanitizirati specijalne karaktere

---

### US-81
*Kao korisnik, agent ili tehničar, želim da vidim i preuzmem priloge dodane na tiket, kako bih mogao pregledati materijale koje je drugi učesnik priložio.*

**Acceptance Criteria:**
- Kada korisnik otvori tiket koji ima priloge, sistem mora prikazati listu svih priloga u detaljnom prikazu tiketa
- Za svaki prilog sistem mora prikazati: naziv fajla, veličinu, vrijeme uploada i korisnika koji je priložio fajl
- Kada je prilog slika, sistem mora prikazati thumbnail u listi priloga
- Kada korisnik klikne na sliku, sistem mora otvoriti pregled slike u uvećanom prikazu
- Kada korisnik klikne na dokument (PDF, DOCX, TXT), sistem mora omogućiti preuzimanje fajla
- Sistem ne smije dozvoliti pristup prilozima korisnicima koji nemaju pravo pregleda tiketa
- Kada tiket nema priloga, sistem ne smije prikazivati praznu sekciju za priloge
- Sistem mora prikazati prilog kao dio chronološkog toka poruke ako je priložen uz poruku
- Sistem ne smije dozvoliti brisanje priloga nakon što je priložen na tiket

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
