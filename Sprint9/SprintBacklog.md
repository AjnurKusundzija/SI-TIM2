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
| SB-02 | PB-51 Upravljanje korisničkim nalozima | US-73, US-74, US-75, US-89, US-90, US-91, US-92, US-93 | Ajdin | Done | CRUD operacije nad nalozima agenata, tehničara i klijenata, te logika prosljeđivanja kada postoje deaktivirani agenti |
| SB-03 | PB-52 Upravljanje katalogom paketa i pretplata | US-76, US-77 | Eldar | Done | Admin definiše pakete i dodjeljuje pretplate klijentima |
| SB-04 | PB-53 Pregled audit log-a aktivnosti | US-78, US-79 | Hana, Lamija | Done | Praćenje ključnih akcija u sistemu sa filtriranjem |
| SB-05 | PB-56 Prilozi na tiketima | US-80, US-81 | Merisa | Done | Upload i preuzimanje priloga (slike, dokumenti) na tiketima |
| SB-06 | PB-38 Izvještaj o broju tiketa | US-41 | Uma | Done | `TICKET_COUNT` vraća ukupan broj + bucket razbreak (dan/sedmica/mjesec) |
| SB-07 | PB-39 Izvještaj po statusu tiketa | US-43 | Uma | Done | `TICKET_STATUS` s postocima, pie chart, drill-down i upozorenje za veliki period |
| SB-08 | PB-40 Izvještaj po tipu problema | US-45 | Uma | Done | `PROBLEM_TYPE` s bar chartom i drill-down po kategoriji |
| SB-09 | PB-41 Prosječno rješavanje — zaseban on-demand izvještaj | US-47 | Uma | Done | Novi `AVG_RESOLUTION` tip izvještaja s agregatom i bucket tabelom |
| SB-10 | PB-43 Izvještaj o opterećenju agenata | US-94 | Uma | Done | `TEAM_WORKLOAD` vraća ukupne zbirove + pivot tabelu period × agent |
| SB-11 | PB-44 Izvještaj o ocjenama korisnika | US-95 | Uma | Done | `USER_RATINGS` vraća distribuciju po zvjezdicama + trend tabelu po pod-periodima |
| SB-12 | PB-29 Preraspodjela agenata po timovima - partial | US-23 | Ajdin | Done | Administrator može preraspodijeliti agente po timovima; pregled raspodjele timova s filtriranjem |

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
- Kada administrator otvori sekciju "Korisnici" i klikne na opciju "Dodaj korisnika", sistem mora prikazati formu za kreiranje novog naloga
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
*Kao administrator ili agent, želim da uredim podatke postojećih korisnika, kako bih održavao tačnost informacija u sistemu.*

**Acceptance Criteria:**
- Kada administrator otvori listu korisnika, sistem mora prikazati paginiranu listu svih korisnika sa osnovnim podacima (ime, prezime, broj telefona, email, lokacija)
- Sistem mora omogućiti filtriranje liste po lokaciji
- Sistem mora omogućiti pretraživanje korisnika po imenu, prezimenu ,emailu ili broju telefona
- Kada administrator klikne na korisnika, sistem mora prikazati detaljnu stranicu sa svim podacima i opcijom za izmjenu
- Administrator može mijenjati ime, prezime, telefon i lokaciju korisnika
- Sistem ne smije dozvoliti administratoru promjenu role postojećeg korisnika
- Kada administrator sačuva izmjene, sistem mora prikazati potvrdu uspješne izmjene
- Sistem mora evidentirati izmjenu u audit log

---

### US-75
*Kao administrator ili agent, želim da pregledam i deaktiviram klijentske naloge, kako bih mogao onemogućiti pristup neaktivnim ili problematičnim korisnicima.*

