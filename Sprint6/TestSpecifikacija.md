# Test Specifikacija - Sprint 6

Dokument baziran na: [Sprint3/TestStrategy.md](Sprint3/TestStrategy.md) + [Sprint6/UpdatedProductBacklog.md](Sprint6/UpdatedProductBacklog.md) + [Sprint6/SprintBacklog.md](Sprint6/SprintBacklog.md)

---

## PB-24 - Detaljan prikaz tiketa

### Cilj testiranja
Provjera da korisnik moze otvoriti tiket iz liste i vidjeti sve detalje i kompletnu historiju komunikacije.

### Obim
Prikaz detalja tiketa (opis, status, datum), prikaz historije poruka, hronoloski redoslijed, prikaz posiljaoca i vremena, bez izostavljenih poruka.

### Kriterij uspjeha
Svi AC iz US-14 i US-15 su zadovoljeni; korisnik dobija kompletan i konzistentan prikaz tiketa i komunikacije.

### Nivoi testiranja

| Nivo | Sta se provjerava |
| --- | --- |
| Unit | Mapiranje podataka tiketa i poruka, sortiranje historije, formatiranje datuma |
| Integracijsko | API dohvat detalja tiketa i historije; validan odgovor za postojece i nepostojece tikete |
| Sistemsko | Tok: korisnik ulazi u listu, otvara tiket i vidi sve detalje i historiju |
| UI | Prikaz detalja, lista poruka, prazno stanje kada nema poruka |
| Sigurnosno | Korisnik ne moze vidjeti tudje tikete; agent/admin ima dozvoljen pristup |
| Performansno | Ucitavanje detalja u prihvatljivom vremenu u ciljnom opterecenju |
| Prihvatno | PO potvrda da prikaz zadovoljava AC |

### Veza sa AC

| Referenca | Kljucni AC | Dokaz ispunjenja |
| --- | --- | --- |
| US-14, US-15 | Prikaz svih informacija i historije komunikacije bez gubitka podataka | API zapis, screenshot UI prikaza, demo toka |

### Evidentirani rezultati

| Datum | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- |
| DD-MM-YYYY | TBD | TBD | TBD |

---

## PB-27 - Komunikacija kroz tiket

### Cilj testiranja
Provjera slanja i prikaza poruka korisnik-agent uz validacije i ogranicenja iz AC.

### Obim
Slanje poruka korisnika i agenta, zabrana praznih poruka, limit od 3 poruke po ciklusu, limit 1000 karaktera, prikaz nove poruke u historiji.

### Kriterij uspjeha
Svi AC iz US-19 i US-20 su zadovoljeni; komunikacija radi bez gresaka i poruke se vide na obje strane.

### Nivoi testiranja

| Nivo | Sta se provjerava |
| --- | --- |
| Unit | Validacija poruka (prazno, duzina), limit poruka po ciklusu |
| Integracijsko | API/WebSocket tok poruka, upis i dohvat historije |
| Sistemsko | End-to-end tok komunikacije korisnik-agent kroz tiket |
| UI | Input forma, poruke greske, prikaz nove poruke u historiji |
| Sigurnosno | Pristup komunikaciji samo na tiketima kojima korisnik pripada |
| Performansno | Kasnjenje isporuke poruke u definisanim granicama |
| Prihvatno | PO potvrda da tok komunikacije zadovoljava AC |

### Veza sa AC

| Referenca | Kljucni AC | Dokaz ispunjenja |
| --- | --- | --- |
| US-19, US-20 | Poruke se spremaju i prikazuju; nema praznih poruka; postovana ogranicenja | API log, UI provjere, demo komunikacije |

### Evidentirani rezultati

| Datum | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- |
| DD-MM-YYYY | TBD | TBD | TBD |

---

## PB-32 - Pregled svih tiketa

### Cilj testiranja
Provjera da agent vidi sve tikete i moze otvoriti detalje bez ogranicenja pristupa.

### Obim
Lista svih tiketa, ucitavanje dodatnih rezultata, prikaz svih stanja, pristup adminu bez ogranicenja.

### Kriterij uspjeha
Svi AC iz US-29 i US-30 su zadovoljeni; lista i detalji rade bez greske.

### Nivoi testiranja

| Nivo | Sta se provjerava |
| --- | --- |
| Unit | Logika paginacije i mapiranja statusa |
| Integracijsko | API lista tiketa, paginacija/ucitavanje dodatnih rezultata |
| Sistemsko | Tok: agent ulazi u listu, ucitava dodatne tikete, otvara detalje |
| UI | Prikaz liste, loader, prazno stanje, prikaz detalja |
| Sigurnosno | Pristup listi za agent/admin; blokada neovlastenog pristupa |
| Performansno | Brzina ucitavanja liste pri vecem broju tiketa |
| Prihvatno | PO potvrda da pregled zadovoljava AC |

