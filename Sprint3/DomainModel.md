# Domain Model

## 1. Uvod

Ovaj dokument opisuje domenski model sistema **Telecom Customer Support System**. Sistem je namijenjen upravljanju korisničkom podrškom u telekomunikacijskom okruženju, sa fokusom na prijavu problema, obradu i zatvaranje tiketa, komunikaciju između korisnika i operativnog osoblja, pregled paketa koje korisnik koristi, organizaciju zaposlenih u timove, te evidentiranje ocjene kvaliteta rješenja.

Domenski model prikazuje ključne entitete sistema, njihove najvažnije atribute, veze između entiteta i osnovna poslovna pravila. Cilj modela nije prikazati tehničku implementaciju, nego poslovne pojmove koji su važni za razumijevanje sistema.

U sistemu svi učesnici su predstavljeni kroz centralni entitet **Korisnik**, dok se njihova funkcionalna razlika određuje pomoću atributa `uloga`. Sistem podržava više tipova korisnika, prije svega:
- klijent
- agent
- tehničar
- administrator

Tiket kreira krajnji klijent, dok sistem podržava osnovne notifikacije vezane za dodjelu i promjenu stanja tiketa. Zbog toga su i entiteti **Notifikacija** i **DodjelaTiketa** uključeni u model.

Pored osnovnog toka rada sa tiketima, model uključuje i sljedeće:
- pakete i njihove karakteristike
- timove zaposlenih
- dodjelu tiketa članovima timova
- ocjenu rješenja i komentar uz ocjenu
- osnovno izvještavanje nad radom sistema

---

## 2. Glavni entiteti

Sistem se zasniva na sljedećim glavnim entitetima:

- **Korisnik**
- **Tim**
- **Tiket**
- **Komentar**
- **Ocjena**
- **PaketPretplata**
- **KarakteristikaPaketa**
- **DodjelaTiketa**
- **Izvještaj**
- **Notifikacija**

Ovi entiteti pokrivaju funkcionalnosti sistema, uključujući:
- autentifikaciju i upravljanje korisnicima
- organizaciju agenata i tehničara u timove
- prijavu i obradu problema kroz tikete
- komunikaciju unutar tiketa
- prikaz korisničkih paketa i njihovih karakteristika
- evidenciju ocjene korisničkog iskustva
- osnovno izvještavanje i obavještavanje

---

## 3. Ključni atributi po entitetima

### 3.1 Korisnik

Korisnik predstavlja svaku osobu koja koristi sistem, bilo kao krajnji korisnik usluge ili kao interni korisnik sistema.

**Ključni atributi:**
- `korisnikId`
- `ime`
- `prezime`
- `email`
- `telefon`
- `adresa`
- `korisnickoIme`
- `lozinkaHash`
- `statusNaloga`
- `uloga`
- `statusRaspolozivosti`

**Napomena:** Entitet **Korisnik** je centralni entitet sistema. Uloga određuje koje funkcionalnosti korisnik može koristiti. Atribut `statusRaspolozivosti` posebno je važan za korisnike sa ulogom agenta i tehničara, jer sistem na osnovu njega može odlučiti da li korisnik može preuzeti novi tiket.

---

### 3.2 Tim

Tim predstavlja organizacionu cjelinu unutar operativnog dijela sistema. Tim okuplja agente i/ili tehničare koji rade na sličnoj vrsti problema.

**Ključni atributi:**
- `timId`
- `nazivTima`
- `opis`
- `tipTima`
- `statusTima`

**Napomena:** Timovi su važni zbog organizacije rada i dodjeljivanja tiketa. Ako jedan član tima nije dostupan, tiket se može dodijeliti drugom članu istog tima.

---

### 3.3 Tiket

Tiket predstavlja osnovni poslovni entitet sistema i opisuje prijavljeni problem, njegov osnovni status, prioritet i tok obrade.

**Ključni atributi:**
- `tiketId`
- `naslov`
- `opis`
- `datumKreiranja`
- `datumZatvaranja`
- `status`
- `prioritet`
- `kategorijaProblema`

