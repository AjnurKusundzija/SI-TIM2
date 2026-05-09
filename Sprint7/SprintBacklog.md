# Sprint Backlog – Sprint 7

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati funkcionalnosti za upravljanje životnim ciklusom tiketa kroz zatvaranje tiketa, upravljanje prioritetima, automatsku dodjelu i prosljeđivanje tiketa, kao i omogućiti tehničarima pregled osnovnih informacija o dodijeljenim otvorenim tiketima radi efikasnijeg rada support sistema.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-25 Zatvaranje tiketa | US-16, US-17 | Ajdin | To Do | Implementacija zatvaranja tiketa od strane korisnika i zahtjeva za zatvaranje od strane agenta |
| SB-02 | PB-28 Upravljanje prioritetima tiketa | US-21 | Ajdin | To Do | Implementacija korisničkog i internog prioriteta tiketa |
| SB-03 | PB-30 Automatska dodjela tiketa | US-25 | Uma | To Do | Implementacija automatske dodjele i pravila dodjele tiketa |
| SB-04 | PB-37 Tehničar vidi osnovne informacije | US-39, US-49 | Eldar | To Do | Prikaz samo otvorenih assigned tiketa sa osnovnim informacijama |
| SB-05 | PB-31 Prosljeđivanje tiketa | US-?? | Hana, Lamija | To Do | Implementacija prosljeđivanja tiketa između agenata |
| SB-06 | PB-48 Pregled dodijeljenih i historija tiketa za agente | US-53, US-54 | Merisa | To Do | Implementacija pregleda aktivnih i zatvorenih tiketa po agentu |

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
- Ako korisnik ne odgovori na zahtjev zatvaranja unutar 7 dana od zadnje poruke, tada se tiket automatski zatvara, a sistem evidentira koji je agent inicirao zahtjev za zatvaranje  
- Sistem mora omogućiti praćenje statusa zahtjeva za zatvaranje, automatsko zatvaranje tiketa nakon 7 dana bez odgovora, i prikaz agenta koji je zatvorio tiket  

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

### Lamija i Hana trebaju definisati svoje US kako su rijesile problem
### paziti da broj US-a nije vec iskoristen, US-27 smatrajte da ne postoji
### ovo ce Lejan evidentirari kroz DecisionLog
primjer:

us-1: kao agent želim da proslijedim tiket drugom agentu koji će riješiti problem
us-2: Kao agent želim da proslijedim tiket tehničarima koji su na toj lokaciji kako bi ....
us-3 kao agent zelim da proslijedim tiket drugom "TIMU" agenata kako bi neko iz njihove stručnosti rijesio ovaj problem

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