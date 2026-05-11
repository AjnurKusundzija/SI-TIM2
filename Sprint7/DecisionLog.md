# Decision Log - UPDATE ZA SPRINT 7

Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka.

Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.

---

## Odluka #1

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-1 |
| **Datum** | 26.04.2026 |
| **Kratak naziv odluke** | Korištenje HTTPS protokola za komunikaciju između frontend i backend kontejnera |
| **Opis problema ili pitanja** | Potrebno je definisati protokol za komunikaciju između kontejnera. Iako su unutar iste mreže, postavlja se pitanje da li koristiti HTTP ili HTTPS. |
| **Razmatrane opcije** | 1. HTTP – jednostavnija konfiguracija, manji overhead. <br> 2. HTTPS – veća sigurnost, usklađenost sa produkcijom. |
| **Odabrana opcija** | 2. HTTPS |
| **Razlog izbora** | Primarni razlog je **lakši i sigurniji deployment**. Većina modernih cloud servisa i ingress kontrolera (poput Nginx ili Traefik-a) zahtijeva SSL/TLS za ispravno rukovanje sesijama i kolačićima. Korištenjem HTTPS-a u razvojnoj fazi eliminišemo "Mixed Content" greške i osiguravamo da se aplikacija ponaša identično u lokalnom i produkcijskom okruženju. |
| **Posljedice odluke** | Potrebno je konfigurisati certifikate unutar Docker okruženja (self-signed) i ažurirati backend/frontend konfiguracije. |
| **Status odluke** | aktivna |

---

## Odluka #2

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-2 |
| **Datum** | 27.04.2026 |
| **Kratak naziv odluke** | Uskladjivanje Sprint 5 backloga sa PB-22 i PB-23 |
| **Opis problema ili pitanja** | Sprint backlog je trebalo uskladiti sa trenutnim prioritetima tako da ukljuci PB-22 i PB-23, a da se iz sprint scope-a ukloni detaljan prikaz tiketa. |
| **Razmatrane opcije** | 1. Zadrzati detaljan prikaz tiketa u Sprint 5 backlogu  2. Dodati PB-22 i PB-23 u Sprint 5 backlog i ukloniti detaljan prikaz tiketa  3. Odgoditi promjenu sprint backloga za naredni sprint |
| **Odabrana opcija** | Dodati PB-22 i PB-23 u Sprint 5 backlog i ukloniti detaljan prikaz tiketa |
| **Razlog izbora** | PB-22 i PB-23 predstavljaju osnovni tok rada sa tiketima nakon login funkcionalnosti, dok detaljan prikaz tiketa nije dio trenutnog fokusa sprinta. |
| **Posljedice odluke** | Sprint 5 backlog sada obuhvata kreiranje novog tiketa i pregled vlastitih tiketa, dok se detaljan prikaz tiketa ne realizuje u ovom sprintu. |
| **Status odluke** | aktivna |

---
## Odluka #3

| Polje | Detalji |
|---|---|
| ID odluke | ODL-3 |
| Datum | 03.05.2026 |
| Kratak naziv odluke | Korištenje SignalR-a za real-time komunikaciju |
| Opis problema ili pitanja | Potrebno je omogućiti real-time komunikaciju između klijenta i agenta unutar detaljnog prikaza tiketa bez ručnog osvježavanja stranice. |
| Razmatrane opcije | 1. Klasični HTTP polling<br>2. WebSocket komunikacija korištenjem SignalR-a |
| Odabrana opcija | 2. WebSocket komunikacija korištenjem SignalR-a |
| Razlog izbora | SignalR omogućava trenutni prijenos poruka i ažuriranje prikaza bez refreshanja stranice. Integracija sa ASP.NET backendom je jednostavna i pogodna za ticketing sistem. |
| Posljedice odluke | Potrebno je održavati SignalR hub konekcije i sinhronizaciju komunikacije između korisnika i agenata. Ako je tiket zatvoren, komunikacija više nije dozvoljena. Maksimalna dužina poruke je 1000 karaktera, a neprikladne riječi se filtriraju i zamjenjuju sa ****. |
| Status odluke | aktivna |

---

## Odluka #4

| Polje | Detalji |
|---|---|
| ID odluke | ODL-4 |
| Datum | 08.05.2026 |
| Kratak naziv odluke | Refaktorisanje ticket workflow logike i proširenje agent/tehničar funkcionalnosti |
| Opis problema ili pitanja | Tokom planiranja Sprinta 7 identificirana je potreba za reorganizacijom ticket workflow logike kako bi sistem podržao automatsku dodjelu tiketa, upravljanje prioritetima, pregled dodijeljenih tiketa za tehničare i proširene funkcionalnosti notifikacija i statusa tiketa. Također je bilo potrebno redefinisati određene user story zahtjeve i proširiti postojeće backlog stavke. |
| Razmatrane opcije | 1. Zadržati postojeću logiku ručne dodjele tiketa i postojeće user story zahtjeve<br>2. Refaktorisati workflow i dio logike prebaciti isključivo na backend uz proširenje PB i US funkcionalnosti |
| Odabrana opcija | 2. Refaktorisati workflow i dio logike prebaciti isključivo na backend uz proširenje PB i US funkcionalnosti |
| Razlog izbora | Postojeća struktura nije bila dovoljno fleksibilna za automatsku dodjelu tiketa, routing po prioritetima i proširenje tehničarskog workflow-a. Centralizacija logike na backend omogućava sigurniju i konzistentniju obradu dodjele tiketa, lakšu integraciju notifikacija i bolju skalabilnost sistema. |
| Posljedice odluke | US-26 se uklanja iz trenutnog oblika i njegova logika se provodi isključivo kroz backend sistem. PB-37 dobija novi user story US-52 koji omogućava tehničaru pregled dodatnih informacija o dodijeljenim tiketima, uključujući prioritet, status, kategoriju problema i osnovne podatke o korisniku. Dodan je novi PB-48 sa user story zahtjevima US-53 i US-54. US-53 omogućava agentima prosljeđivanje tiketa između timova ili tehničara, dok US-54 omogućava pregled historije prosljeđivanja i promjena statusa tiketa. Sprint 7 fokus prebacuje se na automatsku dodjelu tiketa, upravljanje prioritetima, pregled dodijeljenih tiketa za tehničare, ažuriranje statusa i zatvaranje tiketa. Planirano je i dalje proširenje user story zahtjeva u narednim iteracijama. |
| Status odluke | aktivna |

---

Napomena: Ovaj Decision Log je živi dokument i ažurira se kroz sprintove.