**Napomena:** Tiket je centralna jedinica rada u sistemu. U ovom modelu tiket ima statuse: otvoren i zatvoren.

---

### 3.4 Komentar

Komentar predstavlja poruku ili napomenu vezanu za određeni tiket. Komentar mogu ostavljati klijent, agent, tehničar ili administrator.

**Ključni atributi:**
- `komentarId`
- `sadrzaj`
- `datumVrijeme`
- `jeInterni`

**Napomena:** Komentari omogućavaju komunikaciju unutar tiketa. Interni komentari nisu vidljivi klijentu.

---

### 3.5 Ocjena

Ocjena predstavlja korisničku povratnu informaciju nakon zatvaranja tiketa. Pored numeričke vrijednosti, ocjena može sadržavati i tekstualni komentar kojim korisnik obrazlaže zašto je dao određenu ocjenu.

**Ključni atributi:**
- `ocjenaId`
- `vrijednostOcjene`
- `komentarOcjene`
- `datumOcjenjivanja`

**Napomena:** Ocjena je poseban entitet jer predstavlja formalnu evaluaciju usluge nakon završetka obrade tiketa.

---

### 3.6 PaketPretplata

PaketPretplata predstavlja uslugu ili skup usluga koje klijent koristi kod telekom operatera.

**Ključni atributi:**
- `paketId`
- `nazivPaketa`
- `tipPaketa`
- `statusPaketa`
- `mjesecnaCijena`
- `opisPaketa`

**Napomena:** PaketPretplata je važan jer korisnički problemi često zavise od toga koji paket korisnik koristi.

---

### 3.7 KarakteristikaPaketa

KarakteristikaPaketa predstavlja konkretna svojstva paketa ili pretplate.

**Ključni atributi:**
- `karakteristikaId`
- `naziv`
- `vrijednost`
- `jedinicaMjere`
- `opis`

**Napomena:** Ovaj entitet omogućava da paket bude detaljnije opisan.

---

### 3.8 DodjelaTiketa

DodjelaTiketa predstavlja informaciju o tome kome je tiket dodijeljen i da li je dodjela izvršena automatski ili ručno.

**Ključni atributi:**
- `dodjelaId`
- `datumDodjele`
- `tipDodjele`
- `napomena`

**Napomena:** Ovaj entitet omogućava modeliranje automatske dodjele tiketa konkretnom članu tima.

---

### 3.9 Izvještaj

Izvještaj predstavlja pregled podataka o radu sistema, tiketima, timovima i korisničkom zadovoljstvu.

**Ključni atributi:**
- `izvjestajId`
- `naziv`
- `tipIzvjestaja`
- `periodOd`
- `periodDo`
- `datumGenerisanja`

**Napomena:** Format izvoza izvještaja je uvijek **PDF**.

---

### 3.10 Notifikacija

Notifikacija predstavlja sistemsku obavijest poslanu korisniku u vezi sa tiketom ili nekom drugom važnom promjenom u sistemu.

**Ključni atributi:**
- `notifikacijaId`
- `naslov`
- `sadrzaj`
- `tipNotifikacije`
- `datumSlanja`
- `procitano`

**Napomena:** Dokumentacija sistema predviđa osnovne notifikacije, npr. obavijest agentu o dodjeli tiketa i obavijest pri prosljeđivanju tiketa. Arhitektura sistema također predviđa Notification servis zasnovan na WebSocket komunikaciji.

---

## 4. Enumeracije

Sistem koristi sljedeće enumeracijske tipove:

### 4.1 Uloga
- `KLIJENT`
- `AGENT`
- `TEHNICAR`
- `ADMINISTRATOR`

---

### 4.2 StatusNaloga
- `AKTIVAN`
- `NEAKTIVAN`

---

### 4.3 StatusRaspolozivosti
- `SLOBODAN`
- `ZAUZET`
- `NEDOSTUPAN`

---

### 4.4 StatusTiketa
- `OTVOREN`
- `ZATVOREN`

---

### 4.5 Prioritet
- `NIZAK`
- `SREDNJI`
- `VISOK`

---