**Acceptance Criteria:**
- Kada administrator ili agent otvori sekciju "Korisnici" i klikne na opciju "Klijenti", sistem mora prikazati listu samo aktivnih klijenata
- Lista klijenata mora biti paginirana i sadržavati osnovne podatke (ime, prezime, email, broj telefona, lokacija)
- Sistem mora omogućiti pretragu klijenata po imenu, prezimenu, emailu ili broju telefona
- Kada administrator ili agent klikne na klijenta, sistem mora prikazati detaljnu stranicu korisnika
- Administrator i agent mogu mijenjati podatke klijenta
- Kada administrator ili agent deaktivira klijentski nalog, status korisnika se mijenja u `INACTIVE`
- Sistem ne smije dozvoliti prijavu korisnicima sa statusom `INACTIVE`
- Sistem mora zadržati sve historijske podatke deaktiviranog korisnika (tiketi, poruke, ocjene)
- Agent ne može deaktivirati administratorski ili agentski nalog
- Deaktivirani klijenti se ne smiju prikazivati u sekciji "Klijenti"

---

### US-89
*Kao administrator, želim da pregledam i upravljam agentskim nalozima, kako bih mogao održavati agentski tim i kontrolisati pristup sistemu.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju "Korisnici", sistem mora prikazati opciju "Agenti"
- Opcija "Agenti" mora biti vidljiva samo administratorima
- Kada administrator klikne na opciju "Agenti", sistem mora prikazati listu samo aktivnih agenata
- Lista agenata mora sadržavati osnovne podatke (ime, prezime, email, broj telefona, lokacija, kategorija stručnosti)
- Sistem mora omogućiti pretragu i filtriranje agenata
- Kada administrator klikne na agenta, sistem mora prikazati detaljnu stranicu sa opcijom izmjene podataka
- Administrator može mijenjati podatke bilo kojeg agenta
- Administrator može deaktivirati agentski nalog
- Agent ne može pristupiti sekciji "Agenti"
- Sistem ne smije dozvoliti administratoru deaktivaciju vlastitog naloga
- Kada administrator deaktivira agenta koji ima aktivno dodijeljene tikete, sistem mora prikazati upozorenje i tražiti potvrdu
- Deaktivirani agenti se ne smiju prikazivati u sekciji "Agenti"
- Sistem ne smije dodjeljivati nove tikete deaktiviranom agentu

---

### US-90
*Kao administrator ili agent, želim da pregledam i uređujem tehničarske naloge, kako bih mogao održavati tačne informacije o tehničkom osoblju.*

**Acceptance Criteria:**
- Kada administrator ili agent otvori sekciju "Korisnici" i klikne na opciju "Tehničari", sistem mora prikazati listu samo aktivnih tehničara
- Lista tehničara mora sadržavati osnovne podatke (ime, prezime, email, broj telefona, lokacija)
- Sistem mora omogućiti pretragu i filtriranje tehničara
- Kada administrator ili agent klikne na tehničara, sistem mora prikazati detaljnu stranicu sa opcijom izmjene podataka
- Administrator i agent mogu mijenjati podatke tehničara
- Samo administrator može deaktivirati tehničarski nalog
- Kada administrator deaktivira tehničarski nalog, status korisnika se mijenja u `INACTIVE`
- Sistem ne smije dozvoliti prijavu korisnicima sa statusom `INACTIVE`
- Deaktivirani tehničari se ne smiju prikazivati u sekciji "Tehničari"
- Sistem ne smije dodjeljivati nove tikete deaktiviranom tehničaru

---

### US-91
*Kao administrator, želim da pregledam deaktivirane korisničke naloge i reaktiviram ih, kako bih mogao vratiti pristup korisnicima kada je to potrebno.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju "Korisnici", sistem mora prikazati opciju "Deaktivirani"
- Opcija "Deaktivirani" mora biti vidljiva samo administratorima
- Kada administrator klikne na opciju "Deaktivirani", sistem mora prikazati listu svih deaktiviranih korisnika
- Sistem mora omogućiti filtriranje deaktiviranih korisnika po roli (klijent, agent, tehničar, administrator)
- Sistem mora omogućiti pretragu deaktiviranih korisnika po imenu, prezimenu, emailu ili broju telefona
- Kada administrator klikne na deaktiviranog korisnika, sistem mora prikazati detalje korisnika i opciju za reaktivaciju
- Kada administrator reaktivira korisnika, status naloga se vraća u `ACTIVE`
- Reaktivirani korisnici se ponovo prikazuju u odgovarajućim sekcijama ("Klijenti", "Agenti", "Tehničari")
- Sistem mora evidentirati deaktivaciju i reaktivaciju korisnika u audit log
- Agent ne može pristupiti sekciji "Deaktivirani"

