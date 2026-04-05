# Dokumentacija Korisničkih Priča (User Stories)

---

## PB-19 Login korisnika

### User Stories

- **US-1:** Kao registrovani korisnik, želim da se prijavim u sistem koristeći email i lozinku, kako bih pristupio svom nalogu.
- **US-2:** Kao korisnik, želim da se odjavim iz sistema, kako bih zaštitio svoj nalog.
- **US-3:** Kao korisnik, želim da budem obaviješten ako unesem pogrešne podatke za prijavu, kako bih mogao ponovo pokušati.

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

---

## PB-20 Upravljanje korisničkim profilom

### User Stories

- **US-4:** Kao korisnik, želim da promijenim svoj email, kako bih imao tačne kontakt informacije.
- **US-5:** Kao korisnik, želim da promijenim lozinku, kako bih povećao sigurnost svog naloga.

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

---

## PB-21 Prikaz paketa i pretplata

### User Stories

- **US-6:** Kao korisnik, želim da vidim svoje aktivne pakete i pretplate, kako bih imao pregled usluga koje koristim.
- **US-7:** Kao korisnik, želim da vidim detalje paketa (internet, TV, mobilni), kako bih razumio šta moj paket uključuje.

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

---

## PB-22 Kreiranje novog ticketa

### User Stories

- **US-8:** Kao korisnik, želim da kreiram novi tiket unosom problema, kako bih prijavio poteškoću.
- **US-9:** Kao korisnik, želim da odaberem tip i prioritet tiketa, kako bih bolje opisao problem.
- **US-10:** Kao korisnik, želim da unesem opis problema, kako bih agentu dao dovoljno informacija.

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

---

## PB-23 Pregled vlastitih tiketa

### User Stories

- **US-11:** Kao korisnik, želim da vidim listu svih svojih tiketa, kako bih pratio njihove statuse.
- **US-12:** Kao korisnik, želim da vidim status tiketa (otvoren, u toku, zatvoren), kako bih znao u kojoj fazi je rješavanje.
- **US-13:** Kao korisnik, želim mogućnost filtriranja tiketa (po prioritetu, datumu i slično), kako bih lakše pronašao željeni tiket.

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

---

## PB-24 Detaljan prikaz ticketa

### User Stories

- **US-13:** Kao korisnik, želim da vidim detalje tiketa, kako bih imao potpuni uvid u problem.
- **US-14:** Kao korisnik, želim da vidim historiju komunikacije, kako bih pratio tok rješavanja.

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

---

## PB-25 Zatvaranje ticketa

### User Stories

- **US-15:** Kao korisnik, želim da zatvorim tiket kada je problem riješen, kako bih završio proces.
- **US-16:** Kao agent ili tehničar, želim da zatvorim tiket nakon rješavanja problema, kako bih označio zadatak kao završen.

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

---

## PB-26 Ocjenjivanje ticketa

### User Stories

- **US-17:** Kao korisnik, želim da ocijenim rješenje ticketa, kako bih dao feedback o kvaliteti usluge.

### Poslovna vrijednost

Omogućava unapređenje kvaliteta usluge.

### Prioritet

5

### Zavisnosti

Zavisi od zatvorenog ticketa.

### Pretpostavke

- Tiket mora biti zatvoren

### Otvorena pitanja

- Koja skala ocjenjivanja se koristi?

---

## PB-27 Komunikacija kroz tiket

### User Stories

- **US-18:** Kao korisnik, želim da šaljem poruke kroz tiket, kako bih komunicirao sa agentom.
- **US-19:** Kao agent, želim da odgovaram na poruke korisnika, kako bih riješio problem.

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

---

## PB-28 Upravljanje prioritetima tiketa

### User Stories

- **US-20:** Kao agent, želim da postavim interni prioritet tiketa, kako bih efikasno upravljao zadacima.