### 4.6 KategorijaProblema
- `INTERNET`
- `TV`
- `MOBILNA_MREZA`
- `NAPLATA`
- `TEHNICKA_PODRSKA`

---

### 4.7 TipPaketa
- `INTERNET`
- `TV`
- `MOBILNI`
- `KOMBINOVANI`

---

### 4.8 StatusPaketa
- `AKTIVAN`
- `NEAKTIVAN`

---

### 4.9 TipTima
- `AGENTI`
- `TEHNICARI`

---

### 4.10 StatusTima
- `AKTIVAN`
- `NEAKTIVAN`

---

### 4.11 TipDodjele
- `AUTOMATSKA`
- `RUCNA`

---

### 4.12 TipIzvjestaja
- `BROJ_TIKETA`
- `STATUS_TIKETA`
- `TIP_PROBLEMA`
- `OPTERECENJE_TIMOVA`
- `OCJENE_KORISNIKA`

---

### 4.13 TipNotifikacije
- `DODJELA_TIKETA`
- `PROSLJEDJIVANJE_TIKETA`
- `PROMJENA_STATUSA`
- `ODGOVOR_NA_TIKET`
- `ZATVARANJE_TIKETA`

---

## 5. Veze između entiteta

### Korisnik i Tiket
Jedan korisnik sa ulogom klijenta može kreirati više tiketa, dok svaki tiket pripada tačno jednom korisniku koji ga je kreirao.

**Veza:** 1 : N

---

### Korisnik i Komentar
Jedan korisnik može napisati više komentara, dok svaki komentar pripada tačno jednom korisniku.

**Veza:** 1 : N

---

### Tiket i Komentar
Jedan tiket može imati više komentara, dok svaki komentar pripada tačno jednom tiketu.

**Veza:** 1 : N

---

### Korisnik i Ocjena
Jedan korisnik može ostaviti više ocjena kroz vrijeme, dok svaka ocjena pripada tačno jednom korisniku.

**Veza:** 1 : N

---

### Tiket i Ocjena
Jedan tiket može imati najviše jednu ocjenu, dok svaka ocjena pripada tačno jednom tiketu.

**Veza:** opcionalna 1 : 1

---

### Korisnik i PaketPretplata
Jedan korisnik sa ulogom klijenta može imati više paketa, dok svaki paket pripada tačno jednom korisniku.

**Veza:** 1 : N

---

### PaketPretplata i KarakteristikaPaketa
Jedan paket može imati više karakteristika, dok svaka karakteristika pripada tačno jednom paketu.

**Veza:** 1 : N

---

### Tim i Korisnik
Jedan tim može imati više članova, dok jedan korisnik sa ulogom agenta ili tehničara pripada jednom timu.

**Veza:** 1 : N

---

### Tim i Tiket
Jedan tim može biti odgovoran za više tiketa, dok jedan tiket može biti vezan za jedan tim odgovoran za obradu.

**Veza:** 1 : N

---

### Tiket i DodjelaTiketa
Jedan tiket može imati više zapisa o dodjeli, dok svaki zapis dodjele pripada tačno jednom tiketu.

**Veza:** 1 : N

---

### Korisnik i DodjelaTiketa
Jedan korisnik sa ulogom agenta ili tehničara može biti povezan sa više zapisa dodjele, dok svaki zapis dodjele referencira tačno jednog korisnika kojem je tiket dodijeljen.

**Veza:** 1 : N

---

### Tim i DodjelaTiketa
Jedan tim može biti povezan sa više zapisa dodjele, dok svaka dodjela može biti izvršena u okviru jednog tima.

**Veza:** 1 : N

---

### Korisnik i Notifikacija
Jedan korisnik može primiti više notifikacija, dok svaka notifikacija pripada tačno jednom korisniku.

**Veza:** 1 : N

---

### Izvještaj i Tiket
Jedan izvještaj može obuhvatati više tiketa, a jedan tiket može biti uključen u više različitih izvještaja.

**Veza:** M : N

---

## 6. Poslovna pravila važna za model

