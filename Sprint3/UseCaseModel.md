# Use Case Model

---

## UC-01: Login korisnika

**Akter:** Korisnik, Agent, Tehničar, Menadžment

**Naziv use casea:** Prijava u sistem (Login)

**Kratak opis:**
Akter pristupa sistemu unosom e-mail adrese i lozinke uz validaciju podataka

**Preduslovi:**
- Akter ima kreiran nalog u sistemu
- Akter nije trenutno prijavljen

**Glavni tok:**
1. Akter otvara formu za prijavu
2. Akter unosi email adresu i lozinku
3. Sistem validira format e-mail adrese
4. Akter klikne na dugme "Login"
5. Sistem validira unesene podatke
6. Sistem preusmjerava korisnika na dashboard prema njegovoj ulozi

**Alternativni tokovi:**
- A1: Prazna polja
 > Ako akter ne unese e-mail ili lozinku, sistem prikazuje poruku: "Sva polja su obavezna."
- A2: Pogrešni podaci
> Ako su uneseni pogrešni podaci, sistem priakzuje poruka o grešci "Pogrešan e-mail ili lozinka."
- A3: Sistem nedostupan
> Ukoliko je sistem nedostupan prikazuje se poruka :"Trenutno nije moguće izvršiti prijavu. Pokušajte kasnije."

**Ishod:** Korisnik pristupa sistemu

---

## UC-02: Upravljanje korisničkim profilom

**Akter:** Korisnik

**Naziv use casea:** Izmjena korisničkih podataka

**Kratak opis:**
Korisnik mijenja svoju email adresu ili lozinku kako bi imao tačne kontakt informacije u sistemu

**Preduslovi:**
- Korisnik je prijavljen u sistem

**Glavni tok:**
1. Korisnik otvara sekciju "Profil"
2. Korisnik pristupa sekciji za upravljanje profilom
3. Korisnik odabire opciju za promjenu podataka "Uredi profil"
4. Korisnik unosi trenutne podatke za validaciju
5. Korisnik unosi nove podatke
6. Korisnik potvrđuje izmjene
7. Sistem validira unese podatke
8. Sistem ažurira podatke

**Alternativni tokovi:**
- A1: Nevalidan format e-maila
> Ukoliko korisnik unese nevalidan email sistem odbija unos i zahtijeva ispravku 
- A2: Neispravna trenutna lozinka
> Ako korisnik unose neispravnu trenutnu lozinku sistem odbija unos i zahtjeva ispravu ; korisniku prikazuje poruku :
> "Netačna lozinka!" 
- A3: Unos identičnih podataka
> Sistem ne vrši izmjene jer nema promjena, korisniku prikazuje poruku : "Nova lozinka se mora razlikovati od prethodne!"
- A4: Nevalidna lozinka 
> Sistem ne prihvata novu lozinku dok ne zadovolji kriterije 
- A5: Odustajanje 
> Korisnik napušta stranicu bez spremanja izmjena

**Ishod:** Podaci uspješno ažurirani u sistemu

---

## UC-03: Pregled paketa

**Akter:** Korisnik

**Naziv use casea:** Pregled paketa i pretplata

**Kratak opis:**
Korisnik pregledava listu svojih aktivnih paketa i pretplata kako bi imao pregled usluga koje koristi.

**Preduslovi:**
- Korisnik je prijavljen
- Korisnik ima aktivne pakete u sistemu

**Glavni tok:**
1. Korisnik otvara sekciju "Paketi"
2. Sistem prikazuje listu svih aktivnih paketa i pretplata
3. Korisnik pregleda pakete
4. Korisnik bira specifičan paket
5. Sistem prikazuje detalje o odabranom paketu

**Alternativni tokovi:**
- A1: Nema aktivnih paketa 
> Sistem prikazuje prazno stanje bez liste paketa
- A2: Greška pri dohvatanju paketa
> Sistem ne učitava listu paketa i prikazuje korisniku poruku : "Greška pri učitavanju!"
- A3: Paket više nije dostupan
> Sistem ne može prikazatu detalje i vraća korisnika na listu

**Ishod:** Korisnik ima pregled svojih paketa

---

## UC-04: Pregled vlastitih tiketa

**Akter:** Korisnik, Tehničar

**Naziv use casea:** Pregled liste vlastitih tiketa

**Kratak opis:**
Korisnik pregledava listu svih svojih tiketa s mogućnošću uvida u detalje istih

**Preduslovi:**
- Akter je prijavljen
- Akter ima kreirane tikete

**Glavni tok:**
1. Akter otvara sekciju "Moji tiketi"
2. Sistem prikazuje listu tiketa sa osnovnim informacijama
3. Akter odabire filtere (prioritet, datum, status)
4. Sistem prikazuje filtrirane rezultate
5. Akter bira tiket za detaljnije informacije
6. Sistem prikazuje odabrani tiket s detaljnijim opisom

