# Sprint Backlog – Sprint 7

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati funkcionalnosti za upravljanje životnim ciklusom tiketa kroz zatvaranje tiketa, upravljanje prioritetima, automatsku dodjelu i prosljeđivanje tiketa, kao i omogućiti tehničarima pregled osnovnih informacija o dodijeljenim otvorenim tiketima radi efikasnijeg rada support sistema.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-25 Zatvaranje tiketa | US-16, US-17 | Ajdin | Done | Implementacija zatvaranja tiketa od strane korisnika i zahtjeva za zatvaranje od strane agenta |
| SB-02 | PB-28 Upravljanje prioritetima tiketa | US-21 | Ajdin | Done | Implementacija korisničkog i internog prioriteta tiketa |
| SB-03 | PB-30 Automatska dodjela tiketa | US-25 | Eldar | Done | Implementacija automatske dodjele i pravila dodjele tiketa |
| SB-04 | PB-37 Tehničar vidi osnovne informacije | US-39, US-42 | Eldar | Done | Prikaz samo otvorenih assigned tiketa sa osnovnim informacijama |
| SB-05 | PB-31 Prosljeđivanje tiketa | US-55, US-56, US-57 | Uma, Ajdin | Done | Implementacija prosljeđivanja tiketa između agenata i tehničarima na osnovu lokacije |
| SB-06 | PB-48 Pregled dodijeljenih i historija tiketa za agente | US-53, US-54 | Merisa | Done | Implementacija pregleda aktivnih i zatvorenih tiketa po agentu |

---

# Detaljni User Stories (US)

---

## PB-25 Zatvaranje tiketa

### US-16
*Kao korisnik, želim da zatvorim tiket kada je problem riješen, kako bih završio proces.*

**Acceptance Criteria:**
- Kada je tiket riješen, ako korisnik klikne “Zatvori tiket”, tada se status mijenja u zatvoren  
- Sistem ne smije dozvoliti zatvaranje već zatvorenog tiketa  
- Korisnik treba dobiti potvrdu o zatvaranju tiketa  

---

### US-17
*Kao agent ili tehničar, želim da zatvorim tiket nakon rješavanja problema, kako bih označio zadatak kao završen.*

**Acceptance Criteria:**
- Kada agent želi zatvoriti tiket, ako smatra da je zadatak završen, tada može poslati zahtjev za zatvaranje korisniku  
- Kada korisnik primi zahtjev za zatvaranje, tada može prihvatiti ili odbiti zatvaranje tiketa  
- Ako korisnik prihvati zahtjev, tada se tiket zatvara, poprima status zatvoren i sistem evidentira koji je agent zatvorio tiket  
- Ako korisnik odbije zahtjev, tada tiket ostaje otvoren  
- Ako korisnik ne odgovori na zahtjev zatvaranja unutar 7 dana od zadnje poruke, tada agent ima mogućnost prisilnog zatvaranja tiketa
- Sistem mora omogućiti praćenje statusa zahtjeva za zatvaranje  

---

## PB-28 Upravljanje prioritetima tiketa

### US-21
*Kao agent, želim da postavim interni prioritet tiketa, kako bih efikasno upravljao zadacima.*

**Acceptance Criteria:**
- Kada je agent prijavljen u sistem i nalazi se na stranici tiketa, ako odabere opciju za postavljanje prioriteta, tada sistem mora postaviti odabrani prioritet  
- Sistem mora omogućiti listu predefinisanih prioriteta  
- Kada tiket ima postavljen interni prioritet, ako korisnik pristupi tiketu, tada sistem ne smije dozvoliti prikaz niti izmjenu internog prioriteta  
- Kada agent postavi ili izmijeni prioritet tiketa, ako je akcija uspješno izvršena, tada sistem mora prikazati potvrdu  
- Kada agent postavi ili izmijeni prioritet tiketa, ako je akcija neuspješno izvršena, tada sistem mora prikazati poruku upozorenja  
- Kada nema odabranog internog prioriteta, tada sistem mora prikazati poruku da prioritet nije postavljen  