**Acceptance Criteria:**
- Kada je agent prijavljen u sistem i nalazi se na stranici tiketa, ako odabere opciju za postavljanje prioriteta, tada sistem mora postaviti odabrani prioritet
- Sistem mora omogućiti listu predefinisanih prioriteta
- Kada tiket ima postavljen interni prioritet, ako korisnik pristupi tiketu, tada sistem ne smije dozvoliti prikaz niti izmjenu internog prioriteta
- Kada agent postavi ili izmijeni prioritet tiketa, ako je akcija uspješno izvršena, tada sistem mora prikazati potvrdu
- Kada agent postavi ili izmijeni prioritet tiketa, ako je akcija neuspješno izvršena, tada sistem mora prikazati poruku upozorenja
- Kada nema odabranog internog priroteta, tada sistem mora prikazati poruku da prioritet nije postavljen


- **US-21:** Kao korisnik, želim da postavim prioritet svog problema, kako bih označio hitnost.

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

---

## PB-29 Preraspodjela agenata po timovima

### User Stories

- **US-22:** Kao administrator, želim da preraspodijelim agente po timovima, kako bih optimizirao rad.

**Acceptance Criteria:**
- Kada je administrator prijavljen i nalazi se u sekciji upravljanja timovima, ako odabere agenta i premjesti ga u drugi tim, tada sistem mora izvršiti promjenu 
- Sistem mora omogućiti primjenu bez gubitka informacija
- Kada administrator izvrši preraspodjelu agenta, ako je akcija potvrđena, tada sistem mora evidentirati promjenu sa vremenskim pečatom i imenom administratora
- Sistem mora omogućiti da ne dođe do promjene podataka ukoliko administrator ne potvrdi akciju promjene agenata
- Kada administrator izvrši pokušaj preraaspodjele agenata, ako dođe do greške ili nemogućnosti odabrane perraspodjele, sistme mora poslati poruku upozorenja



- **US-23:** Kao administrator, želim da vidim pregled raspodjele timova, kako bih donio bolje odluke.

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

---

## PB-30 Automatska dodjela tiketa

### User Stories

- **US-24:** Kao sistem, želim da automatski dodijelim tiket agentu, kako bi se ubrzalo rješavanje.

- **US-25:** Kao administrator, želim da definišem pravila dodjele, kako bih kontrolisao proces.

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

---

## PB-31 Prosljeđivanje tiketa

### User Stories

- **US-26:** Kao agent, želim da proslijedim tiket drugom agentu, kako bi problem bio riješen.

- **US-27:** Kao agent, želim da dodam komentar prilikom prosljeđivanja, kako bih objasnio situaciju.

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

---

## PB-32 Pregled svih tiketa

### User Stories

- **US-28:** Kao administrator, želim da vidim sve tikete, kako bih imao potpuni pregled.

- **US-29:** Kao administrator, želim da vidim detalje svakog tiketa, kako bih imao detaljniji uvid.

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

---

## PB-33 Pretraživanje i filtriranje tiketa

### User Stories

- **US-30:** Kao korisnik, želim da pretražujem tikete, kako bih brzo pronašao željeni.

- **US-31:** Kao korisnik, želim da filtriram tikete, kako bih lakše upravljao listom.

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

---

## PB-34 Pregled i uređivanje korisničkih profila

### User Stories

- **US-32:** Kao administrator, želim da pregledam korisničke profile, kako bih imao uvid u podatke.

- **US-33:** Kao administrator, želim da uređujem profile, kako bih održavao tačnost podataka.

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

---

## PB-35 Pregled dodijeljenih tiketa (tehničari)

### User Stories

- **US-34:** Kao tehničar, želim da vidim svoje tikete, kako bih znao na čemu radim.

- **US-35:** Kao tehničar, želim da filtriram tikete, kako bih organizovao rad.

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

---

## PB-36 Ažuriranje statusa tiketa

### User Stories

- **US-36:** Kao tehničar, želim da promijenim status tiketa, kako bih označio napredak.