---

### US-92
*Kao administrator, želim da pregledam detaljne profile agenata i tehničara sa relevantnom statistikom, kako bih mogao pratiti njihov rad i upravljati njihovim podacima.*

**Acceptance Criteria:**
- Kada administrator otvori sekciju "Agenti" ili "Tehničari" i klikne na određenog korisnika, sistem mora prikazati detaljan profil agenta ili tehničara
- Ako je korisnik agent, sistem mora prikazati i kategoriju stručnosti
- Administrator mora imati mogućnost uređivanja podataka agenta ili tehničara
- Pravila validacije prilikom uređivanja moraju biti ista kao kod kreiranja korisnika
- Sistem ne smije dozvoliti promjenu role postojećeg korisnika
- Umjesto korisničkih podataka, sistem mora prikazati statistiku rada agenta ili tehničara
- Sistem mora prikazati potvrdu nakon uspješnog uređivanja podataka

---

### US-93
*Kao agent, želim da prilikom prosljeđivanja tiketa drugom agentu vidim samo aktivne agente, kako bih mogao uspješno dodijeliti tiket dostupnim članovima tima.*

**Acceptance Criteria:**
- Kada agent otvori opciju za prosljeđivanje tiketa, sistem mora prikazati listu samo aktivnih agenata
- Deaktivirani agenti se ne smiju prikazivati u listi dostupnih agenata
- Kada agent odabere drugog agenta i potvrdi dodjelu, sistem mora uspješno proslijediti tiket ako je odabrani agent i dalje aktivan
- Ako je agent deaktiviran nakon učitavanja liste, a prije potvrde dodjele, sistem ne smije dozvoliti dodjelu tiketa tom agentu
- U slučaju neuspjele dodjele zbog deaktivacije agenta, sistem mora prikazati odgovarajuću poruku:
  - da dodjela nije izvršena
- Sistem mora zahtijevati ponovno biranje aktivnog agenta

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

## PB-38 Izvještaj o broju tiketa

### US-41
*Kao administrator, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani, kako bih imao uvid o situaciji i količini tiketa.*

**Acceptance Criteria:**
- Kada administrator odabere vremenski period (sedmica/mjesec/godina/prilagođeno), tada se prikazuje ukupan broj tiketa za taj period
- Kada administrator generiše izvještaj, tada vidi i razbreak po pod-periodima (dan/sedmica/mjesec) u tabeli
- Kada podaci postoje u sistemu, tada podaci odgovaraju stvarnom stanju u bazi
- Sistem mora omogućiti pregled izvještaja isključivo administratoru
- Sistem ne smije prikazati pogrešne ili duplirane podatke
- Administrator treba dobiti poruku ako nema podataka za odabrani period

---

## PB-39 Izvještaj po statusu tiketa

### US-43
*Kao administrator, želim da imam izvještaj o statusu tiketa, kako bih mogao lakše analizirati i imati uvid o tiketima.*

**Acceptance Criteria:**
- Kada administrator otvori izvještaj, tada vidi ukupan broj tiketa po statusima (otvoren, čeka zatvaranje, zatvoren) sa procentualnim udjelom
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima u bazi
- Sistem mora grupisati tikete po statusu i prikazati procentualnu raspodjelu
- Kada je odabran veliki vremenski opseg (>90 dana), sistem mora prikazati upozorenje o pouzdanosti postotaka
- Administrator treba dobiti poruku ako nema podataka za odabrani period
- Sistem ne smije dozvoliti pristup izvještaju korisnicima koji nemaju admin rolu

---

## PB-40 Izvještaj po tipu problema

### US-45
*Kao administrator, želim da imam izvještaj po tipovima problema u tiketima, kako bih imao uvid o najčešćim tipovima problema i daljnje analize i poboljšanja cijelog telekom sistema.*

**Acceptance Criteria:**
- Kada administrator otvori izvještaj, tada vidi ukupan broj tiketa po tipu problema (Internet, TV, Mobilna mreža, Računi, Tehnička podrška)
- Kada podaci postoje, tada su prikazani tačni tipovi i količine
- Sistem mora grupisati tikete po tipu problema i prikazati ih sortirane po broju (opadajuće)
- Sistem mora omogućiti drill-down na listu tiketa za odabrani tip problema
- Administrator treba dobiti poruku ako nema podataka za odabrani period
- Sistem ne smije dozvoliti pristup izvještaju korisnicima koji nemaju admin rolu