---

## PB-30 Automatska dodjela tiketa

### US-25
*Kao sistem, želim da automatski dodijelim tiket agentu, kako bi se ubrzalo rješavanje.*

**Acceptance Criteria:**
- Kada korisnik kreira novi tiket, ako su definisana pravila dodjele, tada sistem mora automatski dodijeliti tiket odgovarajućem agentu  
- Kada sistem dodjeljuje tiket, ako je agent nedostupan, tada sistem ne smije dodijeliti tiket tom agentu  
- Kada je tiket dodijeljen agentu, ako je dodjela uspješna, tada agent može vidjeti taj tiket u sekciji dodijeljenih tiketa  
- Kada sistem ne može pronaći dostupnog agenta, ako se izvrši pokušaj dodjele, tada tiket mora biti označen kao "Nedodijeljen"  
- Sistem mora dodijeliti tiket prema predefinisanim pravilima dodjele  
- Sistem mora prikazati poruku ukoliko nema definisanih pravila dodjele  

---

## PB-37 Tehničar vidi osnovne informacije

### US-39
*Kao tehničar, želim da vidim osnovne informacije o tiketu, kako bih razumio problem.*

**Acceptance Criteria:**
- Kada tehničar pregleda tikete, ako otvori postojeći tiket, tada sistem mora prikazati osnovne informacije   
- Kada tehničar otvori ekran tiketa, ako su podaci učitani, tada sve informacije moraju biti dostupne bez dodatnih klikova  
- Sistem ne smije omogućiti pregled tiketa koji nisu dodijeljeni tehničaru  
- Sistem mora prikazivati samo otvorene assigned tikete tehničaru  

### US-52
*Kao tehničar, želim da šaljem poruke kroz dodijeljene tikete, kako bih mogao direktno komunicirati sa korisnikom tokom rješavanja problema.*

**Acceptance Criteria:**
- Kada tehničar otvori tiket koji mu je dodijeljen, tada sistem mora omogućiti slanje poruka korisniku  
- Kada tehničar unese poruku i klikne na dugme za slanje, tada se poruka sprema i prikazuje u historiji komunikacije  
- Sistem ne smije dozvoliti slanje praznih poruka  
- Sistem ne smije dozvoliti tehničaru slanje poruka kroz tikete koji mu nisu dodijeljeni  
- Sistem ne smije ograničiti tehničara na maksimalan broj poruka (nema spam block ograničenja od 3 poruke)
- Kada korisnik pregleda poruku poslanu od strane tehničara, tada sistem mora jasno prikazati da je poruku poslao tehničar  
- Sistem mora prikazati vrijeme slanja i pošiljaoca poruke u historiji komunikacije  

---

## PB-31 Prosljeđivanje tiketa

### US-55
*Kao agent, želim proslijediti tiket automatski najkompetentnijem dostupnom agentu kako bih brzo prenio odgovornost bez ručnog odabira.*

**Acceptance Criteria:**

- Kada agent otvori tiket koji mu je dodijeljen i odabere opciju "Proslijedi tiket", sistem prikazuje dvije opcije: "Proslijedi najboljom agentu" i "Odaberi agenta"
- Kada agent odabere "Proslijedi najboljom agentu", sistem automatski kalkuliše score svih dostupnih agenata na osnovu iskustva u kategoriji problema, prosječne ocjene i trenutnog opterećenja
- Težine u kalkulaciji se dinamički prilagođavaju prioritetu tiketa
- Sistem ne smije razmatrati agenta koji je trenutno vlasnik tiketa
- Sistem razmatra samo agente sa statusom SLOBODAN
- Tiket se automatski dodjeljuje agentu s najvišim score-om
- Kada prosljeđivanje nije uspješno, sistem prikazuje poruku greške i dodjela se ne mijenja
---

### US-56
Kao agent, želim vidjeti listu agenata sa njihovim score-ovima i odabrati konkretnog agenta kojem ću proslijediti tiket.

