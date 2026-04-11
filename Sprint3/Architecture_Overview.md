# Architecture Overview

## 1. Arhitektonski pristup

Sistem korisničke podrške telekomunikacijske kompanije gradi se kao web aplikacija sa jasno odvojenim frontend i backend slojevima, oslanjajući se na modernu klijent-server arhitekturu s komponentama koje komuniciraju putem REST API-ja i WebSocket protokola.

Odabrani pristup je **troslojna (three-tier) arhitektura**:

- **Prezentacijski sloj (Frontend):** React-bazirana SPA aplikacija koja komunicira isključivo putem API-ja s backendom.
- **Aplikacijski sloj (Backend):** RESTful API server zadužen za poslovnu logiku, autentifikaciju, autorizaciju i upravljanje podacima.
- **Sloj podataka (Database):** Relaciona baza podataka za perzistenciju svih poslovnih entiteta — korisnika, tiketa, poruka, logova.

Ovaj pristup osigurava jasnu razdvojenost odgovornosti (separation of concerns), olakšava nezavisno testiranje svake komponente i omogućava horizontalno skaliranje pojedinih slojeva bez narušavanja ostatka sistema.

Sistem podržava četiri korisničke uloge: **Klijent, Agent, Tehničar i Administrator**, pri čemu se pristup podacima i funkcionalnostima kontroliše putem RBAC mehanizma (Role-Based Access Control).

---

## 2. Glavne komponente sistema

| Komponenta | Tip | Opis |
|---|---|---|
| Frontend SPA | Prezentacijski sloj | React aplikacija – korisnički interfejs za sve uloge |
| Auth modul | Backend | Prijava, odjava, upravljanje sesijama i JWT tokenima |
| Ticket modul | Backend | Kreiranje, pregled, ažuriranje i zatvaranje tiketa |
| Notification servis | Backend / Real-time | WebSocket server za real-time obavještenja |
| User Management | Backend | Upravljanje korisničkim profilima i ulogama |
| Reporting modul | Backend | Generisanje izvještaja i agregatnih metrika |
| Admin Dashboard | Frontend + Backend | Pregled ključnih metrika za administratora |
| Baza podataka | Sloj podataka | Relaciona baza za čuvanje svih poslovnih podataka |
| RBAC engine | Backend | Kontrola pristupa na osnovu korisničkih uloga |
| Audit log servis | Backend | Evidentiranje svih promjena statusa i akcija korisnika |

---

## 3. Odgovornosti komponenti

### 3.1 Frontend SPA (React)

Korisnički interfejs dostupan putem web browsera (Chrome, Firefox, Edge). Prikazuje relevantne ekrane na osnovu uloge prijavljenog korisnika, tako da se klijentu prikazuju njegovi tiketi, agentu lista svih tiketa, tehničaru samo dodijeljeni tiketi, a adminu dashboard s metrikama. Komunicira s backendom isključivo putem REST API poziva i WebSocket veze za real-time ažuriranja.

### 3.2 Auth modul

Zadužen za autentifikaciju korisnika putem emaila i lozinke. Lozinke se čuvaju kao bcrypt ili Argon2 hash (nikada u plain text obliku). Nakon uspješne prijave, korisniku se izdaje JWT token koji se koristi za autorizaciju svih narednih zahtjeva. Implementira sigurnosne zahtjeve OWASP-a: generičke poruke greške bez otkrivanja detalja, blokiranje slabih lozinki, i sl.

### 3.3 Ticket modul

Centralna poslovna logika sistema. Klijenti kreiraju tikete s opisom problema, tipom kvara i prioritetom. Agenti upravljaju tiketima: mijenjaju status, postavljaju interni prioritet, prosljeđuju tiket drugom agentu ili tehničaru, te komunikaciju s klijentom vode unutar samog tiketa. Svaka promjena statusa zapisuje se u audit log.

### 3.4 Notification servis (WebSocket)

Real-time komponenta sistema bazirana na WebSocket protokolu. Osigurava da klijent vidi promjenu statusa tiketa u roku od 1 sekunde bez potrebe za ručnim osvježavanjem stranice. U slučaju prekida veze, sistem pokušava reconnect za manje od 3 sekunde, s maksimalno 5 pokušaja. Notifikacija o prekidu stiže za manje od 1 sekunde.

