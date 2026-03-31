# Product Backlog

## Uvod

Ovaj dokument služi za praćenje i upravljanje zadacima u okviru razvoja proizvoda. Backlog treba biti **živ dokument i redovno ažuriran**.

### Legenda za oznake

- **Statusi za backlog:** Backlog, To-Do, In Progress, Testing, Done
- **Procjena složenosti:** XS, S, M, L, XL
- **Oznaka prioriteta:** 1, 2, 3, 4, 5 (1 je najbitnije, 5 je najmanje bitno)
- **Tipovi stavki:** Feature, Bug, Dokumentacija, Research, Technical Task
- Konvencija za ID Backlogova: SP[Broj sprinta]-[Broj Id-a]

---

## Tabelarni prikaz Backloga

| ID                      | Naziv stavke                                   | Tip stavke                     | Prioritet | Složenost | Status  | Sprint      |
| :---------------------- | :--------------------------------------------- | :----------------------------- | :-------: | :-------: | :------ | :---------- |
| [SP1-01](#sp1-01)       | Definisati Team Charter                        | Dokumentacija                  |     1     |     -     | To-Do   | Sprint 1    |
| [SP1-02](#sp1-02)       | Definisati Product Vision                      | Dokumentacija                  |     1     |     -     | To-Do   | Sprint 1    |
| [SP1-03](#sp1-03)       | Definisati Stakeholder Map                     | Dokumentacija                  |     1     |     -     | To-Do   | Sprint 1    |
| [SP1-04](#sp1-04)       | Definisati početni Product Backlog             | Dokumentacija                  |     1     |     -     | To-Do   | Sprint 1    |
| [SP2-01](#sp2-01)       | Definisati Acceptance Criteria                 | Dokumentacija                  |     2     |     S     | To-Do   | Sprint 2    |
| [SP2-02](#sp2-02)       | Definisanje User Stories                       | Dokumentacija                  |     1     |    XS     | To-Do   | Sprint 2    |
| [SP2-03](#sp2-03)       | Definisati listu NFR zahtjeva                  | Dokumentacija                  |     2     |     M     | To-Do   | Sprint 2    |
| [SP3-01](#sp3-01)       | Kreirati Risk Register                         | Dokumentacija                  |     1     |     S     | Backlog | Sprint 3    |
| [SP3-02](#sp3-02)       | Izraditi Domain Model                          | Dokumentacija                  |     1     |     M     | Backlog | Sprint 3    |
| [SP3-03](#sp3-03)       | Izraditi Use Case Model                        | Dokumentacija                  |     1     |     M     | Backlog | Sprint 3    |
| [SP3-04](#sp3-04)       | Izraditi Architecture Overview                 | Dokumentacija                  |     1     |     M     | Backlog | Sprint 3    |
| [SP3-05](#sp3-05)       | Definisati Test Strategy                       | Dokumentacija                  |     1     |    ML     | Backlog | Sprint 3    |
| [SP4-01](#sp4-01)       | Definisati Definition of Done                  | Dokumentacija                  |     1     |     M     | Backlog | Sprint 4    |
| [SP4-02](#sp4-02)       | Kreirati Initial Release Plan                  | Dokumentacija                  |     1     |     M     | Backlog | Sprint 4    |
| [SP4-03](#sp4-03)       | Uspostaviti osnovni skeleton projekta          | Technical Task / Dokumentacija |     2     |     S     | Backlog | Sprint 4    |
| [SP4-04](#sp4-04)       | Postaviti inicijalnu strukturu repozitorija... | Technical Task / Dokumentacija |     2     |     L     | Backlog | Sprint 4    |
| [SP5-01](#sp5-01)       | Uspostava AI Usage Loga                        | Dokumentacija                  |     1     |    XS     | Backlog | Sprint 5    |
| [SP5-02](#sp5-02)       | Uspostava Decision Loga                        | Dokumentacija                  |     1     |    XS     | Backlog | Sprint 5    |
| [SP6-10-01](#sp6-10-01) | Login korisnika                                | Feature                        |     1     |     L     | Backlog | Sprint 6-10 |
| [SP6-10-02](#sp6-10-02) | Upravljanje korisničkim profilom               | Feature                        |     2     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-03](#sp6-10-03) | Prikaz paketa i pretplata                      | Feature                        |     4     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-05](#sp6-10-05) | Kreiranje novog ticketa                        | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-06](#sp6-10-06) | Pregled vlastitih tiketa                       | Feature                        |     1     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-07](#sp6-10-07) | Detaljan prikaz tiketa                         | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-08](#sp6-10-08) | Zatvaranje ticketa                             | Feature                        |     1     |     L     | Backlog | Sprint 6-10 |
| [SP6-10-09](#sp6-10-09) | Ocjenjivanje ticketa                           | Feature                        |     5     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-10](#sp6-10-10) | Komunikacija kroz tiket                        | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-11](#sp6-10-11) | Upravljanje prioritetima tiketa                | Feature                        |     2     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-12](#sp6-10-12) | Preraspodjela agenata po timovima              | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-13](#sp6-10-13) | Automatska dodjela tiketa timovima             | Feature                        |     3     |    XS     | Backlog | Sprint 6-10 |
| [SP6-10-14](#sp6-10-14) | Prosljeđivanje tiketa                          | Feature                        |     2     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-15](#sp6-10-15) | Pregled svih tiketa                            | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-16](#sp6-10-16) | Pretraživanje i filtriranje tiketa             | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-17](#sp6-10-17) | Pregled i uređivanje korisničkih profila       | Feature                        |     2     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-18](#sp6-10-18) | Pregled dodijeljenih tiketa (tehničari)        | Feature                        |     1     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-19](#sp6-10-19) | Ažuriranje statusa tiketa                      | Feature                        |     1     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-21](#sp6-10-21) | Tehničar vidi osnovne informacije...           | Feature                        |     2     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-22](#sp6-10-22) | Izvještaj o broju tiketa                       | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-23](#sp6-10-23) | Izvještaj po statusu tiketa                    | Feature                        |     5     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-24](#sp6-10-24) | Izvještaj po tipu problema                     | Feature                        |     3     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-25](#sp6-10-25) | Prosječno vrijeme rješavanja tiketa            | Feature                        |     1     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-26](#sp6-10-26) | Vrijeme prvog odgovora                         | Feature                        |     2     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-27](#sp6-10-27) | Izvještaj o opterećenju agenata                | Feature                        |     2     |     M     | Backlog | Sprint 6-10 |
| [SP6-10-28](#sp6-10-28) | Izvještaj o ocjenama korisnika                 | Feature                        |     2     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-29](#sp6-10-29) | Admin Dashboard sa ključnim metrikama          | Feature                        |     1     |     L     | Backlog | Sprint 6-10 |
| [SP6-10-30](#sp6-10-30) | Export izvještaja                              | Feature                        |     3     |     S     | Backlog | Sprint 6-10 |
| [SP6-10-31](#sp6-10-31) | FAQ segment                                    | Feature                        |     3     |     S     | Backlog | Sprint 6-10 |

---

## Detalji Backlog stavki

### SP1-01

- **Naziv Stavke:** Team Charter
- **Opis:** Kreirati i dogovoriti Team Charter: sastav tima, komunikacija, radna pravila, odgovornosti i pravila neispunjavanja
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** -
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 1

---

### SP1-02

- **Naziv Stavke:** Product Vision
- **Opis:** Kreirati Product Vision: problem, ciljni korisnici, vrijednst sistema, MVP, scope i ograničenja
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** -
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 1

---

### SP1-03

- **Naziv Stavke:** Stakeholder Map
- **Opis:** Idetifikovati sve stakeholdere sistema, njihove uloge, i interese
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** -
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 1

---

### SP1-04

- **Naziv Stavke:** Product Backlog
- **Opis:** Kreirati početnu listu stavki za izvođenje projekta
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** -
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 1

---

### SP2-01

- **Naziv Stavke:** Acceptance Criteria
- **Opis:** Za svaki User Story definisati jasne i mjerljive uslove koje funkcionalnost mora zadovoljiti kako bi bila smatrana gotovom
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 2

---

### SP2-02

- **Naziv Stavke:** User Stories
- **Opis:** Kreirati strukturisanu listu User Stories, iz perspektive krajnjeg korisnika
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 2

---

### SP2-03

- **Naziv Stavke:** NFR zahtjevi
- **Opis:** Identifikovati i dokumentovati nefunkcionalne zahtjeve sistema (brzina učitavanja, broj istovremenih korisnika, sigurnost)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** To-Do -**Veza sa sprintom ili release planom:** Sprint 2

---

### SP3-01

- **Naziv Stavke:** Risk Register
- **Opis:** Potrebno je napraviti dokument u kojem se procijenjuju sve prijetnje i rizici prije, tokom i poslije implementacije projekta
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-02

- **Naziv Stavke:** Domain Model
- **Opis:** Potrebno je izradii dokument Domain model u kojem se nalazi reprezentacija specificnog problema u nasem domenu. Model se koristi kao most izmedju stakeholdera i developera. Definisati ključne entitete sistema njihove atribute, ponašanja i međusobne veze
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-03

- **Naziv Stavke:** Use Case Model
- **Opis:** Definisati funkcionalne zahtjeve sistema kroz UML Use Case dijagrame
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-04

- **Naziv Stavke:** Architecture Overview
- **Opis:** Dokumentovati arhitekturu sistema. Prikazati ključne komponente, njihove veze, i odgovornosti
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-05

- **Naziv Stavke:** Test Strategy
- **Opis:** Dokumentovati pristup testiranju (vrste testova, odgovornosti, alate)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP4-01

- **Naziv Stavke:** Definition of Done
- **Opis:** Kreirati i dokumentovati zajednički dogovoreni skup kriterija koje svaki product increment mora zadovoljiti prije nego što se smatra završenim
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP4-02

- **Naziv Stavke:** Initial Release Plan
- **Opis:** Napraviti pregled planiranih isporuka funkcionalnosti
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP4-03

- **Naziv Stavke:** Osnovni skeleton projekta
- **Opis:** Kreirati minimalnu, ali funkcionalnu strukturu foldera
- **Tip Stavke:** Technical Task / Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP4-04

- **Naziv Stavke:** Inicijalna struktura repozitorija i osnovni tehnički setup
- **Opis:** Kreirati repozitorij i konfigurisati razvojno okruženje
- **Tip Stavke:** Technical Task / Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** L
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP5-01

- **Naziv Stavke:** AI Usage log
- **Opis:** Kreirati i održavati dokument u kojem se bilježi svako korištenje AI alata tokom free AI usage faze razvoja softvera
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 5

---

### SP5-02

- **Naziv Stavke:** Decision Log
- **Opis:** Napraviti Decision Log dokument koji se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka. Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 5

---

### SP6-10-01

- **Naziv Stavke:** Login korisnika
- **Opis:** Implementirati autentikaciju i autorizaciju korisnika za koristenje Ticket/Helpdesk sistema
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10
- **Napomena:** Za korisnika smatramo da već ima registrovan profil gdje pristupni podaci se nalaze u njegovom ugovoru za paket koji koristi

---

### SP6-10-02

- **Naziv Stavke:** Upravljanje korisničkim profilom
- **Opis:** Implementirati feature gdje korisnik može da mijenjati neke podatke vezane za svoj profil (email, lozinka)
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-03

- **Naziv Stavke:** Prikaz paketa i pretplata
- **Opis:** Implementirati feature gdje korisnik ima uvid i svoje pakete: Internet, TV, mobilni paketi
- **Tip Stavke:** Feature
- **Prioritet:** 4
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-05

- **Naziv Stavke:** Kreiranje novog ticketa
- **Opis:** Implementirati feature za prikaz forme za prijavu problema i postavljanje pitanja (subject, tip, opis, prioritet)
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-06

- **Naziv Stavke:** Pregled vlastitih tiketa
- **Opis:** Implementirati feature za prikaz liste svih tiketa koje je korisnik kreirao
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-07

- **Naziv Stavke:** Detaljan prikaz tiketa
- **Opis:** Implementirati feature za prikaz kompletnog sadržaja tiketa (status, prioritet, historija komunikacije)
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-08

- **Naziv Stavke:** Zatvaranje ticketa
- **Opis:** Implementirati feature za zatvaranje ticketa.
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10
- **Napomena:** Ticket može zatvoriti i korisnik i agent kome je ticket dodijeljen kao i tehničar koji je na terenu riješio problem

---

### SP6-10-09

- **Naziv Stavke:** Ocjenjivanje ticketa
- **Opis:** Implementirati feature gdje korisnik može ocijeniti kvalitet rješenja
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-10

- **Naziv Stavke:** Komunikacija kroz tiket
- **Opis:** Implementirati feature gdje korisnik i agent/tehničar mogu razmjenjivati poruke kroz tiket. Korisnik je ograničen na jednu poruku po odgovoru
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-11

- **Naziv Stavke:** Upravljanje prioritetima tiketa
- **Opis:** Definisati 2 različita prioriteta za svaki tiket. Korisnički i interni prioritet tiketa. Nakon definisanja i implementirati navedeni feature
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10
- **Napomena:** Korisnik nema uvid u internu evaluaciju tiketa

---

### SP6-10-12

- **Naziv Stavke:** Preraspodjela agenata po timovima
- **Opis:** Implementirati feature gdje administrator ima mogućnost da organizuje agente po timovima i stručnosti
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-13

- **Naziv Stavke:** Automatska dodjela tiketa timovima
- **Opis:** Implementirati feature za prikaz sistema raspodjele tiketa prema tipu problema
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** XS
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-14

- **Naziv Stavke:** Prosljeđivanje tiketa
- **Opis:** Implementirati feature gdje Agent može daprebaciti tiket drugom timu/osobi
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-15

- **Naziv Stavke:** Pregled svih tiketa
- **Opis:** Implementirat feature da agent može da vidi sve tikete i odgovori na tikete van svoje struke
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-16

- **Naziv Stavke:** Pretraživanje i filtriranje tiketa
- **Opis:** Implementirati feature za pretragu i filtriranje tiketa prema različitim kriterijima.
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10
- **Napomena:** Agentima omogućiti pregled i filtriranje svih tiketa (status, prioritet, tip, korisnik, vremenski opseg), dok krajnji korisnici mogu pretraživati i filtrirati isključivo vlastite tikete.

---

### SP6-10-17

- **Naziv Stavke:** Pregled i uređivanje korisničkih profila
- **Opis:** Implementirati feature da agent može da vidi detalje korisnika, historiju tiketa i upravljati njegovim paketima i pretplatama
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-18

- **Naziv Stavke:** Pregled dodijeljenih tiketa (tehničari)
- **Opis:**Implementirati feature za pregled i prikaz liste radnih naloga za teren
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-19

- **Naziv Stavke:** Ažuriranje statusa tiketa
- **Opis:** Implementirati mogucnost i feature da tehničar promjeni stanje tiketa i na kraju ga i zatvori ako je problem riješen
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-21

- **Naziv Stavke:** Tehničar vidi osnovne informacije o korisniku
- **Opis:** Implementirati mogucnost za tehničara da vidi osnovne informacije ko korisniku koji je prijavio problem. To podrazumijeva ime, prezime, adresu, broj telefona, pakete, pretplate, instalirane uređaje
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-22

- **Naziv Stavke:** Izvještaj o broju tiketa
- **Opis:** Implementirati mogucnost za izracunavanje i prikaz ukupnog broja tiketa po vremenskom periodu (dnevno, sedmično, mjesečno)
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-23

- **Naziv Stavke:** Izvještaj po statusu tiketa
- **Opis:** Implementirati mogucnost za izracunavanje i prikaz izvjestaja za broj tiketa po statusima (u nekom vremenskom opsegu), i procentualni udio.
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10
- **Napomena:** Ovaj izvještaj gubi smisao za veće vremenske opsege.

---

### SP6-10-24

- **Naziv Stavke:** Izvještaj po tipu problema
- **Opis:** Implementirati mogucnost za izracunavanje i prikaz izvjestaja za analizu tiketa po predefinisanim kategorijama problema
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-25

- **Naziv Stavke:** Prosječno vrijeme rješavanja tiketa
- **Opis:** Implementirati mogucnost za izracunavanja i prikaz za izračun prosječnog vremena od kreiranja do zatvaranja tiketa
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-26

- **Naziv Stavke:** Vrijeme prvog odgovora
- **Opis:** Implementirati mogucnost za izracunavanje i prikaz prosječno vrijeme do prvog odgovora na tiket
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-27

- **Naziv Stavke:** Izvještaj o opterećenju agenata
- **Opis:** Implementirati mogucnost za Detaljan izvještaj o broju riješenih tiketa po danu | sedmici | mjesecu za sve agente
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-28

- **Naziv Stavke:** Izvještaj o ocjenama korisnika
- **Opis:** Implementirati mogucnost feature za analizu ocjena koje korisnici daju nakon zatvaranja tiketa
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-29

- **Naziv Stavke:** Admin Dashboard sa ključnim metrikama
- **Opis:**Dizajnirati i implementirati admin dashboard gdje se prikazuju ključne metrike sistema i mogućnost generisanja dostupnih izvještaja
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-30

- **Naziv Stavke:** Export izvještaja
- **Opis:** implementirati mogucnost za izvoz izvještaja u CSV formatu
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10

---

### SP6-10-31

- **Naziv Stavke:** FAQ segment
- **Opis:** Napraviti, dokumentovati i implementirati Listu često postavljenih pitanja i odgovora
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 6-10
