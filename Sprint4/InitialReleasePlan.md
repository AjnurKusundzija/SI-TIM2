# Initial Release Plan


## Inkrement 1 – Osnovni Ticketing Sistem

**Cilj inkrementa:**  
Omogućiti korisnicima osnovnu interakciju sa sistemom uključujući autentifikaciju, kreiranje tiketa i osnovni pregled sistema.

**Glavne funkcionalnosti:**
- US-1, US-2, US-3 (Autentifikacija korisnika)  
  Omogućava korisnicima prijavu u sistem putem emaila i lozinke uz validaciju unosa, sigurnu kontrolu pristupa i logout funkcionalnost koja prekida korisničku sesiju i štiti nalog od neovlaštenog pristupa.

- US-8, US-9, US-10 (Kreiranje tiketa)  
  Omogućava korisniku kreiranje tiketa sa opisom problema, izborom tipa i prioriteta. Sistem validira obavezna polja i osigurava ispravno spremanje tiketa u bazu.

- US-11, US-12, US-13 (Pregled vlastitih tiketa)  
  Korisnik može pregledati sve svoje tikete sa osnovnim informacijama (status, datum, prioritet) te koristiti filtriranje radi lakšeg snalaženja.

- US-14, US-15 (Detaljan prikaz tiketa)  
  Omogućava detaljan pregled pojedinačnog tiketa uključujući sve informacije i historiju komunikacije.

- US-56 (FAQ segment)  
  Pruža korisnicima listu najčešćih pitanja i odgovora radi samostalnog rješavanja problema bez potrebe za kreiranjem tiketa.

**Zavisnosti:**
- US-1, US-2, US-3 su osnova za sve ostale funkcionalnosti (auth sistem)
- US-8–US-10 zavise od uspješne autentifikacije (US-1)
- US-11–US-15 zavise od kreiranih tiketa (US-8–US-10)

**Glavni rizici:**
- Neispravna autentifikacija i sigurnosni propusti  
- Nedosljedno spremanje tiketa  
- Neadekvatno definisan FAQ sadržaj  

**Sprintovi:** Sprint 5 – Sprint 6  
**Release:** Na kraju Sprinta 6  

---

## Inkrement 2 – Upravljanje Tiketima i Komunikacija

**Cilj inkrementa:**  
Omogućiti kompletan životni ciklus tiketa kroz komunikaciju, izmjene statusa i osnovnu analitiku ponašanja sistema.

**Glavne funkcionalnosti:**
- US-16, US-17 (Zatvaranje tiketa)  
  Omogućava korisniku i agentu zatvaranje tiketa nakon rješenja problema uz kontrolisani workflow odobravanja i automatsko zatvaranje nakon određenog perioda.

- US-19, US-20 (Komunikacija kroz tiket)  
  Omogućava dvosmjernu komunikaciju između korisnika i agenata unutar tiketa uz ograničenja na broja poruka i kontrole validacije sadržaja.

- US-21, US-22 (Upravljanje prioritetima)  
  Omogućava definisanje korisničkog i internog prioriteta tiketa radi bolje organizacije rada.

- US-31, US-32 (Pretraga i filtriranje tiketa)  
  Omogućava pretragu i filtriranje tiketa po različitim kriterijima uz ograničenja pristupa.

- US-37, US-38 (Ažuriranje statusa tiketa)  
  Omogućava tehničarima promjenu statusa tiketa i praćenje historije promjena.

- US-39, US-40 (Tehničarski pregled informacija)  
  Omogućava tehničarima uvid u osnovne informacije i kontekst korisnika bez pristupa osjetljivim podacima.

- US-49, US-50, US-51 (Vrijeme prvog odgovora)  
  Prati i prikazuje vrijeme prvog odgovora na tiket za sve uloge u sistemu.

- US-18 (Ocjenjivanje tiketa)  
  Omogućava korisnicima ocjenjivanje rješenja nakon zatvaranja tiketa.

