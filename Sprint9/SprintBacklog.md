# Sprint Backlog – Sprint 9

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati administratorski dio sistema kroz kontrolni panel sa ključnim metrikama, upravljanje korisničkim nalozima, upravljanje katalogom paketa i pretplata, te audit log aktivnosti, kao i omogućiti dodavanje priloga na tikete radi efikasnije komunikacije između korisnika, agenata i tehničara.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-45 Admin Dashboard sa ključnim metrikama | US-71, US-72 | Merisa | To-Do | Prikaz ključnih metrika sistema i navigacija ka admin sekcijama |
| SB-02 | PB-51 Upravljanje korisničkim nalozima | US-73, US-74, US-75 | Ajdin | To-Do | CRUD operacije nad nalozima agenata, tehničara i klijenata |
| SB-03 | PB-52 Upravljanje katalogom paketa i pretplata | US-76, US-77 | Eldar | To-Do | Admin definiše pakete i dodjeljuje pretplate klijentima |
| SB-04 | PB-53 Pregled audit log-a aktivnosti | US-78, US-79 | Hana, Lamija | To-Do | Praćenje ključnih akcija u sistemu sa filtriranjem |
| SB-05 | PB-56 Prilozi na tiketima | US-80, US-81 | Uma | To-Do | Upload i preuzimanje priloga (slike, dokumenti) na tiketima |

---

# Detaljni User Stories (US)

---

## PB-45 Admin Dashboard sa ključnim metrikama

### US-71
*Kao administrator, želim da imam centralni dashboard sa pregledom ključnih metrika sistema, kako bih u svakom trenutku imao jasan uvid u stanje sistema bez potrebe za otvaranjem više stranica.*

**Acceptance Criteria:**
- Kada je administrator prijavljen u sistem, sistem mora prikazati admin dashboard kao prvu stranicu nakon logina
- Sistem mora prikazati ukupan broj korisnika u sistemu razvrstanih po rolama (klijent, agent, tehničar, admin)
- Sistem mora prikazati broj trenutno otvorenih tiketa
- Sistem mora prikazati broj tiketa zatvorenih u posljednjih 7 dana
- Sistem mora prikazati broj tiketa u statusu "Čeka se" (`CLOSURE_REQUESTED`)
- Sistem mora prikazati broj nedodijeljenih tiketa
- Sistem mora prikazati prosječnu ocjenu zatvorenih tiketa za posljednjih 30 dana
- Kada ne postoje podaci za određenu metriku, sistem mora prikazati odgovarajuću poruku umjesto numeričke vrijednosti
- Sistem ne smije dozvoliti pristup dashboardu korisnicima koji nemaju admin rolu
- Metrike se moraju automatski osvježavati pri svakom otvaranju dashboarda

---

### US-72
*Kao administrator, želim da iz dashboarda mogu brzo navigirati ka pojedinačnim admin sekcijama, kako bih efikasno upravljao sistemom.*

**Acceptance Criteria:**
- Sistem mora prikazati navigacijske kartice ili linkove ka sekcijama: Upravljanje korisnicima, Upravljanje paketima, Audit log, Pregled tiketa
- Kada admin klikne na karticu, sistem ga mora preusmjeriti na odgovarajuću sekciju
- Sistem mora prikazati link ka detaljnijim izvještajima (koji će biti implementirani u Sprint 11)
- Kartice moraju biti vizualno konzistentne sa ostatkom aplikacije

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
