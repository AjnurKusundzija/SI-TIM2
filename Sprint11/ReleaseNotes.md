# Release Notes

## Finalna verzija

Finalna verzija dokumentovano predstavlja Helpdesk i Ticketing sistem za telekom okruženje. Sistem je razvijan kroz Scrum sprintove i uključuje web aplikaciju dostupnu preko primarnog URL-a `http://46.224.179.251/` i rezervnog URL-a `https://telecomsupport.hodzicmirza.com/`.

## Šta je uključeno u finalnu verziju

Prema finalnom izvještaju i ažuriranom Product Backlogu, isporučeno je:

- autentifikacija i autorizacija korisnika,
- role-based pristup za klijente, agente, tehničare i administratore,
- kreiranje tiketa,
- pregled vlastitih tiketa,
- pregled svih tiketa za agente i administratore,
- detaljan prikaz tiketa,
- komunikacija kroz tiket,
- interni komentari za osoblje,
- dodjela, automatska dodjela i prosljeđivanje tiketa,
- upravljanje korisničkim i internim prioritetima,
- promjena statusa i zatvaranje tiketa,
- ocjenjivanje zatvorenih tiketa,
- FAQ modul sa administratorskim CRUD funkcionalnostima,
- korisnički profili i promjena emaila/lozinke,
- pregled paketa i pretplata,
- upravljanje katalogom paketa i pretplatama,
- upravljanje korisničkim nalozima,
- deaktivacija i reaktivacija korisnika,
- upravljanje timovima i preraspodjela agenata,
- availability status agenata,
- pregled dodijeljenih tiketa za tehničare,
- osnovne informacije o korisniku za tehničare,
- notifikacije putem SignalR-a,
- upload i preuzimanje priloga na tiketima,
- audit log aktivnosti,
- admin dashboard sa metrikama,
- administrativni izvještaji,
- CSV export izvještaja,
- AI prijedlog odgovora za agente i tehničare,
- AI Insights za administratore,
- MCP Admin Copilot,
- SLA praćenje i upozorenja za agente i administratore,
- login putem broja telefona (+387 format) kao alternativa email prijavi,
- redizajnirani korisnički interfejs.

## Najvažnije funkcionalnosti

Najvažnije dokumentovane funkcionalnosti finalne verzije su kompletan životni ciklus tiketa, komunikacija korisnika i podrške, administracija korisnika/timova/paketa, izvještavanje, audit log, notifikacije, prilozi, AI pomoć za osoblje i administratore, te MCP Admin Copilot za administratorska pitanja nad živim podacima sistema.

## Planirano, ali nije završeno

Prema Sprint 11 finalnom izvještaju i `FinalProductBacklog.md`, sljedeće stavke su odgođene i nisu završene:

- PB-64 Linked Tickets (Deferred),
- PB-66 Bulk akcije na tiketima (Deferred).

Finalni izvještaj dodatno navodi da nisu realizovani:

- mobilna aplikacija,
- CRM integracije,
- napredni BI dashboard,
- automatska AI preraspodjela tiketa.

## Poznata ograničenja

- Produkcijski deployment koristi `docker compose down -v`, što briše Docker volumen i resetuje produkcijsku bazu pri svakom deploymentu na `main`.
- Produkcijski `docker-compose.yml` i `.env` na serveru nisu dio repozitorija i moraju se ručno konfigurirati.
- Deployment zavisi od dostupnosti Docker Hub servisa.
- CD pipeline nema eksplicitni health check nakon pokretanja aplikacije.
- PB-52, upravljanje katalogom paketa i pretplata, dokumentovano je verifikovan manualno kroz UI, bez automatizovanih xUnit/Vitest testova u Sprintu 9.
- Za izvještaje PB-38, PB-39, PB-40 i PB-41 tehničarski dio user storyja je označen kao out of scope po dogovoru.
- Sistem koristi testne seed podatke u development okruženju; dokumentacija navodi da su lozinke namijenjene isključivo za testiranje.

## Poznati bugovi

- Dokumentovano je da produkcijski deployment resetuje bazu zbog brisanja Docker volumena.
- Dokumentovano je da backend može biti resetovan ako SQL Server ne bude spreman ni nakon retry mehanizma.

## Šta nije dio finalne isporuke

Finalna isporuka ne treba biti predstavljena kao da uključuje Linked Tickets, bulk akcije nad tiketima, mobilnu aplikaciju, CRM integracije, napredni BI dashboard ili automatsku AI preraspodjelu tiketa.
