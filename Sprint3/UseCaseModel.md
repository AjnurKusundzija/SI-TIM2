# Use Case Model



<a id="uc-01"></a>
# UC-01: Login korisnika
### Akter:
Korisnik / Agent / Tehničar / Menadžment
### Naziv use casea:
Login u sistem
### Kratak opis:
Korisnik se prijavljuje u sistem unosom email adrese i lozinke kako bi pristupio svom profilu i odgovarajućim funkcionalnostima sistema
### Preduslovi:
* Korisnik ima kreiran nalog u sistemu
* Korisnik nije trenutno prijavljen
### Glavni tok:
1. Korisnik unosi email adresu i lozinku
2. Korisnik klikne na dugme “Login”
3. Sistem validira unesene podatke
4. Korisnik se uspješno prijavljuje
5. Korisnik je usmjeren na početnu stranicu sistema i ima pristup daljim funkcionalnostima
### Alternativni tokovi:
A1 : Korisnik ne unese podatke → sistem upozorava korisnika da polja nisu popunjena

A2 : Pogrešni podaci → prikazuje se poruka o grešci -> korinsiku se nudi mogućnost ponovnog pokušaja

A3 : Nalog ne postoji → poruka o registraciji

A4 : Sistem nedostupan → login nije moguć
### Ishod:
Korisnik uspješno pristupa sistemu


---

<a id="uc-02"></a>
# UC-02: Upravljanje korisničkim profilom
### Akter: 
Korisnik 
### Naziv use casea:
Izmjena korisničkih podataka 
### Kratak opis: 
Korisnik mijenja svoju email adresu ili lozinku kako bi imao tačne kontakt informacije u sistemu
### Preduslovi:
* Korisnik je prijavljen u sistem
### Glavni tok: 
1. Korisnik otvara profil 
2. Korisnik pristupa sekciji za upravljanje profilom
3. Korisnik odabire opciju za promjenu podataka
4. Korisnik unosi trenutke podatke za validaciju
4. Korisnik unosi nove podatke
5. Korisnik potvrđuje izmjene 
6. Sistem ažurira podatke 
### Alternativni tokovi: 
A1 : Nevalidan format novog emaila → sistem prikazuje grešku i odbija izmjene

A2 : Pogrešna trenutna lozinka → odbijanje izmjene 

A3 : Korisnik unosi trenutne podatke kao nove → sistem prikazuje grešku i odbija izmjene

A4 : Nevalidan format nove lozinke → sistem prikazuje grešku i odbija izmjene

A5 : Korisnik odustaje od promjene → sistem ne donosi izmjene
### Ishod:
 Podaci uspješno ažurirani u sistemu

---

<a id="uc-03"></a>
# UC-03: Pregled paketa
### Akter: 
Korisnik 
### Naziv use casea: 
Pregled paketa i pretplata 
### Kratak opis: 
Korisnik pregledava listu svojih aktivnih paketa i pretplata kako bi imao pregled usluga koje koristi
### Preduslovi: 
* Korisnik je prijavljen 
* Korisnik ima aktivne pakete u sistemu
### Glavni tok: 
1. Korisnik otvara sekciju “Paketi” 
2. Sistem prikazuje listu svih aktivnih paketa i pretplata
3. Korisnik pregleda pakete
4. Korisnik bira specifičan paket 
5. Sistem prikazuje detalje o odabranom paketu
### Alternativni tokovi: 
A1 : Nema paketa → prikaz poruke 
### Ishod: 
Korisnik vidi informacije o paketima

---

<a id="uc-04"></a>
# UC-04: Kreiranje novog tiketa
### Akter:
Korisnik
### Naziv use casea:
Kreiranje novog tiketa
### Kratak opis:
Korisnik kreira novi tiket unosom opisa problema, tipa i prioriteta kako bi prijavio poteškoću tehničkoj podršci
### Preduslovi:
* Korisnik je prijavljen
### Glavni tok:
1. Korisnik otvara formu za kreiranje tiketa
2. Korisnik unosi podatke ili bira iz predefinisane liste (naslov, opis, tip, prioritet)
3. Korisnik klikne na dugme “Pošalji”
4. Sistem validira podatke
5. Sistem kreira tiket i dodjeljuje ID
6. Sistem prikazuje potvrdu o uspješnom kreiranju tiketa
### Alternativni tokovi:
A1 : Nepotpuni podaci → sistem traži ispravku

