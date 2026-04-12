# Dokumentacija Korisničkih Priča (User Stories)

## Navigacija po sprintovima

| Sprint | Povezani zahtjevi (Product Backlog Items) |
| :--- | :--- |
| [**Sprint 5**](#sprint-5) | PB-19 (Login korisnika), PB-22 (Kreiranje novog tiketa) |
| [**Sprint 6**](#sprint-6) | PB-23 (Pregled vlastitih tiketa), PB-24 (Detaljan prikaz tiketa), PB-47 (FAQ Segment) |
| [**Sprint 7**](#sprint-7) | PB-25 (Zatvaranje tiketa), PB-27 (Komunikacija kroz tiket), PB-28 (Upravljanje prioritetima tiketa), PB-37 (Tehničar vidi osnovne informacije) |
| [**Sprint 8**](#sprint-8) | PB-26 (Ocjenjivanje tiketa), PB-33 (Pretraživanje i filtriranje tiketa), PB-36 (Ažuriranje statusa tiketa), PB-42 (Vrijeme prvog odgovora) |
| [**Sprint 9**](#sprint-9) | PB-20 (Upravljanje korisničkim profilom), PB-21 (Prikaz paketa i pretplata), PB-34 (Pregled i uređivanje korisničkih profila) |
| [**Sprint 10**](#sprint-10) | PB-29 (Preraspodjela agenata po timovima), PB-30 (Automatska dodjela tiketa), PB-31 (Prosljeđivanje tiketa), PB-32 (Pregled svih tiketa), PB-35 (Pregled dodijeljenih tiketa) |
| [**Sprint 11**](#sprint-11) | PB-38, PB-39, PB-40, PB-41, PB-43, PB-44, PB-45, PB-46 (Razni izvještaji i dashboard) |

---

<a id="sprint-5"></a>
# Sprint 5

## PB-19 Login korisnika

### User Stories

<a id="us-1"></a>
### US-1: *Kao registrovani korisnik, želim da se prijavim u sistem koristeći email i lozinku, kako bih pristupio svom nalogu.*
**Acceptance Criteria:**

- Kada korisnik nije prijavljen, ako unese ispravan email i lozinku, tada se uspješno prijavljuje u sistem  
- Kada korisnik unese validne podatke, ako klikne na “Login”, tada se preusmjerava na početnu stranicu  
- Sistem mora omogućiti unos emaila i lozinke  
- Sistem ne smije dozvoliti prijavu bez unosa obaveznih polja  
- Korisnik treba dobiti potvrdu o uspješnoj prijavi 
---    
<a id="us-2"></a>
### US-2: *Kao korisnik, želim da se odjavim iz sistema, kako bih zaštitio svoj nalog.*
**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako klikne na “Logout”, tada se odjavljuje iz sistema  
- Kada se korisnik odjavi, tada se preusmjerava na login stranicu  
- Sistem mora onemogućiti pristup zaštićenim stranicama nakon logout-a  
- Korisnik ne smije ostati autentifikovan nakon odjave  
---
<a id="us-3"></a>
### US-3: *Kao korisnik, želim da budem obaviješten ako unesem pogrešne podatke za prijavu, kako bih mogao ponovo pokušati.*
**Acceptance Criteria:**

- Kada korisnik unese pogrešan email ili lozinku, ako pokuša login, tada sistem odbija prijavu  
- Kada login ne uspije, tada korisnik dobija poruku o grešci  
- Sistem ne smije otkriti da li je email ili lozinka pogrešna   
 - Korisnik treba dobiti mogućnost ponovnog pokušaja prijave 
---
### Poslovna vrijednost

Omogućava korisnicima pristup sistemu i svim ostalim funkcionalnostima. Bez login-a sistem nema smisla jer korisnici ne mogu koristiti usluge.

### Prioritet

1

### Zavisnosti

Zavisi od postojanja korisničkih naloga u bazi podataka.

### Pretpostavke

- Korisnici već imaju kreirane naloge
- Sistem podržava autentikaciju putem emaila i lozinke

### Otvorena pitanja

- Da li je potrebna dvofaktorska autentikacija?
- Koliko pokušaja logina je dozvoljeno?

### Poveznica sa sprintom
Sprint 5

---

## PB-22 Kreiranje novog tiketa

### User Stories

<a id="us-8"></a>
### US-8: *Kao korisnik, želim da kreiram novi tiket unosom problema, kako bih prijavio poteškoću.*
**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako popuni formu i klikne "Pošalji", tada se tiket kreira  
- Kada tiket bude kreiran, tada se sprema u bazu podataka  
- Sistem mora omogućiti unos potrebnih podataka za tiket  
- Sistem ne smije dozvoliti kreiranje tiketa bez obaveznih polja  
- Korisnik treba dobiti potvrdu o uspješnom kreiranju tiketa  
---
<a id="us-9"></a>
### US-9: *Kao korisnik, želim da odaberem tip i prioritet tiketa, kako bih bolje opisao problem.*
**Acceptance Criteria:**

- Kada korisnik kreira tiket, tada može odabrati tip tiketa  
- Kada korisnik kreira tiket, tada može odabrati prioritet tiketa  
- Sistem mora ponuditi unaprijed definisane opcije tipa i prioriteta  
- Sistem ne smije dozvoliti unos nepostojećih vrijednosti  
- Korisnik treba vidjeti jasno označene opcije za izbor 
---
<a id="us-10"></a>
### US-10: *Kao korisnik, želim da unesem opis problema, kako bih agentu dao dovoljno informacija.*
**Acceptance Criteria:**

- Kada korisnik popunjava formu za novi tiket, ako unese opis problema, tada se opis sprema uz tiket    
- Kada korisnik pokuša kreirati tiket bez opisa, tada sistem prikazuje grešku  
- Sistem mora omogućiti unos tekstualnog opisa  
- Sistem ne smije dozvoliti prazan opis problema  
- Korisnik treba dobiti povratnu informaciju ako opis nedostaje  

---

### Poslovna vrijednost

Osnovna funkcionalnost sistema jer omogućava prijavu problema.

### Prioritet

1

### Zavisnosti

Zavisi od login funkcionalnosti.

### Pretpostavke

- Korisnik mora biti prijavljen
- Sistem podržava čuvanje tiketa

### Otvorena pitanja

- Koja polja su obavezna?
- Da li se mogu dodavati prilozi?

### Poveznica sa sprintom
Sprint 5

---

<a id="sprint-6"></a>
# Sprint 6

## PB-23 Pregled vlastitih tiketa

### User Stories

<a id="us-11"></a>
### US-11: *Kao korisnik, želim da vidim listu svih svojih tiketa, kako bih pratio njihove statuse.*
**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako pristupi sekciji “Moji tiketi”, tada vidi listu svojih tiketa  
- Kada korisnik ima više tiketa, tada sistem prikazuje sve tikete u listi  
- Sistem mora prikazati osnovne informacije (naslov, status, datum)  
- Sistem ne smije prikazivati tikete drugih korisnika  
- Korisnik treba dobiti pregled svih svojih tiketa  

---
<a id="us-12"></a>
### US-12: *Kao korisnik, želim da vidim status tiketa (otvoren, zatvoren), kako bih znao u kojoj fazi je rješavanje.*
**Acceptance Criteria:**

- Kada korisnik pregleda svoje tikete, tada vidi njihov status  
- Kada se status promijeni, tada se ažurira prikaz statusa  
- Sistem mora koristiti jasno definisane statuse  
- Sistem ne smije prikazivati nepoznate statuse  
- Korisnik treba dobiti jasan prikaz trenutnog statusa tiketa  

---
<a id="us-13"></a>
### US-13: *Kao korisnik, želim mogućnost filtriranja tiketa (po prioritetu, datumu i slično), kako bih lakše pronašao željeni tiket.*
**Acceptance Criteria:**

- Kada korisnik pregledava svoje tikete, ako odabere opciju za filtriranje, tada sistem prikazuje filtrirani prikaz tiketa  
- Kada primijenjeni filter ne daje rezultate, tada sistem prikazuje poruku da nema odgovarajućih tiketa  
- Sistem mora omogućiti filtriranje po prioritetu, datumu, statusu i tipu  
- Sistem ne smije prikazivati tikete koji ne odgovaraju odabranim filterima  
- Korisnik treba dobiti tačne rezultate filtriranja   

---

### Poslovna vrijednost

Omogućava korisnicima uvid u stanje zahtjeva.

### Prioritet

1

### Zavisnosti

Zavisi od kreiranja tiketa.

### Pretpostavke

- Tiketi su već kreirani i dostupni u bazi

### Otvorena pitanja

- Da li je potrebno ograničenje historije tiketa na određeni vremenski period?

### Poveznica sa sprintom
Sprint 6

---

## PB-24 Detaljan prikaz tiketa

### User Stories

<a id="us-14"></a>
### US-14: *Kao korisnik, želim da vidim detalje tiketa, kako bih imao potpuni uvid u problem.*
**Acceptance Criteria:**

- Kada korisnik pregledava listu svojih tiketa, ako odabere jedan tiket, tada sistem prikazuje njegove detalje   
- Sistem mora prikazati sve relevantne informacije (opis, status, datum)  
- Sistem ne smije prikazivati nepotpune podatke  
- Korisnik treba dobiti jasan prikaz svih informacija o tiketu  

---
<a id="us-15"></a>
### US-15: *Kao korisnik, želim da vidim historiju komunikacije, kako bih pratio tok rješavanja.*
**Acceptance Criteria:**

- Kada korisnik pregleda tiket, tada vidi historiju komunikacije  
- Kada postoji više poruka, tada se prikazuju hronološki  
- Sistem mora prikazati pošiljaoca i vrijeme poruke  
- Sistem ne smije izostaviti nijednu poruku  
- Korisnik treba dobiti jasan pregled komunikacije  

---

### Poslovna vrijednost

Omogućava detaljan uvid i transparentnost.

### Prioritet

1

### Zavisnosti

Zavisi od pregleda tiketa.

### Pretpostavke

- Postoje podaci o komunikaciji i statusu

### Otvorena pitanja

- Kako prikazati historiju komunikacije (timeline ili lista)?

### Poveznica sa sprintom
Sprint 6

---

## PB-47 FAQ Segment

### User Stories

<a id="us-56"></a>
### US-56: *Kao korisnik, želim da vidim listu najčešće postavljanih pitanja koje su vezane za razne probleme, kako bih mogao eventualno riješiti problem bez postavljanja tiketa*
**Acceptance Criteria:**

- Kada korisnik otvori FAQ sekciju, tada vidi listu pitanja i odgovora  
- Kada podaci postoje, tada su prikazani tačni odgovori  
- Sistem mora omogućiti pregled svih FAQ stavki  
- Sistem ne smije prikazati praznu listu bez obavještenja  
- Korisnik treba dobiti poruku ako nema dostupnih pitanja  
---
### Poslovna vrijednost
Eventualno smanjenje slanja tiketa 

### Prioritet
3

### Zavisnosti
Potrebno je analizirati najčešće probleme koji se javljaju u telekom sistemu radi efikasnog sastavljanja FAQ Segmenta

### Pretpostavke

Za svako pitanje u FAQ Segmentu postavljen je odgovor koji zasigurno rješava nedoumicu iz postavljenog pitanja

### Otvorena pitanja

- Koja pitanja u relevantna za ubacivanje u FAQ Segment?

### Poveznica sa sprintom
Sprint 6

---

<a id="sprint-7"></a>
# Sprint 7

## PB-25 Zatvaranje tiketa

### User Stories

<a id="us-16"></a>
### US-16: *Kao korisnik, želim da zatvorim tiket kada je problem riješen, kako bih završio proces.*
**Acceptance Criteria:**

- Kada je tiket riješen, ako korisnik klikne “Zatvori tiket”, tada se status mijenja u zatvoren 
- Sistem ne smije dozvoliti zatvaranje već zatvorenog tiketa  
- Korisnik treba dobiti potvrdu o zatvaranju tiketa  

---
<a id="us-17"></a>
### US-17: *Kao agent ili tehničar, želim da zatvorim tiket nakon rješavanja problema, kako bih označio zadatak kao završen.*
**Acceptance Criteria:**

- Kada agent želi zatvoriti tiket, ako smatra da je zadatak završen, tada može poslati zahtjev za zatvaranje korisniku  
- Kada korisnik primi zahtjev za zatvaranje, tada može prihvatiti ili odbiti zatvaranje tiketa  
- Ako korisnik prihvati zahtjev, tada se tiket zatvara, poprima status zatvoren i sistem evidentira koji je agent zatvorio tiket  
- Ako korisnik odbije zahtjev, tada tiket ostaje otvoren
- Ako korisnik ne odgovori na zahtjev zatvaranja unutar 7 dana od zadnje poruke, tada se tiket automatski zatvara, a sistem evidentira koji je agent inicirao zahtjev za zatvaranje 
- Sistem mora omogućiti praćenje statusa zahtjeva za zatvaranje, automatsko zatvaranje tiketa nakon 7 dana bez odgovora, i prikaz agenta koji je zatvorio tiket    

---

### Poslovna vrijednost

Omogućava upravljanje životnim ciklusom tiketa.

### Prioritet

1

### Zavisnosti

Zavisi od postojanja tiketa.

### Pretpostavke

- Tiket mora biti riješen prije zatvaranja

### Otvorena pitanja

- Može li se tiket ponovo otvoriti nakon zatvaranja?

### Poveznica sa sprintom
Sprint 7

---

## PB-27 Komunikacija kroz tiket

### User Stories

<a id="us-19"></a>
### US-19: *Kao korisnik, želim da šaljem poruke kroz tiket, kako bih komunicirao sa agentom.*
**Acceptance Criteria:**

- Kada korisnik unese novu poruku, ako klikne na dugme za slanje, tada se poruka sprema i prikazuje u historiji komunikacije    
- Sistem mora omogućiti unos poruke  
- Sistem ne smije dozvoliti slanje prazne poruke  
- Sistem mora ograničiti korisnika na maksimalno 3 poruke po ciklusu (inicijalno 3, a nakon svakog odgovora agenta, korisnik dobija ponovo do 3 poruke)  
- Sistem mora ograničiti dužinu poruke na maksimalan broj karaktera (1000 karaktera) 
- Korisnik treba vidjeti svoju poslanu poruku  

---
<a id="us-20"></a>
### US-20: *Kao agent, želim da odgovaram na poruke korisnika, kako bih riješio problem.*
**Acceptance Criteria:**

- Kada agent napiše odgovor na korisnikov upit, ako klikne na dugme za slanje odgovora, tada se poruka sprema i prikazuje korisniku  
- Kada postoji nova poruka, tada se dodaje u historiju komunikacije  
- Sistem mora omogućiti agentu slanje poruka  
- Sistem ne smije dozvoliti slanje praznih poruka  
- Korisnik treba vidjeti odgovor agenta   

---

### Poslovna vrijednost

Omogućava direktnu komunikaciju.

### Prioritet

1

### Zavisnosti

Zavisi od postojanja tiketa.

### Pretpostavke

- Sistem podržava razmjenu poruka

### Otvorena pitanja

- Da li postoji limit poruka ili notifikacije?

### Poveznica sa sprintom
Sprint 7

---

## PB-28 Upravljanje prioritetima tiketa

### User Stories

#### **US-21:** Kao agent, želim da postavim interni prioritet tiketa, kako bih efikasno upravljao zadacima.

**Acceptance Criteria:**
- Kada je agent prijavljen u sistem i nalazi se na stranici tiketa, ako odabere opciju za postavljanje prioriteta, tada sistem mora postaviti odabrani prioritet
- Sistem mora omogućiti listu predefinisanih prioriteta
- Kada tiket ima postavljen interni prioritet, ako korisnik pristupi tiketu, tada sistem ne smije dozvoliti prikaz niti izmjenu internog prioriteta
- Kada agent postavi ili izmijeni prioritet tiketa, ako je akcija uspješno izvršena, tada sistem mora prikazati potvrdu
- Kada agent postavi ili izmijeni prioritet tiketa, ako je akcija neuspješno izvršena, tada sistem mora prikazati poruku upozorenja
- Kada nema odabranog internog priroteta, tada sistem mora prikazati poruku da prioritet nije postavljen


#### **US-22:** Kao korisnik, želim da postavim prioritet svog problema, kako bih označio hitnost.

**Acceptance Criteria:**
- Kada je korisnik prijavljen u sistem i kreira novi tiket, ako dođe do koraka odabira prioriteta, tada sistem mora omogućiti izbor iz predefinisane liste
- Kada je korisnik dođe do koraka odabira prioriteta, ako odabere jednu od predefinisanih opcija, tada sistem mora dodijeliti tiketu taj atribut
- Kada korisnik unosi prioritet, ako vrijednost nije iz predefinisane liste, tada sistem ne smije dozvoliti nastavak
- Kada je tiket kreiran, ako korisnik pregleda tiket, tada sistem mora prikazati odabrani prioritet
- Kada agent pregleda postojeći tiket tada sistem mora prikazati korisnikov prioritet



### Poslovna vrijednost

Omogućava bolju organizaciju rada.

### Prioritet

2

### Zavisnosti

Zavisi od sistema za tikete.

### Pretpostavke

- Postoji definisana lista prioriteta

### Otvorena pitanja

- Ko može mijenjati prioritet i da li se vidi korisniku?

### Poveznica sa sprintom
Sprint 7

---

## PB-37 Tehničar vidi osnovne informacije

### User Stories

#### **US-39:** Kao tehničar, želim da vidim osnovne informacije o tiketu, kako bih razumio problem.

**Acceptance Criteria:**
- Kada tehničar pregleda tikete, ako otvori postojeći tiket, tada sistem mora prikazati osnovne informacije
- Kada se prikazuju podaci, ako su interne napomene prisutne, tada sistem ne smije prikazati te napomene
- Kada tehničar otvori ekran tiketa, ako su podaci učitani, tada sve informacije moraju biti dostupne bez dodatnih klikova
- Sistem ne smije omogućiti pregled tiketa koji nisu dodijeljeni tehničaru


#### **US-40:** Kao tehničar, želim da vidim podatke o korisniku, kako bih imao kontekst.

**Acceptance Criteria:**
- Kada tehničar pregleda tiket, ako želi pogledati detaljne informacije, tada sistem mora prikazati korisničke informacije 
- Kada se prikazuju podaci korisnika, ako su osjetljivi, tada sistem ne smije prikazati te podatke
- Kada tehničar pregleda korisnika, ako postoje prethodni tiketi, tada sistem mora omogućiti uvid u njih
- Sistem ne sme omogućiti promjenu korisničkih podatak prilikom pregleda 



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

2

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 7

---

<a id="sprint-8"></a>
# Sprint 8

## PB-26 Ocjenjivanje tiketa

### User Stories

<a id="us-18"></a>
### US-18: *Kao korisnik, želim da ocijenim rješenje tiketa, kako bih dao feedback o kvaliteti usluge.*
**Acceptance Criteria:**

- Kada je tiket zatvoren, ako korisnik klikne na opciju "Ocijeni", tada sistem omogućava unos i slanje ocjene  
- Kada korisnik pošalje ocjenu, tada se ona sprema u sistem  
- Sistem mora omogućiti izbor ocjene  
- Sistem ne smije dozvoliti ocjenjivanje otvorenog tiketa  
- Korisnik treba dobiti potvrdu o uspješnom slanju ocjene  

---

### Poslovna vrijednost

Omogućava unapređenje kvaliteta usluge.

### Prioritet

5

### Zavisnosti

Zavisi od zatvorenog tiketa.

### Pretpostavke

- Tiket mora biti zatvoren

### Otvorena pitanja

- Koja skala ocjenjivanja se koristi?

### Poveznica sa sprintom
Sprint 8

---

## PB-33 Pretraživanje i filtriranje tiketa

### User Stories

#### **US-31:** Kao korisnik, želim da pretražujem tikete, kako bih brzo pronašao željeni.

**Acceptance Criteria:**
- Kada korisnik unese tekst u polje za pretragu, ako pokrene pretragu, tada sistem mora pretraživati po ID-u
- Sistem traži podudaranje pretrage sa tiketom, neovisno od malog i velikog slova
- Kada se prikazuju rezultati, ako tiket ne pripada korisniku, tada sistem ne smije prikazati taj tiket
- Kada pretraga ne vrati rezultate, ako nema podudaranja, tada sistem mora prikazati poruku "Nema rezultata"


#### **US-32:** Kao korisnik, želim da filtriram tikete, kako bih lakše upravljao listom.

**Acceptance Criteria:**
- Kada je korisnik na listi tiketa, ako primijeni filtere, tada sistem mora filtrirati po odabranim kriterijima
- Sistem mora imati predefinisanu listu kriterija iz koje korisnik može birati 
- Kada su filteri aktivni, ako korisnik pregleda listu, tada sistem mora jasno prikazati aktivne filtere
- Sistem ne može dozvoliti korisniku da ubacuje vlastite filtere
- Kada je korisnik odabrao više filtera, ako postoje tiketi koji zadovoljavaju iste, tada sistem mora imati prioritizaciju



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

1

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 8

---

## PB-36 Ažuriranje statusa tiketa

### User Stories

#### **US-37:** Kao tehničar, želim da promijenim status tiketa, kako bih označio napredak.

**Acceptance Criteria:**
- Kada tehničar pregleda tiket, ako želi promijeniti status, tada sistem mora omogućiti izbor između definisanih stanja
- Kada tehničar promijeni status tiketa, ako je promjena uspješna, tada korisnik mora dobiti obavijest
- Kada je tiket zatvoren, ako tehničar pokuša promijeniti status, tada sistem ne smije dozvoliti promjenu
- Sistem mora evidentirati promjenu stanja tiketa


#### **US-38:** Kao tehničar, želim da evidentiram promjene, kako bih pratio historiju.

**Acceptance Criteria:**
- Kada tehničar promijeni status tiketa, ako se promjena izvrši, tada sistem mora evidentirati promjenu
- Korisnik ne može izvršiti izmjene ili brisanje tiketa
- Kada tehničar otvori tiket, ako postoji historija, tada sistem mora prikazati kompletnu historiju
- Sistem ne smije omogućiti tehničaru da vidi neovlaštene podatke



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

1

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 8

---

## PB-42 Vrijeme prvog odgovora

### User Stories

<a id="us-49"></a>
### US-49: *Kao administrator, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih imao informaciju da li su tehničari responzivni i odgovorni*
**Acceptance Criteria:**

- Kada administrator otvori tiket, tada vidi vrijeme prvog odgovora  
- Kada odgovor postoji, tada se prikazuje tačan timestamp prvog odgovora  
- Sistem mora zabilježiti vrijeme prvog odgovora na tiket  
- Sistem ne smije mijenjati vrijeme prvog odgovora nakon inicijalnog zapisa  
- Korisnik treba dobiti informaciju ako odgovor još nije poslan
---
<a id="us-50"></a>
### US-50: *Kao tehničar, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih mogao znati da poredam prioritete za poslane tikete, da li su druge kolege odgovorili na tiket i da li korisnik dugo čeka na odgovor*
**Acceptance Criteria:**

- Kada tehničar otvori tiket, tada vidi vrijeme prvog odgovora  
- Kada odgovor postoji, tada se prikazuje tačan timestamp prvog odgovora  
- Sistem mora zabilježiti vrijeme prvog odgovora na tiket  
- Sistem ne smije mijenjati vrijeme prvog odgovora nakon inicijalnog zapisa  
- Korisnik treba dobiti informaciju ako odgovor još nije poslan
---
<a id="us-51"></a>
### US-51: *Kao korisnik, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih znao koliko se mogu pouzdati na cijeli helpdesk/tiket sistem*
**Acceptance Criteria:**

- Kada korisnik otvori tiket, tada vidi vrijeme prvog odgovora  
- Kada odgovor postoji, tada se prikazuje tačan timestamp prvog odgovora  
- Sistem mora zabilježiti vrijeme prvog odgovora na tiket  
- Sistem ne smije mijenjati vrijeme prvog odgovora nakon inicijalnog zapisa  
- Korisnik treba dobiti informaciju ako odgovor još nije poslan
---
### Poslovna vrijednost
Omogućava se bolji uvid o pouzdanosti i težini problema koji je postavljen u tiketu

### Prioritet
3

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketu i vremenu slanja i odgovora tiketa

### Pretpostavke
- Baza podataka je dobro napravljena kako bi se mogli podaci o tiketima dobavljati pomoću backenda

### Otvorena pitanja
- Da li se postavlja vrijeme samo prvoga odgovora ili vrijeme i ostalih odgovora na isti tiket?

### Poveznica sa sprintom
Sprint 8

---

<a id="sprint-9"></a>
# Sprint 9

## PB-20 Upravljanje korisničkim profilom

### User Stories

<a id="us-4"></a>
### US-4: *Kao korisnik, želim da promijenim svoj email, kako bih imao tačne kontakt informacije.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako unese novi email i potvrdi izmjenu, tada se email ažurira u sistemu  
- Kada korisnik unese nevalidan email, tada sistem prikazuje grešku  
- Sistem mora provjeriti format email adrese  
- Sistem ne smije dozvoliti unos već postojećeg emaila u sistemu  
- Korisnik treba dobiti potvrdu o uspješnoj promjeni emaila  
---

<a id="us-5"></a>
### US-5: *Kao korisnik, želim da promijenim lozinku, kako bih povećao sigurnost svog naloga.*
**Acceptance Criteria:**
    
- Kada je korisnik prijavljen, ako unese trenutnu i novu lozinku, tada se lozinka uspješno mijenja  
- Kada korisnik unese pogrešnu trenutnu lozinku, tada sistem odbija promjenu  
- Sistem mora zahtijevati unos trenutne lozinke prije promjene  
- Sistem mora provjeriti minimalne sigurnosne zahtjeve lozinke  
- Sistem ne smije dozvoliti lozinku koja ne zadovoljava sigurnosne zahtjeve 
- Korisnik treba dobiti potvrdu o uspješnoj promjeni lozinke  
---

### Poslovna vrijednost

Omogućava korisnicima kontrolu nad svojim podacima i povećava sigurnost naloga.

### Prioritet

2

### Zavisnosti

Zavisi od login funkcionalnosti.

### Pretpostavke

- Korisnik mora biti prijavljen

### Otvorena pitanja

- Da li je potrebna verifikacija emaila nakon promjene?
- Koji su zahtjevi za jačinu lozinke?

### Poveznica sa sprintom
Sprint 9

---

## PB-21 Prikaz paketa i pretplata

### User Stories

<a id="us-6"></a>
### US-6: *Kao korisnik, želim da vidim svoje aktivne pakete i pretplate, kako bih imao pregled usluga koje koristim.*
**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako pristupi sekciji "Paketi", tada vidi listu svojih aktivnih paketa i pretplata  
- Kada korisnik ima više paketa, tada sistem prikazuje sve aktivne pakete u listi  
- Sistem mora prikazati osnovne informacije o svakom paketu 
- Sistem ne smije prikazivati pakete koji nisu povezani sa korisnikom  
- Korisnik treba dobiti jasan pregled svojih aktivnih usluga  
---

<a id="us-7"></a>
### US-7: *Kao korisnik, želim da vidim detalje paketa (internet, TV, mobilni), kako bih razumio šta moj paket uključuje.*
**Acceptance Criteria:**

- Kada korisnik odabere paket, tada sistem prikazuje detalje paketa  
- Kada paket sadrži više usluga, tada sistem prikazuje sve uključene usluge  
- Sistem mora prikazati relevantne informacije (brzina interneta, broj kanala, mobilni podaci)  
- Sistem ne smije prikazivati netačne ili nepotpune informacije o paketu  
- Korisnik treba dobiti jasan i razumljiv prikaz sadržaja paketa  
---

### Poslovna vrijednost

Pruža korisnicima jasan pregled usluga i povećava transparentnost.

### Prioritet

4

### Zavisnosti

Zavisi od login funkcionalnosti i baze podataka.

### Pretpostavke

- Podaci o paketima su već dostupni u sistemu

### Otvorena pitanja

- Da li korisnik može mijenjati paket kroz sistem?
- Koliko detalja prikazati?

### Poveznica sa sprintom
Sprint 9

---

## PB-34 Pregled i uređivanje korisničkih profila

### User Stories

#### **US-33:** Kao administrator, želim da pregledam korisničke profile, kako bih imao uvid u podatke.

**Acceptance Criteria:**
- Kada administrator pregleda postojeće korisnike, ako odabere specifičan korinsički profil, tada sistem omogućava dostup više informacija
- Kada administrator otvori profil korisnika, tada sistem mora prikazati sve relevantne podatke
- Sistem adminsitratoru prikazuje samo spisak trenutno postojećih korisnika
- Kada se prikazuju podaci, tada sistem ne smije prikazivati lozinke ili osjetljive informacije.
- Kada administrator traži korisnika, tada sistem mora omogućiti pretragu po imenu ili emailu.
- Kada administrator vrši pretragu korisnika, ako nema podudaranja, tada sistem šalje poruku da korisnik nike pronađen


#### **US-34:** Kao administrator, želim da uređujem profile, kako bih održavao tačnost podataka.

**Acceptance Criteria:**
- Kada administrator mijenja podatke, tada sistem mora omogućiti uređivanje osnovnih informacija
- Kada se radi o lozinki, tada sistem ne smije dozvoliti direktnu izmjenu
- Kada se izmjena sačuva, tada sistem mora prikazati potvrdu
- Kada se napravi izmjena, tada sistem mora evidentirati promjenu
- Kada administrator izvrši promjenu podataka, ukoliko ne potvrdi promjenu, tada sistem ne smije gubiti niti mijenjati prethodne informacije



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

2

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 9

---

<a id="sprint-10"></a>
# Sprint 10

## PB-29 Preraspodjela agenata po timovima

### User Stories

#### **US-23:** Kao administrator, želim da preraspodijelim agente po timovima, kako bih optimizirao rad.

**Acceptance Criteria:**
- Kada je administrator prijavljen i nalazi se u sekciji upravljanja timovima, ako odabere agenta i premjesti ga u drugi tim, tada sistem mora izvršiti promjenu 
- Sistem mora omogućiti primjenu bez gubitka informacija
- Kada administrator izvrši preraspodjelu agenta, ako je akcija potvrđena, tada sistem mora evidentirati promjenu sa vremenskim pečatom i imenom administratora
- Sistem mora omogućiti da ne dođe do promjene podataka ukoliko administrator ne potvrdi akciju promjene agenata
- Kada administrator izvrši pokušaj preraaspodjele agenata, ako dođe do greške ili nemogućnosti odabrane perraspodjele, sistme mora poslati poruku upozorenja


#### **US-24:** Kao administrator, želim da vidim pregled raspodjele timova, kako bih donio bolje odluke.

**Acceptance Criteria:**
- Kada administrator pristupi sekciji timova tada sistem mora prikazati sve timove i njihove članove
- Kada se prikazuju članovi tima, ako agent nije aktivan ili je obrisan, tada sistem ne smije prikazivati tog agenta kao aktivnog
- Kada administrator koristi opcije pregleda, ako primijeni filter/sortiranje, tada sistem mora ažurirati prikaz prema odabranim kriterijima
- Sistem mora prikazati aktivno stanje timova



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

1

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

- Da li može doći do zamjene tiketa prilikom preraspodjele?


### Poveznica sa sprintom
Sprint 10

---

## PB-30 Automatska dodjela tiketa

### User Stories

#### **US-25:** Kao sistem, želim da automatski dodijelim tiket agentu, kako bi se ubrzalo rješavanje.

**Acceptance Criteria:**
- Kada korisnik kreira novi tiket, ako su definisana pravila dodjele, tada sistem mora automatski dodijeliti tiket odgovarajućem agentu
- Kada sistem dodjeljuje tiket, ako je agent nedostupan, tada sistem ne smije dodijeliti tiket tom agentu
- Kada je tiket dodijeljen agentu, ako je dodjela uspješna, tada agent mora dobiti notifikaciju
- Kada sistem ne može pronaći dostupnog agenta, ako se izvrši pokušaj dodjele, tada tiket mora biti označen kao "Nedodijeljen"
- Sistem mora dodijeliti tiket prema predefinisanin pravilima dodjele
- Sistem mora prikazati poruku ukoliko nema definisanih pravila dodjele


#### **US-26:** Kao administrator, želim da definišem pravila dodjele, kako bih kontrolisao proces.

**Acceptance Criteria:**
- Kada administrator pristupi sekciji pravila, ako želi upravljati pravilima, tada sistem mora omogućiti administratoru editovanje pravila
- Sistem mora omogućiti pravljenje, uređivanje, i brisanje pravila
- Kada administrator pregleda pravila, ako postoje aktivna pravila, tada sistem mora prikazati sva aktivna pravila
- Sistem mora prikazati poruku ukoliko nema definisanih pravila
- Kada tiket odgovara više pravila, ako se primjenjuju pravila, tada sistem mora koristiti prioritizaciju
 


### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

3

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 10

---

## PB-31 Prosljeđivanje tiketa

### User Stories

#### **US-27:** Kao agent, želim da proslijedim tiket drugom agentu, kako bi problem bio riješen.

**Acceptance Criteria:**
- Kada je agent prijavljen i nalazi se na tiketu, ako želi proslijediti tiket, tada sistem mora omogućiti izbor samo aktivnih agenata
- Sistem mora omogućiti prosljeđivanje tiketa od strane agenta ukoliko on njemu nije dodijeljen
- Kada agent pokušava proslijediti tiket, ako je tiket zatvoren, tada sistem ne smije dozvoliti prosljeđivanje
- Kada agent proslijedi tiket, ako je akcija uspješna, tada novi agent mora dobiti obavijest
- Kada agent pokuša proslijediti tiket samom sebi, ako izvrši tu akciju, tada sistem mora spriječiti prosljeđivanje


#### **US-28:** Kao agent, želim da dodam komentar prilikom prosljeđivanja, kako bih objasnio situaciju.

**Acceptance Criteria:**
- Kada agent prosljeđuje tiket, ako želi dodati komentar, tada sistem mora omogućiti unos komentara
- Kada postoji interni komentar, ako korisnik pregleda tiket, tada sistem ne smije prikazati taj komentar
- Sistem mora ograničiti dužinu komentara
- Kada novi agent primi tiket, ako postoji komentar, tada sistem mora prikazati komentar u detaljima tiketa
- Kada novi agent primi tiket sa internim komentarom, tada sistem mora onemogućiti opciju da agent mijenja komentar tiketa
 


### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

2

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 10

---

## PB-32 Pregled svih tiketa

### User Stories

#### **US-29:** Kao agent, želim da vidim sve tikete, kako bih imao potpuni pregled.

**Acceptance Criteria:**
- Kada agent otvori listu tiketa, ako se podaci učitaju, tada sistem mora prikazati sve tikete
- Kada postoji veliki broj tiketa, ako korisnik skrola ili traži više, tada sistem mora omogućiti učitavanje dodatnih rezultata
- Sistem prikazuje sve tikete neovisno od stanja
- Kada administrator koristi sistem, ako pristupa tiketu, tada sistem ne smije ograničiti 


#### **US-30:** Kao agent, želim da vidim detalje svakog tiketa, kako bih imao detaljniji uvid.

**Acceptance Criteria:**
- Kada agent otvori tiket, ako tiket postoji, tada sistem mora prikazati sve informacije o tiketu
- Kada agent koristi sistem, ako pristupa tiketu, tada sistem ne smije ograničiti pristup



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

1

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 10

---

## PB-35 Pregled dodijeljenih tiketa (tehničari)

### User Stories

#### **US-35:** Kao tehničar, želim da vidim svoje tikete, kako bih znao na čemu radim.

**Acceptance Criteria:**
- Kada tehničar otvori listu tiketa, ako je prijavljen, tada sistem mora prikazati samo njegove tikete
- Kada se prikazuje lista tiketa, ako tiket nije dodijeljen tom tehničaru, tada sistem ne smije prikazati taj tiket
- Kada se prikazuju tiketi, ako imaju različite statuse, tada sistem mora jasno razlikovati statuse
- Kada tehničar nema dodijeljenih tiketa, ako otvori listu, tada sistem mora prikazati poruku o praznoj listi


#### **US-36:** Kao tehničar, želim da filtriram tikete, kako bih organizovao rad.

**Acceptance Criteria:**
- Kada je tehničar prijavljen i nalazi se na listi tiketa, ako primijeni filtere, tada sistem mora filtrirati po odabranim kriterijima
- Kada tehničar primijeni filtere, ako nijedan tiket ne odgovara kriterijima, tada sistem mora prikazati odgovarajuću poruku
- Kada tehničar unosi raspon datuma, ako je početni datum veći od krajnjeg, tada sistem ne smije dozvoliti primjenu filtera



### Poslovna vrijednost

Omogućava efikasnije upravljanje tiketima i resursima.

### Prioritet

1

### Zavisnosti

Zavisi od sistema za tikete i korisničkih uloga.

### Pretpostavke

- Sistem podržava rad sa tiketima i korisnicima

### Otvorena pitanja

- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 10

---

<a id="sprint-11"></a>
# Sprint 11

## PB-38 Izvještaj o broju tiketa

### User Stories

<a id="us-41"></a>
#### US-41: *Kao administrator, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani kako bih imao uvid o situaciji i količini tiketa*
**Acceptance Criteria:**

- Kada administrator odabere vremenski period (dnevni, sedmični, mjesečni, godišnji), tada se prikazuje ukupan broj tiketa za taj period 
- Kada podaci postoje u sistemu, tada podaci odgovaraju stvarnom stanju u bazi   
- Sistem mora omogućiti izbor vremenskog perioda
- Sistem mora omogućiti pregled izvještaja administratoru
- Sistem ne smije prikazati pogrešne ili duplirane podatke
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Administrator treba dobiti poruku ako nema podataka za odabrani period
---
<a id="us-42"></a>
#### US-42: *Kao tehničar, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani, kako bih imao uvid o količini tiketa, situaciji i količini posla kojeg moram uraditi u kancelariji ili na terenu*
**Acceptance Criteria:**

- Kada tehničar odabere vremenski period, tada vidi broj tiketa koji su raspoređeni njemu za taj period  
- Kada podaci postoje u sistemu, tada podaci odgovaraju stvarnom stanju u bazi
- Sistem mora omogućiti izbor vremenskog perioda  
- Sistem mora omogućiti pregled izvještaja tehničaru
- Sistem ne smije prikazati pogrešne ili duplirane podatke  
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Tehničar treba dobiti poruku ako nema podataka za odabrani period 
___
### Poslovna vrijednost
Omogućava se bolji uvid o statistici, problemima i analizi o količini tiketa

### Prioritet
5

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketima 

### Pretpostavke
- Baza podataka je dobro napravljena kako bi se mogli podaci o tiketima dobavljati pomoću backenda

### Otvorena pitanja
- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 11

---

## PB-39 Izvještaj po statusu tiketa

### User Stories

<a id="us-43"></a>
### US-43: *Kao administrator, želim da imam izvještaj o statusu tiketa, kako bih mogao lakše analizirati i imati uvid o tiketima*
**Acceptance Criteria:**

- Kada administrator otvori izvještaj, tada vidi ukupni broj tiketa po statusima (otvoren, u toku, zatvoren)  
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima  
- Sistem mora grupisati tikete po statusu  
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Administrator treba dobiti poruku ako nema podataka
---
<a id="us-44"></a>
### US-44: *Kao tehničar, želim da imam izvještaj o statusu tiketa, kako bih imao uvid o stanju na terenu i koji tiket ću prije riješiti*
**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi raspodjelu svojih tiketa po statusima  
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima  
- Sistem mora omogućiti pristup izvještaju tehničaru  
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten
- Tehničar treba dobiti poruku ako nema podataka
---
### Poslovna vrijednost
Omogućava se bolji uvid o statistici, problemima i analizi o statusima tiketa

### Prioritet
5

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketima 

### Pretpostavke
- Baza podataka je dobro napravljena kako bi se mogli podaci o tiketima dobavljati pomoću backenda

### Otvorena pitanja
- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 11

---

## PB-40 Izvještaj po tipu problema

### User Stories

<a id="us-45"></a>
### US-45: *Kao Administrator, želim da imam izvještaj po tipovima problema u tiketima, kako bih imao uvid o najčešćim tipovima problema i daljnje analize i poboljšanja cijelog telekom sistema*
**Acceptance Criteria:**

- Kada administrator otvori izvještaj, tada vidi ukupni broj tiketa po tipu problema  
- Kada podaci postoje, tada su prikazani tačni tipovi i količine  
- Sistem mora grupisati tikete po tipu problema  
- Korisnik treba dobiti poruku ako nema podataka
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten 
---
<a id="us-46"></a>
### US-46: *Kao Tehničar, želim da imam izvještaj po tipovima problema u tiketima, kako bih imao uvid o najčešćim tipovim problema koje se trebaju riješiti na terenu i kako bih bio spremniji da riješim isti*
**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi ukupni broj tiketa koji su raspoređeni njemu po tipu problema 
- Kada podaci postoje, tada izvještaj odgovara stvarnim podacima
- Sistem mora grupisati tikete po tipu problema  
- Korisnik treba dobiti poruku ako nema dostupnih podataka
- Sistem ne smije dozvoliti pristup izvještaju ako korisnik nema odgovarajuću ulogu, uz poruku da korisnik nije ovlašten 
---
### Poslovna vrijednost
Omogućava se bolji uvid o statistici, problemima i analizi po tipovima problema pojedinačnog tiketa

### Prioritet
5

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketima 

### Pretpostavke
- Baza podataka je dobro napravljena kako bi se mogli podaci o tiketima dobavljati pomoću backenda

### Otvorena pitanja
- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?

### Poveznica sa sprintom
Sprint 11

---

## PB-41 Prosječno vrijeme rješavanja tiketa

### User Stories

<a id="us-47"></a>
### US-47: *Kao administrator, želim da imam uvid o prosječnom vremenu rješavanja (zatvaranja) tiketa, kako bih imao uvid u efikasnost naših radnika*
**Acceptance Criteria:**

- Kada administrator otvori izvještaj, tada vidi prosječno vrijeme zatvaranja tiketa  
- Kada podaci postoje, tada je izračun tačan (vrijeme zatvaranja - vrijeme kreiranja)  
- Sistem mora izračunati prosjek na osnovu svih zatvorenih tiketa  
- Sistem ne smije uključiti nezatvorene tikete u izračun  
- Korisnik treba dobiti poruku ako nema podataka
---
<a id="us-48"></a>
### US-48: *Kao tehničar, želim da imam uvid o prosječnom vremenu rješavanja (zatvaranja) tiketa, kako bih imao uvid o težini problema koji se riješio*
**Acceptance Criteria:**

- Kada tehničar otvori izvještaj, tada vidi prosječno vrijeme rješavanja  
- Kada podaci postoje, tada izračun odgovara stvarnim podacima  
- Sistem