# Sprint Review Summary

## Sprint broj
- Sprint 5

## Planirani sprint goal
- Započeti AI-enabled fazu projekta kroz isporuku prvog funkcionalnog inkrementa, uz jasno vođenje evidencije odluka i korištenja AI alata kako bi implementacioni tok bio transparentan i provjerljiv.

## Sta je zavrseno
- **PB-17 – AI Usage Log** (Ajnur Kusundzija): Dokument kreiran i evidentiran prvi usage zapis.
- **PB-18 – Decision Log** (Ajnur Kusundzija): Decision log uspostavljen i spreman za dalje odluke.
- **PB-19 – Login korisnika** (Uma, Hana, Lamija): Prijava korisnika, validacija unosa i kontrola pristupa — backend + frontend isporučeno.
- **Unit testovi za autentifikaciju, kreiranje i prikaz tiketa** (Lejan, Uma): 51 unit test (backend) i 48 frontend Vitest testova.
- **PB-22 – Kreiranje novog tiketa** (Hana, Lamija): Forma za kreiranje tiketa, validacija obaveznih polja i spremanje tiketa — backend + frontend isporučeno.
- **PB-23 – Pregled vlastitih tiketa** (Eldar, Merisa): Lista vlastitih tiketa sa statusom i filtriranjem — backend + frontend isporučeno.
- **PB-33 – Pretraživanje i filtriranje tiketa**: Implementirano pretraživanje i filtriranje tiketa prema različitim kriterijima — backend + frontend isporučeno.
- **Koordinacija sprint dokumentacije i artefakata** (Ajnur Kusundzija): Sprint Goal, Sprint Backlog, AI Usage Log, Decision Log i Test Strategy usklađeni.

## Sta nije zavrseno
- Sve planirane stavke su završene. Nijedna stavka nije prenesena u sljedeći sprint.

## Demonstrirane funkcionalnosti ili artefakti
- **Login korisnika** – funkcionalan login sa validacijom unosa i kontrolom pristupa (backend + frontend).
- **Kreiranje novog tiketa** – forma sa validacijom i pohranom tiketa u sistem (backend + frontend).
- **Pregled vlastitih tiketa** – lista tiketa sa statusima i filtriranjem (backend + frontend).
- **Pretraživanje i filtriranje tiketa** – pretraga i filtriranje tiketa po različitim kriterijima (backend + frontend).
- **Testna pokrivenost** – 51 unit test za backend i 48 Vitest testova za frontend.
- **AI Usage Log** – evidencija korištenja AI alata tokom sprinta.
- **Decision Log** – dokumentirane ključne odluke (ODL-1 i ODL-2).

## Glavni problemi i blokeri
- Tokom sprinta došlo je do potrebe za prilagodbom scope-a (ODL-2): detaljan prikaz tiketa je uklonjen iz Sprint 5 backloga i prebačen u Sprint 6, a umjesto toga su dodani PB-22 i PB-23 kao prioritetniji zadaci.
- Konfiguracija HTTPS protokola unutar Docker okruženja (self-signed certifikati) zahtijevala je dodatno usklađivanje backend i frontend konfiguracija.

## Kljucne odluke donesene u sprintu
- **ODL-1 (26.04.2026)** – Odabran HTTPS protokol za komunikaciju između frontend i backend kontejnera radi sigurnijeg deploymentа i usklađenosti sa produkcijskim okruženjem.
- **ODL-2 (27.04.2026)** – Sprint 5 backlog usklađen: dodani PB-22 i PB-23 (kreiranje i pregled tiketa), a detaljan prikaz tiketa (PB-24) premješten u Sprint 6.

## Povratna informacija Product Ownera
- Ažurni projektni dokumenti moraju se nalaziti na `main` branchu kako bi svi članovi tima imali pristup najnovijim verzijama artefakata i sprint dokumentacije.
- Sprint Backlog treba sadržavati detaljan prikaz svakog User Story-ja zajedno sa pripadajućim acceptance kriterijima radi jasnijeg praćenja implementacije i validacije funkcionalnosti.

## Zakljucak za naredni sprint
- Sprint 6 će se fokusirati na PB-24 (Detaljan prikaz tiketa), PB-47 (FAQ segment) i PB-27(Komunikacija kroz tiket), kao i dalje unapređenje isporučenih funkcionalnosti prema povratnoj informaciji PO-a.

Ovaj dokument se piše tek nakon sastanka sa PO.