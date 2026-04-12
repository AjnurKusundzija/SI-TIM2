# Use Case Model

---

## UC-1: Login korisnika

**Akter:** Korisnik / Agent / Tehničar / Menadžment

**Naziv use casea:** Login u sistem

**Kratak opis:**
Korisnik se prijavljuje u sistem unosom email adrese i lozinke kako bi pristupio svom profilu i odgovarajućim funkcionalnostima sistema.

**Preduslovi:**
- Korisnik ima kreiran nalog u sistemu
- Korisnik nije trenutno prijavljen

**Glavni tok:**
1. Korisnik unosi email adresu i lozinku
2. Korisnik klikne na dugme "Login"
3. Sistem validira unesene podatke
4. Korisnik se uspješno prijavljuje

**Alternativni tokovi:**
- A1: Polja prazna → sistem traži unos
- A2: Pogrešni podaci → poruka o grešci
- A3: Nalog ne postoji → ponuda registracije
- A4: Sistem nedostupan → login nije moguć

**Ishod:** Korisnik pristupa sistemu

---

## UC-2: Upravljanje korisničkim profilom

**Akter:** Korisnik

**Naziv use casea:** Izmjena korisničkih podataka

**Kratak opis:**
Korisnik mijenja svoju email adresu ili lozinku kako bi imao tačne kontakt informacije u sistemu.

**Preduslovi:**
- Korisnik je prijavljen u sistem

**Glavni tok:**
1. Korisnik otvara profil
2. Korisnik pristupa sekciji za upravljanje profilom
3. Korisnik odabire opciju za promjenu podataka
4. Korisnik unosi trenutne podatke za validaciju
5. Korisnik unosi nove podatke
6. Korisnik potvrđuje izmjene
7. Sistem ažurira podatke

**Alternativni tokovi:**
- A1: Nevalidan email → odbijanje
- A2: Pogrešna lozinka → odbijanje
- A3: Isti podaci → greška
- A4: Nevalidna lozinka → odbijanje
- A5: Odustajanje → bez izmjene

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
- A1: Nema paketa → poruka
- A2: Greška učitavanja → pokušaj ponovo

**Ishod:** Korisnik vidi informacije o paketima

---

## UC-4: Kreiranje novog tiketa

**Akter:** Korisnik

**Naziv use casea:** Kreiranje novog tiketa

**Kratak opis:**
Korisnik kreira novi tiket unosom opisa problema, tipa i prioriteta kako bi prijavio poteškoću tehničkoj podršci.

**Preduslovi:**
- Korisnik je prijavljen

**Glavni tok:**
1. Korisnik otvara formu za kreiranje tiketa
2. Korisnik unosi podatke ili bira iz predefinisane liste (naslov, opis, tip, prioritet)
3. Korisnik klikne na dugme "Pošalji"
4. Sistem validira podatke
5. Sistem kreira tiket i dodjeljuje ID
6. Sistem prikazuje potvrdu o uspješnom kreiranju tiketa

**Alternativni tokovi:**
- A1: Nepotpuni podaci → ispravka
- A2: Nevalidan unos → odbijanje
- A3: Odustajanje → zatvaranje
- A4: Greška sistema → tiket nije kreiran

**Ishod:** Tiket je uspješno kreiran

---

## UC-5: Pregled vlastitih tiketa

**Akter:** Korisnik

**Naziv use casea:** Pregled liste vlastitih tiketa

**Kratak opis:**
Korisnik pregledava listu svih svojih tiketa s mogućnošću uvida u detalje istih.

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
- A1: Nema tiketa → poruka
- A2: Greška → pokušaj ponovo
- A3: Tiket ne postoji → greška

**Ishod:** Korisnik ima pregled svih svojih tiketa

---

## UC-6: Pregled dodijeljenih tiketa

**Akter:** Tehničar

**Naziv use casea:** Pregled vlastitih tiketa

**Kratak opis:**
Tehničar pregledava listu tiketa koji su mu dodijeljeni kako bi mogao upravljati svojim zadacima.

