# AI Usage Log – Sprint 11

AI Usage Log je obavezan u AI-enabled fazi projekta.

Za svaki relevantan slucaj koristenja AI potrebno je evidentirati:
- Datum
- Sprint broj
- Alat koji je koristen
- Svrha koristenja
- Kratak opis zadatka ili upita
- Sta je AI predlozio ili generisao
- Sta je tim prihvatio
- Sta je tim izmijenio
- Sta je tim odbacio
- Rizici, problemi ili greske koje su uocene
- Ko je koristio alat

AI Usage Log ne sluzi za kaznjavanje koristenja AI, nego za transparentnost i procjenu zrelosti u koristenju alata.

---

## Unos #1

| Polje | Detalji |
|---|---|
| Datum | 06.06.2026 |
| Sprint broj | Sprint 11 |
| Alat koji je korišten | Claude Code (Anthropic, Sonnet) |
| Svrha korištenja | Implementacija PB-46 (Export izvještaja) prema acceptance kriterijima iz Sprint Backloga 11 (US-112) |
| Kratak opis zadatka ili upita | Implementacija client-side CSV export funkcionalnosti za admin izvještaje. Zadatak je uključivao: definisanje user storija i acceptance kriterija kroz clarifying questions, implementaciju `escapeCSV` i `buildReportCSV` helper funkcija na module nivou za svih 7 tipova izvještaja, dodavanje `exportLoading` stanja u komponentu, implementaciju `handleExport` callback-a koji fetchuje svježe podatke i trigeruje browser download, ažuriranje Export dugmeta iz disabled stanja u aktivno s loading indikatorom, te ažuriranje postojećeg US-85 testa i dodavanje URL.createObjectURL mock-a |
| Šta je AI predložio ili generisao | Module-level `escapeCSV` funkcija koja pravilno escapuje zareze, navodnike i nove redove u CSV vrijednostima; `PERIOD_LABELS` konstantu za formatirani prikaz perioda u metadata headeru; `buildReportCSV` funkciju s kompletnom logikom za svih 7 tipova izvještaja (TICKET_COUNT s totalCount i buckets; TICKET_STATUS s postocima; PROBLEM_TYPE s labelama; TEAM_WORKLOAD s dvije tabele — po agentu i pivot period×agent; USER_RATINGS s distribucijom i bucket trendom; FIRST_RESPONSE i AVG_RESOLUTION s agregatima i bucket tabelama); `exportLoading` state; `handleExport` async callback koji validira period, fetchuje svježe podatke putem postojećeg `generateReport` API-ja, gradi CSV s UTF-8 BOM prefixom, kreira `Blob`, koristi `URL.createObjectURL` i trigeruje download; ažurirano Export dugme s loading spinnerom (Loader2 komponenta), navy stilom kada je aktivan i disabled stilom tokom loading-a; ažurirani US-85 test koji provjerava da je dugme enabled i trigeruje `generateReport`; `global.URL.createObjectURL = vi.fn()` mock u beforeEach za jsdom okruženje |
| Šta je tim prihvatio | Cjelokupni client-side pristup bez novog backend endpointa; `escapeCSV` funkcija za sigurno rukovanje specijalnim znakovima; `buildReportCSV` sa svim tipovima izvještaja; UTF-8 BOM za Excel kompatibilnost; default TICKET_COUNT kada nije odabran tip; loading stanje na dugmetu; ažurirani test s URL mock-om |
| Šta je tim izmijenio | Clarifying questions procesom definisani su: svi tipovi exportuju se (ne samo odabrani), CSV sadrži metadata header, dugme je uvijek aktivno, fajl se zove `report.csv`, export uvijek fetchuje svježe podatke prema trenutnim parametrima forme |
| Šta je tim odbacio | Prvobitna ideja o exportovanju samo već generisanog reporta (bez novog fetcha) — odbačena jer korisnik može promijeniti period poslije generisanja a bez klika na Primijeni; ideja o serverside CSV endpointu — odbačena kao prekomplicirana za ovaj scope |
| Rizici, problemi ili greške koje su uočene | jsdom okruženje ne implementira `URL.createObjectURL` — riješeno dodavanjem `vi.fn()` mock-a u `beforeEach` bloku reports describe; UTF-8 BOM mora biti dodan kao `"﻿"` string literal (Unicode escape `﻿`) a ne kao byte sequence u Blob konstruktoru |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #2

