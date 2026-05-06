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

| Datum | Nivo | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- | --- |
| 07-05-2026 | Unit | Regression | xUnit + Moq: `TicketDetailServiceTests` — 5 testova; provjera: klijent vidi vlastiti tiket, KeyNotFound za nepostojeci tiket, UnauthorizedAccess za tudji tiket, agent vidi svaki tiket, ispravno mapiranje svih DTO polja (ClientName, AssignedAgentName) | PASS |
| 07-05-2026 | Unit | Regression | xUnit + Moq: `TicketDetailControllerTests` — 4 testa; provjera: 200 OK za vlasnika, 404 NotFound, 403 Forbid za klijenta koji pristupa tuđem tiketu, 401 Unauthorized bez JWT claimova | PASS |
| 07-05-2026 | UI | Regression | Vitest + Testing Library: `TicketDetail.test.jsx` — 8 testova; provjera: prikaz detalja tiketa (naslov, opis), ime klijenta i agenta, historija komentara, input forma za otvoreni tiket, skriven input za zatvoreni tiket, error empty state pri API grešci | PASS |
| 07-05-2026 | Integracijsko | Regression | xUnit + EF InMemory: `TicketDetailIntegrationTests` — 4 testa; provjera: CLIENT owner → 200 sa svim ispravno mapiranim poljima (Title, Description, Status, ClientName); AGENT → 200 na svakom tiketu; CLIENT na tuđem tiketu → 403 Forbid; nepostojeci tiket → 404 NotFound | PASS |
| 07-05-2026 | Performansno | Stress | xUnit + Stopwatch: `TicketDetailPerformanceTests` — 1 test; provjera: prikaz detalja tiketa (s Creator i Assignments include) < 2s u lokalnom test okruženju | PASS |
| 07-05-2026 | Sistemsko | Regression | Vitest + Testing Library: `TicketDetailSystem.test.jsx` — 1 test; provjera: korisnik otvara tiket i vidi naslov, opis i historiju komentara bez console.error greske | PASS |
| 07-05-2026 | Prihvatno | UAT smoke | Vitest + Testing Library: `TicketDetailAcceptance.test.jsx` — 1 test; provjera: klijent vidi naslov, opis, ime agenta i historiju komunikacije; nema poruke o gresci | PASS |

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

| Datum | Nivo | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- | --- |
| 07-05-2026 | Unit | Regression | xUnit + Moq: `CommentServiceTests` — 6 testova; provjera: uspjesno slanje komentara, ArgumentException za sadrzaj >1000 znakova, KeyNotFound za nepostojeci tiket, UnauthorizedAccess za klijenta koji nije vlasnik tiketa, GetComments za vlasnika vraca listu, GetComments za tredju stranu baca UnauthorizedAccess | PASS |
| 07-05-2026 | Unit | Regression | xUnit + Moq + SignalR stub: `CommentControllerTests` — 4 testa; provjera: 200 OK sa listom komentara za autoriziranog korisnika, 401 Unauthorized bez JWT claimova, 400 BadRequest za prazan sadrzaj, 401 Unauthorized za POST bez claimova | PASS |
| 07-05-2026 | UI | Regression | Vitest + Testing Library: `TicketDetail.test.jsx` — 8 testova (pokriva i PB-24 i PB-27); provjera: input polje vidljivo za otvoreni tiket, skriveno za zatvoreni tiket, dugme Pošalji onemoguceno za prazan unos, slanje poruke poziva addComment sa ispravnim parametrima | PASS |
| 07-05-2026 | Integracijsko | Regression | xUnit + EF InMemory + SignalR stub: `CommentIntegrationTests` — 4 testa; provjera: vlasnik dohvata komentare → lista s autorom; slanje komentara persistira u bazi i vraca DTO; sadrzaj >1000 znakova → 400 BadRequest; klijent na tudem tiketu → 403 Forbid | PASS |
| 07-05-2026 | Performansno | Stress | xUnit + Stopwatch: `CommentPerformanceTests` — 1 test; provjera: lista od 100 komentara < 2s u lokalnom test okruženju | PASS |
| 07-05-2026 | Sistemsko | Regression | Vitest + Testing Library: `CommunicationSystem.test.jsx` — 1 test; provjera: korisnik unosi poruku i šalje je — addComment pozvan s ispravnim ticketId i sadrzajem; nema console.error greske | PASS |
| 07-05-2026 | Prihvatno | UAT smoke | Vitest + Testing Library: `CommunicationAcceptance.test.jsx` — 2 testa; provjera: klijent šalje poruku na otvorenom tiketu (addComment pozvan); prazan unos blokira slanje; input skriven za zatvoreni tiket | PASS |

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