---

## PB-41 Prosječno vrijeme rješavanja tiketa

### US-47
*Kao administrator, želim da imam uvid o prosječnom vremenu rješavanja (zatvaranja) tiketa, kako bih imao uvid u efikasnost naših radnika.*

**Acceptance Criteria:**
- Kada administrator generiše izvještaj, tada vidi prosječno vrijeme zatvaranja tiketa za odabrani period
- Kada administrator generiše izvještaj, tada vidi i razbreak po pod-periodima (dan/sedmica/mjesec) — broj tiketa, broj zatvorenih i prosječno rješavanje po periodu
- Kada podaci postoje, tada je izračun tačan (ClosedDate − CreatedDate, samo za zatvorene tikete)
- Sistem ne smije uključiti nezatvorene tikete u izračun prosječnog vremena
- Kada nema zatvorenih tiketa u periodu, sistem prikazuje odgovarajuću poruku
- Sistem ne smije dozvoliti pristup izvještaju korisnicima koji nemaju admin rolu

---

## PB-43 Izvještaj o opterećenju agenata

### US-94
*Kao administrator, želim da vidim izvještaj o opterećenju agenata i tehničara po vremenskim intervalima, kako bih pratio raspoređenost posla i efikasnost tima.*

**Acceptance Criteria:**
- Kada administrator generiše izvještaj, tada vidi ukupan broj zatvorenih tiketa po agentu/tehničaru za odabrani period
- Kada administrator generiše izvještaj, tada vidi i pivot tabelu: redovi su pod-periodi (dan/sedmica/mjesec), kolone su agenti — vrijednosti su broj zatvorenih tiketa po agentu u tom pod-periodu
- Sistem mora prikazati sve agente i tehničare koji su imali riješene tikete u periodu, sortirane po ukupnom broju (opadajuće)
- Za agente: dodjeljuje se samo posljednjem assigniranom agentu po tiketu; za tehničare: svi assignirani tehničari dobivaju bod
- Kada nema podataka za period, sistem prikazuje odgovarajuću poruku
- Sistem ne smije dozvoliti pristup izvještaju korisnicima koji nemaju admin rolu

---

## PB-44 Izvještaj o ocjenama korisnika

### US-95
*Kao administrator, želim da vidim izvještaj o ocjenama korisnika s trendom kroz vrijeme, kako bih pratio zadovoljstvo korisnika i kvalitet usluge.*

**Acceptance Criteria:**
- Kada administrator generiše izvještaj, tada vidi prosječnu ocjenu i ukupan broj ocijenjenih tiketa za odabrani period
- Kada administrator generiše izvještaj, tada vidi distribuciju ocjena po broju zvjezdica (1–5)
- Kada administrator generiše izvještaj, tada vidi i trend tabelu po pod-periodima: period, prosječna ocjena, broj ocjena
- Kada nema ocjena u odabranom periodu, sistem prikazuje odgovarajuću poruku
- Sistem ne smije dozvoliti pristup izvještaju korisnicima koji nemaju admin rolu

---

## PB-29 Preraspodjela agenata po timovima

### US-23
*Kao administrator, želim da preraspodijelim agente po timovima, kako bih optimizirao rad.*

**Acceptance Criteria:**
- Kada je administrator prijavljen i nalazi se u sekciji upravljanja timovima, ako odabere agenta i premjesti ga u drugi tim, tada sistem mora izvršiti promjenu
- Sistem mora omogućiti primjenu bez gubitka informacija
- Kada administrator izvrši preraspodjelu agenta, ako je akcija potvrđena, tada sistem mora evidentirati promjenu sa vremenskim pečatom i imenom administratora
- Sistem mora omogućiti da ne dođe do promjene podataka ukoliko administrator ne potvrdi akciju promjene agenata
- Kada administrator izvrši pokušaj preraspodjele agenata, ako dođe do greške ili nemogućnosti odabrane preraspodjele, sistem mora poslati poruku upozorenja

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