**Alternativni tokovi:**
- A1: Nema tiketa 
> Lista ostaje prazna
- A2: Nevažeći filter
> Ukoliko akter primjeni filter koji ne vrijedi za postojeće tikete, tada prikaz liste tiketa se ne mijenja

**Ishod:** Korisnik ima pregled svih svojih tiketa

---

## UC-05: Kreiranje novog tiketa

**Akter:** Korisnik

**Naziv use casea:** Kreiranje novog tiketa

**Kratak opis:**
Korisnik kreira novi tiket unosom opisa problema, tipa i prioriteta kako bi prijavio poteškoću tehničkoj podršci

**Preduslovi:**
- Korisnik je prijavljen

**Glavni tok:**
1. Korisnik otvara sekciju ""Kreiraj tiket"
2. Sistem prikazuje formu ta unos podataka
3. Korisnik unosi podatke ili bira iz predefinisane liste (naslov, opis, tip, prioritet)
4. Korisnik klikne na dugme "Pošalji"
5. Sistem validira podatke
6. Sistem kreira tiket sa statusom "OTVOREN" i dodjeljuje ID
7. Sistem prikazuje potvrdu o uspješnom kreiranju tiketa

**Alternativni tokovi:**
- A1: Nepotpuni podaci 
> Ukoliko obavezna polja nisu popunjena sistem priakzuje poruku : "Molimo popunite sva obavezna polja!"
- A2: Nevalidan unos 
> Ako opis sadrži nedozvoljene znakove ili je prekratak, sistem ne nastavlja sa kreiranjem tiketa
- A3: Odustajanje 
> Ako korisnik odustane prilikom procesa kreiranja tiketa sistem ne čuva unese podatke
- A4: Greška sistema 
> Ukoliko dođe do greške pri kreiranju tiketa sistem priakzuje poruu : "Došlo je do greške. Tiket nije kreiran."

**Ishod:** Tiket je uspješno kreiran

---

## UC-06: Dodjela tiketa

**Akter:** Sistem

**Naziv use casea:** Automatska dodjela tiketa

**Kratak opis:**
Sistem automatski dodjeljuje novokreirani tiket odgovarajućem dostupnom agentu prema predefinisanim pravilima

**Preduslovi:**
- Tiket postoji
- Postoje definisana pravila dodjele
- Postoji dostupan agent

**Glavni tok:**
1. Sistem detektuje novokreirani tiket
2. Sistem primjenjuje predefinisana pravila dodjele
3. Sistem pronalazi odgovarajućeg agenta
4. Sistem dodjeljuje tiket agentu
5. SIstem evidetnira dodjelu
6. Agent prima notifikaciju o dodjeli

**Alternativni tokovi:**
- A1: Nema dostupnih agenata
> Ukoliko u trenutku kreiranja tiketa nema dostupnih agenata tiket ostaje nedidjeljen i tako označen u sistemu
- A2: Neuspješna evaluacija pravila
> Sistem koristi fallback strategiju (npr. random dodjela)

**Ishod:** Tiket je automatski dodijeljen dostupnom agentu ili označen kao nedodijeljen

---

## UC-07: Komunikacija kroz tiket

**Akter:** Korisnik, Agent, Tehničar

**Naziv use casea:** Razmjena poruka

**Kratak opis:**
Omogućava komunikaciju između korisnika i podrške.

**Preduslovi:**
- Tiket postoji

**Glavni tok:**
1. Akter otvara tiket za detaljniji pregled
2. Sistem prikazuje historiju poruka
3. Akter unosi novu poruku u tekstualno polje
4. Akter klikne na dugme "Pošalji"
5. Sistem validira sadržaj poruke
6. Sistem prikazuje novu poruku u thread-u
7. Ostali učesnici razgovora dobijaju notifikaciju o novoj poruci

**Alternativni tokovi:**
- A1: Prazna poruka 
> Ukoliko poruka ima prazan sadržaj, sistem prikazuje notifikaciju : "Poruka ne može biti prazna!"
- A2: Preduga poruka 
> Ukoliko poruka prekorači maksimalan broj karaktera dozovljen, prikazuje se notifikacija : "Prekoračen maksimalan broj karaktera."
- A3: Greška pri slanju 
> Sistem ne ažurira thread komunikacije, i šalje korisniku obavijest o neuspjelom slanju
- A4: Tiket zatvoren
> Ukoliko je tiket označen sa statusom "ZATVOREN" komunikacije je onemogućena

**Ishod:** Poruka je evidentirana

---

## UC-08: Ažuriranje statusa tiketa

**Akter:** Tehničar