**Preduslovi:**
- Tiketi dodijeljeni
- Tehničar je prijavljen u sistem

**Glavni tok:**
1. Tehničar otvara listu
2. Sistem prikazuje njegove tikete

**Alternativni tokovi:**
- A1: Nema tiketa → poruka
- A2: Greška učitavanja → pokušaj ponovo

**Ishod:** Tehničar ima pregled svojih dodijeljenih tiketa

---

## UC-7: Komunikacija kroz tiket

**Akter:** Korisnik / Agent / Tehničar

**Naziv use casea:** Razmjena poruka

**Kratak opis:**
Omogućava komunikaciju između korisnika i podrške.

**Preduslovi:**
- Tiket postoji

**Glavni tok:**
1. Akter otvara tiket
2. Piše poruku
3. Klikne "Pošalji"
4. Sistem sprema i prikazuje poruku

**Alternativni tokovi:**
- A1: Prazna poruka → odbijanje
- A2: Preduga poruka → ograničenje
- A3: Greška slanja → poruka nije sačuvana
- A4: Tiket zatvoren → slanje nije moguće

**Ishod:** Poruka je evidentirana

---

## UC-8: Pregled i upravljanje svim tiketima

**Akter:** Agent

**Naziv use casea:** Pregled i upravljanje svim tiketima

**Kratak opis:**
Agent vidi sve tikete u sistemu.

**Preduslovi:**
- Agent je prijavljen

**Glavni tok:**
1. Agent otvara listu tiketa
2. Sistem prikazuje sve tikete
3. Agent bira tiket
4. Mijenja status ili prioritet
5. Sistem sprema izmjene

**Alternativni tokovi:**
- A1: Nema tiketa → poruka
- A2: Greška učitavanja → pokušaj ponovo
- A3: Nevažeći unos → odbijanje
- A4: Nema prava → zabrana

**Ishod:** Agent ima pregled svih tiketa

---

## UC-9: Dodjela tiketa

**Akter:** Sistem

**Naziv use casea:** Automatska dodjela tiketa

**Kratak opis:**
Sistem automatski dodjeljuje novokreirani tiket odgovarajućem dostupnom agentu prema predefinisanim pravilima.

**Preduslovi:**
- Tiket postoji
- Postoje definisana pravila dodjele
- Postoji dostupan agent

**Glavni tok:**
1. Korisnik uspješno kreira novi tiket
2. Sistem primjenjuje predefinisana pravila dodjele
3. Sistem pronalazi dostupnog agenta
4. Sistem dodjeljuje tiket agentu
5. Agent prima notifikaciju o dodjeli

**Alternativni tokovi:**
- A1: Nema agenta → nedodijeljen
- A2: Greška → nije dodijeljen

**Ishod:** Tiket je automatski dodijeljen dostupnom agentu ili označen kao nedodijeljen

---

## UC-10: Ažuriranje statusa tiketa

**Akter:** Tehničar

**Naziv use casea:** Promjena statusa

**Kratak opis:**
Tehničar mijenja status tiketa tokom rada.

**Preduslovi:**
- Tiket je dodijeljen tehničaru

**Glavni tok:**
1. Tehničar otvara tiket
2. Mijenja status
3. Sistem sprema promjenu

**Alternativni tokovi:**
- A1: Nevalidan status → greška
- A2: Tiket zatvoren → zabrana
- A3: Nema pristupa → greška

**Ishod:** Status tiketa je ažuriran

---

## UC-11: Generisanje izvještaja

**Akter:** Menadžment

**Naziv use casea:** Izvještaji

**Kratak opis:**
Menadžment generiše statističke izvještaje.

**Preduslovi:**
- Menadžer je prijavljen

**Glavni tok:**
1. Menadžer bira tip izvještaja
2. Sistem obrađuje podatke
3. Prikazuje rezultate

**Alternativni tokovi:**
- A1: Nema podataka → poruka
- A2: Greška → neuspjeh

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
