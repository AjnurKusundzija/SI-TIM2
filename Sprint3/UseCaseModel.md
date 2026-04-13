# Use Case Model

---

## UC-1: Login korisnika

**Akter:** Korisnik / Agent / Tehničar / Menadžment

**Naziv use casea:** Prijava u sistem (Login)

**Kratak opis:**
Proces omogućava korisniku siguran pristup sistemu unosom e-mail adrese i lozinke, uz validaciju podataka i preusmjeravanje na odgovarajući interfejs u skladu s korisničkom ulogom

**Preduslovi:**
- Korisnik ima kreiran nalog u sistemu
- Korisnik nije trenutno prijavljen

**Glavni tok:**
1. Korinsik otvara formu za prijavu
2. Korisnik unosi email adresu i lozinku
3. Sistem validira format e-mail adrese
4. Korisnik klikne na dugme "Login"
5. Sistem validira unesene podatke
6. Sistem preusmjerava korisnika na dashboard prema njegovoj ulozi

**Alternativni tokovi:**
- A1: 
 Ako korisnik ne unese e-mail ili lozinku, sistem prikazuje poruku:
 > "Sva polja su obavezna."
- A2: Ako su uneseni pogrešni podaci, sistem priakzuje poruka o grešci
> "Pogrešan e-mail ili lozinka."
- A3: Ako nalog ne postoji, korinsiku se nudi mogučnost registracije
- A4: Ukoliko je sistem nedostupan prikazuje se poruka :
> "Trenutno nije moguće izvršiti prijavu. Pokušajte kasnije."

**Ishod:** Korisnik pristupa sistemu

---

## UC-2: Upravljanje korisničkim profilom

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

## UC-3: Pregled paketa

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

## UC-4: Kreiranje novog tiketa

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

## UC-5: Pregled vlastitih tiketa

**Akter:** Korisnik / Tehničar

**Naziv use casea:** Pregled liste vlastitih tiketa

**Kratak opis:**
Korisnik pregledava listu svih svojih tiketa s mogućnošću uvida u detalje istih

**Preduslovi:**
- Korisnik je prijavljen
- Korisnik ima kreirane tikete

**Glavni tok:**
1. Korisnik otvara sekciju "Moji tiketi"
2. Sistem prikazuje listu tiketa sa osnovnim informacijama
3. Korisnik odabire filtere (prioritet, datum, status)
4. Sistem prikazuje filtrirane rezultate
5. Korisnik bira tiket za detaljnije informacije
6. Sistem prikazuje odabrani tiket s detaljnijim opisom

**Alternativni tokovi:**
- A1: Nema tiketa 
> Lista ostaje prazna
- A2: Nevažeći filter
> Ukoliko korisnik primjeni filter koji ne vrijedi za postojeće tikete, tada prikaz liste tiketa se ne mijenja

**Ishod:** Korisnik ima pregled svih svojih tiketa

---

## UC-7: Komunikacija kroz tiket

**Akter:** Korisnik / Agent / Tehničar

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

## UC-8: Pregled i upravljanje svim tiketima

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

## UC-9: Dodjela tiketa

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

## UC-10: Ažuriranje statusa tiketa

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

## UC-11: Generisanje izvještaja

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

## UC-12: Zatvaranje tiketa

**Akter:** Korisnik, Agent

**Naziv use casea:** Zatvaranje tiketa

**Kratak opis:**
Tiket se zatvara nakon rješenja.

**Preduslovi:**
- Tiket riješen

**Glavni tok:**
1. Agent inicira zatvaranje
2. Korisnik prihvata
3. Sistem zatvara tiket

**Alternativni tokovi:**
- A1: Korisnik odbija rješenje → tiket ostaje otvoren
- A2: Nema odgovora → automatsko zatvaranje
- A3: Tiket nije riješen → zatvaranje nije dozvoljeno
- A4: Greška sistema → status nije promijenjen