**Naziv use casea:** Promjena statusa tiketa

**Kratak opis:**
Proces omogućava tehničaru da ažurira status tiketa tokom njegovog životnog ciklusa

**Preduslovi:**
- Tiket je dodijeljen tehničaru

**Glavni tok:**
1. Tehničar otvara tiket
2. Sistem prikazuje detalje tiketa, uključujući trenutni status
3. Tehničar bira novi status
4. Sistem validira promjenu
5. Sistem sprema novi status

**Alternativni tokovi:**
- A1: Neispravan redoslijed statusa
> Ukoliko dođe do ilegalnog redoslijeda statusa (npr. zatvoren → otvoren) sistem blokira promjenu
- A2: Tiket nije dodijeljen tehničaru
> Sistem ne dozvoljava izmjene

**Ishod:** Status tiketa je ažuriran

---

## UC-09: Zatvaranje tiketa

**Akter:** Korisnik, Agent

**Naziv use casea:** Zatvaranje tiketa

**Kratak opis:**
Tiket se zatvara nakon rješenja.

**Preduslovi:**
- Tiket riješen

**Glavni tok:**
1. Agent označava tiket kao riješen
2. Sistem obavještava korisnika
3. Korisnik pregledava rješenje
4. Korisnik potvrđuje rješenje
5. Sistem mijenja status u "ZATVOREN"

**Alternativni tokovi:**
- A1: Korisnik odbija rješenje 
> Tiket se vraća u status "otvoren"
- A2: Nema odgovora od korisnika
> Sistem automatski zatvara tiket nakon definisanog vremena
- A4: Greška sistema 
> Status tiketa nije promijenjen

**Ishod:** Tiket zatvoren

---

## UC-10: Ocjenjivanje tiketa

**Akter:** Korisnik

**Naziv use casea:** Ocjena usluge

**Kratak opis:**
Korisnik daje feedback na kvalitet rješavanja tiketa

**Preduslovi:**
- Tiket ima status "ZATVOREN"

**Glavni tok:**
1. Sistem prikazuje opciju ocjenjivanja
2. Korisnik bira ocjenu
3. Sistem sprema ocjenu

**Alternativni tokovi:**
- A1: Korisnik ne unese ocjenu 
> Proces se završava bez unosa

**Ishod:** Ocjena evidentirana

---

## UC-11: FAQ pregled

**Akter:** Korisnik

**Naziv use casea:** Pregled FAQ

**Kratak opis:**
Korisnik vidi često postavljena pitanja.

**Preduslovi:**
- FAQ postoji

**Glavni tok:**
1. Korisnik otvara sekciju "FAQ"
2. Sistem dohvaća listu pitanja i odgovora
3. Sistem prikazuje FAQ listu

**Alternativni tokovi:**
- A1: FNema FAQ sadržaja
> Sistem prikazuje prazno stanje uz poruku: "Trenutno nema dostupnih pitanja."
- A2: Greška učitavanja 
> Sistem ne prikazuje sadržaj i omogućava ponovno učitavanje

**Ishod:** Korisnik dobija relevantne informacije

---

## UC-12: Pretraga tiketa

**Akter:** Korisnik, Agent

**Naziv use casea:** Pretraga tiketa

**Kratak opis:**
Akter vrši pretragu tiketa prema različitim kriterijima

**Preduslovi:**
- Postoje tiketi

**Glavni tok:**
1. Akter unosi kriterij pretrage (ID, ključna riječ, filteri)
2. Pokreće pretragu klikom na "Pretraži"
3. Sistem pretražuje bazu podataka
4. Sistem prikazuje rezultate

**Alternativni tokovi:**
- A1: Nema rezultata 
> Sistem prikazuje poruku: "Nema pronađenih tiketa."
- A2: Nevalidan unos 
> Sistem ne pokreće pretragu
- A3: Greška pretrage 
> Sistem ne vraća rezultate i omogućava ponovni pokušaj
- A4: Nedozvoljen pristup rezultatima
> Sistem filtrira rezultate prema pravima pristupa

**Ishod:** Pronađeni odgovarajući tiketi

---

## UC-13: Prosljeđivanje tiketa

**Akter:** Agent

**Naziv use casea:** Prosljeđivanje tiketa

**Kratak opis:**
Agent prosljeđuje tiket drugom agentu

**Preduslovi:**
- Tiket postoji i nije zatvoren
- Agent ima pristup tiketu

**Glavni tok:**
1. Agent otvara tiket
2. Bira opciju "Proslijedi"
3. Sistem prikazuje listu dostupnih agenata/timova
4. Agent bira novog vlasnika tiketa
5. Agent potvrđuje akciju
6. Sistem mijenja dodjelu tiketa
7. Novi agent dobija notifikaciju

