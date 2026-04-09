# Strategija testiranja

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
| Unit testiranje | Validacija pojedinačnih funkcija/komponenti | Dev tim | Sve jedinične provjere prolaze |
| Integracijsko testiranje | Provjera komunikacije između modula/servisa | Dev + QA | Kritične integracije bez blokera |
| Sistemsko testiranje | Cjelovita provjera kompletnog sistema (od početka do kraja) | QA tim | Ključni poslovni tokovi prolaze |
| Prihvatno testiranje | Potvrda usklađenosti sa poslovnim zahtjevima | Vlasnik proizvoda/interesni akteri + QA | Kriteriji prihvatanja zadovoljeni |

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