Acceptance Criteria:
• Kada agent odabere opciju "Odaberi agenta", sistem prikazuje listu svih dostupnih agenata sa statusom `SLOBODAN`, sortiranu po score-u silazno
• Sistem ne smije prikazivati agenta koji je trenutno vlasnik tiketa
• Za svakog agenta prikazuje se score u procentima, broj riješenih tiketa iste kategorije, prosječna ocjena i broj trenutno otvorenih tiketa
• Kada agent odabere konkretnog agenta i potvrdi akciju, tiket se dodjeljuje odabranom agentu
• Kada prosljeđivanje nije uspješno, sistem prikazuje poruku greške i dodjela se ne mijenja

---

### US-57
*Kao agent, želim proslijediti tiket tehničaru na osnovu lokacije kreatora tiketa, kako bih osigurao da problem rješava tehničar koji je fizički najbliži korisniku i ima najmanje trenutnih obaveza.*

**Acceptance Criteria:**
- Kada agent otvori tiket koji mu je dodijeljen i odabere opciju "Proslijedi tiket", sistem prikazuje opciju "Proslijedi tehničaru"
- Kada agent odabere "Proslijedi tehničaru", sistem automatski dohvaća lokaciju kreatora tiketa
- Sistem filtrira samo aktivne tehničare (`AccountStatus = ACTIVE`, `Role = TECHNICIAN`) koji se nalaze na istoj lokaciji kao kreator tiketa
- Kada postoji više tehničara na toj lokaciji, sistem dodjeljuje tiket tehničaru s najmanjim brojem trenutno otvorenih tiketa
- Kada postoji tehničar s nula otvorenih tiketa, sistem daje prednost njemu pri dodjeli
- Sistem ne smije dodijeliti tiket tehničaru s drugom lokacijom od lokacije kreatora tiketa
- Kada na lokaciji kreatora tiketa ne postoji nijedan aktivni tehničar, sistem prikazuje poruku greške i dodjela se ne mijenja
- Kada prosljeđivanje uspije, tiket dobiva tip dodjele `FORWARDED_TO_TECHNICIAN` i agent vidi potvrdu o uspješnom prosljeđivanju
- Samo agent koji je trenutni vlasnik tiketa može proslijediti tiket tehničaru
- Kada prosljeđivanje nije uspješno, sistem prikazuje odgovarajuću poruku greške i dodjela ostaje nepromijenjena

---

## PB-48 Pregled i historija dodijeljenih tiketa za agente

### US-53
*Kao agent, želim da vidim sve trenutno otvorene tikete koji su mi dodijeljeni, kako bih imao jasan pregled aktivnih zadataka.*

**Acceptance Criteria:**
- Kada je agent prijavljen u sistem, ako otvori sekciju "Dodijeljeni tiketi”, tada sistem mora prikazati sve trenutno otvorene tikete dodijeljene tom agentu  
- Kada postoji više tiketa, tada se prikazuju svi otvoreni tiketi bez izostavljanja  
- Sistem ne smije prikazivati tikete koji nisu dodijeljeni tom agentu  
- Sistem ne smije prikazivati zatvorene tikete u ovoj sekciji  
- Kada agent nema otvorenih tiketa, tada sistem mora prikazati poruku da nema aktivnih tiketa  


---

### US-54
*Kao agent, želim da imam uvid u sve tikete koji su mi bili dodijeljeni i koji su uspješno riješeni, kako bih mogao pratiti svoj rad i historiju zadataka.*

**Acceptance Criteria:**
- Kada je agent prijavljen u sistem, ako otvori sekciju "Dodijeljeni tiketi” i na njoj izabeze opciju "Zatvoreni tiketi", tada sistem mora prikazati sve zatvorene tikete koji su bili dodijeljeni tom agentu  
- Kada postoji više zatvorenih tiketa, tada se prikazuju svi bez izostavljanja  
- Sistem mora prikazivati samo tikete koji su završeni  
- Sistem ne smije prikazivati tikete koji nisu bili dodijeljeni tom agentu  
- Kada agent nema historiju zatvorenih tiketa, tada sistem mora prikazati odgovarajuću poruku  

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.