**Alternativni tokovi:**
- A1: Prosljeđivanje samom sebi 
> Sistem ignoriše akciju ili je blokira, i prikazuje poruku :  "Zabranjena samododjela!"
- A2: Greška sistema
> Dodjela se ne mijenja

**Ishod:** Tiket je dodijeljen novom agentu

---

## UC-14: Pregled i upravljanje svim tiketima

**Akter:** Agent

**Naziv use casea:** Pregled i upravljanje svim tiketima

**Kratak opis:**
Agent vidi sve tikete u sistemu.

**Preduslovi:**
- Agent je prijavljen
- Sistem sadrži tikete

**Glavni tok:**
1. Agent otvara sekciju "Tiketi"
2. Sistem prikazuje listu svih tiketa
3. Agent primjenjuje filter
4. Sistem prikazje sortiranu listu tiketa prema odabranom tiketu
5. Agwnt mijwnja status ili prioritet
6. Sistem validira izmjene
7. Sistem sprema izmjene

**Alternativni tokovi:**
- A1: Nema tiketa 
> Sistem prikazuje praznu listu
- A3: Nevažeće izmjene
> Sistem odbija unos i prikazuje upozorenje agentu
- A4: Nedostatak privilegija
> Sistem blokira akciju

**Ishod:** Agent ima pregled svih tiketa

---

## UC-15: Upravljanje timovima

**Akter:** Administrator

**Naziv use casea:** Upravljanje timovima agenata

**Kratak opis:**
Admin vrši kreiranje i organizaciju timova agenata radi efikasnije strukture rada

**Preduslovi:**
- Postoje agenti

**Glavni tok:**
1. Administrator otvara sekciju "Timovi"
2. Sistem prikazuje postojeće timove
3. Administrator bira tim ili kreira novi
4. Dodaje ili uklanja agente iz tima
5. Administrator potvrđuje izmjene
6. Sistem validira promjene
7. Sistem sprema konfiguraciju timova

**Alternativni tokovi:**
- A1: Agent već pripada timu 
> Sistem ne dodaje agenta ponovo i šalje upozorenje : "Agent je već dodijeljen postojećem timu"
- A2: Tim ne postoji 
> Ukoliko administrator pokuša dodijeliti agenta nepostojećem timu sistem ne mijenja pripadnost agenta i šalje upozorenje "Tim ne postoji!"
- A3: Greška spremanja 
> Izmjene nisu sačuvane
- A4: Prazan tim
> Sistem dozvoljava ali označava tim kao neaktivan

**Ishod:** Timovi ažurirani

---

## UC-16: Upravljanje korisnicima 

**Akter:** Administrator

**Naziv use casea:** Upravljanje korisnicima 

**Kratak opis:**
Proces omogućava administratoru upravljanje korisnicima sistema — kreiranje, izmjenu, deaktivaciju naloga i dodjelu u timove

**Preduslovi:**
- Admin prijavljen

**Glavni tok:**
1. dministrator otvara sekciju "Korisnici"
2. Sistem prikazuje listu svih korisnika
3. Administrator bira korisnika ili opciju "Novi korisnik"
4. Administrator unosi ili mijenja podatke
5. Administrator dodjeljuje ulogu i tim
6. Klikne na "Sačuvaj"
7. Sistem validira podatke
8. Sistem sprema izmjene u bazu

**Alternativni tokovi:**
- A1: Nevalidni podaci 
> Sistem ne dozvoljava spremanje i označava problematična polja
- A2: Korisnik ne postoji 
> Ukoliko administrator pokuašava manipulirati podacima korisnika koji ne postoji sistem prekida akciju i vraća na listu
- A3: Nedostatak privilegija
> Sistem blokira pristup funkcionalnosti
A4: Duplikat e-mail adrese
> Sistem odbija unos uz poruku: "Korisnik sa ovom e-mail adresom već postoji."

**Ishod:** Podaci ažurirani

---

## UC-17: Generisanje izvještaja

**Akter:** Menadžment

**Naziv use casea:** Generisanje izvještaja

**Kratak opis:**
Menadžment generiše statističke izvještaje o radu sistema

**Preduslovi:**
- Menadžer je prijavljen

**Glavni tok:**
1. Menadžer otvara sekciju "Izvještaji"
2. Menadžer bira tip izvještaja
3. Sistem obrađuje podatke
4. Sistem generiše izvještaj
5. Sistem prikazuje rezultate

**Alternativni tokovi:**
- A1: Nema podataka 
> Sistem prikazuje poruku : "Nema aktuelnih podataka za izvještaj."
- A2: Greška obrade
> Izvještaj nije generisan
- A3: Prevelik skup podataka
> Sistem ograničava ili parcijalno prikazuje rezultate

**Ishod:** Izvještaj je generisan

---
