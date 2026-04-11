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
| Verifikacija tačnosti autentifikacije i upravljanja sesijama ([PB-19](../Sprint2/UpdatedProductBacklog.md#pb-19)) | Login/logout tok za sve korisničke uloge (Klijent, Agent, Tehničar, Administrator) | Svi AC iz [US-1](../Sprint2/UserStories.md#us-1), [US-2](../Sprint2/UserStories.md#us-2) i [US-3](../Sprint2/UserStories.md#us-3) zadovoljeni; neautorizovan pristup zaštićenim rutama je onemogućen |
| Validacija centralnog tiketing toka od kreiranja do zatvaranja tiketa ([PB-22](../Sprint2/UpdatedProductBacklog.md#pb-22), [PB-25](../Sprint2/UpdatedProductBacklog.md#pb-25), [PB-36](../Sprint2/UpdatedProductBacklog.md#pb-36)) | Kreiranje tiketa, promjena statusa, zatvaranje uz korisničku potvrdu i automatsko zatvaranje nakon 7 dana | Tiket se kreira u <3 sekunde i zadovoljanava ([NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)); svi definisani statusi rade ispravno i konzistentno |
| Provjera real-time ažuriranja statusa tiketa putem WebSocket komunikacije ([NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02)) | WebSocket kanal između klijenta i servera; prikaz promjene statusa u za manje od 1 sekunde bez ručnog refresh-a | Promjena statusa vidljiva svim aktivnim sesijama u za manje od 1 sekunde; mehanizam ponovnog spajanja radi u <3 sekunde ([NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08)) |
| Upravljanje pravima pristupa putem korisničkih uloga za sve četiri korisničke uloge ([NFR-28](../Sprint2/NonFunctionalRequirements.md#nfr-28), [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38)) | Pristup rutama, UI elementima i podacima za uloge: Klijent, Agent, Tehničar, Administrator | Korisnik ne može pristupiti resursima van svoje uloge; tehničar vidi samo minimalne korisničke podatke; lozinke nisu izložene u admin panelu |
| Provjera usklađenosti s GDPR zahtjevima ([NFR-27](../Sprint2/NonFunctionalRequirements.md#nfr-27), [NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35), [NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36), [NFR-37](../Sprint2/NonFunctionalRequirements.md#nfr-37)) | Šifriranje podataka u prijenosu (HTTPS/TLS), heširanje lozinki, anonimizacija podataka | Sav saobraćaj odvija se isključivo putem HTTPS/TLS; lozinke se čuvaju u heširanom obliku; anonimizacija podataka ne narušava integritet historije tiketa. |
| Provjera performansi pri normalnom i povećanom opterećenju ([NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01), [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03)) | Sve stranice sistema pri 50 istovremenih korisnika (normalno opterećenje) i 100 istovremenih korisnika (maksimalno opeterećenje) | Sve stranice učitane za manje od 2 sekunde pri 50 korisnika; nema degradacije u pefromansama više od 50% pri 100 korisnika |
| Validacija korisničkog iskustva i pristupačnosti za stariju populaciju ([NFR-09](../Sprint2/NonFunctionalRequirements.md#nfr-09), [NFR-10](../Sprint2/NonFunctionalRequirements.md#nfr-10), [NFR-12](../Sprint2/NonFunctionalRequirements.md#nfr-12)) | Forma za prijavu kvara, dashboard prikaz i error poruke | Novi agent uspješno obrađuje tiket bez obuke za manje od 20 minuta; kontrast ratio ≥4.5:1; font ≥14px; max 3 koraka za prijavu kvara |
| Provjera ispravnosti modula za izvještavanje i admin dashboarda ([PB-38](../Sprint2/UpdatedProductBacklog.md#pb-38) do [PB-45](../Sprint2/UpdatedProductBacklog.md#pb-45)) | Izvještaji po broju tiketa, statusu, tipu problema, prosječnom vremenu rješavanja, opterećenju agenata | Podaci u izvještajima odgovaraju stvarnom stanju u bazi; pristup ograničen po ulozi; nema duplikata ni netačnih vrijednosti |
| Validacija pokrivenosti unit testovima poslovne logike ([NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25)) | Backend moduli: autentifikacija,prava pristupa putem korisničkih uloga, tiketing tok, automatska dodjela, WebSocket logika | Minimalno 60% pokrivenosti unit testovima ključnih backend servisa |

---

## Nivoi testiranja

| Nivo testiranja | Fokus | Odgovorni | Izlazni kriterij |
| --- | --- | --- | --- |
| Unit testiranje | Validacija pojedinačnih funkcija i komponenti backend logike: autentifikacijski servis (bcrypt hash verifikacija, JWT generisanje/validacija), prava pristupa putem korisničkih uloga (provjera dozvola po ulozi), tiketing servis (kreiranje tiketa, generisanje UUID-a, promjena statusa, validacija prijelaza iz jednog u drugo stanje), logika automatske dodjele tiketa prema pravilima (prioritet), izračunavanje prosječnog vremena rješavanja tiketa | Dev tim | Minimalno 60% pokrivenosti ([NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25)); sve jedinične provjere prolaze bez grešaka u CI pipeline-u ([NFR-26](../Sprint2/NonFunctionalRequirements.md#nfr-26)); nema failing testova u main grani |
| Integracijsko testiranje | Provjera kako ključni dijelovi sistema rade zajedno: API endpointi i baza podataka (kreiranje, čitanje i ažuriranje tiketa), WebSocket server i klijentska aplikacija (real-time prenos promjena statusa), autentifikacijski servis, kontrola pristupa po korisničkim ulogama i zaštićene rute, ORM sloj na PostgreSQL i MySQL ([NFR-14](../Sprint2/NonFunctionalRequirements.md#nfr-14)), te razmjena podataka između backenda i frontenda (prikaz, filtriranje i pretraga tiketa) | Dev + QA | Sve kritične integracije prolaze bez blokera; WebSocket konekcija se uspješno uspostavlja i prenosi promjene statusa; ORM radi na oba DB sistema bez izmjene koda |
| Sistemsko testiranje | Sveobuhvatna provjera end-to-end tokova od prijave do zatvaranja tiketa za svaku korisničku ulogu: za Klijenta obuhvata registraciju, prijavu, kreiranje tiketa, praćenje statusa, komunikaciju, zatvaranje i ocjenu; za Agenta obuhvata prijavu, pregled tiketa, promjenu prioriteta, komunikaciju, prosljeđivanje i zatvaranje; za Tehničara obuhvata prijavu, pregled dodijeljenih tiketa, ažuriranje statusa s terena i zatvaranje; za Administratora obuhvata prijavu, upravljanje korisnicima, preraspodjelu agenata, pregled dashboarda i generisanje izvještaja. Uključuje i testiranje performansi ([NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01), [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03), [NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)), scenarije ponovnog povezivanja WebSocket veze ([NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08)), provjeru HTTPS/TLS zaštite ([NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35)) i testiranje na različitim web-preglednicimaV Chrome, Firefox i Edge ([NFR-13](../Sprint2/NonFunctionalRequirements.md#nfr-13)) | QA tim | Svi ključni poslovni tokovi prolaze bez blokatora; performansni zahtjevi su zadovoljeni u mjerljivim granicama ([NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01), [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03), [NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)); sistem je dostupan na sva tri navedena preglednika |
| Testiranje prihvatljivosti | Potvrda usklađenosti sa poslovnim zahtjevima u realnim uvjetima: Product Owner verificira AC iz US priča visokog prioriteta (1); Agenti i Tehničari testiraju intuitivnost interfejsa ([NFR-09](../Sprint2/NonFunctionalRequirements.md#nfr-09) - uspješno obavljanje zadatka bez obuke u <20 min); Administrator provjerava upravljanje korisnicima i izvještaje; BH Telecom (naručilac) potvrđuje ukupnu isporuku u skladu sa dogovorenim MVP scope-om; provjera pristupačnosti forme za prijavu kvara za stariju populaciju ([NFR-12](../Sprint2/NonFunctionalRequirements.md#nfr-12)) | Product Owner + predstavnici BH Telekoma + QA | Svi kriteriji prihvatanja iz US prioriteta 1 su zadovoljeni; Product Owner potpisuje sprint review; nema otvorenih blokera i defekata |

---

## Šta se testira u kojem nivou

| Funkcionalnost | Unit | Integracijsko | Sistemsko | Prihvatno |
| --- | --- | --- | --- | --- |
| Login putem email/lozinka ([US-1](../Sprint2/UserStories.md#us-1), [US-2](../Sprint2/UserStories.md#us-2), [US-3](../Sprint2/UserStories.md#us-3)) | DA - validacija kredencijala, JWT i hash provjere; generička poruka greške | DA - login API, baza korisnika i zaštita ruta | DA - login, redirect na dashboard, logout i blokada zaštićenih ruta | DA - potvrda za sve 4 uloge |
| Upravljanje korisničkim profilom - promjena emaila i lozinke ([US-4](../Sprint2/UserStories.md#us-4), [US-5](../Sprint2/UserStories.md#us-5)) | DA - format emaila, jedinstvenost i pravila lozinke ([NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36)) | DA - API i baza za ažuriranje podataka | DA - korisnik mijenja podatke i dobija potvrdu | NE - niži prioritet za UAT |
| Kreiranje tiketa ([US-8](../Sprint2/UserStories.md#us-8), [US-9](../Sprint2/UserStories.md#us-9), [US-10](../Sprint2/UserStories.md#us-10)) | DA - obavezna polja, UUID i validan tip/prioritet | DA - POST /tickets i upis u bazu; potvrda <3 sek ([NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)) | DA - forma -> kreiran tiket -> potvrda s ID-om -> tiket u listi | DA - ključna MVP funkcionalnost |
| Pregled vlastitih tiketa i filteri ([US-11](../Sprint2/UserStories.md#us-11), [US-12](../Sprint2/UserStories.md#us-12), [US-13](../Sprint2/UserStories.md#us-13)) | DA - filteri, sortiranje i izolacija po korisniku | DA - GET /tickets i ispravan povrat filtera | DA - korisnik vidi samo svoje tikete; poruka za prazne rezultate | DA - bitno za transparentnost prema klijentu |
| Detaljan prikaz tiketa i historija komunikacije ([US-14](../Sprint2/UserStories.md#us-14), [US-15](../Sprint2/UserStories.md#us-15)) | DA - hronološki prikaz poruka | DA - GET /tickets/:id i ispravan redoslijed komentara | DA - vidljiv pošiljalac i timestamp za svaku poruku | DA - Product Owner potvrđuje transparentnost |
| Zatvaranje tiketa: korisnik, agent, auto nakon 7 dana ([US-16](../Sprint2/UserStories.md#us-16), [US-17](../Sprint2/UserStories.md#us-17)) | DA - prelazi statusa, zabrana ponovnog zatvaranja, timer logika | DA - API za zatvaranje, evidencija agenta, WebSocket obavijest | DA - prihvatanje/odbijanje i simulacija auto-zatvaranja | DA - kritičan poslovni tok |
| Komunikacija kroz tiket ([US-19](../Sprint2/UserStories.md#us-19), [US-20](../Sprint2/UserStories.md#us-20)) | DA - limit poruka, limit karaktera, zabrana prazne poruke | DA - API poruka i WebSocket prenos drugoj strani | DA - korisnik i agent vide poruke u realnom vremenu (<1 sek, [NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02)) | DA - obje strane potvrđuju tok |
| Real-time status tiketa preko WebSocket-a ([NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02), [NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08)) | DA - event handler i reconnect logika | DA - server i klijent razmjena statusa | DA - simulacija prekida veze i oporavak <3 sek | NE |
| Upravljanje prioritetima tiketa ([US-21](../Sprint2/UserStories.md#us-21), [US-22](../Sprint2/UserStories.md#us-22)) | DA - validacija prioriteta i skrivanje internog prioriteta od korisnika | DA - PATCH /tickets/:id/priority i audit log | DA - agent postavlja interni prioritet, korisnik ga ne vidi | NE |
| Automatska dodjela tiketa ([US-25](../Sprint2/UserStories.md#us-25), [US-26](../Sprint2/UserStories.md#us-26)) | DA - pravila dodjele i slučaj "Nedodijeljen" | DA - assignment API i notifikacija agentu | DA - kreiranje tiketa -> auto dodjela -> notifikacija | NE |
| Prosljeđivanje tiketa drugom agentu ([US-27](../Sprint2/UserStories.md#us-27), [US-28](../Sprint2/UserStories.md#us-28)) | DA - zabrana self-forward i forward zatvorenog tiketa; validacija komentara | DA - API prosljeđivanja i notifikacija novom agentu | DA - novi agent vidi komentar, korisnik ga ne vidi | NE |
| Pregled i filtriranje svih tiketa - agent ([US-29](../Sprint2/UserStories.md#us-29), [US-30](../Sprint2/UserStories.md#us-30), [US-31](../Sprint2/UserStories.md#us-31), [US-32](../Sprint2/UserStories.md#us-32)) | DA - pretraga po ID-u i filter logika | DA - GET /tickets za agent/admin scope i paginacija | DA - agent vidi sve tikete i dobija tačne rezultate pretrage | DA - Agent potvrđuje kompletnost prikaza |
| Preraspodjela agenata po timovima - administrator ([US-23](../Sprint2/UserStories.md#us-23), [US-24](../Sprint2/UserStories.md#us-24)) | DA - validacija promjene i evidencija promjene | DA - API za promjenu tima i audit log | DA - promjena tima se odmah vidi u sistemu | DA - Administrator potvrđuje |
| Pregled i uređivanje korisničkih profila - administrator ([US-33](../Sprint2/UserStories.md#us-33), [US-34](../Sprint2/UserStories.md#us-34)) | DA - zabrana direktne izmjene lozinke i evidencija izmjena | DA - GET/PUT /users/:id; lozinka nije u API odgovoru | DA - pretraga -> uređivanje -> potvrda -> evidentiranje | DA - potvrda [NFR-28](../Sprint2/NonFunctionalRequirements.md#nfr-28) |
| Tehničar: pregled tiketa i promjena statusa ([US-35](../Sprint2/UserStories.md#us-35), [US-36](../Sprint2/UserStories.md#us-36), [US-37](../Sprint2/UserStories.md#us-37), [US-38](../Sprint2/UserStories.md#us-38)) | DA - samo dodijeljeni tiketi i audit log promjena | DA - technician API, promjena statusa i WebSocket obavijest | DA - tehničar mijenja status, korisnik dobija notifikaciju | DA - Tehničar potvrđuje tok rada |
| Minimizacija podataka za tehničara ([US-39](../Sprint2/UserStories.md#us-39), [US-40](../Sprint2/UserStories.md#us-40), [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38)) | DA - API vraća samo potrebne podatke | DA - provjera endpointa i baze bez osjetljivih podataka | DA - ekran tehničara ne prikazuje višak ličnih podataka | DA - potvrda RBAC i minimizacije |
| Admin Dashboard s ključnim metrikama ([US-54](../Sprint2/UserStories.md#us-54), [PB-45](../Sprint2/UpdatedProductBacklog.md#pb-45)) | NE | DA - tačnost agregacija iz baze | DA - dashboard prikazuje ažurne i tačne metrike | DA - Administrator/BH Telecom potvrđuju |
| Izvještaji ([PB-38](../Sprint2/UpdatedProductBacklog.md#pb-38) do [PB-44](../Sprint2/UpdatedProductBacklog.md#pb-44)) | DA - proračuni, grupisanje i filteri po periodu | DA - reports API, role-based pristup i bez duplikata | DA - tačni podaci i blokada neovlaštenih uloga | DA - Administrator i Agent potvrđuju |
| Export izvještaja u CSV ([US-55](../Sprint2/UserStories.md#us-55), [PB-46](../Sprint2/UpdatedProductBacklog.md#pb-46)) | DA - ispravan CSV i obrada praznog exporta | DA - export API i file streaming | DA - preuzimanje tačnog CSV-a i upozorenje za prazan export | NE - niži prioritet |
| FAQ segment ([US-56](../Sprint2/UserStories.md#us-56), [PB-47](../Sprint2/UpdatedProductBacklog.md#pb-47)) | NE | DA - GET /faq i baza | DA - prikaz FAQ liste i poruka kada je prazno | NE |
| Prikaz paketa i pretplata ([US-6](../Sprint2/UserStories.md#us-6), [US-7](../Sprint2/UserStories.md#us-7)) | NE | DA - subscriptions API i filter po korisniku | DA - korisnik vidi samo svoje pakete | NE - prioritet 4 |
| HTTPS/TLS 1.2+ enkripcija ([NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35)) | NE | DA - provjera server konfiguracije i sigurnosnih headera | DA - sav promet HTTPS, redirect sa HTTP, bez mixed content | NE |
| Sigurno čuvanje lozinki - bcrypt/Argon2 ([NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36)) | DA - hash logika i pravila lozinke | DA - u bazi su lozinke samo kao hash | DA - lozinka se ne može dobiti kao plain text | NE |
| Anonimizacija i pravo na brisanje ([NFR-37](../Sprint2/NonFunctionalRequirements.md#nfr-37)) | DA - uklanjanje PII bez brisanja tiketa | DA - API za anonimizaciju i provjera baze | DA - historija ostaje, lični podaci su uklonjeni | DA - BH Telecom potvrđuje usklađenost |
| Responzivnost desktop/tablet ([NFR-11](../Sprint2/NonFunctionalRequirements.md#nfr-11)) | NE | NE | DA - test na 1280x720 i 768x1024 bez horizontalnog scrolla | NE |
| Podrška za Chrome, Firefox i Edge ([NFR-13](../Sprint2/NonFunctionalRequirements.md#nfr-13)) | NE | NE | DA - isti ključni tok prolazi na sva 3 browsera | NE |
| Dostupnost sistema 99% uptime ([NFR-05](../Sprint2/NonFunctionalRequirements.md#nfr-05)) | NE | NE | DA - monitoring dostupnosti i evidencija zastoja | DA - BH Telecom pregleda izvještaj |
| Konzistentnost podataka pri prekidu WebSocket veze ([NFR-07](../Sprint2/NonFunctionalRequirements.md#nfr-07)) | DA - transakcije i rollback pri grešci | DA - simulacija prekida tokom kreiranja tiketa | DA - tiket nije ni duplikovan ni izgubljen | NE |
| Horizontalna skalabilnost ([NFR-15](../Sprint2/NonFunctionalRequirements.md#nfr-15)) | NE | DA - 2 backend instance iza load balancera, sesije bez vezanja za jednu instancu | DA - sesije rade bez obzira na instancu | NE |
| Pokrivenost unit testovima >=60% ([NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25)) | DA - izvještaj pokrića | NE | NE | NE |
| CI/CD provjera pri PR-u ([NFR-26](../Sprint2/NonFunctionalRequirements.md#nfr-26)) | NE | DA - PR se ne spaja bez prolaska build/test provjera | NE | NE |

---

## Veza sa acceptance kriterijima

| Korisnička priča | AC kriterij | ID testnog slučaja | Status |
| --- | --- | --- | --- |
| [US-1](../Sprint2/UserStories.md#us-1): Login s email/lozinkom | Sa ispravnim pristupnim podacima korisnik se uspješno prijavljuje i otvara dashboard | TC-001 | PENDING |
| [US-1](../Sprint2/UserStories.md#us-1): Login s email/lozinkom | Ako su obavezna polja prazna, sistem odbija prijavu bez otkrivanja koje je polje pogrešno | TC-002 | PENDING |
| [US-2](../Sprint2/UserStories.md#us-2): Logout | Klikom na "Logout" korisnik se vraća na login stranicu, a zaštićene stranice postaju nedostupne | TC-003 | PENDING |
| [US-3](../Sprint2/UserStories.md#us-3): Pogrešni pristupni podaci | Kod pogrešnog emaila ili lozinke prikazuje se generička poruka greške bez otkrivanja detalja | TC-004 | PENDING |
| [US-8](../Sprint2/UserStories.md#us-8): Kreiranje tiketa | Nakon popunjavanja forme i klika na "Pošalji", tiket se kreira, dodjeljuje se jedinstveni ID i potvrda stiže za manje od 3 sekunde | TC-005 | PENDING |
| [US-8](../Sprint2/UserStories.md#us-8): Kreiranje tiketa | Ako je obavezno polje, npr. opis, prazno, sistem odbija kreiranje i prikazuje poruku greške | TC-006 | PENDING |
| [US-9](../Sprint2/UserStories.md#us-9): Tip i prioritet tiketa | Kada korisnik izabere tip i prioritet iz predefinisane liste, ti atributi se ispravno upisuju na tiket | TC-007 | PENDING |
| [US-9](../Sprint2/UserStories.md#us-9): Tip i prioritet tiketa | Ako se unese nepostojeći tip ili prioritet, sistem ne dozvoljava nastavak | TC-008 | PENDING |
| [US-10](../Sprint2/UserStories.md#us-10): Opis problema | Pri kreiranju tiketa bez opisa sistem prikazuje poruku "Opis je obavezan" | TC-009 | PENDING |
| [US-11](../Sprint2/UserStories.md#us-11): Lista vlastitih tiketa | Korisnik u "Moji tiketi" vidi samo svoje tikete s naslovom, statusom i datumom | TC-010 | PENDING |
| [US-11](../Sprint2/UserStories.md#us-11): Lista vlastitih tiketa | Korisnik ne vidi tikete koji mu ne pripadaju (provjera izolacije) | TC-011 | PENDING |
| [US-12](../Sprint2/UserStories.md#us-12): Status tiketa | Status tiketa se ažurira u prikazu bez ručnog refresh-a (real-time, [NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02)) | TC-012 | PENDING |
| [US-13](../Sprint2/UserStories.md#us-13): Filtriranje tiketa | Filter po prioritetu, statusu i datumu vraća samo tikete koji odgovaraju zadanim kriterijima | TC-013 | PENDING |
| [US-13](../Sprint2/UserStories.md#us-13): Filtriranje tiketa | Ako filter ne vrati nijedan rezultat, prikazuje se poruka "Nema odgovarajućih tiketa" | TC-014 | PENDING |
| [US-15](../Sprint2/UserStories.md#us-15): Historija komunikacije | Sve poruke prikazane hronološki s pošiljaocem i timestampom | TC-015 | PENDING |
| [US-16](../Sprint2/UserStories.md#us-16): Zatvaranje tiketa od strane korisnika | Klikom na "Zatvori tiket" status prelazi u "Zatvoren" i korisnik dobija potvrdu | TC-016 | PENDING |
| [US-16](../Sprint2/UserStories.md#us-16): Zatvaranje tiketa | Ako korisnik pokuša zatvoriti već zatvoren tiket, sistem odbija akciju | TC-017 | PENDING |
| [US-17](../Sprint2/UserStories.md#us-17): Zatvaranje tiketa od strane agenta | Agent šalje zahtjev za zatvaranje, korisnik prihvati i tiket se zatvara uz evidenciju agenta | TC-018 | PENDING |
| [US-17](../Sprint2/UserStories.md#us-17): Auto-zatvaranje nakon 7 dana | Ako korisnik ne odgovori 7 dana, tiket se automatski zatvara i inicijator se evidentira | TC-019 | PENDING |
| [US-17](../Sprint2/UserStories.md#us-17): Odbijanje zatvaranja | Kada korisnik odbije zahtjev za zatvaranje, tiket ostaje otvoren | TC-020 | PENDING |
| [US-19](../Sprint2/UserStories.md#us-19): Slanje poruke kroz tiket | Kada korisnik pošalje poruku, poruka se vidi u historiji, a prazan unos je blokiran | TC-021 | PENDING |
| [US-19](../Sprint2/UserStories.md#us-19): Limit poruka | Ako korisnik pokuša poslati četvrtu poruku bez odgovora agenta, sistem blokira slanje | TC-022 | PENDING |
| [US-19](../Sprint2/UserStories.md#us-19): Limit karaktera | Ako poruka ima više od 1000 karaktera, sistem blokira slanje | TC-023 | PENDING |
| [US-20](../Sprint2/UserStories.md#us-20): Agent odgovara | Kada agent pošalje odgovor, korisnik ga vidi odmah u realnom vremenu ([NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02)) | TC-024 | PENDING |
| [US-21](../Sprint2/UserStories.md#us-21): Interni prioritet kod agenta | Agent može postaviti interni prioritet i dobiti potvrdu, dok korisnik taj prioritet ne vidi | TC-025 | PENDING |
| [US-22](../Sprint2/UserStories.md#us-22): Prioritet od korisnika | Korisnik bira prioritet pri kreiranju tiketa; agent ga vidi na tiketu | TC-026 | PENDING |
| [US-23](../Sprint2/UserStories.md#us-23): Preraspodjela agenata | Kada administrator premjesti agenta u drugi tim, promjena se evidentira sa timestampom | TC-027 | PENDING |
| [US-25](../Sprint2/UserStories.md#us-25): Automatska dodjela | Novi tiket se dodjeljuje agentu prema pravilima, a agent dobija notifikaciju | TC-028 | PENDING |
| [US-25](../Sprint2/UserStories.md#us-25): Nema dostupnog agenta | Ako nijedan agent nije dostupan, tiket se označava kao "Nedodijeljen" | TC-029 | PENDING |
| [US-27](../Sprint2/UserStories.md#us-27): Prosljeđivanje tiketa | Agent prosljeđuje tiket, novi agent dobija notifikaciju, a komentar je vidljiv samo novom agentu | TC-030 | PENDING |
| [US-27](../Sprint2/UserStories.md#us-27): Zabrana samo-prosljeđivanja | Ako agent pokuša proslijediti tiket sam sebi, sistem to blokira | TC-031 | PENDING |
| [US-27](../Sprint2/UserStories.md#us-27): Prosljeđivanje zatvorenog tiketa | Ako agent pokuša proslijediti zatvoren tiket, sistem blokira akciju | TC-032 | PENDING |
| [US-28](../Sprint2/UserStories.md#us-28): Interni komentar pri prosljeđivanju | Komentar vidljiv novom agentu; korisnik ga ne vidi; komentar neizmjenjiv | TC-033 | PENDING |
| [US-33](../Sprint2/UserStories.md#us-33): Admin pregled profila | Administrator pretražuje korisnika po imenu/emailu; otvara profil; lozinka nije vidljiva | TC-034 | PENDING |
| [US-34](../Sprint2/UserStories.md#us-34): Admin edituje profil | Administrator mijenja podatke, dobija potvrdu i promjena se evidentira, dok je direktna izmjena lozinke blokirana | TC-035 | PENDING |
| [US-35](../Sprint2/UserStories.md#us-35): Tehničar, lista tiketa | Tehničar vidi samo tikete koji su njemu dodijeljeni i jasno razlikuje njihove statuse | TC-036 | PENDING |
| [US-36](../Sprint2/UserStories.md#us-36): Tehničar, filtriranje | Tehničar filtrira tikete po datumu, a sistem blokira filter kada je početni datum veći od krajnjeg | TC-037 | PENDING |
| [US-37](../Sprint2/UserStories.md#us-37): Ažuriranje statusa, tehničar | Kada tehničar promijeni status, korisnik dobija notifikaciju i promjena se evidentira | TC-038 | PENDING |
| [US-37](../Sprint2/UserStories.md#us-37): Zabrana promjene zatvorenog statusa | Ako tehničar pokuša promijeniti status zatvorenog tiketa, sistem blokira akciju | TC-039 | PENDING |
| [US-39](../Sprint2/UserStories.md#us-39): Minimalni podaci za tehničara | Tehničar vidi samo: ime, adresu, kontakt, tip usluge ([NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38)) | TC-040 | PENDING |
| [US-40](../Sprint2/UserStories.md#us-40): Zabrana izmjene korisničkih podataka | Tehničar ne može mijenjati podatke korisnika u pregledu tiketa | TC-041 | PENDING |
| [US-41](../Sprint2/UserStories.md#us-41): Izvještaj o broju tiketa | Administrator bira dnevni, sedmični, mjesečni ili godišnji period i dobija tačan broj tiketa | TC-042 | PENDING |
| [US-41](../Sprint2/UserStories.md#us-41): Zabrana pristupa bez uloge | Kada korisnik bez admin ili tehničar uloge pokuša otvoriti izvještaj, dobija poruku "Niste ovlašteni" | TC-043 | PENDING |
| [US-47](../Sprint2/UserStories.md#us-47): Prosječno vrijeme rješavanja | Izračun: (datum zatvaranja - datum kreiranja); nezatvoreni tiketi nisu uključeni | TC-044 | PENDING |
| [US-49](../Sprint2/UserStories.md#us-49): Vrijeme prvog odgovora | Timestamp prvog odgovora se bilježi i ne mijenja; prikaz "Bez odgovora" ako ne postoji | TC-045 | PENDING |
| [US-52](../Sprint2/UserStories.md#us-52): Izvještaj o opterećenju agenata | Nakon filtriranja po periodu prikazuje se tačan broj riješenih tiketa po agentu, bez duplikata | TC-046 | PENDING |
| [US-54](../Sprint2/UserStories.md#us-54): Admin Dashboard | Dashboard prikazuje: ukupan broj tiketa po statusima, prosječno vrijeme, opterećenje agenata | TC-047 | PENDING |
| [US-55](../Sprint2/UserStories.md#us-55): Export u CSV | Nakon klika na "Export CSV" preuzima se fajl s tačnim podacima, a za prazan export se prikazuje upozorenje | TC-048 | PENDING |
| [NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01): Stranice u <2 sek | Lighthouse/k6 mjerenje pri 50 korisnika: dashboard, lista tiketa, detalji tiketa u <2 sek | TC-049 | PENDING |
| [NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02): Real-time status (<1 sek) | Promjena statusa vidljiva drugom korisniku u <1 sekundi bez refresh-a | TC-050 | PENDING |
| [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03): 100 istovremenih korisnika | k6 load test: 100 sesija 5 minuta; povećanje vremena odgovora <50%; error rate <1% | TC-051 | PENDING |
| [NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04): Kreiranje tiketa <3 sek | Selenium mjerenje od klika "Pošalji" do prikaza potvrde s ID-om | TC-052 | PENDING |
| [NFR-07](../Sprint2/NonFunctionalRequirements.md#nfr-07): Konzistentnost podataka pri prekidu veze | Kod prekida veze tokom kreiranja tiketa, tiket ne smije biti ni duplikovan ni izgubljen | TC-053 | PENDING |
| [NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08): WebSocket reconnect | Nakon prekida veze obavijest stiže za manje od 1 sekunde, prvi reconnect pokušaj ide za manje od 3 sekunde, uz maksimalno 5 pokušaja | TC-054 | PENDING |
| [NFR-09](../Sprint2/NonFunctionalRequirements.md#nfr-09): Intuitivnost interfejsa | 5 ispitanika bez prethodne obuke uspješno obrađuju tiket u <20 minuta | TC-055 | PENDING |
| [NFR-12](../Sprint2/NonFunctionalRequirements.md#nfr-12): Pristupačnost, font i kontrast | Lighthouse test potvrđuje kontrast ratio od najmanje 4.5:1, font od najmanje 14px i najviše 3 koraka za prijavu kvara | TC-056 | PENDING |
| [NFR-13](../Sprint2/NonFunctionalRequirements.md#nfr-13): Cross-browser podrška | Kompletni E2E tok na Chrome, Firefox i Edge (latest verzije) | TC-057 | PENDING |
| [NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35): HTTPS/TLS 1.2+ | DevTools/security test: sav promet HTTPS; HTTP redirectovan; bez mixed content | TC-058 | PENDING |
| [NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36): Hash lozinki | U pregledu baze lozinka je sačuvana kao bcrypt ili Argon2 hash, a pokušaj postavljanja slabe lozinke je blokiran | TC-059 | PENDING |
| [NFR-37](../Sprint2/NonFunctionalRequirements.md#nfr-37): Anonimizacija podataka | Nakon anonimizacije korisnika PII podaci su uklonjeni, a historija tiketa ostaje netaknuta | TC-060 | PENDING |
| [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38): Minimizacija podataka | Svaka uloga vidi samo podatke neophodne za njen zadatak | TC-061 | PENDING |
| [NFR-28](../Sprint2/NonFunctionalRequirements.md#nfr-28): RBAC (Role-Based Access Control) | Svaka uloga ne može pristupiti rutama/podacima van svog scope-a | TC-062 | PENDING |
| [NFR-11](../Sprint2/NonFunctionalRequirements.md#nfr-11): Responzivnost | U Chrome DevTools na desktopu 1280x720 i tabletu 768x1024 interfejs radi bez horizontalnog scrolla i preklapanja elemenata | TC-063 | PENDING |
| [NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25): Test pokrivenost ≥60% | izvještaj pokrića iz CI-a pokazuje ≥60% pokrivenosti backend poslovne logike | TC-064 | PENDING |

---

## Način evidentiranja rezultata testiranja

U ovoj sekciji pratimo rezultate testiranja kroz jednu centralnu tabelu. Svaki red je jedan testni slučaj sa datumom izvođenja, kratkim opisom, trenutnim statusom, eventualnim ID-om defekta i napomenom (sprint/NFR). Cilj je da na jednom mjestu jasno vidimo šta je već testirano, šta još čeka i gdje treba otvoriti bug.

| Datum | ID testnog slučaja | Scenarij | Rezultat | ID defekta | Napomena |
| --- | --- | --- | --- | --- | --- |
| 01-01-2025 | TC-001 | Prijava sa ispravnim emailom i lozinkom, korisnik se preusmjerava na dashboard | PENDING | — | Sprint 5 |
| 01-01-2025 | TC-002 | Prijava sa praznim poljem lozinke, sistem odbija unos | PENDING | — | Sprint 5 |
| 01-01-2025 | TC-003 | Nakon odjave korisnik ide na login, a dashboard više nije dostupan | PENDING | — | Sprint 5 |
| 01-01-2025 | TC-004 | Prijava s pogrešnom lozinkom vraća generičku poruku bez otkrivanja detalja | PENDING | — | Sprint 5; OWASP zahtjev |
| 01-01-2025 | TC-005 | Kreiranje tiketa sa ispravnim podacima, generiše se ID i potvrda stiže za <3 sek | PENDING | — | Sprint 5; [NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04) |
| 01-01-2025 | TC-006 | Pokušaj kreiranja tiketa bez opisa kroz formu, slanje je blokirano uz poruku greške | PENDING | — | Sprint 5 |
| 01-01-2025 | TC-007 | Odabran tip "Nestanak interneta" i prioritet "Visok" ostaju upisani na tiketu | PENDING | — | Sprint 5 |
| 01-01-2025 | TC-008 | Ručni unos nepostojećeg tipa preko API-ja vraća 422 Unprocessable Entity | PENDING | — | Sprint 5; backend validacija |
| 01-01-2025 | TC-009 | Kreiranje tiketa s praznim opisom preko API-ja vraća poruku "Opis je obavezan" | PENDING | — | Sprint 5 |
| 01-01-2025 | TC-010 | Klijent otvara "Moji tiketi" i vidi listu sa naslovom, statusom i datumom | PENDING | — | Sprint 6 |
| 01-01-2025 | TC-011 | Korisnik A ne može vidjeti tiket korisnika B ni preko direktnog GET /tickets/:id | PENDING | — | Sprint 6; RBAC (Role-Based Access Control) test |
| 01-01-2025 | TC-012 | Kad agent promijeni status, klijent vidi promjenu bez refresh-a za <1 sek | PENDING | — | Sprint 6; [NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02); WebSocket |
| 01-01-2025 | TC-016 | Klijent klikne "Zatvori tiket", status postaje "Zatvoren" i prikazuje se potvrda | PENDING | — | Sprint 7 |
| 01-01-2025 | TC-017 | Klijent pokušava zatvoriti već zatvoren tiket i dobija odgovarajuću grešku | PENDING | — | Sprint 7 |
| 01-01-2025 | TC-018 | Agent pokrene zatvaranje, klijent prihvati i tiket se zatvara uz evidenciju agenta | PENDING | — | Sprint 7 |
| 01-01-2025 | TC-019 | Simuliran je istek 7 dana bez odgovora klijenta i tiket se automatski zatvara | PENDING | — | Sprint 7; timer mock u testnom env |
| 01-01-2025 | TC-021 | Korisnik pošalje poruku, poruka se vidi u historiji, a prazan unos je blokiran | PENDING | — | Sprint 7 |
| 01-01-2025 | TC-022 | Nakon tri poruke bez odgovora agenta, četvrta poruka se blokira po pravilu sistema | PENDING | — | Sprint 7; business rule |
| 01-01-2025 | TC-025 | Agent postavi interni prioritet "Kritičan", ali korisnik taj prioritet ne vidi | PENDING | — | Sprint 7 |
| 01-01-2025 | TC-030 | Agent A proslijedi tiket agentu B s komentarom, B dobije notifikaciju, korisnik ne vidi komentar | PENDING | — | Sprint 10 |
| 01-01-2025 | TC-031 | Agent A pokuša proslijediti tiket sam sebi i sistem to blokira | PENDING | — | Sprint 10 |
| 01-01-2025 | TC-034 | Admin pretraži korisnika po emailu i otvori profil bez prikaza lozinke | PENDING | — | Sprint 9; [NFR-28](../Sprint2/NonFunctionalRequirements.md#nfr-28) |
| 01-01-2025 | TC-036 | Tehničar na listi tiketa vidi samo one koji su njemu dodijeljeni | PENDING | — | Sprint 10; [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38) |
| 01-01-2025 | TC-038 | Tehničar promijeni status, korisnik dobije notifikaciju, a promjena ide u audit log | PENDING | — | Sprint 8; [NFR-30](../Sprint2/NonFunctionalRequirements.md#nfr-30) |
| 01-01-2025 | TC-040 | Tehničar u tiketu vidi ime, adresu, kontakt i tip usluge, bez dodatnih PII podataka | PENDING | — | Sprint 7; [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38) |
| 01-01-2025 | TC-042 | Admin odabere sedmični period i izvještaj prikazuje tačan broj tiketa | PENDING | — | Sprint 11 |
| 01-01-2025 | TC-043 | Klijent pokuša pristupiti /reports/* i dobije 403 Forbidden uz poruku "Niste ovlašteni" | PENDING | — | Sprint 11; RBAC (Role-Based Access Control) |
| 01-01-2025 | TC-047 | Admin otvori dashboard i vidi tačne metrike: ukupan broj tiketa, statuse i prosječno vrijeme | PENDING | — | Sprint 11; [NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02) |
| 01-01-2025 | TC-049 | Lighthouse test za dashboard, listu i detalje tiketa prolazi pri opterećenju od 50 korisnika | PENDING | — | [NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01); alat: Lighthouse + k6 |
| 01-01-2025 | TC-050 | U dvije paralelne sesije promjena statusa je vidljiva u oba prozora za <1 sek | PENDING | — | [NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02); ručni test + DevTools |
| 01-01-2025 | TC-051 | k6 load test sa 100 istovremenih sesija u trajanju od 5 minuta | PENDING | — | [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03); max 50% degradacija |
| 01-01-2025 | TC-053 | Tokom POST /tickets simuliran prekid mreže, nakon reconnect-a tiket nije ni duplikovan ni izgubljen | PENDING | — | [NFR-07](../Sprint2/NonFunctionalRequirements.md#nfr-07); DevTools Network tab |
| 01-01-2025 | TC-054 | U offline modu obavijest stiže za <1 sek, reconnect za <3 sek, maksimalno 5 pokušaja | PENDING | — | [NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08) |
| 01-01-2025 | TC-058 | U Security tabu sav promet ide preko HTTPS | PENDING | — | [NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35) |
| 01-01-2025 | TC-059 | U bazi je lozinka sačuvana kao bcrypt hash, unos lozinke "12345" se odbija validacijom | PENDING | — | [NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36) |





Notacija defekata je definisana u formatu BUG-[DD-MM-YYYY]-[sekvenca]. Primjer zapisa je BUG-15-01-2025-001, što znači da je to prvi defekt prijavljen 15-01-2025.

Za procjenu ozbiljnosti koristi se jedinstvena severity skala od S1 do S5, gdje je S1 bloker, S2 kritičan, S3 visok, S4 srednji i S5 nizak nivo uticaja.

Svaki defekt se evidentira kroz GitHub Issues i obavezno dobija odgovarajuće oznake bug, severity:S1 do severity:S5 i sprint:X, kako bi praćenje i prioritizacija bili jasni cijelom timu.

---

## Glavni rizici kvaliteta

| Rizik | Utjecaj | Vjerovatnoća | Mitigacija |
| --- | --- | --- | --- |
| **R-01: Nestabilna WebSocket veza na mobilnoj mreži tehničara** — Tehničari često rade na slabijem internetu. Ako pukne WebSocket (real-time) veza, može doći do gubitka podataka, prikaza starog statusa tiketa ili nemogućnosti ažuriranja statusa ([NFR-07](../Sprint2/NonFunctionalRequirements.md#nfr-07), [NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08)). | Visok | Visoka | Uvesti i testirati automatsko ponovno spajanje s postepenim čekanjem (max 5 pokušaja, na 3 sekunde), korisniku prikazati obavijest o prekidu u <1 sek (TC-054); simulirati gubitak mreže u DevTools nakon reconnecta provjeriti da su podaci u bazi ispravni i bez duplikata (TC-053) |
| **R-02: Propusti u kontroli pristupa po ulogama** — Sistem ima 4 uloge s različitim pravima. Ako kontrola pristupa nije dobro podešena, Klijent može vidjeti tuđe tikete, Tehničar osjetljive podatke, a Agent adminsko područje. To direktno ugrožava GDPR ([NFR-27](../Sprint2/NonFunctionalRequirements.md#nfr-27), [NFR-28](../Sprint2/NonFunctionalRequirements.md#nfr-28), [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38)). | Visok | Srednja | Brzi osnovni RBAC (Role-Based Access Control) testovi za sve kombinacije uloga (TC-062); negativni testovi gdje svaka uloga pokušava pristupiti tuđim resursima; automatizovani API testovi koji očekuju 403 za neovlašten pristup; obavezan pregled RBAC (Role-Based Access Control) dijela koda u svakom PR-u ([NFR-26](../Sprint2/NonFunctionalRequirements.md#nfr-26)) |
| **R-03: GDPR neusklađenost - curenje ličnih podataka ili loša anonimizacija** — Sistem obrađuje lične podatke korisnika telekoma. Greška u anonimizaciji, čuvanju lozinki ili enkripciji može dovesti do kazni i gubitka povjerenja BH Telekoma ([NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35), [NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36), [NFR-37](../Sprint2/NonFunctionalRequirements.md#nfr-37)). | Visok | Srednja | Provjeriti da su lozinke sačuvane kao bcrypt/Argon2 hash (TC-059); provjeriti HTTPS/TLS zaštitu (TC-058); testirati anonimizaciju tako da su lični podaci uklonjeni, a historija tiketa ostane netaknuta (TC-060); pregledati API odgovore da ne vraćaju osjetljive podatke neovlaštenim ulogama; uključiti pravni pregled na sprint review-u |
| **R-04: Pad performansi kod masovnih kvarova u mreži** — Ako dođe do velikog kvara u mreži BH Telekoma, može doći do stotina prijava odjednom. Sistem mora ostati stabilan za ≥100 istovremenih korisnika, bez povećanja vremena odziva >50% ([NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03)). Problem je što nemamo potpuno realne produkcione podatke za testiranje. | Visok | Srednja | k6 load test sa 100 sesija tokom 5 minuta (TC-051); mjeriti broj grešaka (error rate) i prosječno/maksimalno vrijeme odziva |
| **R-05: Promjena zahtjeva od strane BH Telekoma tokom MVP razvoja** — BH Telecom kao naručilac može mijenjati prioritete. Ako se zahtjevi promijene kasno u sprintu, mogu se ugroziti rokovi isporuke i testni plan. | Visok | Srednja | Redovni sprint review demo sa BH Telecomom i Product Ownerom; jasno definisati Definition of Done (šta znači da je zadatak stvarno gotov) i Change Request proces (kako se uvode promjene); svaka promjena prolazi kroz Product Ownera kao jedinu tačku odobrenja; ažurirati Test Strategy nakon svake odobrene promjene |
| **R-06: Složena logika zatvaranja tiketa (7 dana, prihvatanje/odbijanje)** — [US-17](../Sprint2/UserStories.md#us-17) ima više scenarija: agent pokreće zatvaranje, korisnik prihvati/odbije, ili se tiket automatski zatvara nakon 7 dana. Greška u logici prelaza statusa može ostaviti tiket "zaglavljen" ili napraviti netačnu evidenciju ([PB-25](../Sprint2/UpdatedProductBacklog.md#pb-25)). | Visok | Srednja | Detaljno testirati sve prelaze statusa na unit nivou; testovi od početka do kraja za tri glavna scenarija zatvaranja (TC-018, TC-019, TC-020); pokriti rubne slučajeve (tiketi blizu isteka 7 dana); koristiti simulaciju vremena u test okruženju za istek roka |
| **R-07: Pristupačnost za stariju populaciju** — Starijim korisnicima trebaju veći fontovi (≥14px), dobar kontrast (≥4.5:1) i jednostavan tok prijave (max 3 koraka). Ako ovo ne ispunimo, povećava se broj poziva podršci i slabi korisničko iskustvo BH Telekoma ([NFR-12](../Sprint2/NonFunctionalRequirements.md#nfr-12), [NFR-09](../Sprint2/NonFunctionalRequirements.md#nfr-09)). | Srednji | Srednja | Automatska Lighthouse provjera kontrasta i veličine fonta (TC-056); ručno testiranje forme za prijavu kvara i brojanje koraka; usability test s 5 ispitanika (TC-055) uz mjerenje vremena završetka zadatka i broja grešaka |
| **R-08: Razlike između browsera** — Sistem mora raditi na Chrome, Firefox i Edge ([NFR-13](../Sprint2/NonFunctionalRequirements.md#nfr-13)). Real-time funkcije i WebSocket mogu se drugačije ponašati na Edge-u ili starijim Firefox verzijama. | Srednji | Niska | Testirati kompletan tok od početka do kraja na sva tri browsera (TC-057); posebno pratiti WebSocket konekciju i real-time prikaz; pokrenuti automatske osnovne testove u CI procesu preko Selenium alata ([NFR-26](../Sprint2/NonFunctionalRequirements.md#nfr-26)) |
| **R-09: Nema realnih produkcionih podataka za testiranje** — Razvoj se radi na testnim i simuliranim podacima (Product Vision - ograničenja). Zbog toga procjena opterećenja i ponašanja u produkciji može biti manje tačna, posebno za load testove [NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01) i [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03). | Srednji | Visoka | Pripremiti realistične testne scenarije za maksimalno opterećenje, npr. masovni kvar u mreži; parametrizovati k6 testove za više volumena; pokušati dobiti anonimiziran uzorak produkcionih podataka od BH Telekoma; dokumentovati pretpostavke i ponovo ih provjeriti pri Go-Live fazi |
| **R-10: Nepotpuni audit log i evidencija promjena** — Sistem mora zapisivati sve ključne akcije (kreiranje, promjena statusa, dodjela, zatvaranje, izmjena korisničkih podataka) zbog revizije ([NFR-30](../Sprint2/NonFunctionalRequirements.md#nfr-30), [NFR-31](../Sprint2/NonFunctionalRequirements.md#nfr-31)). Ako dnevnik aktivnosti (audit log) nije potpun, poslije je teško rekonstruisati šta se desilo i mogu nastati pravni problemi za BH Telecom. | Srednji | Niska | Provjera audit log-a nakon svakog kritičnog toka (TC-027, TC-035, TC-038); provjeriti da svaki log zapis ima: ko, šta, kada; negativni test da izmjena log zapisa nije dozvoljena; pregled DB strukture log tablice tokom pregleda koda |
| **R-11: Slaba pokrivenost unit testovima i rast tehničkog duga** — [NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25) traži najmanje 60% pokrivenosti testovima. Ako padnemo ispod tog praga, raste rizik od regresija| Srednji | Srednja | Izvještaj pokrića mora biti obavezan artefakt u CI procesu (TC-064); blokirati PR merge ako pokriće padne ispod 60%; fokus testova na kritične module: autentifikacija, RBAC (Role-Based Access Control), tiketing stanje i automatska dodjela |

---