- **US-37:** Kao tehničar, želim da evidentiram promjene, kako bih pratio historiju.

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

---

## PB-37 Tehničar vidi osnovne informacije

### User Stories

- **US-38:** Kao tehničar, želim da vidim osnovne informacije o tiketu, kako bih razumio problem.

- **US-39:** Kao tehničar, želim da vidim podatke o korisniku, kako bih imao kontekst.

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

---

## PB-38 Izvještaj o broju tiketa

### User Stories

- **US-40:** Kao administrator, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani kako bih imao uvid o situaciji i količini tiketa

- **US-41:** Kao tehničar, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani, kako bih imao uvid o količini tiketa, situaciji i količini posla kojeg moram uraditi u kancelariji ili na terenu


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

---

## PB-39 Izvještaj po statusu tiketa

### User Stories

- **US-42:** Kao administrator, želim da imam izvještaj o statusu tiketa, kako bih mogao lakše analizirati i imati uvid o tiketima

- **US-43:**Kao tehničar, želim da imam izvještaj o statusu tiketa, kako bih imao uvid o stanju na terenu i koji tiket ću prije riješiti


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


---

## PB-40 Izvještaj po tipu problema

### User Stories

- **US-44:** Kao Administrator, želim da imam izvještaj po tipovima problema u tiketima, kako bih imao uvid o najčešćim tipovima problema i daljnje analize i poboljšanja cijelog telekom sistema

- **US-45:**Kao Tehničar, želim da imam izvještaj po tipovima problema u tiketima, kako bih imao uvid o najčešćim tipovim problema koje se trebaju riješiti na terenu i kako bih bio spremniji da riješim isti

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


---

## PB-41 Prosječno vrijeme rješavanja tiketa

### User Stories

- **US-46:** Kao administrator, želim da imam uvid o prosječnom vremenu rješavanja (zatvaranja) tiketa, kako bih imao uvid u efikasnost naših radnika

- **US-47:** Kao tehničar, želim da imam uvid o prosječnom vremenu rješavanja (zatvaranja) tiketa, kako bih imao uvid o težini problema koji se riješio

### Poslovna vrijednost
Omogućava se bolji uvid o statistici, težini problemima i analizi svakog tiketa

### Prioritet
5

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketima 

### Pretpostavke
- Baza podataka je dobro napravljena kako bi se mogli podaci o tiketima dobavljati pomoću backenda

### Otvorena pitanja
- Ko ima pristup ovoj funkcionalnosti i pod kojim uslovima?


---

## PB-42 Vrijeme prvog odgovora

### User Stories

- **US-48:** Kao administrator, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih imao informaciju da li su tehničari responzivni i odgovorni

- **US-49:** Kao tehničar, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih mogao znati da poredam prioritete za poslane tikete

- **US-50:** Kao tehničar, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih mogao znati da li su druge kolege odgovorili na tiket

- **US-51:** Kao tehničar, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih mogao znati da li korisnik dugo čeka na odgovor

- **US-52:** Kao korisnik, želim da imam uvid o informaciji kada se desio prvi odgovor na tiket, kako bih znao koliko se mogu pouzdati na cijeli helpdesk/tiket sistem

### Poslovna vrijednost
Omogućava se bolji uvid o pouzdanosti i težini problema koji je postavljen u tiketu

### Prioritet
2

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketu i vremenu slanja i odgovora tiketa

### Pretpostavke
- Baza podataka je dobro napravljena kako bi se mogli podaci o tiketima dobavljati pomoću backenda

### Otvorena pitanja
- Da li se postavlja vrijeme samo prvoga odgovora ili vrijeme i ostalih odgovora na isti tiket?


---

## PB-43 Izvještaj o opterećenju agenata

### User Stories

- **US-53:** Kao Administrator, želim da imam uvid u detaljan izvještaj o broju riješenih tiketa po agentu u dnevnim, sedmičnim i mjesečnim intervalima,kako bih mogao da pratim opterećenje tima, identifikujem najproduktivnije radnike i optimizujem raspodjelu resursa.