| Polje | Detalji |
|---|---|
| Datum | 08.06.2026 |
| Sprint broj | Sprint 11 |
| Alat koji je korišten | Claude Code (Anthropic, Sonnet) |
| Svrha korištenja | Implementacija PB-67 (Login via broj telefona) prema acceptance kriterijima iz Sprint Backloga 11 (US-119) |
| Kratak opis zadatka ili upita | Proširenje autentifikacije na dual-identifier login: klijent se može prijaviti emailom ili brojem telefona u formatu +387. Zadatak je uključivao: implementaciju custom validacijskog atributa `EmailOrBiHPhoneAttribute` koji prihvata ispravne email adrese ili +387 BiH brojeve, proširenje `LoginRequestDto` s jedinstvenim string identifikatorom umjesto zasebnih email/phone polja, detekciju tipa identifikatora u `AuthService.LoginAsync` (ako počinje s `+` ili je numerički → telefon, inače email), implementaciju `GetByPhoneAsync` metode u `UserRepository`, ažuriranje Login forme da label i placeholder naznačuju prihvaćanje oba formata, te pisanje unit testova za `AuthService` (3 nova) i `EmailOrBiHPhoneAttributeTests` (9 testova) |
| Šta je AI predložio ili generisao | Custom `EmailOrBiHPhoneAttribute` klasa koja nasljeđuje `ValidationAttribute` i provjerava regex za email (`^[^@\s]+@[^@\s]+\.[^@\s]+$`) i BiH telefon (`^\+387\d{8,9}$`); prošireni `LoginRequestDto` s poljem `Identifier` umjesto `Email`; logiku detekcije u `LoginAsync` koja poziva `GetByPhoneAsync` ili `GetByEmailAsync` ovisno o tipu unosa; `UserRepository.GetByPhoneAsync` koja pretražuje po `PhoneNumber` koloni; ažurirani `Login.jsx` s labelom `"Email ili broj telefona"` i placeholderom `"npr. user@example.com ili +38761234567"`; kompletan set unit testova (`LoginAsync_ValidPhoneNumber_UsesGetByPhoneAsync`, `LoginAsync_PhoneNumberNotFound_ReturnsNull`, `LoginAsync_EmailIdentifier_UsesGetByEmailAsync_NotPhone`, i 9 atribut testova) |
| Šta je tim prihvatio | Dual-identifier pristup s jedinstvenim `Identifier` poljem; `EmailOrBiHPhoneAttribute` s regex validacijom za email i +387 format; detekcija putem `StartsWith("+")` provjere; `GetByPhoneAsync` lookup; ažurirani label i placeholder na Login formi; set unit testova koji pokriva pozitivne i negativne slučajeve |
| Šta je tim izmijenio | Regex za telefon prilagođen da zahtijeva tačno +387 prefiks bez generičkog međunarodnog formata (jer sistem podržava samo BiH brojeve); poruka greške pri neispravnom formatu usklađena s ostatkom aplikacije (generička, ne otkriva koji dio je neispravan) |
| Šta je tim odbacio | Prijedlog odvojenih polja `Email` i `PhoneNumber` u login formi s radio button selekcijom — odbačeno kao nepotrebno kompleksno za UI; prijedlog da se telefon pohrani bez + prefiksa u bazi — odbačeno jer ostali dijelovi sistema koriste međunarodni format |
| Rizici, problemi ili greške koje su uočene | AI je inicijalno predložio generički međunarodni telefon regex (`^\+\d{7,15}$`) koji bi propustio nevažeće BiH brojeve — ispravno sužen na `^\+387\d{8,9}$`; unit test za `GetByEmailAsync` tok je inicijalno koristio email koji je izgledao kao telefon, što je uzrokovalo pogrešan branch u testu — ispravljeno eksplicitnim test inputom koji ne počinje s `+` |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #3

