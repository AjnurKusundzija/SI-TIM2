# Acceptance Criteria

## Template:

Za svaki User Story koristiti sljedeće obrasce:

### 1. Početni šablon
- Kada [početni uslov], ako [akcija], tada [očekivani rezultat]

 **Kada** - opisuje početno stanje sistema  
 **Ako** - akcija korisnika ili događaj  
 **Tada** - očekivani rezultat
### 2. Sistem pravila
- Sistem mora omogućiti…
- Sistem ne smije dozvoliti…
- Korisnik treba dobiti…

---

## Napomena:
Prema uputama sa prošlog sastanka: 
- svaki kriterij mora biti testabilan  
- izbjegavati nejasne izraze poput “brzo”, “dobro”, “efikasno”  

  

# PB-19 Login korisnika

### US-1: *Kao registrovani korisnik, želim da se prijavim u sistem koristeći email i lozinku, kako bih pristupio svom nalogu.*

**Acceptance Criteria:**

- Kada korisnik nije prijavljen, ako unese ispravan email i lozinku, tada se uspješno prijavljuje u sistem  
- Kada korisnik unese validne podatke, ako klikne na “Login”, tada se preusmjerava na početnu stranicu  
- Sistem mora omogućiti unos emaila i lozinke  
- Sistem ne smije dozvoliti prijavu bez unosa obaveznih polja  
- Korisnik treba dobiti potvrdu o uspješnoj prijavi  

---

### US-2: *Kao korisnik, želim da se odjavim iz sistema, kako bih zaštitio svoj nalog.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako klikne na “Logout”, tada se odjavljuje iz sistema  
- Kada se korisnik odjavi, tada se preusmjerava na login stranicu  
- Sistem mora onemogućiti pristup zaštićenim stranicama nakon logout-a  
- Korisnik ne smije ostati autentifikovan nakon odjave  

---

### US-3: *Kao korisnik, želim da budem obaviješten ako unesem pogrešne podatke za prijavu, kako bih mogao ponovo pokušati.* 

**Acceptance Criteria:**

- Kada korisnik unese pogrešan email ili lozinku, ako pokuša login, tada sistem odbija prijavu  
- Kada login ne uspije, tada korisnik dobija poruku o grešci  
- Sistem ne smije otkriti da li je email ili lozinka pogrešna   
- Korisnik treba dobiti mogućnost ponovnog pokušaja prijave  

---

# PB-20 Upravljanje korisničkim profilom

### US-4: *Kao korisnik, želim da promijenim svoj email, kako bih imao tačne kontakt informacije.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako unese novi email i potvrdi izmjenu, tada se email ažurira u sistemu  
- Kada korisnik unese nevalidan email, tada sistem prikazuje grešku  
- Sistem mora provjeriti format email adrese  
- Sistem ne smije dozvoliti unos već postojećeg emaila u sistemu  
- Korisnik treba dobiti potvrdu o uspješnoj promjeni emaila  

---

### US-5: *Kao korisnik, želim da promijenim lozinku, kako bih povećao sigurnost svog naloga.*

**Acceptance Criteria:**

- Kada je korisnik prijavljen, ako unese trenutnu i novu lozinku, tada se lozinka uspješno mijenja  
- Kada korisnik unese pogrešnu trenutnu lozinku, tada sistem odbija promjenu  
- Sistem mora zahtijevati unos trenutne lozinke prije promjene  
- Sistem mora provjeriti minimalne sigurnosne zahtjeve lozinke  
- Sistem ne smije dozvoliti lozinku koja ne zadovoljava sigurnosne zahtjeve 
- Korisnik treba dobiti potvrdu o uspješnoj promjeni lozinke  

---