### Poslovna vrijednost
Omogućava menadžmentu donošenje odluka zasnovanih na podacima (data-driven decisions). Pomaže u prepoznavanju "uskih grla" u podršci i pravednijoj raspodjeli posla, što direktno utiče na smanjenje sagorijevanja (burnout) zaposlenih.

### Prioritet
2

### Zavisnosti
Potrebno je imati modul za upravljanje tiketima i relevantne podatke u bazi podatake za prikaz količine poslanih tiketa 

### Pretpostavke
Sistem već bilježi tačno vrijeme zatvaranja svakog tiketa i ID agenta koji ga je riješio.

### Otvorena pitanja
- Da li je potreban vizuelni prikaz (grafikon) ili je dovoljna tabela sa mogućnošću filtriranja?

---

## PB-44 Izvještaj o ocjenama korisnika

### User Stories

- **US-54:** Kao Agent, želim da analiziram ocjene koje korisnici ostavljaju nakon zatvaranja tiketa kako bih mogao da procijenim kvalitet pružene podrške i identifikujem oblasti u kojima je potrebna dodatna edukacija agenata.


### Poslovna vrijednost
Direktno mjerenje zadovoljstva korisnika je ključno za zadržavanje klijenata. Ovaj feedback omogućava timu da reaguje na negativna iskustva prije nego što ona postanu kritična za poslovanje.

### Prioritet
2

### Zavisnosti
Navedeni User Story zavisi od funkcionalnosti zatvaranja tiketa i sistema za prikupljanje feedbacka.

Također potrebno je imati podatak u bazi podataka o statusu samog tiketa

### Pretpostavke
Implementiran je mehanizam koji šalje upit za ocjenu korisniku odmah nakon zatvaranja tiketa.


### Otvorena pitanja
- Da li se analiziraju samo numeričke ocjene (npr. 1-5) ili i tekstualni komentari?

---

## PB-45 Admin Dashboard sa ključnim metrikama

### User Stories

- **US-55:** Kao administrator, želim da imam spreman cijeli dashboard sa ključnim metrikama o tiketima, kako bih imao uvid o cijelom sistemu i radu naših tehničara


### Poslovna vrijednost

Lakši uvid o cijelom tiket/helpdesk sistemu za lakši pregled statistike i analizu istog


### Prioritet
1

### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketu i vremenu slanja i odgovora tiketa



### Pretpostavke
Svi relevanti tabele i podaci su dostupni u bazi podataka za prikaz na admin dashboardu

### Otvorena pitanja
- Koji podaci su relevantni za prikaz istih na admin dashboardu?

---

## PB-46 Export Izvještaja

### User Stories

- **US-56:** Kao tehničar, želim da imam mogućnost za export izvještaja tiketa u CSV formati, radi lakšeg prosljeđivanja podataka kolegama koji nisu direktno povezani sa tiket/helpdesk sistemom.


### Poslovna vrijednost
- Prijenos podataka i izvještaja u okviru cijeloga telekoma, a ne samo za radnike koji su povezani u tiket/helpdesk sistem


### Prioritet
5


### Zavisnosti
Sistem i features za dobavljanje podataka u real-time-u su dobro napravljeni kako bi se efikasno mogli dobavljati podaci o tiketima za sigurno exportiranje 

Eventualno će se trebati uključiti druga biblioteka za exportiranje podataka u .csv fileu


### Pretpostavke
U bazi podataka se nalaze svi relevantni podaci za export izvještaja tiketa 


### Otvorena pitanja
- Koji podaci su relevantni za ubacivanje u .csv file?

---

## PB-47 FAQ Segment

### User Stories

- **US-57:** Kao korisnik, želim da vidim listu najčešće postavljanih pitanja koje su vezane za razne probleme, kako bih mogao eventualno riješiti problem bez postavljanja tiketa


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