**Zavisnosti:**
- Zavisi od Inkrementa 1 (kreirani tiketi i autentifikacija)
- US-16 i US-17 zavise od US-8–US-10 (postojanje tiketa)
- US-19 i US-20 zavise od US-14–US-15 (detaljan prikaz tiketa)
- US-31 i US-32 zavise od US-11–US-13 (lista tiketa)
- US-18 zavisi od US-16–US-17 (zatvoren tiket)
- US-37–US-40 zavise od uloga sistema (tehničar/agent)

**Glavni rizici:**
- Kompleksna logika statusa tiketa  
- Neusklađenost komunikacije u realnom vremenu  
- Konflikti pri paralelnim izmjenama  

**Sprintovi:** Sprint 7 – Sprint 8  
**Release:** Na kraju Sprinta 8  

---

## Inkrement 3 – Upravljanje korisnicima i operativni alati

**Cilj inkrementa:**  
Omogućiti napredno upravljanje korisnicima, agentima i organizacijom rada u sistemu.

**Glavne funkcionalnosti:**
- US-4, US-5 (Upravljanje korisničkim profilom)  
  Omogućava izmjenu emaila i lozinke uz validaciju sigurnosnih pravila.

- US-6, US-7 (Paketi i pretplate)  
  Prikazuje korisnicima njihove aktivne pakete i detalje usluga.

- US-23, US-24 (Preraspodjela agenata)  
  Omogućava administratoru upravljanje rasporedom agenata po timovima.

- US-25, US-26 (Automatska dodjela tiketa)  
  Sistem automatski dodjeljuje tikete agentima prema definisanim pravilima.

- US-27, US-28 (Prosljeđivanje tiketa)  
  Omogućava agentima prosljeđivanje tiketa drugim agentima uz opcionalne interne komentare.

- US-29, US-30 (Pregled svih tiketa)  
  Omogućava agentima i administratorima pregled svih tiketa u sistemu.

- US-33, US-34 (Admin upravljanje korisnicima)  
  Omogućava pregled i uređivanje korisničkih profila uz sigurnosna ograničenja.

- US-35, US-36 (Pregled dodijeljenih tiketa)  
  Omogućava tehničarima pregled i filtriranje samo njihovih tiketa.

**Zavisnosti:**
- Zavisi od Inkrementa 1 i 2 (tiketi + korisnici + statusi)
- US-23 i US-24 zavise od US-29 i US-30 (postojanje tiketa i agenata)
- US-25 i US-26 zavise od US-8-US-10 (kreirani tiketi)
- US-27 i US-28 zavise od US-19 i US-20 (komunikacija)
- US-33 i US-34 zavise od US-1 (autentifikacija)
- US-35 i US-36 zavise od uloga sistema (tehničar)

**Glavni rizici:**
- Greške u automatskoj dodjeli  
- Sigurnosni problemi sa privilegijama  
- Nekonzistentno upravljanje agentima  

**Sprintovi:** Sprint 9 – Sprint 10  
**Release:** Na kraju Sprinta 10  

---

## Inkrement 4 – Izvještaji i analitika

**Cilj inkrementa:**  
Omogućiti analizu performansi sistema kroz dashboard sa različitim izvještajima i metrikama.

**Glavne funkcionalnosti:**
- US-41, US-42 (Broj tiketa po periodu)  
- US-43, US-44 (Statistika po statusu tiketa)  
- US-45, US-46 (Statistika po tipu problema)  
- US-47, US-48 (Prosječno vrijeme rješavanja tiketa)  

Svi izvještaji omogućavaju analizu rada sistema kroz različite vremenske periode i kategorije, pružajući uvid u efikasnost agenata i opterećenje sistema.

**Zavisnosti:**
- Zavisi od svih prethodnih inkremenata (historijski podaci tiketa)
- US-41–US-48 zavise od zatvorenih tiketa (US-16–US-17)
- US-47 i US-48 zavise od tačnih timestamp podataka (US-37, US-49–US-51)

**Glavni rizici:**
- Netačni ili nepotpuni podaci  
- Performanse pri velikoj količini podataka
- Kašnjenje u generisanju izvještaja  

**Sprintovi:** Sprint 11  
**Release:** Na kraju Sprinta 11  