A2 : Nevalidan format (npr. prekratak opis)

A3 : Korisnik odustane → forma se zatvara

A4 : Greška u sistemu → tiket nije kreiran

### Ishod:
Tiket je uspješno kreiran

---

<a id="uc-05"></a>
# UC-05: Dodjela tiketa
### Akter:
Sistem
### Naziv use casea:
Automatska dodjela tiketa
### Kratak opis:
Sistem automatski dodjeljuje novokreirani tiket odgovarajućem dostupnom agentu prema predefinisanim pravilima
### Preduslovi:
* Tiket postoji
* Psotoje definisana pravila dodjele
* Postoji dostupan agent
### Glavni tok:
1. Korisnik uspješno kreira novi tiket
2. Sistem primjenjuje predefinisana pravila dodjele
3. Sistem pronalazi dostupnog agenta
4. Sistem dodjeljuje tiket agentu
5. Agent prima notifikaciju o dodjeli
### Alternativni tokovi:
A1 : Nema dostupnog tehničara
A2 : Nevažeći izbor tehničara
A3 : Agent odustane
### Ishod:
Tiket je automatski dodijeljen dostupnom agentu ili označen kao nedodijeljen

---

<a id="uc-06"></a>
# UC-06 : Prosljeđivanje tiketa
### Akter:
Agent 
### Naziv use casea: 
Prosljeđivanje tiketa 
### Kratak opis:
Agent prosljeđuje tiket drugom aktivnom agentu uz opcioni interni komentar radi efikasnog rješavanja problem
### Preduslovi: 
* Otvoren tiket
* Agent je prijavljen
* Postoji više od jednog agenta 
### Glavni tok: 
1. Agent bira opciju za prosljeđivanje tiketa 
2. Sistem prikazuje listu aktivnih agenata
3. Agent odabire novog agenta
4. (Opcionalno)Agent dodaje komentar 
5. Sistem dodjeljuje tiket novom agentu 
6. Odabrani agent prima notifikaciju o prosljeđenom tiketu 
### Alternativni tokovi: 
A1 : Tiket zatvoren → zabrana 
A2 : Agent pokušava poslati sebi → greška 
### Ishod:
Tiket je prosljeđen novom agentu koji prima notifikaciju i interni komentar

---

<a id="uc-07"></a>
# UC-07: Pregled vlastitih tiketa 
### Akter:
Korisnik 
### Naziv use casea:
Pregled liste vlastitih tiketa
### Kratak opis:
Korisnik pregledava listu svih svojih tiketa s mogućnošću uvida u detalje istih
### Preduslovi:
* Korisnik je prijavljen
* Korisnik ima kreirane tikete
### Glavni tok:
1. Korisnik otvara sekciju “Moji tiketi”
2. Sistem prikazuje listu tiketa sa osnovnim informacijama 
3. Korisnik odabire filtere (prioritet, datum, status)
4. Sistem prikazuje filtrirane rezultate
5. Korisnik bira tiket za detaljnije informacije
6. Sistem prikazuje odabrani tiket s detalnjijim opisom
### Alternativni tokovi:
A1 : Nema tiketa → prikaz poruke

A2 : Greška pri učitavanju → pokušaj ponovo

A3 : Tiket ne postoji → poruka o grešci
### Ishod:
Korisnik ima pregled svih svojih tiketa

---

<a id="uc-08"></a>
# UC-08: Pregled dodijeljenih tiketa 
### Akter:
Tehničar 
### Naziv use casea: 
Pregled dodijeljenih tiketa 
### Kratak opis:
Tehničar pregledava samo njemu dodijeljene tikete i filtrira ih radi organizacije rada
### Preduslovi: 
* Tehničar je prijavljen
* Tehničar ima dodijeljene tikete
### Glavni tok: 
1. Tehničar pristupa sekciji "Tiketi"
2. Sistem prikazuje samo tikete dodijeljene tehničaru
3. Tehničar bira tiket za detaljniji pregled
4. Sistem prikazuje odabrani tiket sa opširnijim opisom 
5. Tehničar pristupa podacima o korisniku koji je kreirao tiket
6. Sistem prikazuje neosjetljive korisničke podatke
7. Tehničar ima uvid u historiju tiketa korisnika
### Alternativni tokovi: 
Nema tiketa → poruka 
### Ishod: 
Tehničar ima prikaz dodijeljenih tiketa

