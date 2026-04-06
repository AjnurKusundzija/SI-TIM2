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
- od US-13 pa nadalje ispraviti ID **nakon** što se isto uradi sa UserStorys

  

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

# PB-21 Prikaz paketa i pretplata

### US-6: *Kao korisnik, želim da vidim svoje aktivne pakete i pretplate, kako bih imao pregled usluga koje koristim.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako pristupi sekciji "Paketi", tada vidi listu svojih aktivnih paketa i pretplata  
- Kada korisnik ima više paketa, tada sistem prikazuje sve aktivne pakete u listi  
- Sistem mora prikazati osnovne informacije o svakom paketu 
- Sistem ne smije prikazivati pakete koji nisu povezani sa korisnikom  
- Korisnik treba dobiti jasan pregled svojih aktivnih usluga  

---

### US-7: *Kao korisnik, želim da vidim detalje paketa (internet, TV, mobilni), kako bih razumio šta moj paket uključuje.*

**Acceptance Criteria:**

- Kada korisnik odabere paket, tada sistem prikazuje detalje paketa  
- Kada paket sadrži više usluga, tada sistem prikazuje sve uključene usluge  
- Sistem mora prikazati relevantne informacije (brzina interneta, broj kanala, mobilni podaci)  
- Sistem ne smije prikazivati netačne ili nepotpune informacije o paketu  
- Korisnik treba dobiti jasan i razumljiv prikaz sadržaja paketa  

---

# PB-22 Kreiranje novog tiketa

### US-8: *Kao korisnik, želim da kreiram novi tiket unosom problema, kako bih prijavio poteškoću.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako popuni formu i klikne "Pošalji", tada se tiket kreira  
- Kada tiket bude kreiran, tada se sprema u bazu podataka  
- Sistem mora omogućiti unos potrebnih podataka za tiket  
- Sistem ne smije dozvoliti kreiranje tiketa bez obaveznih polja  
- Korisnik treba dobiti potvrdu o uspješnom kreiranju tiketa  

---

### US-9: *Kao korisnik, želim da odaberem tip i prioritet tiketa, kako bih bolje opisao problem.*

**Acceptance Criteria:**

- Kada korisnik kreira tiket, tada može odabrati tip tiketa  
- Kada korisnik kreira tiket, tada može odabrati prioritet tiketa  
- Sistem mora ponuditi unaprijed definisane opcije tipa i prioriteta  
- Sistem ne smije dozvoliti unos nepostojećih vrijednosti  
- Korisnik treba vidjeti jasno označene opcije za izbor  

---

### US-10: *Kao korisnik, želim da unesem opis problema, kako bih agentu dao dovoljno informacija.*

**Acceptance Criteria:**

- Kada korisnik popunjava formu za novi tiket, ako unese opis problema, tada se opis sprema uz tiket    
- Kada korisnik pokuša kreirati tiket bez opisa, tada sistem prikazuje grešku  
- Sistem mora omogućiti unos tekstualnog opisa  
- Sistem ne smije dozvoliti prazan opis problema  
- Korisnik treba dobiti povratnu informaciju ako opis nedostaje  

---

# PB-23 Pregled vlastitih tiketa

### US-11: *Kao korisnik, želim da vidim listu svih svojih tiketa, kako bih pratio njihove statuse.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako pristupi sekciji “Moji tiketi”, tada vidi listu svojih tiketa  
- Kada korisnik ima više tiketa, tada sistem prikazuje sve tikete u listi  
- Sistem mora prikazati osnovne informacije (naslov, status, datum)  
- Sistem ne smije prikazivati tikete drugih korisnika  
- Korisnik treba dobiti pregled svih svojih tiketa  

---

### US-12: *Kao korisnik, želim da vidim status tiketa (otvoren, zatvoren), kako bih znao u kojoj fazi je rješavanje.*

**Acceptance Criteria:**

- Kada korisnik pregleda svoje tikete, tada vidi njihov status  
- Kada se status promijeni, tada se ažurira prikaz statusa  
- Sistem mora koristiti jasno definisane statuse  
- Sistem ne smije prikazivati nepoznate statuse  
- Korisnik treba dobiti jasan prikaz trenutnog statusa tiketa  

---

### US-13: *Kao korisnik, želim mogućnost filtriranja tiketa (po prioritetu, datumu i slično), kako bih lakše pronašao željeni tiket.*

**Acceptance Criteria:**

- Kada korisnik pregledava svoje tikete, ako odabere opciju za filtriranje, tada sistem prikazuje filtrirani prikaz tiketa  
- Kada primijenjeni filter ne daje rezultate, tada sistem prikazuje poruku da nema odgovarajućih tiketa  
- Sistem mora omogućiti filtriranje po prioritetu, datumu, statusu i tipu  
- Sistem ne smije prikazivati tikete koji ne odgovaraju odabranim filterima  
- Korisnik treba dobiti tačne rezultate filtriranja   

---

# PB-24 Detaljan prikaz tiketa

### US-13: *Kao korisnik, želim da vidim detalje tiketa, kako bih imao potpuni uvid u problem.*

**Acceptance Criteria:**

- Kada korisnik pregledava listu svojih tiketa, ako odabere jedan tiket, tada sistem prikazuje njegove detalje   
- Sistem mora prikazati sve relevantne informacije (opis, status, datum)  
- Sistem ne smije prikazivati nepotpune podatke  
- Korisnik treba dobiti jasan prikaz svih informacija o tiketu  

