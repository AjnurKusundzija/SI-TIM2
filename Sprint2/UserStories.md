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
- **US-21:** Kao korisnik, želim da postavim prioritet svog problema, kako bih označio hitnost.

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

- **US-1:** Kao administrator, želim da preraspodijelim agente po timovima, kako bih optimizirao rad.

- **US-2:** Kao administrator, želim da vidim pregled raspodjele timova, kako bih donio bolje odluke.

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

- **US-1:** Kao sistem, želim da automatski dodijelim tiket agentu, kako bi se ubrzalo rješavanje.

- **US-2:** Kao administrator, želim da definišem pravila dodjele, kako bih kontrolisao proces.

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

- **US-1:** Kao agent, želim da proslijedim tiket drugom agentu, kako bi problem bio riješen.

- **US-2:** Kao agent, želim da dodam komentar prilikom prosljeđivanja, kako bih objasnio situaciju.

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

- **US-1:** Kao administrator, želim da vidim sve tikete, kako bih imao potpuni pregled.

- **US-2:** Kao administrator, želim da vidim detalje svakog tiketa, kako bih imao detaljniji uvid.

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

- **US-1:** Kao korisnik, želim da pretražujem tikete, kako bih brzo pronašao željeni.

- **US-2:** Kao korisnik, želim da filtriram tikete, kako bih lakše upravljao listom.

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

- **US-1:** Kao administrator, želim da pregledam korisničke profile, kako bih imao uvid u podatke.

- **US-2:** Kao administrator, želim da uređujem profile, kako bih održavao tačnost podataka.

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

- **US-1:** Kao tehničar, želim da vidim svoje tikete, kako bih znao na čemu radim.

- **US-2:** Kao tehničar, želim da filtriram tikete, kako bih organizovao rad.

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

- **US-1:** Kao tehničar, želim da promijenim status tiketa, kako bih označio napredak.

- **US-2:** Kao tehničar, želim da evidentiram promjene, kako bih pratio historiju.

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

- **US-1:** Kao tehničar, želim da vidim osnovne informacije o tiketu, kako bih razumio problem.

- **US-2:** Kao tehničar, želim da vidim podatke o korisniku, kako bih imao kontekst.

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

- **US-1:** Kao administrator, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani kako bih imao uvid o situaciji i količini tiketa

- **US-2:** Kao tehničar, želim da imam dnevni, sedmični, mjesečni i godišnji izvještaj o broju tiketa koji su poslani, kako bih imao uvid o količini tiketa, situaciji i količini posla kojeg moram uraditi u kancelariji ili na terenu


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

- **US-1:** Kao administrator, želim da imam izvještaj o statusu tiketa, kako bih mogao lakše analizirati i imati uvid o tiketima

- **US-2:**Kao tehničar, želim da imam izvještaj o statusu tiketa, kako bih imao uvid o stanju na terenu i koji tiket ću prije riješiti


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

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja


---

## PB-41 Prosječno vrijeme rješavanja tiketa

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja

---

## PB-42 Vrijeme prvog odgovora

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja

---

## PB-43 Izvještaj o opterećenju agenata

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja

---

## PB-44 Izvještaj o ocjenama korisnika

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja

---

## PB-45 Admin Dashboard sa ključnim metrikama

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja

---

## PB-46 Export Izvještaja

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja

---

## PB-47 FAQ Segment

### User Stories

- **US-1:**


### Poslovna vrijednost


### Prioritet


### Zavisnosti


### Pretpostavke


### Otvorena pitanja