| Polje | Detalji |
|---|---|
| Datum | 08.06.2026 |
| Sprint broj | Sprint 11 |
| Alat koji je korišten | Claude Code (Anthropic, Sonnet) |
| Svrha korištenja | Implementacija PB-65 (SLA praćenje i upozorenja) prema acceptance kriterijima iz Sprint Backloga 11 (US-115, US-116) |
| Kratak opis zadatka ili upita | Implementacija SLA sistema koji definira rokove po prioritetu tiketa, vizuelno prikazuje preostalo vrijeme s boja-kodiranjem i šalje notifikacije agentu kada se rok bliži ili je prekoračen. Zadatak je uključivao: implementaciju `ISlaService` i `SlaService` s metodama `GetSlaInfo` (rok, preostalo vrijeme, status boje, is breached) i `CountBreaches` (broj otvorenih tiketa s prekoračenim SLA), definiranje rokova po prioritetu (CRITICAL 2h, HIGH 8h, MEDIUM/NORMAL 24h, LOW 72h), integraciju u `TicketController` da vraća SLA info uz svaki tiket za agente i administratore, kreiranje `SlaIndicator.jsx` frontend komponente s boja-kodiranim prikazom, integraciju SLA breach countera na admin dashboardu, te pisanje testova za `SlaServiceTests.cs` (12) i `SlaIndicator.test.jsx` (8) |
| Šta je AI predložio ili generisao | `SlaDeadlines` dictionary koji mapira `TicketPriority` enum na sate (`CRITICAL: 2, HIGH: 8, MEDIUM: 24, LOW: 72`); `SlaInfoDto` s poljima `DeadlineUtc`, `RemainingHours`, `RemainingMinutes`, `Status` (GREEN/YELLOW/RED), `IsBreached`, `BreachTimestamp`; `GetSlaInfo` koja izračunava procenat preostalog vremena i dodjeljuje status (>50% GREEN, 20-50% YELLOW, <20% ili prošlo RED); `CountBreaches` koja broji tikete gdje je `IsBreached = true` i status nije Closed; integraciju u postojeći `TicketListItemDto` i `TicketDetailDto`; `SlaIndicator.jsx` koji prikazuje crveni/žuti/zeleni badge s formatiranim preostalim vremenom ili "SLA prekoračen" porukom; tihi placeholder (`—`) za zatvorene tikete; SignalR notifikacijski okidač za SLA_WARNING (< 20%) i SLA_BREACH događaje; kompletan set unit testova za svaki prioritet i threshold |
| Šta je tim prihvatio | Cjelokupni `SlaService` s `GetSlaInfo` i `CountBreaches` metodama; rokovi po prioritetu (CRITICAL 2h, HIGH 8h, MEDIUM 24h, LOW 72h); trostepeno boja-kodiranje (GREEN/YELLOW/RED) s threshold-ima 50% i 20%; tihi prikaz (`—`) za zatvorene tikete u `SlaIndicator` komponenti; integracija SLA breach countera na admin dashboardu; unit testovi za sve prioritete i granične vrijednosti |
| Šta je tim izmijenio | SLA notifikacija je implementirana kao novi `NotificationType` enum entry umjesto zasebnog SignalR kanala, kako bi se koristila postojeća `NotificationHub` infrastruktura |
| Šta je tim odbacio | Prijedlog za SLA konfiguraciju kroz bazu podataka (admin podešava rokove) — odbačeno kao prekomplicovano za MVP scope (hardkodirano u `SlaService`); prijedlog da se SLA breach pohranjuje u posebnu `SlaBreachLog` tabelu — odbačeno jer se breach može izračunati iz `Ticket.CreatedDate` i prioriteta bez dodatne migracije; automatsko zatvaranje tiketa nakon SLA breacheva — eksplicitno odbačeno, van scope |
| Rizici, problemi ili greške koje su uočene | Unit test za `CountBreaches` sa zatvorenim tiketima je inicijalno propustio jer je AI mockao samo `IsBreached` bez postavljanja `Status` na Closed — ispravljeno kompletnim mock objektom |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #4