---

<a id="uc-09"></a>
# UC-09: Upravljanje tiketima 
### Akter:
Agent 
### Naziv use casea: 
Pregled i upravljanje tiketima 
### Kratak opis: 
Agent pregleda i ažurira tikete 
### Preduslovi: 
* Agent prijavljen
* Tiket postoji 
### Glavni tok: 
1. Agent otvara sekciju sa tiketima
2. Sistem prikazuje sve tikete
3. Agent bira tiket
4. Agent vidi detalje tiketa
5. Agent mijenja status tiketa
6. Sistem ažurira promjene
### Alternativni tokovi: 
A1: Nevažeći status → greška 
A2 : Greška pri učitavanju
A3: Nedozvoljen pristup → agent nema prava
### Ishod:
Tiket ažuriran 

---

<a id="uc-10"></a>
# PROVJERITI ZA PORUKE UC-10. Komunikacija kroz tiket
### Akter:
Korisnik / Agent / Tehničar
### Naziv use casea:
Razmjena poruka 
### Kratak opis:
Omogućava komunikaciju između korisnika i podrške.
Preduslovi:
Tiket postoji
Glavni tok:
Akter otvara tiket
Piše poruku
Klikne “Pošalji”
Sistem sprema i prikazuje poruku

Alternativni tokovi:
Prazna poruka → odbijanje slanja
Preduga poruka → sistem ograničava unos
Greška pri slanju → poruka nije sačuvana
Tiket zatvoren → slanje nije moguće

Ishod:
Poruka je evidentirana

--- 

<a id="uc-11"></a>
# UC-11: Ažuriranje statusa tiketa
### Akter:
Tehničar
### Naziv use casea:
Promjena statusa tiketa
### Kratak opis:
Tehničar mijenja status tiketa u toku rada, u svhru označavanja napretka i informisanja korisnika 
### Preduslovi:
* Tehničar je prijavljen
* Tiket je dodijeljen tehničaru
### Glavni tok:
1. Tehničar otvara tiket
2. Tehničar odabire opciju za promjenu statusa
3. Sistem prikazuje predefinisana stanja
4. Tehničar odabire novi status
5. Sistem evidentira promjenu
6. Korisnik prima obavijest o promjeni statusa
### Alternativni tokovi:
A1: Nevalidan status
A2: Tiket već zatvoren
A3: Greška pri spremanju
A4: Tehničar nema pristup tiketu
### Ishod:
Status tiketa je ažuriran, historija promjena je evidentirana, korisnik je obaviješten

---

<a id="uc-12"></a>
# UC-12 : Zatvaranje tiketa -> vidjeti kako radi po novom sistemu
Akter: Korisnik, Agent 
Naziv use casea: Zatvaranje tiketa 
Kratak opis: Tiket se zatvara nakon rješenja 
Preduslovi: Tiket riješen 
Glavni tok: 
Agent inicira zatvaranje 
Korisnik prihvata 
Sistem zatvara tiket 
Alternativni tokovi: 
Korisnik odbija → tiket ostaje otvoren 
Nema odgovora 7 dana → automatsko zatvaranje 
Ishod: Tiket zatvoren

---

<a id="uc-13"></a>
# UC-13: Ocjenjivanje tiketa
# Akter: 
Korisnik 
### Naziv use casea:
Ocjena usluge 
### Kratak opis: 
Korisnik ocjenjuje kvalitetu rješenja zatvorenog tiketa kao feedback o pruženoj usluzi
### Preduslovi: 
* Prijavljen korisnik
* Tiket zatvoren 
### Glavni tok: 
1. Korisnik pristupa sekciji sa tiketima
2. Korisnik bira opciju "Ocijeni"
3. Korisnik odabire ocjenu od opcija 1-5 i potvrđuje odluku
4. Sistem sprema ocjenu
### Alternativni tokovi :

### Ishod:
 Ocjena evidentirana

---