| Datum | Nivo | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- | --- |
| 07-05-2026 | Unit — Repozitorij | Regression | xUnit + EF InMemory: `AllTicketsRepositoryTests` — 4 testa; provjera: GetAllAsync vraca sve tikete, prazna lista, sortiranje od najnovijeg, GetByAssigneeIdAsync filtrira po dodijeljenosti | PASS |
| 07-05-2026 | Unit | Regression | xUnit + Moq: `AllTicketsServiceTests` — 4 testa; provjera: agent bez filtera poziva GetAllAsync, agent sa assignedOnly=true poziva GetByAssigneeIdAsync, tehnicar uvijek poziva GetByAssigneeIdAsync, klijent dobija UnauthorizedAccessException | PASS |
| 07-05-2026 | Unit | Regression | xUnit + Moq: `AllTicketsControllerTests` — 3 testa; provjera: agent dobija 200 OK sa listom tiketa, klijent dobija 403 Forbid, zahtjev bez JWT claimova dobija 401 Unauthorized | PASS |
| 07-05-2026 | UI | Regression | Vitest + Testing Library: `Tickets.test.jsx` — 9 testova (PB-32 i PB-33); provjera: lista tiketa po uspjesnom dohvatu, OPEN→"Otvoren"/CLOSED→"Zatvoren", filter po statusu/tipu/prioritetu, pretraga po naslovu, prazno stanje, greska pri API pozivu, agent vidi toggle Svi tiketi/Dodijeljeni meni | PASS |
| 07-05-2026 | Sistemsko | Regression | Vitest + Testing Library: `TicketsSystem.test.jsx` — 1 test; provjera: agent otvara listu svih tiketa, vidi tikete od razlicitih klijenata i moze pretraziti listu, nema console.error gresaka | PASS |
| 07-05-2026 | Prihvatno | UAT smoke | Vitest + Testing Library: `TicketsAcceptance.test.jsx` — 1 test; provjera: agent vidi tikete iz svih kategorija (BILLING, MOBILE_NETWORK), vidi toggle Svi tiketi/Dodijeljeni meni, nema poruke o gresci | PASS |
| 07-05-2026 | Integracijsko | Regression | xUnit + EF InMemory: `AllTicketsIntegrationTests` — 4 testa; provjera: agent vidi sve tikete od razlicitih klijenata (3); CLIENT → 403 Forbid; agent sa assignedOnly=true → samo 1 dodijeljeni tiket; tiketi sortirani od najnovijeg prema najstarijem | PASS |
| 07-05-2026 | Performansno | Stress | xUnit + Stopwatch: `AllTicketsPerformanceTests` — 1 test; provjera: lista od 500 tiketa < 2s u lokalnom test okruženju | PASS |

---

## PB-33 - Pretraga i filtriranje tiketa

### Cilj testiranja
Provjera da agent moze pretraziti i filtrirati listu tiketa po statusu, tipu problema i prioritetu, te da kombinacija filtera daje ispravne rezultate.

### Obim
Filter po statusu, tipu problema i prioritetu, pretraga po naslovu, kombinacija filtera, prazno stanje kada nema podudaranja.

### Kriterij uspjeha
Svi AC iz US-31 i US-32 su zadovoljeni; filteri i pretraga daju tacne rezultate, a lista se sortira od najnovijeg prema najstarijem.

### Nivoi testiranja

