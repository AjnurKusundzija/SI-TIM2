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
| Verifikacija tačnosti autentifikacije i upravljanja sesijama (PB-19) | Login/logout tok za sve korisničke uloge (Klijent, Agent, Tehničar, Administrator) | Svi AC iz US-1, US-2 i US-3 zadovoljeni; neautorizovan pristup zaštićenim rutama je onemogućen |
| Validacija centralnog tiketing toka od kreiranja do zatvaranja tiketa (PB-22, PB-25, PB-36) | Kreiranje tiketa, promjena statusa, zatvaranje uz korisničku potvrdu i automatsko zatvaranje nakon 7 dana | Tiket se kreira u <3 sekunde i zadovoljanava ([NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)); svi definisani statusi rade ispravno i konzistentno |
| Provjera real-time ažuriranja statusa tiketa putem WebSocket komunikacije ([NFR-02](../Sprint2/NonFunctionalRequirements.md#nfr-02)) | WebSocket kanal između klijenta i servera; prikaz promjene statusa u za manje od 1 sekunde bez ručnog refresh-a | Promjena statusa vidljiva svim aktivnim sesijama u za manje od 1 sekunde; mehanizam ponovnog spajanja radi u <3 sekunde ([NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08)) |
| Upravljanje pravima pristupa putem korisničkih uloga za sve četiri korisničke uloge ([NFR-28](../Sprint2/NonFunctionalRequirements.md#nfr-28), [NFR-38](../Sprint2/NonFunctionalRequirements.md#nfr-38)) | Pristup rutama, UI elementima i podacima za uloge: Klijent, Agent, Tehničar, Administrator | Korisnik ne može pristupiti resursima van svoje uloge; tehničar vidi samo minimalne korisničke podatke; lozinke nisu izložene u admin panelu |
| Provjera usklađenosti s GDPR zahtjevima ([NFR-27](../Sprint2/NonFunctionalRequirements.md#nfr-27), [NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35), [NFR-36](../Sprint2/NonFunctionalRequirements.md#nfr-36), [NFR-37](../Sprint2/NonFunctionalRequirements.md#nfr-37)) | Šifriranje podataka u prijenosu (HTTPS/TLS), heširanje lozinki, anonimizacija podataka | Sav saobraćaj odvija se isključivo putem HTTPS/TLS; lozinke se čuvaju u heširanom obliku; anonimizacija podataka ne narušava integritet historije tiketa. |
| Provjera performansi pri normalnom i povećanom opterećenju ([NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01), [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03)) | Sve stranice sistema pri 50 istovremenih korisnika (normalno opterećenje) i 100 istovremenih korisnika (maksimalno opeterećenje) | Sve stranice učitane za manje od 2 sekunde pri 50 korisnika; nema degradacije u pefromansama više od 50% pri 100 korisnika |
| Validacija korisničkog iskustva i pristupačnosti za stariju populaciju ([NFR-09](../Sprint2/NonFunctionalRequirements.md#nfr-09), [NFR-10](../Sprint2/NonFunctionalRequirements.md#nfr-10), [NFR-12](../Sprint2/NonFunctionalRequirements.md#nfr-12)) | Forma za prijavu kvara, dashboard prikaz i error poruke | Novi agent uspješno obrađuje tiket bez obuke za manje od 20 minuta; kontrast ratio ≥4.5:1; font ≥14px; max 3 koraka za prijavu kvara |
| Provjera ispravnosti modula za izvještavanje i admin dashboarda (PB-38 do PB-45) | Izvještaji po broju tiketa, statusu, tipu problema, prosječnom vremenu rješavanja, opterećenju agenata | Podaci u izvještajima odgovaraju stvarnom stanju u bazi; pristup ograničen po ulozi; nema duplikata ni netačnih vrijednosti |
| Validacija pokrivenosti unit testovima poslovne logike ([NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25)) | Backend moduli: autentifikacija,prava pristupa putem korisničkih uloga, tiketing tok, automatska dodjela, WebSocket logika | Minimalno 60% pokrivenosti unit testovima ključnih backend servisa |

---

## Nivoi testiranja

| Nivo testiranja | Fokus | Odgovorni | Izlazni kriterij |
| --- | --- | --- | --- |
| Unit testiranje | Validacija pojedinačnih funkcija i komponenti backend logike: autentifikacijski servis (bcrypt hash verifikacija, JWT generisanje/validacija), prava pristupa putem korisničkih uloga (provjera dozvola po ulozi), tiketing servis (kreiranje tiketa, generisanje UUID-a, promjena statusa, validacija prijelaza iz jednog u drugo stanje), logika automatske dodjele tiketa prema pravilima (prioritet), izračunavanje prosječnog vremena rješavanja tiketa | Dev tim | Minimalno 60% pokrivenosti ([NFR-25](../Sprint2/NonFunctionalRequirements.md#nfr-25)); sve jedinične provjere prolaze bez grešaka u CI pipeline-u ([NFR-26](../Sprint2/NonFunctionalRequirements.md#nfr-26)); nema failing testova u main grani |
| Integracijsko testiranje | Provjera kako ključni dijelovi sistema rade zajedno: API endpointi i baza podataka (kreiranje, čitanje i ažuriranje tiketa), WebSocket server i klijentska aplikacija (real-time prenos promjena statusa), autentifikacijski servis, kontrola pristupa po korisničkim ulogama i zaštićene rute, ORM sloj na PostgreSQL i MySQL ([NFR-14](../Sprint2/NonFunctionalRequirements.md#nfr-14)), te razmjena podataka između backenda i frontenda (prikaz, filtriranje i pretraga tiketa) | Dev + QA | Sve kritične integracije prolaze bez blokera; WebSocket konekcija se uspješno uspostavlja i prenosi promjene statusa; ORM radi na oba DB sistema bez izmjene koda |
| Sistemsko testiranje | Sveobuhvatna provjera end-to-end tokova od prijave do zatvaranja tiketa za svaku korisničku ulogu: za Klijenta obuhvata registraciju, prijavu, kreiranje tiketa, praćenje statusa, komunikaciju, zatvaranje i ocjenu; za Agenta obuhvata prijavu, pregled tiketa, promjenu prioriteta, komunikaciju, prosljeđivanje i zatvaranje; za Tehničara obuhvata prijavu, pregled dodijeljenih tiketa, ažuriranje statusa s terena i zatvaranje; za Administratora obuhvata prijavu, upravljanje korisnicima, preraspodjelu agenata, pregled dashboarda i generisanje izvještaja. Uključuje i testiranje performansi ([NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01), [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03), [NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)), scenarije ponovnog povezivanja WebSocket veze ([NFR-08](../Sprint2/NonFunctionalRequirements.md#nfr-08)), provjeru HTTPS/TLS zaštite ([NFR-35](../Sprint2/NonFunctionalRequirements.md#nfr-35)) i testiranje na različitim web-preglednicimaV Chrome, Firefox i Edge ([NFR-13](../Sprint2/NonFunctionalRequirements.md#nfr-13)) | QA tim | Svi ključni poslovni tokovi prolaze bez blokatora; performansni zahtjevi su zadovoljeni u mjerljivim granicama ([NFR-01](../Sprint2/NonFunctionalRequirements.md#nfr-01), [NFR-03](../Sprint2/NonFunctionalRequirements.md#nfr-03), [NFR-04](../Sprint2/NonFunctionalRequirements.md#nfr-04)); sistem je dostupan na sva tri navedena preglednika |
| Testiranje prihvatljivosti | Potvrda usklađenosti sa poslovnim zahtjevima u realnim uvjetima: Product Owner verificira AC iz US priča visokog prioriteta (1); Agenti i Tehničari testiraju intuitivnost interfejsa ([NFR-09](../Sprint2/NonFunctionalRequirements.md#nfr-09) — uspješno obavljanje zadatka bez obuke u <20 min); Administrator provjerava upravljanje korisnicima i izvještaje; BH Telecom (naručilac) potvrđuje ukupnu isporuku u skladu sa dogovorenim MVP scope-om; provjera pristupačnosti forme za prijavu kvara za stariju populaciju ([NFR-12](../Sprint2/NonFunctionalRequirements.md#nfr-12)) | Product Owner + predstavnici BH Telekoma + QA | Svi kriteriji prihvatanja iz US prioriteta 1 su zadovoljeni; Product Owner potpisuje sprint review; nema otvorenih blokera i defekata |

---

## Šta se testira u kojem nivou

| Funkcionalnost | Unit | Integracijsko | Sistemsko | Prihvatno |
| --- | --- | --- | --- | --- |
| [] | DA/NE | DA/NE | DA/NE | DA/NE |
| [] | DA/NE | DA/NE | DA/NE | DA/NE |

## Veza sa acceptance kriterijima

| Korisnička priča | AC kriterij | ID testnog slučaja | Status |
| --- | --- | --- | --- |
| [US-01] | [AC-1] | [TC-001] | [PASS/FAIL/BLOCKED] |
| [] | [] | [] | [] |

## Način evidentiranja rezultata testiranja

| Datum | ID testnog slučaja | Scenarij | Rezultat | ID defekta | Napomena |
| --- | --- | --- | --- | --- | --- |
| [YYYY-MM-DD] | [TC-001] | [Kratak opis scenarija] | [PASS/FAIL/BLOCKED] | [dodati id osmisliti notaciju] | [Komentar] |

## Glavni rizici kvaliteta

| Rizik | Utjecaj | Vjerovatnoća | Mitigacija |
| --- | --- | --- | --- |
| [] | [Nizak/Srednji/Visok] | [Nizak/Srednji/Visok] | [Plan ublažavanja rizika] |