### 3.5 User Management

Upravljanje korisničkim profilima za sve uloge. Administrator može pretražiti korisnika po emailu, pregledati i urediti profil bez prikaza lozinke, te upravljati raspodjelom agenata po timovima. Implementira minimizaciju podataka, što znači da svaka uloga vidi samo podatke neophodne za njen zadatak (NFR-38).

### 3.6 Reporting modul

Generiše statičke i agregatne izvještaje za adminsku ulogu: ukupan broj tiketa po periodu, prosječno vrijeme rješavanja, vrijeme prvog odgovora, opterećenje agenata, ocjene korisnika, te distribucija po tipu problema i statusu. Klijent nema pristup `/reports/*` rutama (403 Forbidden).

### 3.7 Admin Dashboard

Vizuelni prikaz ključnih metrika u realnom vremenu: ukupan broj tiketa, distribucija po statusima, prosječno vrijeme rješavanja. Dashboard se ažurira putem WebSocket veze ili polling mehanizma. Podatke crpi iz Reporting modula i Ticket modula.

### 3.8 Relaciona baza podataka

Čuva sve poslovne entitete: korisnike, tikete, poruke unutar tiketa, historiju statusa, audit logove i ocjene. Nakon anonimizacije korisnika, PII (Personally Identifiable Information) podaci su uklonjeni, ali historija tiketa ostaje netaknuta. Baza mora podržavati istovremeni pristup 100+ korisnika.

### 3.9 RBAC engine

Kontrola pristupa bazirana na ulogama: Klijent, Agent, Tehničar, Administrator. Svaka uloga ima tačno definisan skup dostupnih ruta i operacija. Backend provjerava ulogu korisnika pri svakom zahtjevu i odbija neovlašteni pristup s odgovarajućim HTTP statusom (403 Forbidden).

### 3.10 Audit log servis

Zapisuje sve relevantne poslovne akcije: promjene statusa tiketa, prosljeđivanja, zatvaranja, promjene prioriteta. Svaki log zapis sadrži timestamp, ID korisnika koji je izvršio akciju i opis promjene. Audit log nije dostupan klijentima ni tehničarima.

---

## 4. Tok podataka i interakcija

### 4.1 Standardni HTTP tok (REST)

1. Korisnik šalje zahtjev putem Frontend SPA (npr. kreiranje tiketa).
2. Frontend šalje HTTP `POST /tickets` zahtjev s JWT tokenom u `Authorization` headeru.
3. Backend Auth middleware verificira token i RBAC engine provjerava uloge.
4. Ticket modul obrađuje zahtjev, upisuje podatke u bazu i vraća HTTP 201 odgovor za manje od 3 sekunde.
5. Frontend prikazuje potvrdu s generisanim ID-om tiketa.

### 4.2 Real-time tok (WebSocket)

1. Agent mijenja status tiketa putem REST API-ja.
2. Backend Ticket modul upisuje promjenu u bazu i emituje event putem Notification servisa.
3. WebSocket server isporučuje notifikaciju klijentu u roku od 1 sekunde.
4. Klijentska SPA ažurira UI bez potrebe za refreshom stranice.

### 4.3 Autentifikacijski tok

1. Korisnik unosi email i lozinku, Frontend šalje `POST /auth/login`.
2. Backend provjerava hash lozinke (bcrypt/Argon2); pri neuspjehu vraća generičku poruku.
3. Pri uspjehu, backend izdaje JWT token koji Frontend čuva u memoriji ili HttpOnly kolačiću.
4. Sve naredne sesije prenose JWT token u headeru; middleware ga verificira pri svakom zahtjevu.

### 4.4 Komunikacijski kanali

| Komunikacijski kanal | Namjena | Protokol |
|---|---|---|
| Frontend ↔ Backend | CRUD operacije, autentifikacija, izvještaji | HTTPS / REST API |
| Backend → Frontend (push) | Real-time notifikacije, ažuriranja statusa | WebSocket (WSS) |
| Backend ↔ Baza | Čitanje i pisanje poslovnih podataka | SQL (TCP) |
| Backend ↔ Audit log | Evidentiranje akcija | Interni servisni poziv |

---

## 5. Ključne tehničke odluke