| Nivo | Sta se provjerava |
| --- | --- |
| Integracijsko | Sortiranje tiketa od najnovijeg prema najstarijem; assignedOnly filter kroz sve slojeve |
| Sistemsko | Pretraga u sistemskom toku filtrira ispravno |
| UI | Filteri po statusu, tipu, prioritetu i pretraga po naslovu |
| Performansno | Ucitavanje 500 tiketa (s filtriranjem) u prihvatljivom vremenu |
| Prihvatno | PO potvrda da pretraga i filtriranje zadovoljavaju AC |

Napomena: Unit testiranje filtera pokriveno je kroz unit testove za PB-32 (`AllTicketsRepositoryTests`, `AllTicketsServiceTests`). Sigurnosno testiranje (rola/vlasnistvo) je pokriveno u PB-32.

### Veza sa AC

| Referenca | Kljucni AC | Dokaz ispunjenja |
| --- | --- | --- |
| US-31, US-32 | Tiketi sortirani od najnovijeg; filteri i pretraga daju tacne rezultate | UI provjera filtera, integracijsko sortiranje, performansno mjerenje |

### Evidentirani rezultati

| Datum | Nivo | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- | --- |
| 07-05-2026 | UI | Regression | Vitest + Testing Library: `Tickets.test.jsx` — filteri pokriveni unutar 9 testova za PB-32 i PB-33; provjera: filter po statusu, tipu problema, prioritetu i pretraga po naslovu prikazuju samo odgovarajuce tikete | PASS |
| 07-05-2026 | Sistemsko | Regression | Vitest + Testing Library: `TicketsSystem.test.jsx` — pretraga pokrivena u sistemskom toku; provjera: agent pretrazuje tikete i lista se filtrira ispravno | PASS |
| 07-05-2026 | Integracijsko | Regression | xUnit + EF InMemory: `AllTicketsIntegrationTests.GetAllTickets_ReturnsTicketsOrderedByDateDescending` — 1 test (dio od 4 u `AllTicketsIntegrationTests`); provjera: tiketi sortirani od najnovijeg prema najstarijem integracijski potvrđeno | PASS |
| 07-05-2026 | Performansno | Stress | xUnit + Stopwatch: `AllTicketsPerformanceTests` — 1 test (dijeljeno s PB-32); provjera: lista od 500 tiketa (ukljucujuci sortiranje) < 2s u lokalnom test okruženju | PASS |
| 07-05-2026 | Prihvatno | UAT smoke | Vitest + Testing Library: `SearchFilterAcceptance.test.jsx` — 1 test; provjera: agent suzuje listu kombinacijom pretrage i filtera po statusu; reset filtera vraca sve tikete | PASS |

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

| Datum | Nivo | Metoda | Alati i testovi | Rezultat |
| --- | --- | --- | --- | --- |
| 05-05-2026 | Integracijsko | Regression | xUnit: `FaqIntegrationTests.GetFaqs_ShouldReturnValidApiResponse_WithActiveFaqList`, `FaqIntegrationTests.GetFaqs_ShouldReturnEmptyList_WhenNoActiveFaqContentExists`; API dokaz: `200 OK` sa FAQ listom i `200 OK` sa praznom listom | PASS |
| 05-05-2026 | Sistemsko | Regression | Vitest + Testing Library: `FaqSystem.test.jsx`; korisnik otvara FAQ stranicu, vidi pitanja i otvara odgovor bez `console.error` greske | PASS |
| 05-05-2026 | UI | Regression | Vitest + Testing Library: `FaqUi.test.jsx`, postojece `Faq.test.jsx`; provjeren prikaz FAQ kartica/kategorija i fallback poruka kada nema sadrzaja | PASS |
| 05-05-2026 | Performansno | Stress | xUnit + `Stopwatch`: `FaqPerformanceTests.GetFaqs_ShouldLoadFaqListWithinTwoSeconds_InTestEnvironment`; prag: < 2s za 100 FAQ stavki u lokalnom test okruzenju | PASS |
| 05-05-2026 | Prihvatno | UAT smoke | Vitest + Testing Library: `FaqAcceptance.test.jsx`; biljeska: FAQ sadrzi korake za čest problem i pomaže korisniku prije otvaranja tiketa | PASS |

