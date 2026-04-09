# Domain Model

Domain model predstavlja ključne entitete sistema, njihove najvažnije atribute, veze između njih i osnovna poslovna pravila važna za rad **Telecom Customer Support System**. Cilj domain modela je da prikaže osnovnu strukturu podataka i odnose između glavnih pojmova u okviru helpdesk i ticketing sistema.

U ovom modelu svi korisnici sistema predstavljeni su kroz jedinstveni entitet **Korisnik**, dok se razlika između klijenta, agenta, tehničara i administratora definiše pomoću atributa `uloga`. Na taj način model ostaje pregledan, ali i dovoljno fleksibilan za različite tipove korisnika sistema.

---

## Glavni entiteti

Glavni entiteti sistema su:

- **Korisnik**
- **Tiket**
- **Komentar**
- **PaketPretplata**
- **Izvještaj**
---

## Ključni atributi

### 1. Korisnik

Korisnik predstavlja svaku osobu koja koristi sistem, bilo kao krajnji korisnik usluge ili kao interni korisnik sistema.

**Ključni atributi:**
- `korisnikId`
- `ime`
- `prezime`
- `email`
- `telefon`
- `adresa`
- `korisnickoIme`
- `lozinka`
- `uloga`

**Moguće vrijednosti atributa `uloga`:**
- `klijent`
- `agent`
- `tehnicar`
- `administrator`

Napomena: atributi `korisnickoIme` i `lozinka` posebno su važni za prijavu u sistem, dok atribut `adresa` ima veću važnost kod korisnika koji prijavljuju terenske tehničke probleme.

---

### 2. Tiket

Tiket predstavlja osnovni entitet sistema i opisuje prijavljeni problem, njegov status, prioritet i tok obrade.

**Ključni atributi:**
- `tiketId`
- `naslov`
- `opis`
- `datumKreiranja`
- `datumZatvaranja`
- `status`
- `prioritet`
- `kategorijaProblema`

**Primjeri vrijednosti atributa `status`:**
- `otvoreno`
- `zatvoreno`

**Primjeri vrijednosti atributa `prioritet`:**
- `nizak`
- `srednji`
- `visok`

**Primjeri vrijednosti atributa `kategorijaProblema`:**
- `internet`
- `tv`
- `mobilna mreza`
- `naplata`
- `tehnicka podrska`

---

### 3. Komentar

Komentar predstavlja poruku ili napomenu vezanu za određeni tiket. Komentare mogu ostavljati različiti korisnici sistema, u zavisnosti od svoje uloge.

**Ključni atributi:**
- `komentarId`
- `sadrzaj`
- `datumVrijeme`

---

### 4. PaketPretplata

Ovaj entitet predstavlja usluge koje korisnik koristi kod operatera i koje mogu biti relevantne za obradu tiketa.

**Ključni atributi:**
- `paketId`
- `nazivPaketa`
- `tipPaketa`
- `statusPaketa`

**Primjeri vrijednosti atributa `tipPaketa`:**
- `internet`
- `tv`
- `mobilni paket`
- `kombinovani paket`

**Primjeri vrijednosti atributa `statusPaketa`:**
- `aktivan`
- `neaktivan`

---

### 5. Izvještaj

Izvještaj predstavlja generisani pregled podataka o radu sistema i tiketima.

**Ključni atributi:**
- `izvjestajId`
- `naziv`
- `period`
- `datumGenerisanja`

---

## Veze između entiteta

Veze između glavnih entiteta sistema su sljedeće:

- Jedan **Korisnik** sa ulogom `klijent` može kreirati **više Tiketa**
- Jedan **Tiket** mora biti kreiran od strane tačno **jednog Korisnika**

- Jedan **Korisnik** sa ulogom `agent` može biti dodijeljen na **više Tiketa**
- Jedan **Tiket** može biti dodijeljen **jednom Korisniku** sa odgovarajućom ulogom ili može biti trenutno nedodijeljen

- Jedan **Korisnik** sa ulogom `tehnicar` može biti povezan sa **više Tiketa**
- Jedan **Tiket** može imati dodijeljenog **jednog tehničara** ili trenutno ne mora imati dodijeljenog tehničara

- Jedan **Tiket** može imati **više Komentara**
- Jedan **Komentar** pripada tačno **jednom Tiketu**
- Jedan **Komentar** piše tačno **jedan Korisnik**

- Jedan **Korisnik** sa ulogom `klijent` može imati **više PaketPretplata**
- Jedan **PaketPretplata** pripada tačno **jednom Korisniku**

- Jedan **Izvještaj** se generiše na osnovu podataka iz **više Tiketa**

---

## Poslovna pravila važna za model

Za domain model važna su sljedeća poslovna pravila:

1. Svaki tiket mora imati tačno jednog korisnika koji ga je kreirao.

2. Korisnik koji kreira tiket mora imati ulogu `klijent`.

3. Svaki tiket mora imati definisan status, prioritet i kategoriju problema.

4. Tiket može biti dodijeljen korisniku sa ulogom `agent`, korisniku sa ulogom `tehnicar` ili u početnoj fazi može biti nedodijeljen.

5. Korisnik sa ulogom `klijent` može vidjeti samo vlastite tikete i komentare vezane za njih.

6. Korisnik sa ulogom `agent` može pregledati, obrađivati i ažurirati tikete u skladu sa pravilima sistema.

7. Korisnik sa ulogom `tehnicar` može pregledati i mijenjati status samo onih tiketa koji su mu dodijeljeni.

8. Korisnik sa ulogom `administrator` ima najširi nivo pristupa i može upravljati korisnicima, kategorijama i osnovnim postavkama sistema.

9. Svaki komentar mora biti povezan sa jednim tiketom i jednim korisnikom kao autorom komentara.

10. Tiket se može zatvoriti tek nakon što je problem riješen ili obrađen prema pravilima sistema.

11. Jedan korisnik može imati više paketa ili pretplata, a podaci o tim paketima mogu biti relevantni za obradu tiketa.

12. Izvještaji se generišu na osnovu podataka o tiketima i služe za praćenje rada sistema.

---