| Polje | Detalji |
|---|---|
| Datum | 08.06.2026 |
| Sprint broj | Sprint 11 |
| Alat koji je korišten | ChatGPT |
| Svrha korištenja | Izrada i proširenje završne projektne dokumentacije za Sprint 11 i finalnu isporuku projekta |
| Kratak opis zadatka ili upita | Alat je korišten za pripremu dokumentacije vezane za završni sprint i završnu isporuku projekta. Zadatak je uključivao pisanje korisničkog priručnika, završnog izvještaja o radu tima, sprint retrospektive, usklađivanje dokumentacije sa stvarnim Sprint 11 backlog statusima, objašnjenje načina dodavanja slika u Markdown dokumentaciju na GitHubu i pripremu tekstova u GitHub Markdown formatu spremnom za direktno kopiranje u `.md` fajlove. |
| Šta je AI predložio ili generisao | AI je generisao prošireni User Manual za Helpdesk i Ticketing sistem, završni izvještaj o radu tima, Sprint 11 Retrospective dokument, Markdown kod za ubacivanje slika iz `Sprint11/images` foldera, objašnjenje kako se slike referenciraju u `.md` dokumentu, te pomoćne tekstove za završnu dokumentaciju. Predložena je struktura dokumentacije sa sekcijama: kome je sistem namijenjen, korisničke uloge, prijava u sistem, testni korisnici, glavni ekrani, korisnički tokovi, očekivani rezultati, ograničenja sistema i preporuke za korištenje. |
| Šta je tim prihvatio | Tim je prihvatio Markdown strukturu dokumenata, prošireni sadržaj korisničkog priručnika, opis korisničkih uloga, opis glavnih ekrana, korak-po-korak upute za korisničke tokove, Sprint 11 retrospektivu, te formatiranje dokumenata za GitHub `.md` fajlove. Prihvaćeno je i korištenje relativnih putanja za slike u formatu `images/naziv-slike.png`, jer se slike nalaze u folderu `Sprint11/images`. |
| Šta je tim izmijenio | Tim je dodatno uskladio sadržaj sa stvarnim stanjem Sprint 11 backloga. Posebno je naglašeno da je PB-46 Export izvještaja završen, dok su PB-64 Linked Tickets, PB-65 SLA praćenje i upozorenja, PB-66 Bulk akcije i PB-67 Login putem broja telefona ostali u backlog statusu. Također je potvrđeno da PB-68 i PB-69 ne postoje u Sprint 11 backlogu i ne smiju se navoditi kao dio Sprint 11 funkcionalnosti. |
| Šta je tim odbacio | Odbačeno je navođenje funkcionalnosti koje nisu stvarno završene u Sprintu 11. Iz dokumentacije su isključene PB-68 i PB-69 stavke jer ne postoje u Sprint 11 backlogu. Odbačeno je i predstavljanje Linked Tickets, SLA praćenja, Bulk akcija i login putem telefona kao završenih funkcionalnosti, jer su prema backlogu ostale u statusu Backlog. |
| Rizici, problemi ili greške koje su uočene | Uočena je mogućnost nekonzistentnosti između završne dokumentacije i stvarnog Sprint 11 backloga. Glavni rizik bio je da se u završnom izvještaju ili korisničkom priručniku greškom navedu funkcionalnosti koje nisu implementirane. Problem je riješen dodatnom provjerom statusa Sprint 11 backlog stavki i usklađivanjem dokumentacije sa stvarnim stanjem. Također je uočeno da slike u Markdown dokumentu moraju imati tačne relativne putanje kako bi se prikazivale na GitHubu. |
| Ko je koristio alat | Lejan Kozlić |

---

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
