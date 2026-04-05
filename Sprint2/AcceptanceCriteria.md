# Acceptance Criteria

## Template:

Za svaki User Story koristiti sljedeće obrasce:

### 1. Početni šablon
- Kada [početni uslov], ako [akcija], tada [očekivani rezultat]

 **Kada** - opisuje početno stanje sistema  
 **Ako** - akcija korisnika ili događaj  
 **Tada** - očekivani rezultat
### 2. Sistem pravila
- Sistem mora omogućiti…
- Sistem ne smije dozvoliti…
- Korisnik treba dobiti…

---

## Napomena:
Prema uputama sa prošlog sastanka: 
- svaki kriterij mora biti testabilan  
- izbjegavati nejasne izraze poput “brzo”, “dobro”, “efikasno”  

  

# PB-19 Login korisnika

### US-1: *Kao registrovani korisnik, želim da se prijavim u sistem koristeći email i lozinku, kako bih pristupio svom nalogu.*

**Acceptance Criteria:**

- Kada korisnik nije prijavljen, ako unese ispravan email i lozinku, tada se uspješno prijavljuje u sistem  
- Kada korisnik unese validne podatke, ako klikne na “Login”, tada se preusmjerava na početnu stranicu  
- Sistem mora omogućiti unos emaila i lozinke  
- Sistem ne smije dozvoliti prijavu bez unosa obaveznih polja  
- Korisnik treba dobiti potvrdu o uspješnoj prijavi  

---

### US-2: *Kao korisnik, želim da se odjavim iz sistema, kako bih zaštitio svoj nalog.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako klikne na “Logout”, tada se odjavljuje iz sistema  
- Kada se korisnik odjavi, tada se preusmjerava na login stranicu  
- Sistem mora onemogućiti pristup zaštićenim stranicama nakon logout-a  
- Korisnik ne smije ostati autentifikovan nakon odjave  

---

### US-3: *Kao korisnik, želim da budem obaviješten ako unesem pogrešne podatke za prijavu, kako bih mogao ponovo pokušati.* 

**Acceptance Criteria:**

- Kada korisnik unese pogrešan email ili lozinku, ako pokuša login, tada sistem odbija prijavu  
- Kada login ne uspije, tada korisnik dobija poruku o grešci  
- Sistem ne smije otkriti da li je email ili lozinka pogrešna   
- Korisnik treba dobiti mogućnost ponovnog pokušaja prijave  

---

# PB-20 Upravljanje korisničkim profilom

### US-4: *Kao korisnik, želim da promijenim svoj email, kako bih imao tačne kontakt informacije.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako unese novi email i potvrdi izmjenu, tada se email ažurira u sistemu  
- Kada korisnik unese nevalidan email, tada sistem prikazuje grešku  
- Sistem mora provjeriti format email adrese  
- Sistem ne smije dozvoliti unos već postojećeg emaila u sistemu  
- Korisnik treba dobiti potvrdu o uspješnoj promjeni emaila  

---

### US-5: *Kao korisnik, želim da promijenim lozinku, kako bih povećao sigurnost svog naloga.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako unese trenutnu i novu lozinku, tada se lozinka uspješno mijenja  
- Kada korisnik unese pogrešnu trenutnu lozinku, tada sistem odbija promjenu  
- Sistem mora zahtijevati unos trenutne lozinke prije promjene  
- Sistem mora provjeriti minimalne sigurnosne zahtjeve lozinke  
- Sistem ne smije dozvoliti lozinku koja ne zadovoljava sigurnosne zahtjeve 
- Korisnik treba dobiti potvrdu o uspješnoj promjeni lozinke  

---

# PB-37 Tehničar vidi osnovne informacije

### US-38: *Kao tehničar, želim da vidim osnovne informacije o tiketu, kako bih razumio problem.*

**Acceptance Criteria:**

- Kada je tehničar prijavljen i otvori tiket, tada vidi osnovne informacije (naslov, opis, status, prioritet, datum kreiranja)  
- Kada tiket postoji u sistemu, ako tehničar pristupi detaljima tiketa, tada se prikazuju tačni podaci iz baze  
- Sistem mora omogućiti prikaz osnovnih informacija bez dodatnih akcija  
- Sistem ne smije prikazati nepostojeći ili obrisan tiket  
- Tehničar treba dobiti poruku o grešci ako tiket nije dostupan  