### Veza sa AC

| Referenca | Kljucni AC | Dokaz ispunjenja |
| --- | --- | --- |
| US-29, US-30 | Lista prikazuje sve tikete i detalje bez ogranicenja | API log, UI screenshot, demo toka |

### Evidentirani rezultati

| Datum | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- |
| DD-MM-YYYY | TBD | TBD | TBD |

---

## PB-47 - FAQ segment

### Cilj testiranja
Provjera da FAQ segment prikazuje tacna pitanja i odgovore iz baze i daje jasnu poruku kada sadrzaj ne postoji.

### Obim
Dohvat FAQ sadrzaja iz baze/CMS, prikaz liste, fallback poruka kada nema sadrzaja, dostupnost svim ulogama, brzina ucitavanja.

### Kriterij uspjeha
Svi AC iz US-56 su zadovoljeni; FAQ prikaz je tacan i jasan.

### Nivoi testiranja

| Nivo | Sta se provjerava |
| --- | --- |
| Integracijsko | API dohvat FAQ liste, validan odgovor sa aktivnim FAQ stavkama, prazna lista kada nema aktivnog sadrzaja |
| Sistemsko | Korisnik otvara FAQ stranicu i vidi pitanja/odgovore bez greske |
| UI | Prikaz FAQ kartica, kategorija i fallback poruke kada nema sadrzaja |
| Performansno | Ucitavanje FAQ liste u test okruzenju ispod 2 sekunde |
| Prihvatno | Potvrda da FAQ daje korisniku dovoljno informacija da pokusa rijesiti problem bez otvaranja tiketa |

Napomena: Unit i Sigurnosno testiranje se ne provode za FAQ zbog nedostatka izolovane poslovne logike i javne dostupnosti sadrzaja.

### Veza sa AC

| Referenca | Kljucni AC | Dokaz ispunjenja |
| --- | --- | --- |
| US-56 | Prikaz FAQ pitanja/odgovora i poruka kada nema sadrzaja | API zapis, UI provjera, demo korisniku |

### Dokazi

| Tip dokaza | Zapis |
| --- | --- |
| API log / zahtjev-odgovor | xUnit `FaqIntegrationTests`: `GET api/Faq` kroz controller/service/repository vraca `200 OK` sa 2 aktivne FAQ stavke; scenario bez aktivnog sadrzaja vraca `200 OK` i praznu listu `[]` |
| UI prikaz FAQ liste | Vitest `FaqSystem.test.jsx` i `FaqUi.test.jsx`: korisnik vidi FAQ pitanja, kategorije i moze otvoriti odgovor |
| UI fallback prikaz | Vitest `FaqUi.test.jsx`: kada API vrati praznu listu prikazuje se "Nema FAQ pitanja" i opis fallback stanja |
| Mjerenje vremena ucitavanja | xUnit `FaqPerformanceTests`: 100 FAQ stavki ucitano kroz API slojeve ispod praga od 2 sekunde u lokalnom test okruzenju |
| Prihvatna biljeska | Vitest `FaqAcceptance.test.jsx`: FAQ odgovor daje korisniku konkretne korake za cest internet problem prije otvaranja tiketa |

### Evidentirani rezultati

| Datum | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- |
| 05-05-2026 | Integracijsko / Regression | xUnit: `FaqIntegrationTests.GetFaqs_ShouldReturnValidApiResponse_WithActiveFaqList`, `FaqIntegrationTests.GetFaqs_ShouldReturnEmptyList_WhenNoActiveFaqContentExists`; API dokaz: `200 OK` sa FAQ listom i `200 OK` sa praznom listom | PASS |
| 05-05-2026 | Sistemsko / Regression | Vitest + Testing Library: `FaqSystem.test.jsx`; korisnik otvara FAQ stranicu, vidi pitanja i otvara odgovor bez `console.error` greske | PASS |
| 05-05-2026 | UI / Regression | Vitest + Testing Library: `FaqUi.test.jsx`, postojece `Faq.test.jsx`; provjeren prikaz FAQ kartica/kategorija i fallback poruka kada nema sadrzaja | PASS |
| 05-05-2026 | Performansno | xUnit + `Stopwatch`: `FaqPerformanceTests.GetFaqs_ShouldLoadFaqListWithinTwoSeconds_InTestEnvironment`; prag: < 2s za 100 FAQ stavki u lokalnom test okruzenju | PASS |
| 05-05-2026 | Prihvatno / UAT smoke | Vitest + Testing Library: `FaqAcceptance.test.jsx`; biljeska: FAQ sadrzi korake za čest problem i pomaže korisniku prije otvaranja tiketa | PASS |

