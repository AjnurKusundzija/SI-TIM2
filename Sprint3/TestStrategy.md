# Test Strategy

Telecom Customer Support System

## Pregled sekcija

| Sekcija | Šta se popunjava |
| --- | --- |
| Cilj testiranja | Cilj, obim i kriterij uspjeha testiranja |
| Nivoi testiranja | Unit, integracijsko, sistemsko i prihvatno testiranje |
| Šta se testira u kojem nivou | Mapiranje funkcionalnosti na nivoe testiranja |
| Veza sa acceptance kriterijima | Povezivanje testnih slučajeva sa korisničkom pričom/AC |
| Način evidentiranja rezultata testiranja | Evidencija rezultata i defekata |
| Glavni rizici kvaliteta | Rizici, procjena i plan ublažavanja |

---

## Cilj testiranja

| Cilj | Obim | Kriterij uspjeha |
| --- | --- | --- |
| Verifikacija tačnosti autentifikacije i upravljanja sesijama (PB-19) | Login/logout tok za sve korisničke uloge (Klijent, Agent, Tehničar, Administrator) | Svi AC iz [US-1](../Sprint2/UserStories.md#pb-19-login-korisnika), [US-2](../Sprint2/UserStories.md#pb-19-login-korisnika) i [US-3](../Sprint2/UserStories.md#pb-19-login-korisnika) zadovoljeni; neautorizovan pristup zaštićenim rutama je onemogućen |
| Validacija centralnog tiketing toka od kreiranja do zatvaranja tiketa (PB-22, PB-25, PB-36) | Kreiranje tiketa, promjena statusa, zatvaranje uz korisničku potvrdu i automatsko zatvaranje nakon 7 dana | Tiket se kreira u <3 sekunde i zadovoljanava (NFR-04); svi definisani statusi rade ispravno i konzistentno |
| Provjera real-time ažuriranja statusa tiketa putem WebSocket komunikacije (NFR-02) | WebSocket kanal između klijenta i servera; prikaz promjene statusa u za manje od 1 sekunde bez ručnog refresh-a | Promjena statusa vidljiva svim aktivnim sesijama u za manje od 1 sekunde; mehanizam ponovnog spajanja radi u <3 sekunde (NFR-08) |
| Upravljanje pravima pristupa putem korisničkih uloga za sve četiri korisničke uloge (NFR-28, NFR-38) | Pristup rutama, UI elementima i podacima za uloge: Klijent, Agent, Tehničar, Administrator | Korisnik ne može pristupiti resursima van svoje uloge; tehničar vidi samo minimalne korisničke podatke; lozinke nisu izložene u admin panelu |
| Provjera usklađenosti s GDPR zahtjevima (NFR-27, NFR-35, NFR-36, NFR-37) | Šifriranje podataka u prijenosu (HTTPS/TLS), heširanje lozinki, anonimizacija podataka | Sav saobraćaj odvija se isključivo putem HTTPS/TLS; lozinke se čuvaju u heširanom obliku; anonimizacija podataka ne narušava integritet historije tiketa. |
| Provjera performansi pri normalnom i povećanom opterećenju (NFR-01, NFR-03) | Sve stranice sistema pri 50 istovremenih korisnika (normalno opterećenje) i 100 istovremenih korisnika (maksimalno opeterećenje) | Sve stranice učitane za manje od 2 sekunde pri 50 korisnika; nema degradacije u pefromansama više od 50% pri 100 korisnika |
| Validacija korisničkog iskustva i pristupačnosti za stariju populaciju (NFR-09, NFR-10, NFR-12) | Forma za prijavu kvara, dashboard prikaz i error poruke | Novi agent uspješno obrađuje tiket bez obuke za manje od 20 minuta; kontrast ratio ≥4.5:1; font ≥14px; max 3 koraka za prijavu kvara |
| Provjera ispravnosti modula za izvještavanje i admin dashboarda (PB-38 do PB-45) | Izvještaji po broju tiketa, statusu, tipu problema, prosječnom vremenu rješavanja, opterećenju agenata | Podaci u izvještajima odgovaraju stvarnom stanju u bazi; pristup ograničen po ulozi; nema duplikata ni netačnih vrijednosti |
| Validacija pokrivenosti unit testovima poslovne logike (NFR-25) | Backend moduli: autentifikacija,prava pristupa putem korisničkih uloga, tiketing tok, automatska dodjela, WebSocket logika | Minimalno 60% pokrivenosti unit testovima ključnih backend servisa |

---

## Nivoi testiranja

| Nivo testiranja | Fokus | Odgovorni | Izlazni kriterij |
| --- | --- | --- | --- |
| Unit testiranje | Validacija pojedinačnih funkcija i komponenti backend logike: autentifikacijski servis (bcrypt hash verifikacija, JWT generisanje/validacija), prava pristupa putem korisničkih uloga (provjera dozvola po ulozi), tiketing servis (kreiranje tiketa, generisanje UUID-a, promjena statusa, validacija prijelaza iz jednog u drugo stanje), logika automatske dodjele tiketa prema pravilima (prioritet), izračunavanje prosječnog vremena rješavanja tiketa | Dev tim | Minimalno 60% pokrivenosti (NFR-25); sve jedinične provjere prolaze bez grešaka u CI pipeline-u (NFR-26); nema failing testova u main grani |
| Integracijsko testiranje | Provjera kako ključni dijelovi sistema rade zajedno: API endpointi i baza podataka (kreiranje, čitanje i ažuriranje tiketa), WebSocket server i klijentska aplikacija (real-time prenos promjena statusa), autentifikacijski servis, kontrola pristupa po korisničkim ulogama i zaštićene rute, ORM sloj na PostgreSQL i MySQL (NFR-14), te razmjena podataka između backenda i frontenda (prikaz, filtriranje i pretraga tiketa) | Dev + QA | Sve kritične integracije prolaze bez blokera; WebSocket konekcija se uspješno uspostavlja i prenosi promjene statusa; ORM radi na oba DB sistema bez izmjene koda |
| Sistemsko testiranje | Sveobuhvatna provjera end-to-end tokova od prijave do zatvaranja tiketa za svaku korisničku ulogu: za Klijenta obuhvata registraciju, prijavu, kreiranje tiketa, praćenje statusa, komunikaciju, zatvaranje i ocjenu; za Agenta obuhvata prijavu, pregled tiketa, promjenu prioriteta, komunikaciju, prosljeđivanje i zatvaranje; za Tehničara obuhvata prijavu, pregled dodijeljenih tiketa, ažuriranje statusa s terena i zatvaranje; za Administratora obuhvata prijavu, upravljanje korisnicima, preraspodjelu agenata, pregled dashboarda i generisanje izvještaja. Uključuje i testiranje performansi (NFR-01, NFR-03, NFR-04), scenarije ponovnog povezivanja WebSocket veze (NFR-08), provjeru HTTPS/TLS zaštite (NFR-35) i testiranje na različitim web-preglednicimaV Chrome, Firefox i Edge (NFR-13) | QA tim | Svi ključni poslovni tokovi prolaze bez blokatora; performansni zahtjevi su zadovoljeni u mjerljivim granicama (NFR-01, NFR-03, NFR-04); sistem je dostupan na sva tri navedena preglednika |
| Testiranje prihvatljivosti | Potvrda usklađenosti sa poslovnim zahtjevima u realnim uvjetima: Product Owner verificira AC iz US priča visokog prioriteta (1); Agenti i Tehničari testiraju intuitivnost interfejsa (NFR-09 - uspješno obavljanje zadatka bez obuke u <20 min); Administrator provjerava upravljanje korisnicima i izvještaje; BH Telecom (naručilac) potvrđuje ukupnu isporuku u skladu sa dogovorenim MVP scope-om; provjera pristupačnosti forme za prijavu kvara za stariju populaciju (NFR-12) | Product Owner + predstavnici BH Telekoma + QA | Svi kriteriji prihvatanja iz US prioriteta 1 su zadovoljeni; Product Owner potpisuje sprint review; nema otvorenih blokera i defekata |

---

## Šta se testira u kojem nivou

| Funkcionalnost | Unit | Integracijsko | Sistemsko | Prihvatno |
| --- | --- | --- | --- | --- |
| [] | DA/NE | DA/NE | DA/NE | DA/NE |
| [] | DA/NE | DA/NE | DA/NE | DA/NE |

---

## Veza sa acceptance kriterijima

| Korisnička priča | AC kriterij | ID testnog slučaja | Status |
| --- | --- | --- | --- |
| US-1: Login s email/lozinkom | Sa ispravnim pristupnim podacima korisnik se uspješno prijavljuje i otvara dashboard | TC-001 | PENDING |
| US-1: Login s email/lozinkom | Ako su obavezna polja prazna, sistem odbija prijavu bez otkrivanja koje je polje pogrešno | TC-002 | PENDING |
| US-2: Logout | Klikom na "Logout" korisnik se vraća na login stranicu, a zaštićene stranice postaju nedostupne | TC-003 | PENDING |
| US-3: Pogrešne pristupne podatke | Kod pogrešnog emaila ili lozinke prikazuje se generička poruka greške bez otkrivanja detalja | TC-004 | PENDING |
| US-8: Kreiranje tiketa | Nakon popunjavanja forme i klika na "Pošalji", tiket se kreira, dodjeljuje se jedinstveni ID i potvrda stiže za manje od 3 sekunde | TC-005 | PENDING |
| US-8: Kreiranje tiketa | Ako je obavezno polje, npr. opis, prazno, sistem odbija kreiranje i prikazuje poruku greške | TC-006 | PENDING |
| US-9: Tip i prioritet tiketa | Kada korisnik izabere tip i prioritet iz predefinisane liste, ti atributi se ispravno upisuju na tiket | TC-007 | PENDING |
| US-9: Tip i prioritet tiketa | Ako se unese nepostojeći tip ili prioritet, sistem ne dozvoljava nastavak | TC-008 | PENDING |
| US-10: Opis problema | Pri kreiranju tiketa bez opisa sistem prikazuje poruku "Opis je obavezan" | TC-009 | PENDING |
| US-11: Lista vlastitih tiketa | Korisnik u "Moji tiketi" vidi samo svoje tikete s naslovom, statusom i datumom | TC-010 | PENDING |
| US-11: Lista vlastitih tiketa | Korisnik ne vidi tikete koji mu ne pripadaju (provjera izolacije) | TC-011 | PENDING |
| US-12: Status tiketa | Status tiketa se ažurira u prikazu bez ručnog refresh-a (real-time, NFR-02) | TC-012 | PENDING |
| US-13: Filtriranje tiketa | Filter po prioritetu, statusu i datumu vraća samo tikete koji odgovaraju zadanim kriterijima | TC-013 | PENDING |
| US-13: Filtriranje tiketa | Ako filter ne vrati nijedan rezultat, prikazuje se poruka "Nema odgovarajućih tiketa" | TC-014 | PENDING |
| US-15: Historija komunikacije | Sve poruke prikazane hronološki s pošiljaocem i timestampom | TC-015 | PENDING |
| US-16: Zatvaranje tiketa od strane korisnika | Klikom na "Zatvori tiket" status prelazi u "Zatvoren" i korisnik dobija potvrdu | TC-016 | PENDING |
| US-16: Zatvaranje tiketa | Ako korisnik pokuša zatvoriti već zatvoren tiket, sistem odbija akciju | TC-017 | PENDING |
| US-17: Zatvaranje tiketa od strane agenta | Agent šalje zahtjev za zatvaranje, korisnik prihvati i tiket se zatvara uz evidenciju agenta | TC-018 | PENDING |
| US-17: Auto-zatvaranje nakon 7 dana | Ako korisnik ne odgovori 7 dana, tiket se automatski zatvara i inicijator se evidentira | TC-019 | PENDING |
| US-17: Odbijanje zatvaranja | Kada korisnik odbije zahtjev za zatvaranje, tiket ostaje otvoren | TC-020 | PENDING |
| US-19: Slanje poruke kroz tiket | Kada korisnik pošalje poruku, poruka se vidi u historiji, a prazan unos je blokiran | TC-021 | PENDING |
| US-19: Limit poruka | Ako korisnik pokuša poslati četvrtu poruku bez odgovora agenta, sistem blokira slanje | TC-022 | PENDING |
| US-19: Limit karaktera | Ako poruka ima više od 1000 karaktera, sistem blokira slanje | TC-023 | PENDING |
| US-20: Agent odgovara | Kada agent pošalje odgovor, korisnik ga vidi odmah u realnom vremenu (NFR-02) | TC-024 | PENDING |
| US-21: Interni prioritet kod agenta | Agent može postaviti interni prioritet i dobiti potvrdu, dok korisnik taj prioritet ne vidi | TC-025 | PENDING |
| US-22: Prioritet od korisnika | Korisnik bira prioritet pri kreiranju tiketa; agent ga vidi na tiketu | TC-026 | PENDING |
| US-23: Preraspodjela agenata | Kada administrator premjesti agenta u drugi tim, promjena se evidentira sa timestampom | TC-027 | PENDING |
| US-25: Automatska dodjela | Novi tiket se dodjeljuje agentu prema pravilima, a agent dobija notifikaciju | TC-028 | PENDING |
| US-25: Nema dostupnog agenta | Ako nijedan agent nije dostupan, tiket se označava kao "Nedodijeljen" | TC-029 | PENDING |
| US-27: Prosljeđivanje tiketa | Agent prosljeđuje tiket, novi agent dobija notifikaciju, a komentar je vidljiv samo novom agentu | TC-030 | PENDING |
| US-27: Zabrana samo-prosljeđivanja | Ako agent pokuša proslijediti tiket sam sebi, sistem to blokira | TC-031 | PENDING |
| US-27: Prosljeđivanje zatvorenog tiketa | Ako agent pokuša proslijediti zatvoren tiket, sistem blokira akciju | TC-032 | PENDING |
| US-28: Interni komentar pri prosljeđivanju | Komentar vidljiv novom agentu; korisnik ga ne vidi; komentar neizmjenjiv | TC-033 | PENDING |
| US-33: Admin pregled profila | Administrator pretražuje korisnika po imenu/emailu; otvara profil; lozinka nije vidljiva | TC-034 | PENDING |
| US-34: Admin edituje profil | Administrator mijenja podatke, dobija potvrdu i promjena se evidentira, dok je direktna izmjena lozinke blokirana | TC-035 | PENDING |
| US-35: Tehničar, lista tiketa | Tehničar vidi samo tikete koji su njemu dodijeljeni i jasno razlikuje njihove statuse | TC-036 | PENDING |
| US-36: Tehničar, filtriranje | Tehničar filtrira tikete po datumu, a sistem blokira filter kada je početni datum veći od krajnjeg | TC-037 | PENDING |
| US-37: Ažuriranje statusa, tehničar | Kada tehničar promijeni status, korisnik dobija notifikaciju i promjena se evidentira | TC-038 | PENDING |
| US-37: Zabrana promjene zatvorenog statusa | Ako tehničar pokuša promijeniti status zatvorenog tiketa, sistem blokira akciju | TC-039 | PENDING |
| US-39: Minimalni podaci za tehničara | Tehničar vidi samo: ime, adresu, kontakt, tip usluge (NFR-38) | TC-040 | PENDING |
| US-40: Zabrana izmjene korisničkih podataka | Tehničar ne može mijenjati podatke korisnika u pregledu tiketa | TC-041 | PENDING |
| US-41: Izvještaj o broju tiketa | Administrator bira dnevni, sedmični, mjesečni ili godišnji period i dobija tačan broj tiketa | TC-042 | PENDING |
| US-41: Zabrana pristupa bez uloge | Kada korisnik bez admin ili tehničar uloge pokuša otvoriti izvještaj, dobija poruku "Niste ovlašteni" | TC-043 | PENDING |
| US-47: Prosječno vrijeme rješavanja | Izračun: (datum zatvaranja - datum kreiranja); nezatvoreni tiketi nisu uključeni | TC-044 | PENDING |
| US-49: Vrijeme prvog odgovora | Timestamp prvog odgovora se bilježi i ne mijenja; prikaz "Bez odgovora" ako ne postoji | TC-045 | PENDING |
| US-52: Izvještaj o opterećenju agenata | Nakon filtriranja po periodu prikazuje se tačan broj riješenih tiketa po agentu, bez duplikata | TC-046 | PENDING |
| US-54: Admin Dashboard | Dashboard prikazuje: ukupan broj tiketa po statusima, prosječno vrijeme, opterećenje agenata | TC-047 | PENDING |
| US-55: Export u CSV | Nakon klika na "Export CSV" preuzima se fajl s tačnim podacima, a za prazan export se prikazuje upozorenje | TC-048 | PENDING |
| NFR-01: Stranice u <2 sek | Lighthouse/k6 mjerenje pri 50 korisnika: dashboard, lista tiketa, detalji tiketa u <2 sek | TC-049 | PENDING |
| NFR-02: Real-time status (<1 sek) | Promjena statusa vidljiva drugom korisniku u <1 sekundi bez refresh-a | TC-050 | PENDING |
| NFR-03: 100 istovremenih korisnika | k6 load test: 100 sesija 5 minuta; povećanje vremena odgovora <50%; error rate <1% | TC-051 | PENDING |
| NFR-04: Kreiranje tiketa <3 sek | Selenium mjerenje od klika "Pošalji" do prikaza potvrde s ID-om | TC-052 | PENDING |
| NFR-07: Konzistentnost podataka pri prekidu veze | Kod prekida veze tokom kreiranja tiketa, tiket ne smije biti ni duplikovan ni izgubljen | TC-053 | PENDING |
| NFR-08: WebSocket reconnect | Nakon prekida veze obavijest stiže za manje od 1 sekunde, prvi reconnect pokušaj ide za manje od 3 sekunde, uz maksimalno 5 pokušaja | TC-054 | PENDING |
| NFR-09: Intuitivnost interfejsa | 5 ispitanika bez prethodne obuke uspješno obrađuju tiket u <20 minuta | TC-055 | PENDING |
| NFR-12: Pristupačnost, font i kontrast | Lighthouse test potvrđuje kontrast ratio od najmanje 4.5:1, font od najmanje 14px i najviše 3 koraka za prijavu kvara | TC-056 | PENDING |
| NFR-13: Cross-browser podrška | Kompletni E2E tok na Chrome, Firefox i Edge (latest verzije) | TC-057 | PENDING |
| NFR-35: HTTPS/TLS 1.2+ | DevTools/security test: sav promet HTTPS; HTTP redirectovan; bez mixed content | TC-058 | PENDING |
| NFR-36: Hash lozinki | U pregledu baze lozinka je sačuvana kao bcrypt ili Argon2 hash, a pokušaj postavljanja slabe lozinke je blokiran | TC-059 | PENDING |
| NFR-37: Anonimizacija podataka | Nakon anonimizacije korisnika PII podaci su uklonjeni, a historija tiketa ostaje netaknuta | TC-060 | PENDING |
| NFR-38: Minimizacija podataka | Svaka uloga vidi samo podatke neophodne za njen zadatak | TC-061 | PENDING |
| NFR-28: RBAC | Svaka uloga ne može pristupiti rutama/podacima van svog scope-a | TC-062 | PENDING |
| NFR-11: Responzivnost | U Chrome DevTools na desktopu 1280x720 i tabletu 768x1024 interfejs radi bez horizontalnog scrolla i preklapanja elemenata | TC-063 | PENDING |
| NFR-25: Test pokrivenost ≥60% | Coverage report iz CI-a pokazuje ≥60% pokrivenosti backend poslovne logike | TC-064 | PENDING |

---

## Način evidentiranja rezultata testiranja

U ovoj sekciji pratimo rezultate testiranja kroz jednu centralnu tabelu. Svaki red je jedan testni slučaj sa datumom izvođenja, kratkim opisom, trenutnim statusom, eventualnim ID-om defekta i napomenom (sprint/NFR). Cilj je da na jednom mjestu jasno vidimo šta je već testirano, šta još čeka i gdje treba otvoriti bug.

| Datum | ID testnog slučaja | Scenarij | Rezultat | ID defekta | Napomena |
| --- | --- | --- | --- | --- | --- |
| 2025-01-01 | TC-001 | Prijava sa ispravnim emailom i lozinkom, korisnik se preusmjerava na dashboard | PENDING | — | Sprint 5 |
| 2025-01-01 | TC-002 | Prijava sa praznim poljem lozinke, sistem odbija unos | PENDING | — | Sprint 5 |
| 2025-01-01 | TC-003 | Nakon odjave korisnik ide na login, a dashboard više nije dostupan | PENDING | — | Sprint 5 |
| 2025-01-01 | TC-004 | Prijava s pogrešnom lozinkom vraća generičku poruku bez otkrivanja detalja | PENDING | — | Sprint 5; OWASP zahtjev |
| 2025-01-01 | TC-005 | Kreiranje tiketa sa ispravnim podacima, generiše se ID i potvrda stiže za <3 sek | PENDING | — | Sprint 5; NFR-04 |
| 2025-01-01 | TC-006 | Pokušaj kreiranja tiketa bez opisa kroz formu, slanje je blokirano uz poruku greške | PENDING | — | Sprint 5 |
| 2025-01-01 | TC-007 | Odabran tip "Nestanak interneta" i prioritet "Visok" ostaju upisani na tiketu | PENDING | — | Sprint 5 |
| 2025-01-01 | TC-008 | Ručni unos nepostojećeg tipa preko API-ja vraća 422 Unprocessable Entity | PENDING | — | Sprint 5; backend validacija |
| 2025-01-01 | TC-009 | Kreiranje tiketa s praznim opisom preko API-ja vraća poruku "Opis je obavezan" | PENDING | — | Sprint 5 |
| 2025-01-01 | TC-010 | Klijent otvara "Moji tiketi" i vidi listu sa naslovom, statusom i datumom | PENDING | — | Sprint 6 |
| 2025-01-01 | TC-011 | Korisnik A ne može vidjeti tiket korisnika B ni preko direktnog GET /tickets/:id | PENDING | — | Sprint 6; RBAC test |
| 2025-01-01 | TC-012 | Kad agent promijeni status, klijent vidi promjenu bez refresh-a za <1 sek | PENDING | — | Sprint 6; NFR-02; WebSocket |
| 2025-01-01 | TC-016 | Klijent klikne "Zatvori tiket", status postaje "Zatvoren" i prikazuje se potvrda | PENDING | — | Sprint 7 |
| 2025-01-01 | TC-017 | Klijent pokušava zatvoriti već zatvoren tiket i dobija odgovarajuću grešku | PENDING | — | Sprint 7 |
| 2025-01-01 | TC-018 | Agent pokrene zatvaranje, klijent prihvati i tiket se zatvara uz evidenciju agenta | PENDING | — | Sprint 7 |
| 2025-01-01 | TC-019 | Simuliran je istek 7 dana bez odgovora klijenta i tiket se automatski zatvara | PENDING | — | Sprint 7; timer mock u testnom env |
| 2025-01-01 | TC-021 | Korisnik pošalje poruku, poruka se vidi u historiji, a prazan unos je blokiran | PENDING | — | Sprint 7 |
| 2025-01-01 | TC-022 | Nakon tri poruke bez odgovora agenta, četvrta poruka se blokira po pravilu sistema | PENDING | — | Sprint 7; business rule |
| 2025-01-01 | TC-025 | Agent postavi interni prioritet "Kritičan", ali korisnik taj prioritet ne vidi | PENDING | — | Sprint 7 |
| 2025-01-01 | TC-030 | Agent A proslijedi tiket agentu B s komentarom, B dobije notifikaciju, korisnik ne vidi komentar | PENDING | — | Sprint 10 |
| 2025-01-01 | TC-031 | Agent A pokuša proslijediti tiket sam sebi i sistem to blokira | PENDING | — | Sprint 10 |
| 2025-01-01 | TC-034 | Admin pretraži korisnika po emailu i otvori profil bez prikaza lozinke | PENDING | — | Sprint 9; NFR-28 |
| 2025-01-01 | TC-036 | Tehničar na listi tiketa vidi samo one koji su njemu dodijeljeni | PENDING | — | Sprint 10; NFR-38 |
| 2025-01-01 | TC-038 | Tehničar promijeni status, korisnik dobije notifikaciju, a promjena ide u audit log | PENDING | — | Sprint 8; NFR-30 |
| 2025-01-01 | TC-040 | Tehničar u tiketu vidi ime, adresu, kontakt i tip usluge, bez dodatnih PII podataka | PENDING | — | Sprint 7; NFR-38 |
| 2025-01-01 | TC-042 | Admin odabere sedmični period i izvještaj prikazuje tačan broj tiketa | PENDING | — | Sprint 11 |
| 2025-01-01 | TC-043 | Klijent pokuša pristupiti /reports/* i dobije 403 Forbidden uz poruku "Niste ovlašteni" | PENDING | — | Sprint 11; RBAC |
| 2025-01-01 | TC-047 | Admin otvori dashboard i vidi tačne metrike: ukupan broj tiketa, statuse i prosječno vrijeme | PENDING | — | Sprint 11; NFR-02 |
| 2025-01-01 | TC-049 | Lighthouse test za dashboard, listu i detalje tiketa prolazi pri opterećenju od 50 korisnika | PENDING | — | NFR-01; alat: Lighthouse + k6 |
| 2025-01-01 | TC-050 | U dvije paralelne sesije promjena statusa je vidljiva u oba prozora za <1 sek | PENDING | — | NFR-02; ručni test + DevTools |
| 2025-01-01 | TC-051 | k6 load test sa 100 istovremenih sesija u trajanju od 5 minuta | PENDING | — | NFR-03; max 50% degradacija |
| 2025-01-01 | TC-053 | Tokom POST /tickets simuliran prekid mreže, nakon reconnect-a tiket nije ni duplikovan ni izgubljen | PENDING | — | NFR-07; DevTools Network tab |
| 2025-01-01 | TC-054 | U offline modu obavijest stiže za <1 sek, reconnect za <3 sek, maksimalno 5 pokušaja | PENDING | — | NFR-08 |
| 2025-01-01 | TC-058 | U Security tabu sav promet ide preko HTTPS | PENDING | — | NFR-35 |
| 2025-01-01 | TC-059 | U bazi je lozinka sačuvana kao bcrypt hash, unos lozinke "12345" se odbija validacijom | PENDING | — | NFR-36 |





Notacija defekata je definisana u formatu BUG-[YYYY-MM-DD]-[sekvenca]. Primjer zapisa je BUG-2025-01-15-001, što znači da je to prvi defekt prijavljen 15. januara 2025. godine.

Za procjenu ozbiljnosti koristi se jedinstvena severity skala od S1 do S5, gdje je S1 bloker, S2 kritičan, S3 visok, S4 srednji i S5 nizak nivo uticaja.

Svaki defekt se evidentira kroz GitHub Issues i obavezno dobija odgovarajuće oznake bug, severity:S1 do severity:S5 i sprint:X, kako bi praćenje i prioritizacija bili jasni cijelom timu.

## Glavni rizici kvaliteta

| Rizik | Utjecaj | Vjerovatnoća | Mitigacija |
| --- | --- | --- | --- |
| [] | [Nizak/Srednji/Visok] | [Nizak/Srednji/Visok] | [Plan ublažavanja rizika] |