<a id="uc-14"></a>
# UC-14: Pretraga tiketa
### Akter: 
Korisnik, Agent 
### Naziv use casea: 
Pretraga tiketa 
### Kratak opis:
Akter pretražuje tikete po ID-u radi bržeg pronalaska željenog tiketa
### Preduslovi: 
* Prijavljen korisnik
* Postoje tiketi 
### Glavni tok: 
1. Akter unosi tekst u polje za pretragu (pretraga po ID-u, case-insensitive)
2. Akter pokreće pretragu 
3. Sistem prikazuje odgovarajuće tikete koji pripadaju korisniku
### Alternativni tokovi: 
A1 : Nema rezultata → sistem prikazuje poruku 
### Ishod:
 Pronađeni traženi tiketi 

---

<a id="uc-15"></a>
# UC-15 : Pregled i uređivanje korisničkih profila
### Akter :
Administrator
### Naziv use casea :
Pregled i uređivanje korisničkih profila
### Kratak opis :
 Administrator pregledava i uređuje profile korisnika uz evidentiranje izmjena, bez pristupa lozinkama
### Preduslovi :
* Administrator je prijavljen
* Sitem ima aktivne korsinike
### Glavni tok :
1. Adminsitrator pristupa sekciji korisnika
2. Sistem prikazuje listu postojećih korisnika
3. Administator pretražuke korisnika po imenu ili emailu
4. Administrator odabire korisnika
5. Sistem prikazuje odabrani korisnički profil
6. Adminsitrator uređuje osnovne informacije i potvrđuje promjenu
7. ASistem evidentira promjenu podataka
### Alternativni tokovi :
A1 : Korisnik nije pornađen
A2 : Izmjena nije potvrđena
A3 : Pokušaj izmjene lozinke
### Ishod :
Korisnički profil je ažuriran, promjena evidentirana

---

<a id="uc-16"></a>
# UC-16: Upravljanje timovima
###  Akter:
Administrator 
### Naziv use casea: 
Upravljanje timovima agenata 
### Kratak opis: 
Administator ima uvid u postojeće timove, i agente koji ih sačinavanju. Administrator može da preraspodijeljuje agente između timova radi optimizacije rada
### Preduslovi:
* Administator je prijavljen
* Postoje agenti 
### Glavni tok: 
Admin bira agenta 
Dodjeljuje ga timu 
Sistem sprema promjenu 
### Alternativni tokovi :

### Ishod:
Timovi su uspješno ažurirani 

---

<a id="uc-17"></a>
# UC-17 : Generisanje izvještaja
### Akter:
Menadžment
### Naziv use casea:
Izvještaji
### Kratak opis:
Menadžment generiše izvještaje o različitim statistikama radi boljeg uvida u rad i kvalitet sistema i radnika
### Preduslovi:
* Menadžer je prijavljen
### Glavni tok:
1. Menadžer bira tip izvještaja
2. Sistem obrađuje podatke
3. Sistem prikazuje rezultate u vidu formatiranog izvještaja
### Alternativni tokovi:
A1: Neodabran tip izvještaja
A2: ema podataka
A3: Greška u obradi
A4: Neuspješno prikazivanje
### Ishod:
Izvještaj je generisan

---

<a id="uc-18"></a>
# UC-18: FAQ pregled
### Akter: 
Korisnik 
### Naziv use casea: 
Pregled FAQ 
### Kratak opis:
Korisnik pregledava listu najčešće postavljanih pitanja i odgovora radi eventualnog rješavanja problema bez slanja tiketa
### Preduslovi:
* Korisnik je prijavljen
* FAQ postoji 
### Glavni tok: 
1. Korisnik otvara FAQ sekciju
2. Sistem prikazuje listu pitanja i odgovora
3. Korisnik pregledava relevantna pitanja i odgovore
### Alternativni tokovi :

### Ishod: 
Korisnik dobija informacije

---

<a id="uc-19"></a>
# UC-19: Odjava iz sistema
### Akter:
Korisnik/Adminsitrator/Tehničar/Agent
### Naziv use casea:
Odjava iz sistema
### Kratak opis:
Korisnik se odjavljuje iz sistema kako bi zaštitio svoj nalog i spriječio neovlašteni pristup
### Preduslovi:
*Korisnik je prijavljen
### Glavni tok:
1. Korisnik klikne na opciju "Logout"
2. Sistem odjavljuje korisnika i poništava sesiju
3. Sistem preusmjerava korisnika na login stranicu
4. Sistem onemogućava pristup zaštićenim stranicama
### Alternativni tokovi:
### Ishod: 
Korisnik je odjavljen. Sesija je poništena i pristup zaštićenim stranicama je onemogućen


