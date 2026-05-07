# Sprint Backlog – Sprint 6

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati ključne funkcionalnosti za pregled, komunikaciju i upravljanje tiketima (detaljan prikaz, komunikacija kroz tiket, pregled svih tiketa i FAQ segment) radi poboljšanja korisničkog iskustva i efikasnosti support sistema.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-24 Detaljan prikaz tiketa | US-14, US-15 | Lejan | Done | Implementiran prikaz detalja i historije komunikacije |
| SB-02 | PB-27 Komunikacija kroz tiket | US-19, US-20 | Ajdin, Uma | Done | Implementirana razmjena poruka korisnik–agent |
| SB-03 | PB-32 Pregled svih tiketa | US-29, US-30 | Eldar | Done | Implementiran pregled svih tiketa za agente |
| SB-04 | PB-47 FAQ Segment | US-56 | Ajnur | Done | Implementiran FAQ modul |
| SB-05 | Pisanje testova po Test Strategy dokumentu za Sprint 6 funkcionalnosti i ProofOfTesting dokumenta | US-14, US-15, US-19, US-20, US-29, US-30, US-56 | Ajnur, Hana, Lamija, Merisa | Done | Testovi za backend i frontend (komunikacija, tiket pregled, FAQ, detalji tiketa) |

---

# Detaljni User Stories (US)

---

## PB-24 Detaljan prikaz tiketa

### US-14
*Kao korisnik, želim da vidim detalje tiketa, kako bih imao potpuni uvid u problem.*

**Acceptance Criteria:**
- Kada korisnik pregledava listu svojih tiketa, ako odabere jedan tiket, tada sistem prikazuje njegove detalje  
- Sistem mora prikazati sve relevantne informacije (opis, status, datum)  
- Sistem ne smije prikazivati nepotpune podatke  
- Korisnik treba dobiti jasan prikaz svih informacija o tiketu  

---

### US-15
*Kao korisnik, želim da vidim historiju komunikacije, kako bih pratio tok rješavanja.*

**Acceptance Criteria:**
- Kada korisnik pregleda tiket, tada vidi historiju komunikacije  
- Kada postoji više poruka, tada se prikazuju hronološki  
- Sistem mora prikazati pošiljaoca i vrijeme poruke  
- Sistem ne smije izostaviti nijednu poruku  
- Korisnik treba dobiti jasan pregled komunikacije  

---

## PB-27 Komunikacija kroz tiket

### US-19
*Kao korisnik, želim da šaljem poruke kroz tiket, kako bih komunicirao sa agentom.*

**Acceptance Criteria:**
- Kada korisnik unese novu poruku, ako klikne na dugme za slanje, tada se poruka sprema i prikazuje u historiji komunikacije  
- Sistem mora omogućiti unos poruke  
- Sistem ne smije dozvoliti slanje prazne poruke  
- Sistem mora ograničiti korisnika na maksimalno 3 poruke po ciklusu (inicijalno 3, a nakon svakog odgovora agenta, korisnik dobija ponovo do 3 poruke)  
- Sistem mora ograničiti dužinu poruke na 1000 karaktera  
- Korisnik treba vidjeti svoju poslanu poruku  

---

### US-20
*Kao agent, želim da odgovaram na poruke korisnika, kako bih riješio problem.*

**Acceptance Criteria:**
- Kada agent napiše odgovor na korisnikov upit, ako klikne na dugme za slanje odgovora, tada se poruka sprema i prikazuje korisniku  
- Kada postoji nova poruka, tada se dodaje u historiju komunikacije  
- Sistem mora omogućiti agentu slanje poruka  
- Sistem ne smije dozvoliti slanje praznih poruka  
- Korisnik treba vidjeti odgovor agenta   

---

## PB-32 Pregled svih tiketa

### US-29
*Kao agent, želim da vidim sve tikete, kako bih imao potpuni pregled.*

**Acceptance Criteria:**
- Kada agent otvori listu tiketa, ako se podaci učitaju, tada sistem mora prikazati sve tikete
- Kada postoji veliki broj tiketa, ako korisnik skrola ili traži više, tada sistem mora omogućiti učitavanje dodatnih rezultata
- Sistem prikazuje sve tikete neovisno od stanja 


---

### US-30
*Kao agent, želim da vidim detalje svakog tiketa, kako bih imao detaljniji uvid.*

**Acceptance Criteria:**
- Kada agent otvori tiket, ako tiket postoji, tada sistem mora prikazati sve informacije o tiketu
- Kada agent koristi sistem, ako pristupa tiketu, tada sistem ne smije ograničiti pristup
---

## PB-47 FAQ Segment

### US-56
*Kao korisnik, želim da vidim listu najčešće postavljanih pitanja koje su vezane za razne probleme, kako bih mogao eventualno riješiti problem bez postavljanja tiketa.*

**Acceptance Criteria:**
- Kada korisnik otvori FAQ sekciju, tada vidi listu pitanja i odgovora  
- Kada podaci postoje, tada su prikazani tačni odgovori  
- Sistem mora omogućiti pregled svih FAQ stavki  
- Sistem ne smije prikazati praznu listu bez obavještenja  
- Korisnik treba dobiti poruku ako nema dostupnih pitanja   

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.