---

### US-39: *Kao tehničar, želim da vidim podatke o korisniku, kako bih imao kontekst.*

**Acceptance Criteria:**

- Kada tehničar otvori tiket, tada vidi osnovne podatke o korisniku (ime, kontakt, ID korisnika)  
- Kada korisnik postoji u sistemu, ako tehničar pregleda tiket, tada se prikazuju tačni korisnički podaci  
- Sistem mora povezati tiket sa odgovarajućim korisnikom  
- Sistem ne smije prikazati podatke o korisniku koji nije povezan sa tiketom
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Tehničar treba dobiti poruku ako korisnički podaci nisu dostupni  

---

# PB-38 Izvještaj o broju tiketa

### US-40: *Kao administrator, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani kako bih imao uvid o situaciji i količini tiketa.*

**Acceptance Criteria:**

- Kada administrator odabere vremenski period (dnevni, sedmični, mjesečni, godišnji), tada se prikazuje ukupan broj tiketa za taj period 
- Kada podaci postoje u sistemu, tada podaci odgovaraju stvarnom stanju u bazi   
- Sistem mora omogućiti izbor vremenskog perioda
- Sistem mora omogućiti pregled izvještaja administratoru
- Sistem ne smije prikazati pogrešne ili duplirane podatke
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Administrator treba dobiti poruku ako nema podataka za odabrani period  

---

### US-41: *Kao tehničar, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani, kako bih imao uvid o količini tiketa.*

**Acceptance Criteria:**

- Kada tehničar odabere vremenski period, tada vidi broj tiketa koji su raspoređeni njemu za taj period  
- Kada podaci postoje u sistemu, tada podaci odgovaraju stvarnom stanju u bazi
- Sistem mora omogućiti izbor vremenskog perioda  
- Sistem mora omogućiti pregled izvještaja tehničaru
- Sistem ne smije prikazati pogrešne ili duplirane podatke  
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Tehničar treba dobiti poruku ako nema podataka za odabrani period 

---

# PB-39 Izvještaj po statusu tiketa

### US-42: *Kao administrator, želim da imam izvještaj o statusu tiketa, kako bih mogao lakše analizirati.*

**Acceptance Criteria:**

- Kada administrator otvori izvještaj, tada vidi ukupni broj tiketa po statusima (otvoren, u toku, zatvoren)  
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima  
- Sistem mora grupisati tikete po statusu  
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Administrator treba dobiti poruku ako nema podataka

---

### US-43: *Kao tehničar, želim da imam izvještaj o statusu tiketa, kako bih imao uvid o stanju.*

**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi raspodjelu svojih tiketa po statusima  
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima  
- Sistem mora omogućiti pristup izvještaju tehničaru  
- Sistem ne smije dozvoliti pristup neovlaštenim korisnicima  
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Tehničar treba dobiti poruku ako nema podataka

---

# PB-40 Izvještaj po tipu problema

### US-44: *Kao administrator, želim da imam izvještaj po tipovima problema u tiketima.*

**Acceptance Criteria:**

- Kada administrator otvori izvještaj, tada vidi ukupni broj tiketa po tipu problema  
- Kada podaci postoje, tada su prikazani tačni tipovi i količine  
- Sistem mora grupisati tikete po tipu problema  
- Korisnik treba dobiti poruku ako nema podataka  

---

### US-45: *Kao tehničar, želim da imam izvještaj po tipovima problema u tiketima.*

**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi tipove problema i broj tiketa  
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima  
- Sistem mora omogućiti pregled izvještaja  
- Sistem ne smije dozvoliti pristup bez odgovarajuće uloge  
- Korisnik treba dobiti poruku ako nema dostupnih podataka  

---

# PB-41 Prosječno vrijeme rješavanja tiketa

### US-46: *Kao administrator, želim da imam uvid o prosječnom vremenu rješavanja tiketa.*

**Acceptance Criteria:**