### 6.1 Pravila vezana za korisnike
- Svaki korisnik mora imati jedinstven email i jedinstveno korisničko ime.
- Svaki korisnik mora imati tačno jednu ulogu.
- Korisnik sa ulogom `KLIJENT` može kreirati i pregledati samo vlastite tikete.
- Korisnik sa ulogom `AGENT` može obrađivati tikete koji su mu dodijeljeni ili koje vidi prema pravilima sistema.
- Korisnik sa ulogom `TEHNICAR` može raditi na tiketima koji su mu dodijeljeni.
- Korisnik sa ulogom `ADMINISTRATOR` ima najširi nivo pristupa.
- Samo korisnici sa ulogama `AGENT` i `TEHNICAR` koriste atribut `statusRaspolozivosti` kao osnov za dodjelu novih tiketa.

---

### 6.2 Pravila vezana za timove
- Tim može sadržavati samo korisnike sa ulogama `AGENT` i `TEHNICAR`.
- Jedan korisnik može pripadati najviše jednom timu.
- Svaki tim ima definisan tip.
- Ako je jedan član tima zauzet ili nije dostupan, tiket se može dodijeliti drugom članu istog tima.
- Automatska dodjela može uzeti u obzir pripadnost timu, tip problema i status raspoloživosti člana tima.

---

### 6.3 Pravila vezana za tikete
- Svaki tiket mora imati tačno jednog kreatora.
- Kreator tiketa mora biti korisnik sa ulogom `KLIJENT`.
- Svaki tiket mora imati definisan status, prioritet i kategoriju problema.
- Tiket može biti samo otvoren ili zatvoren.
- Tiket može biti dodijeljen timu i/ili konkretnom članu tima.
- Zatvoren tiket ne može primiti novu ocjenu ako je ocjena već unesena.

---

### 6.4 Pravila vezana za dodjelu tiketa
- Svaka dodjela mora biti evidentirana kroz entitet `DodjelaTiketa`.
- Dodjela može biti automatska ili ručna.
- Automatska dodjela se vrši na osnovu unaprijed definisanih pravila.
- Automatska dodjela može se izvršiti samo korisniku čiji je `statusRaspolozivosti` postavljen na `SLOBODAN`.
- Ako je jedan korisnik označen kao `ZAUZET` ili `NEDOSTUPAN`, sistem treba pokušati dodijeliti tiket drugom raspoloživom članu istog tima.
- Jedan tiket može imati više zapisa o dodjeli ako se ponovo dodjeljuje drugom članu tima ili drugom timu.

---

### 6.5 Pravila vezana za komentare
- Svaki komentar mora biti vezan za tačno jedan tiket.
- Svaki komentar mora imati tačno jednog autora.
- Interni komentar nije vidljiv klijentu.
- Komentari služe za komunikaciju između učesnika obrade tiketa.

---

### 6.6 Pravila vezana za ocjene
- Ocjenu može ostaviti samo korisnik sa ulogom `KLIJENT`.
- Ocjena se može ostaviti samo za zatvoren tiket.
- Jedan tiket može imati najviše jednu ocjenu.
- Ocjena mora sadržavati numeričku vrijednost.
- Ocjena može sadržavati i komentar uz ocjenu kao obrazloženje zadovoljstva ili nezadovoljstva.

---

### 6.7 Pravila vezana za pakete
- Paket mora pripadati tačno jednom korisniku.
- Jedan korisnik može imati više paketa.
- Paket može imati više karakteristika.
- Karakteristike detaljno opisuju konkretne mogućnosti paketa.
- Problemi prijavljeni kroz tiket mogu biti povezani sa korisnikovim paketom.

---

### 6.8 Pravila vezana za notifikacije
- Sistem podržava osnovne notifikacije vezane za dodjelu i obradu tiketa.
- Notifikacije se mogu slati korisnicima kada dođe do važne promjene u vezi sa tiketom.
- Svaka notifikacija pripada tačno jednom korisniku.

---

### 6.9 Pravila vezana za izvještaje
- Izvještaji se generišu na osnovu podataka iz sistema.
- Izvještaj može obuhvatati više tiketa.
- Pristup izvještajima zavisi od uloge korisnika.
- Izvještaji se izvoze isključivo u PDF formatu.