---

### US-14: *Kao korisnik, želim da vidim historiju komunikacije, kako bih pratio tok rješavanja.*

**Acceptance Criteria:**

- Kada korisnik pregleda tiket, tada vidi historiju komunikacije  
- Kada postoji više poruka, tada se prikazuju hronološki  
- Sistem mora prikazati pošiljaoca i vrijeme poruke  
- Sistem ne smije izostaviti nijednu poruku  
- Korisnik treba dobiti jasan pregled komunikacije  

---

# PB-25 Zatvaranje tiketa

### US-15: *Kao korisnik, želim da zatvorim tiket kada je problem riješen, kako bih završio proces.*

**Acceptance Criteria:**

- Kada je tiket riješen, ako korisnik klikne “Zatvori tiket”, tada se status mijenja u zatvoren 
- Sistem ne smije dozvoliti zatvaranje već zatvorenog tiketa  
- Korisnik treba dobiti potvrdu o zatvaranju tiketa  

---

### US-16: *Kao agent ili tehničar, želim da zatvorim tiket nakon rješavanja problema, kako bih označio zadatak kao završen.*

**Acceptance Criteria:**

- Kada agent želi zatvoriti tiket, ako smatra da je zadatak završen, tada može poslati zahtjev za zatvaranje korisniku  
- Kada korisnik primi zahtjev za zatvaranje, tada može prihvatiti ili odbiti zatvaranje tiketa  
- Ako korisnik prihvati zahtjev, tada se tiket zatvara i poprima status zatvoren  
- Ako korisnik odbije zahtjev, tada tiket ostaje otvoren
- Ako korisnik ne odgovori na zahtjev zatvaranja unutar 7 dana od zadnje poruke, tada se tiket automatski zatvara  
- Sistem mora omogućiti praćenje statusa zahtjeva za zatvaranje i automatsko zatvaranje tiketa nakon 7 dana bez odgovora  

---

# PB-26 Ocjenjivanje tiketa

### US-17: *Kao korisnik, želim da ocijenim rješenje tiketa, kako bih dao feedback o kvaliteti usluge.*

**Acceptance Criteria:**

- Kada je tiket zatvoren, ako korisnik klikne na opciju "Ocijeni", tada sistem omogućava unos i slanje ocjene  
- Kada korisnik pošalje ocjenu, tada se ona sprema u sistem  
- Sistem mora omogućiti izbor ocjene  
- Sistem ne smije dozvoliti ocjenjivanje otvorenog tiketa  
- Korisnik treba dobiti potvrdu o uspješnom slanju ocjene  

---

# PB-27 Komunikacija kroz tiket

### US-18: *Kao korisnik, želim da šaljem poruke kroz tiket, kako bih komunicirao sa agentom.*

**Acceptance Criteria:**

- Kada korisnik unese novu poruku, ako klikne na dugme za slanje, tada se poruka sprema i prikazuje u historiji komunikacije    
- Sistem mora omogućiti unos poruke  
- Sistem ne smije dozvoliti slanje prazne poruke  
- Korisnik treba vidjeti svoju poslanu poruku  

---

### US-19: *Kao agent, želim da odgovaram na poruke korisnika, kako bih riješio problem.*

**Acceptance Criteria:**

- Kada agent napiše odgovor na korisnikov upit, ako klikne na dugme za slanje odgovora, tada se poruka sprema i prikazuje korisniku  
- Kada postoji nova poruka, tada se dodaje u historiju komunikacije  
- Sistem mora omogućiti agentu slanje poruka  
- Sistem ne smije dozvoliti slanje praznih poruka  
- Korisnik treba vidjeti odgovor agenta   

---

# PB-28 Upravljanje prioritetima tiketa

### US-20: *Kao agent, želim da postavim interni prioritet tiketa, kako bih efikasno upravljao zadacima.*

**Acceptance Criteria:**

- Kada agent pregleda tiket, ako postavi prioritet, tada se prioritet sprema uz tiket  
- Kada agent promijeni prioritet, tada se ažurira prikaz  
- Sistem mora omogućiti izbor prioriteta iz definisane liste  
- Sistem ne smije dozvoliti nepostojeće vrijednosti prioriteta  
- Agent treba vidjeti trenutno postavljeni prioritet  

---

### US-21: *Kao korisnik, želim da postavim prioritet svog problema, kako bih označio hitnost.*

**Acceptance Criteria:**

- Kada korisnik kreira tiket, ako odabere prioritet, tada se prioritet sprema uz tiket  
- Sistem mora ponuditi unaprijed definisane prioritete  
- Sistem ne smije dozvoliti nevažeće vrijednosti  
- Sistem ne smije dozvoliti kreiranje tiketa bez postavljanja prioriteta  
- Korisnik treba vidjeti odabrani prioritet  

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
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten 

---

### US-45: *Kao tehničar, želim da imam izvještaj po tipovima problema u tiketima.*

**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi ukupni broj tiketa koji su raspoređeni njemu po tipu problema 
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima
- Sistem mora grupisati tikete po tipu problema  
- Korisnik treba dobiti poruku ako nema dostupnih podataka
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten 

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