- Kada administrator otvori izvještaj, tada vidi prosječno vrijeme zatvaranja tiketa  
- Kada podaci postoje, tada je izračun tačan (vrijeme zatvaranja - vrijeme kreiranja)  
- Sistem mora izračunati prosjek na osnovu svih zatvorenih tiketa  
- Sistem ne smije uključiti nezatvorene tikete u izračun  
- Korisnik treba dobiti poruku ako nema podataka  

---

### US-47: *Kao tehničar, želim da imam uvid o prosječnom vremenu rješavanja tiketa.*

**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi prosječno vrijeme rješavanja  
- Kada podaci postoje, tada izračun odgovara stvarnim podacima  
- Sistem mora omogućiti pristup izvještaju  
- Sistem ne smije dozvoliti pristup neovlaštenim korisnicima  
- Korisnik treba dobiti poruku ako nema dostupnih podataka  

---

# PB-42 Vrijeme prvog odgovora

### US-48–US-52: *Uvid u vrijeme prvog odgovora na tiket (administrator, tehničar, korisnik).*

**Acceptance Criteria:**

- Kada korisnik otvori tiket, tada vidi vrijeme prvog odgovora  
- Kada odgovor postoji, tada se prikazuje tačan timestamp prvog odgovora  
- Sistem mora zabilježiti vrijeme prvog odgovora na tiket  
- Sistem ne smije mijenjati vrijeme prvog odgovora nakon inicijalnog zapisa  
- Korisnik treba dobiti informaciju ako odgovor još nije poslan  

---

# PB-43 Izvještaj o opterećenju agenata

### US-53: *Kao administrator, želim da imam uvid u broj riješenih tiketa po agentu.*

**Acceptance Criteria:**

- Kada administrator odabere period, tada vidi broj riješenih tiketa po agentu  
- Kada podaci postoje, tada izvještaj prikazuje tačne vrijednosti  
- Sistem mora omogućiti filtriranje po vremenskom periodu  
- Sistem ne smije prikazati netačne ili duplirane podatke  
- Korisnik treba dobiti poruku ako nema podataka  

---

# PB-44 Izvještaj o ocjenama korisnika

### US-54: *Kao agent, želim da analiziram ocjene koje korisnici ostavljaju nakon zatvaranja tiketa.*

**Acceptance Criteria:**

- Kada agent otvori izvještaj, tada vidi prosječnu ocjenu i listu ocjena  
- Kada podaci postoje, tada su prikazane tačne ocjene iz sistema  
- Sistem mora prikazati samo ocjene vezane za zatvorene tikete  
- Sistem ne smije prikazati ocjene za otvorene tikete  
- Korisnik treba dobiti poruku ako nema dostupnih ocjena  

---

# PB-45 Admin Dashboard sa ključnim metrikama

### US-55: *Kao administrator, želim da imam dashboard sa ključnim metrikama.*

**Acceptance Criteria:**

- Kada administrator otvori dashboard, tada vidi ključne metrike (broj tiketa, statusi, prosječno vrijeme)  
- Kada podaci postoje, tada su svi prikazi ažurirani i tačni  
- Sistem mora prikazati više metrika na jednoj stranici  
- Sistem ne smije prikazati nevažeće ili zastarjele podatke  
- Korisnik treba dobiti poruku ako podaci nisu dostupni  

---

# PB-46 Export Izvještaja

### US-56: *Kao tehničar, želim da imam mogućnost exporta izvještaja u CSV format.*

**Acceptance Criteria:**

- Kada tehničar klikne na “Export CSV”, tada se generiše CSV fajl  
- Kada podaci postoje, tada fajl sadrži tačne podatke iz izvještaja  
- Sistem mora omogućiti preuzimanje fajla  
- Sistem ne smije generisati prazan fajl bez upozorenja  
- Korisnik treba dobiti poruku ako export nije moguć  

---

# PB-47 FAQ Segment

### US-57: *Kao korisnik, želim da vidim listu najčešće postavljanih pitanja.*

**Acceptance Criteria:**

- Kada korisnik otvori FAQ sekciju, tada vidi listu pitanja i odgovora  
- Kada podaci postoje, tada su prikazani tačni odgovori  
- Sistem mora omogućiti pregled svih FAQ stavki  
- Sistem ne smije prikazati praznu listu bez obavještenja  
- Korisnik treba dobiti poruku ako nema dostupnih pitanja  

---