| Odluka | Razlog |
|---|---|
| REST API + WebSocket hibridni model | REST za CRUD operacije (jednostavnost, cachiranje), WebSocket za real-time notifikacije (NFR-02: <1 sek ažuriranje) |
| JWT autentifikacija (stateless) | Eliminacija server-side sesija, lakše horizontalno skaliranje, kompatibilnost s API pristupom |
| RBAC na backend sloju | Sigurnost ne smije ovisiti o frontendu; svaka uloga ograničena na minimum potrebnih podataka (NFR-28, NFR-38) |
| bcrypt/Argon2 hashiranje lozinki | Industrijski standard za sigurno čuvanje lozinki; otpornost na brute-force napade (NFR-36) |
| HTTPS/TLS 1.2+ obavezan | Sva komunikacija između klijenta i servera enkriptira se putem TLS 1.2+. Pristup putem HTTP-a automatski se preusmjerava na HTTPS, a učitavanje bilo kakvog resursa bez enkripcije nije dozvoljeno (tzv. mixed content) (NFR-35) |
| SPA arhitektura na frontendu | Bolje korisničko iskustvo (bez full page reload), olakšana real-time integracija, lakše upravljanje stanjem po ulogama |
| Anonimizacija PII podataka | GDPR/compliance zahtjevi — PII se brišu pri anonimizaciji, historija tiketa ostaje radi integriteta podataka (NFR-37) |
| Test pokrivenost ≥ 60% (backend) | Minimum koji osigurava pouzdanost ključne poslovne logike; mjereno CI/CD coverage reportom (NFR-25) |

---

## 6. Ograničenja i rizici arhitekture

| Rizik / Ograničenje | Utjecaj | Vjerovatnoća | Mitigacija |
|---|---|---|---|
| WebSocket skalabilnost pri velikom broju istovremenih veza | Visok | Srednja | Uvođenje message broker-a (npr. Redis Pub/Sub) za horizontalno skaliranje WebSocket servera |
| Single point of failure — monolitni backend | Visok | Srednja | Modularni dizajn s jasnim granicama između modula; priprema za potencijalni raspad na mikroservise |
| JWT token invalidacija (logout problem) | Srednji | Visoka | Implementacija token blocklist mehanizma ili kratki TTL tokeni s refresh token rotacijom |
| Degradacija performansi pri 100+ korisnika | Visok | Srednja | Load testing s k6 (NFR-03), connection pooling na bazi, indeksiranje ključnih kolona |
| Dupliciranje tiketa pri mrežnom prekidu | Srednji | Niska | Idempotentni POST zahtjevi s jedinstvenim client-generated ID-om (NFR-07) |
| Neusklađenost RBAC pravila između frontend i backend sloja | Visok | Srednja | Backend je jedini autoritativni izvor; frontend samo skriva UI elemente, ne štiti podatke |
| Nepotpuna test pokrivenost (ispod 60%) | Srednji | Srednja | Obavezni coverage check u CI pipeline-u; PR merge blokiran ispod praga |
| Ovisnost o jednoj bazi podataka (single DB) | Visok | Niska | Redovni backup, read replica za izvještaje, monitoring dostupnosti |

---

## 7. Otvorena pitanja

| # | Pitanje | Prioritet | Komentar |
|---|---|---|---|
| 1 | Da li koristiti refresh token + access token, ili samo JWT s kratkim TTL-om? | Visok | Direktno utiče na sigurnost i korisničko iskustvo (logout flow) |
| 2 | Koji WebSocket framework koristiti (Socket.io, native WS, SSE kao alternativa)? | Srednji | Socket.io nudi automatski fallback; native WS je lakši ali zahtijeva više ručnog rada |
| 3 | Koji cloud/hosting provider i da li koristiti kontejnerizaciju (Docker)? | Srednji | Utiče na deployment strategiju i horizontalno skaliranje |
| 4 | Da li FAQ segment koristiti statičan sadržaj ili CMS (Content Management System)? | Nizak | PB-47, prioritet 3 — može biti statički Markdown u prvoj verziji |
| 5 | Da li export izvještaja (PB-46) podržavati u PDF, Excel ili oba formata? | Nizak | Prioritet 5 — dovoljno razjasniti u kasnijim sprintovima (11+) |

---

*Dokument se ažurira kontinuirano tokom razvoja projekta.*