**Ishod:** Tiket zatvoren

---

## UC-13: Ocjenjivanje tiketa

**Akter:** Korisnik

**Naziv use casea:** Ocjena usluge

**Kratak opis:**
Korisnik daje feedback.

**Preduslovi:**
- Tiket zatvoren

**Glavni tok:**
1. Korisnik bira ocjenu
2. Šalje ocjenu
3. Sistem sprema podatke

**Alternativni tokovi:**
- A1: Korisnik ne unese ocjenu → preskakanje
- A2: Nevalidna ocjena → odbijanje
- A3: Greška spremanja → ocjena nije sačuvana

**Ishod:** Ocjena evidentirana

---

## UC-14: Upravljanje korisnicima i timovima

**Akter:** Administrator

**Naziv use casea:** Upravljanje korisnicima

**Kratak opis:**
Admin upravlja korisnicima i timovima.

**Preduslovi:**
- Admin prijavljen

**Glavni tok:**
1. Admin pregleda korisnike
2. Mijenja podatke ili tim
3. Sistem sprema promjene

**Alternativni tokovi:**
- A1: Nevalidni podaci → odbijanje
- A2: Korisnik ne postoji → greška
- A3: Nema prava → zabrana
- A4: Greška spremanja → izmjene nisu sačuvane

**Ishod:** Podaci ažurirani

---

## UC-15: FAQ pregled

**Akter:** Korisnik

**Naziv use casea:** Pregled FAQ

**Kratak opis:**
Korisnik vidi često postavljena pitanja.

**Preduslovi:**
- FAQ postoji

**Glavni tok:**
1. Korisnik otvara FAQ
2. Sistem prikazuje pitanja i odgovore

**Alternativni tokovi:**
- A1: FAQ ne postoji → poruka
- A2: Nema rezultata → poruka
- A3: Greška učitavanja → pokušaj ponovo

**Ishod:** Korisnik dobija informacije

---

## UC-16: Prosljeđivanje tiketa

**Akter:** Agent

**Naziv use casea:** Prosljeđivanje tiketa

**Kratak opis:**
Agent prosljeđuje tiket drugom agentu.

**Preduslovi:**
- Tiket otvoren

**Glavni tok:**
1. Agent bira opciju prosljeđivanja
2. Odabire drugog agenta
3. (Opcionalno) dodaje komentar
4. Sistem dodjeljuje tiket novom agentu

**Alternativni tokovi:**
- A1: Tiket zatvoren → zabrana
- A2: Nevažeći agent → greška
- A3: Prosljeđivanje samom sebi → odbijanje
- A4: Greška sistema → tiket nije proslijeđen

---

## UC-17: Upravljanje timovima

**Akter:** Administrator

**Naziv use casea:** Upravljanje timovima agenata

**Kratak opis:**
Admin raspoređuje agente u timove.

**Preduslovi:**
- Postoje agenti

**Glavni tok:**
1. Admin bira agenta
2. Dodjeljuje ga timu
3. Sistem sprema promjenu

**Alternativni tokovi:**
- A1: Agent već u timu → upozorenje
- A2: Tim ne postoji → greška
- A3: Greška spremanja → izmjene nisu sačuvane

**Ishod:** Timovi ažurirani

---

## UC-18: Pretraga tiketa

**Akter:** Korisnik, Agent

**Naziv use casea:** Pretraga tiketa

**Kratak opis:**
Omogućava pronalazak tiketa po ID-u.

**Preduslovi:**
- Postoje tiketi

**Glavni tok:**
1. Akter unosi pojam za pretragu
2. Pokreće pretragu
3. Sistem prikazuje rezultate

**Alternativni tokovi:**
- A1: Nema rezultata → poruka
- A2: Nevalidan unos → odbijanje
- A3: Greška pretrage → pokušaj ponovo
- A4: Nedozvoljen pristup → ograničen prikaz

**Ishod:** Pronađeni odgovarajući tiketi
