# Product Backlog

## Uvod

Ovaj dokument služi za praćenje i upravljanje zadacima u okviru razvoja proizvoda. Backlog treba biti **živ dokument i redovno ažuriran**.

### Legenda za oznake

- **Statusi za backlog:** Backlog, To-Do, In Progress, Testing, **Partial**, Done
- **Partial:** admin/MVP je implementiran u kodu, ali acceptance kriteriji PB stavke nisu u potpunosti ispunjeni (vidi napomenu kod stavke)
- **Procjena složenosti:** XS, S, M, L, XL
- **Oznaka prioriteta:** 1, 2, 3, 4, 5 (1 je najbitnije, 5 je najmanje bitno)
- **Tipovi stavki:** Feature, Bug, Dokumentacija, Research, Technical Task

---

## Tabelarni prikaz Backloga


| ID              | Naziv stavke                                       | Tip stavke     | Prioritet | Složenost | Status  | Sprint    |
| --------------- | -------------------------------------------------- | -------------- | --------- | --------- | ------- | --------- |
| [PB-01](#pb-01) | Team Charter                                       | Dokumentacija  | 1         | S         | Done    | Sprint 1  |
| [PB-02](#pb-02) | Product Vision                                     | Dokumentacija  | 1         | M         | Done    | Sprint 1  |
| [PB-03](#pb-03) | Stakeholder Map                                    | Dokumentacija  | 1         | S         | Done    | Sprint 1  |
| [PB-04](#pb-04) | Product Backlog                                    | Dokumentacija  | 1         | M         | Done    | Sprint 1  |
| [PB-05](#pb-05) | Acceptance Criteria                                | Dokumentacija  | 2         | S         | Done    | Sprint 2  |
| [PB-06](#pb-06) | User Stories                                       | Dokumentacija  | 1         | XS        | Done    | Sprint 2  |
| [PB-07](#pb-07) | NFR zahtjevi                                       | Dokumentacija  | 2         | M         | Done    | Sprint 2  |
| [PB-08](#pb-08) | Risk Register                                      | Dokumentacija  | 1         | M         | Done    | Sprint 3  |
| [PB-09](#pb-09) | Domain Model                                       | Dokumentacija  | 2         | M         | Done    | Sprint 3  |
| [PB-10](#pb-10) | Use Case Model                                     | Dokumentacija  | 1         | M         | Done    | Sprint 3  |
| [PB-11](#pb-11) | Architecture Overview                              | Dokumentacija  | 1         | L         | Done    | Sprint 3  |
| [PB-12](#pb-12) | Test Strategy                                      | Dokumentacija  | 1         | M         | Done    | Sprint 3  |
| [PB-13](#pb-13) | Definition of Done                                 | Dokumentacija  | 3         | M         | Done    | Sprint 4  |
| [PB-14](#pb-14) | Initial Release Plan                               | Dokumentacija  | 2         | S         | Done    | Sprint 4  |
| [PB-15](#pb-15) | Osnovni skeleton projekta                          | Technical Task | 1         | S         | Done    | Sprint 4  |
| [PB-16](#pb-16) | Inicijalna struktura repozitorija                  | Technical Task | 1         | L         | Done    | Sprint 4  |
| [PB-17](#pb-17) | AI Usage Log                                       | Dokumentacija  | 3         | XS        | Done    | Sprint 5  |
| [PB-18](#pb-18) | Decision Log                                       | Dokumentacija  | 2         | XS        | Done    | Sprint 5  |
| [PB-19](#pb-19) | Login korisnika                                    | Feature        | 1         | L         | Done    | Sprint 5  |
| [PB-20](#pb-20) | Upravljanje korisničkim profilom                   | Feature        | 2         | M         | Done    | Sprint 8  |
| [PB-21](#pb-21) | Prikaz paketa i pretplata                          | Feature        | 4         | M         | Done    | Sprint 8  |
| [PB-22](#pb-22) | Kreiranje novog tiketa                             | Feature        | 1         | M         | Done    | Sprint 5  |
| [PB-23](#pb-23) | Pregled vlastitih tiketa                           | Feature        | 1         | S         | Done    | Sprint 5  |
| [PB-24](#pb-24) | Detaljan prikaz tiketa                             | Feature        | 1         | M         | Done    | Sprint 6  |
| [PB-25](#pb-25) | Zatvaranje tiketa                                  | Feature        | 1         | L         | Done    | Sprint 7  |
| [PB-26](#pb-26) | Ocjenjivanje tiketa                                | Feature        | 5         | S         | Done    | Sprint 8  |
| [PB-27](#pb-27) | Komunikacija kroz tiket                            | Feature        | 1         | M         | Done    | Sprint 6  |
| [PB-28](#pb-28) | Upravljanje prioritetima tiketa                    | Feature        | 2         | M         | Done    | Sprint 7  |
| [PB-29](#pb-29) | Preraspodjela agenata po timovima                  | Feature        | 1         | M         | Done    | Sprint 9-10  |
| [PB-30](#pb-30) | Automatska dodjela tiketa                          | Feature        | 3         | XS        | Done    | Sprint 7  |
| [PB-31](#pb-31) | Prosljeđivanje tiketa                              | Feature        | 2         | M         | Done    | Sprint 7  |
| [PB-32](#pb-32) | Pregled svih tiketa                                | Feature        | 1         | M         | Done    | Sprint 6  |
| [PB-33](#pb-33) | Pretraživanje i filtriranje tiketa                 | Feature        | 1         | M         | Done    | Sprint 7  |
| [PB-34](#pb-34) | Pregled i uređivanje korisničkih profila           | Feature        | 2         | M         | Done    | Sprint 8  |
| [PB-35](#pb-35) | Pregled dodijeljenih tiketa (tehničari)            | Feature        | 1         | S         | Done    | Sprint 7  |
| [PB-36](#pb-36) | Ažuriranje statusa tiketa                          | Feature        | 1         | S         | Done    | Sprint 8  |
| [PB-37](#pb-37) | Tehničar vidi osnovne informacije                  | Feature        | 2         | S         | Done    | Sprint 7  |
| [PB-38](#pb-38) | Izvještaj o broju tiketa                           | Feature        | 1         | M         | Done    | Sprint 9  |
| [PB-39](#pb-39) | Izvještaj po statusu tiketa                        | Feature        | 5         | M         | Done    | Sprint 9  |
| [PB-40](#pb-40) | Izvještaj po tipu problema                         | Feature        | 3         | S         | Done    | Sprint 9  |
| [PB-41](#pb-41) | Prosječno vrijeme rješavanja tiketa                | Feature        | 1         | M         | Done    | Sprint 9  |
| [PB-42](#pb-42) | Statistika agenta i tehničara                      | Feature        | 2         | M         | Done    | Sprint 8  |
| [PB-43](#pb-43) | Izvještaj o opterećenju agenata                    | Feature        | 2         | M         | Done    | Sprint 9  |
| [PB-44](#pb-44) | Izvještaj o ocjenama korisnika                     | Feature        | 2         | S         | Done    | Sprint 9  |
| [PB-45](#pb-45) | Admin Dashboard sa ključnim metrikama              | Feature        | 1         | L         | Done    | Sprint 9  |
| [PB-46](#pb-46) | Export izvještaja                                  | Feature        | 5         | S         | Done    | Sprint 11 |
| [PB-47](#pb-47) | FAQ segment                                        | Feature        | 3         | S         | Done    | Sprint 6  |
| [PB-48](#pb-48) | Pregled historije dodijeljenih tiketa za agente    | Feature        | 1         | M         | Done    | Sprint 7  |
| [PB-49](#pb-49) | Notifikacije                                       | Feature        | 1         | L         | Done    | Sprint 8  |
| [PB-50](#pb-50) | Prosječno vrijeme prvog odgovora (admin izvještaj) | Feature        | 2         | S         | Done    | Sprint 9  |
| [PB-51](#pb-51) | Upravljanje korisničkim nalozima                   | Feature        | 1         | L         | Done    | Sprint 9  |
| [PB-52](#pb-52) | Upravljanje katalogom paketa i pretplata           | Feature        | 2         | M         | Partial | Sprint 9  |
| [PB-53](#pb-53) | Pregled audit log-a aktivnosti                     | Feature        | 2         | M         | Done    | Sprint 9  |
| [PB-56](#pb-56) | Prilozi na tiketima                                | Feature        | 2         | M         | Done    | Sprint 9  |
| [PB-57](#pb-57) | AI prijedlog odgovora za agente i tehničare        | Feature        | 2         | M         | Done    | Sprint 10 |
| [PB-58](#pb-58) | AI uvidi za administratore                         | Feature        | 2         | L         | Done    | Sprint 10 |
| [PB-59](#pb-59) | Redizajn korisničkog sučelja                       | Feature        | 3         | L         | Done    | Sprint 10 |
| [PB-60](#pb-60) | Interni komentari na tiketima                      | Feature        | 1         | S         | Done    | Sprint 10 |
| [PB-61](#pb-61) | Admin CRUD FAQ                                     | Feature        | 2         | S         | Done    | Sprint 10 |
| [PB-62](#pb-62) | Assign to me — samodjelovanje tiketa               | Feature        | 2         | S         | Done    | Sprint 10 |
| [PB-63](#pb-63) | Agent availability status                          | Feature        | 3         | S         | Done    | Sprint 10 |
| [PB-64](#pb-64) | Linked Tickets — veza između tiketa                | Feature        | 2         | M         | Deferred | — |
| [PB-65](#pb-65) | SLA praćenje i upozorenja                          | Feature        | 2         | M         | Done    | Sprint 11 |
| [PB-66](#pb-66) | Bulk akcije na tiketima                            | Feature        | 3         | M         | Deferred | — |
| [PB-67](#pb-67) | Login via broj telefona                            | Feature        | 2         | M         | Done    | Sprint 11 |
| [PB-70](#pb-70) | MCP Admin Copilot                                  | Feature        | 2         | XL        | Done    | Sprint 10 |

---

## Detalji Backlog stavki

### PB-01

- **Naziv Stavke:** Team Charter
- **Opis:** Kreirati dokument koji definiše: sastav tima, komunikacija, radna pravila, odgovornosti i pravila neispunjavanja
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 1

---

### PB-02

- **Naziv Stavke:** Product Vision
- **Opis:** Kreirati dokument koji će sadržavati problem, ciljni korisnici, vrijednost sistema, MVP, scope i ograničenja
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 1

---

### PB-03

- **Naziv Stavke:** Stakeholder Map
- **Opis:** Identifikovati i dokumentirati sve stakeholdere sistema, njihove uloge i interese
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 1

---

### PB-04

- **Naziv Stavke:** Product Backlog
- **Opis:** Kreirati početnu listu stavki za izvođenje projekta (opis, tip, prioritet, procjena složenosti, status)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 1

---

### PB-05

- **Naziv Stavke:** Acceptance Criteria
- **Opis:** Za svaki User Story definisati jasne i mjerljive uslove koje funkcionalnost mora zadovoljiti kako bi bila smatrana gotovom
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 2

---

### PB-06

- **Naziv Stavke:** User Stories
- **Opis:** Kreirati strukturisanu listu User Stories, iz perspektive krajnjeg korisnika
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 2

---

### PB-07

- **Naziv Stavke:** NFR zahtjevi
- **Opis:** Identifikovati i dokumentovati nefunkcionalne zahtjeve sistema (performansa, sigurnost)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 2

---

### PB-08

- **Naziv Stavke:** Risk Register
- **Opis:** Potrebno je napraviti dokument u kojem se procijenjuju sve prijetnje i rizici prije, tokom i poslije implementacije projekta
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 3

---

### PB-09

- **Naziv Stavke:** Domain Model
- **Opis:** Potrebno je izraditi dokument Domain Model u kojem se nalazi reprezentacija specifičnog problema u našem domenu. Model se koristi kao most između stakeholdera i developera. Definisati ključne entitete sistema, njihove atribute, ponašanja i međusobne veze
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 3

---

### PB-10

- **Naziv Stavke:** Use Case Model
- **Opis:** Definisati funkcionalne zahtjeve sistema kroz UML Use Case dijagrame
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 3

---

### PB-11

- **Naziv Stavke:** Architecture Overview
- **Opis:** Dokumentovati arhitekturu sistema. Prikazati ključne komponente, njihove veze i odgovornosti
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 3

---

### PB-12

- **Naziv Stavke:** Test Strategy
- **Opis:** Dokumentovati pristup testiranju (vrste testova, odgovornosti, alate)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 3

---

### PB-13

- **Naziv Stavke:** Definition of Done
- **Opis:** Kreirati i dokumentovati zajednički dogovoreni skup kriterija koje svaki product increment mora zadovoljiti prije nego što se smatra završenim
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 3
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 4

---

### PB-14

- **Naziv Stavke:** Initial Release Plan
- **Opis:** Napraviti pregled planiranih isporuka funkcionalnosti
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 4

---

### PB-15

- **Naziv Stavke:** Osnovni skeleton projekta
- **Opis:** Kreirati minimalnu, ali funkcionalnu strukturu projekta (folderi, osnovna arhitektura, konfiguracija)
- **Tip Stavke:** Technical Task
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 4

---

### PB-16

- **Naziv Stavke:** Inicijalna struktura repozitorija i osnovni tehnički setup
- **Opis:** Kreirati repozitorij i konfigurisati razvojno okruženje
- **Tip Stavke:** Technical Task
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 4

---

### PB-17

- **Naziv Stavke:** AI Usage Log
- **Opis:** Kreirati i održavati dokument u kojem se bilježi svako korištenje AI alata tokom free AI usage faze razvoja softvera
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 3
- **Procjena složenosti ili napora:** XS
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 5

---

### PB-18

- **Naziv Stavke:** Decision Log
- **Opis:** Napraviti Decision Log dokument koji se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka.
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** XS
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 5

---

### PB-19

- **Naziv Stavke:** Login korisnika
- **Opis:** Implementirati autentikaciju i autorizaciju korisnika za korištenje Tiket/Helpdesk sistema
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 5
- **Napomena:** Za korisnika smatramo da već ima registrovan profil gdje pristupni podaci se nalaze u njegovom ugovoru za paket koji koristi

---

### PB-20

- **Naziv Stavke:** Upravljanje korisničkim profilom
- **Opis:** Implementirati feature gdje korisnik može da mijenja određene podatke vezane za svoj profil (email, lozinka)
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8

---

### PB-21

- **Naziv Stavke:** Prikaz paketa i pretplata
- **Opis:** Implementirati feature gdje korisnik ima uvid u svoje pakete i pretplate: Internet, TV, mobilni paketi
- **Tip Stavke:** Feature
- **Prioritet:** 4
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8

---

### PB-22

- **Naziv Stavke:** Kreiranje novog tiketa
- **Opis:** Implementirati feature za prikaz forme za prijavu problema i postavljanje pitanja (subject, tip, opis, prioritet)
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 5

---

### PB-23

- **Naziv Stavke:** Pregled vlastitih tiketa
- **Opis:** Implementirati feature za prikaz liste svih tiketa koje je korisnik kreirao
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 5

---

### PB-24

- **Naziv Stavke:** Detaljan prikaz tiketa
- **Opis:** Implementirati feature za prikaz kompletnog sadržaja tiketa (status, prioritet, historija komunikacije)
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 6

---

### PB-25

- **Naziv Stavke:** Zatvaranje tiketa
- **Opis:** Implementirati feature za zatvaranje tiketa
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7
- **Napomena:** Tiket može zatvoriti i korisnik i agent kome je tiket dodijeljen kao i tehničar koji je na terenu riješio problem

---

### PB-26

- **Naziv Stavke:** Ocjenjivanje tiketa
- **Opis:** Implementirati feature gdje korisnik može ocijeniti kvalitet rješenja nakon zatvaranja tiketa (skala 1–5, opcionalni komentar)
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8

---

### PB-27

- **Naziv Stavke:** Komunikacija kroz tiket
- **Opis:** Implementirati feature gdje korisnik i agent/tehničar mogu razmjenjivati poruke kroz tiket. Korisnik je ograničen na jednu poruku po odgovoru
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 6

---

### PB-28

- **Naziv Stavke:** Upravljanje prioritetima tiketa
- **Opis:** Definisati 2 različita prioriteta za svaki tiket. Korisnički i interni prioritet tiketa. Nakon definisanja potrebno je implementirati navedeni feature
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7
- **Napomena:** Korisnik nema uvid u internu evaluaciju tiketa

---

### PB-29

- **Naziv Stavke:** Preraspodjela agenata po timovima
- **Opis:** Implementirati feature gdje administrator ima mogućnost da organizuje agente po timovima i stručnosti
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano (Sprint 9): administrator može preraspodijeliti agente između timova; pregled raspodjele timova s filtriranjem; promjena se evidentira sa timestamp-om i imenom administratora; UI potvrda prije primjene promjene. Sprint 10 dopuna (US-24 — SB-05): admin vidi sve timove s članovima, filtrima, statusom aktivnosti tima i statusom dostupnosti agenata direktno iz sekcije Timovi.

---

### PB-30

- **Naziv Stavke:** Automatska dodjela tiketa timovima
- **Opis:** Implementirati feature za prikaz sistema raspodjele tiketa prema tipu problema
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** XS
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7

---

### PB-31

- **Naziv Stavke:** Prosljeđivanje tiketa
- **Opis:** Implementirati feature gdje agent može proslijediti tiket drugom timu/osobi
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7
- **Napomena:** Prošireno u Sprint 10 (US-101): administrator može koristiti isti forward modal za preraspodjelu agenta/tehničara na tiket bez ograničenja `assignedAgentId` provjere.

---

### PB-32

- **Naziv Stavke:** Pregled svih tiketa
- **Opis:** Implementirati feature da agent može da vidi sve tikete, kao i da odgovori na tikete van svoje struke
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 6

---

### PB-33

- **Naziv Stavke:** Pretraživanje i filtriranje tiketa
- **Opis:** Implementirati feature za pretragu i filtriranje tiketa prema različitim kriterijima
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7
- **Napomena:** Agentima omogućiti pregled i filtriranje svih tiketa (status, prioritet, tip, korisnik, vremenski opseg), dok krajnji korisnici mogu pretraživati i filtrirati isključivo vlastite tikete.

---

### PB-34

- **Naziv Stavke:** Pregled i uređivanje korisničkih profila
- **Opis:** Implementirati feature da agent može da vidi detalje korisnika, historiju tiketa i korisnikove pakete i pretplate
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8

---

### PB-35

- **Naziv Stavke:** Pregled dodijeljenih tiketa (tehničari)
- **Opis:** Implementirati feature koji omogućuje tehničarima pregled i prikaz liste radnih naloga za teren
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7

---

### PB-36

- **Naziv Stavke:** Ažuriranje statusa tiketa
- **Opis:** Implementirati funkcionalnost koja tehničaru omogućuje promjenu statusa tiketa te njegovo konačno zatvaranje nakon što se problem riješi
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8

---

### PB-37

- **Naziv Stavke:** Tehničar vidi osnovne informacije o korisniku
- **Opis:** Implementirati mogućnost za tehničara da vidi osnovne informacije o korisniku koji je prijavio problem. To podrazumijeva ime, prezime, adresu, broj telefona, pakete, pretplate, instalirane uređaje
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7

---

### PB-38

- **Naziv Stavke:** Izvještaj o broju tiketa
- **Opis:** Implementirati mogućnost za izračunavanje i prikaz ukupnog broja tiketa po vremenskom periodu (dnevno, sedmično, mjesečno)
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano za admina: ukupan broj tiketa (KPI na dashboardu) + on-demand `TICKET_COUNT` izvještaj s agregatom i bucket razreakom po danu/sedmici/mjesecu. US-42 (tehničar) — out of scope po dogovoru.

---

### PB-39

- **Naziv Stavke:** Izvještaj po statusu tiketa
- **Opis:** Implementirati mogućnost za izračunavanje i prikaz izvještaja za broj tiketa po statusima (u nekom vremenskom opsegu) i procentualni udio
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano za admina: `statusBreakdown` na dashboardu, pie chart, on-demand `TICKET_STATUS` s postocima i drill-down, upozorenje za veliki period. US-44 (tehničar) — out of scope po dogovoru.

---

### PB-40

- **Naziv Stavke:** Izvještaj po tipu problema
- **Opis:** Implementirati mogućnost za izračunavanje i prikaz izvještaja za analizu tiketa po predefinisanim kategorijama problema
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano za admina: top kategorije na dashboardu, bar chart s drill-down, on-demand `PROBLEM_TYPE`. US-46 (tehničar) — out of scope po dogovoru.

---

### PB-41

- **Naziv Stavke:** Prosječno vrijeme rješavanja tiketa
- **Opis:** Implementirati mogućnost za izračunavanje i prikaz prosječnog vremena od kreiranja do zatvaranja tiketa
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano za admina: KPI `avgResolutionHours` na dashboardu + novi on-demand `AVG_RESOLUTION` tip izvještaja s agregatom i bucket tabelom (dan/sedmica/mjesec). US-48 (tehničar) — out of scope po dogovoru.

---

### PB-42

- **Naziv Stavke:** Statistika agenta i tehničara
- **Opis:** Implementirati sekciju na profilnoj stranici gdje agent i tehničar mogu vidjeti svoju ličnu statistiku rada: broj otvorenih i zatvorenih tiketa, broj tiketa koji čekaju zatvaranje, prosječno vrijeme prvog odgovora, prosječno vrijeme rješavanja tiketa i prosječnu ocjenu korisnika (samo za agente)
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8
- **Napomena:** Redefinisano u Sprint 8 planiranju. Implementirano: backend `GET /api/user/my-statistics` endpoint, dedikirana stranica `/statistics`, te kondenzovani prikaz statistike i nedavnih tiketa na Dashboard-u za AGENT i TECHNICIAN role. Originalna metrika za admin izvještaje evidentirana je kao PB-50.

---

### PB-43

- **Naziv Stavke:** Izvještaj o opterećenju agenata
- **Opis:** Implementirati mogućnost za detaljan izvještaj o broju riješenih tiketa po danu | sedmici | mjesecu za sve agente
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano za admina: ukupan broj riješenih po agentu/tehničaru (`TEAM_WORKLOAD`, top lista na dashboardu) + pivot tabela period × agent s razreakom po danu/sedmici/mjesecu.

---

### PB-44

- **Naziv Stavke:** Izvještaj o ocjenama korisnika
- **Opis:** Implementirati mogućnost feature za analizu ocjena koje korisnici daju nakon zatvaranja tiketa
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano za admina: KPI prosječne ocjene na dashboardu, on-demand `USER_RATINGS` (prosjek, broj, distribucija po zvjezdicama) + trend tabela po pod-periodima (dan/sedmica/mjesec).

---

### PB-45

- **Naziv Stavke:** Admin Dashboard sa ključnim metrikama
- **Opis:** Dizajnirati i implementirati admin dashboard gdje se prikazuju ključne metrike sistema i mogućnost generisanja dostupnih izvještaja
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano: proširenje `/dashboard` za `ADMINISTRATOR`, `GET /api/admin/dashboard`, globalni filter (sedmica/mjesec/godina/custom), KPI kartice, grafovi, on-demand izvještaji (`POST /api/reports/generate`), drill-down na `/tickets`. Vizualni redizajn i AI panel integracija završeni u Sprint 10 (PB-58, PB-59). CSV Export implementiran u Sprint 11 (PB-46).

---

### PB-46

- **Naziv Stavke:** Export izvještaja
- **Opis:** Implementirati mogućnost za izvoz izvještaja u CSV formatu
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 11
- **Napomena:** Implementirano (Sprint 11): client-side CSV generisanje u browseru bez novog backend endpointa; Export dugme uvijek aktivno na stranici Izvještaji; klik fetchuje svježe podatke putem postojećeg `POST /api/reports/generate`; CSV sadrži metadata header (naziv izvještaja, period, datum exporta) + tabularni podaci; UTF-8 BOM za ispravno otvaranje u Excelu; preuzimanje kao `report.csv`; podržano svih 7 tipova izvještaja (TICKET_COUNT, TICKET_STATUS, PROBLEM_TYPE, TEAM_WORKLOAD, USER_RATINGS, FIRST_RESPONSE, AVG_RESOLUTION); loading spinner tokom fetcha.

---

### PB-47

- **Naziv Stavke:** FAQ segment
- **Opis:** Napraviti, dokumentovati i implementirati listu često postavljanih pitanja i odgovora
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 6

---

### PB-48

- **Naziv Stavke:** Pregled historije dodijeljenih tiketa za agente
- **Opis:** Implementirati funkcionalnost koja omogućava agentu da vidi kompletnu historiju svih tiketa koji su mu bili dodijeljeni, uključujući trenutno aktivne (otvorene) tikete kao i sve prethodno uspješno riješene ili zatvorene tikete. Pregled treba sadržavati osnovne informacije o tiketu uz mogućnost filtriranja po statusu i vremenskom periodu.
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 7

---

### PB-49

- **Naziv Stavke:** Notifikacije
- **Opis:** Implementirati sistem notifikacija koji automatski obavještava korisnike o relevantnim događajima na tiketima (dodjela, prosljeđivanje, nova poruka, promjena statusa, zatvaranje). Infrastruktura (entitet `Notification`, `NotificationType` enum, repozitorij, servis i kontroler kao skeleton) pripremljena je u Sprint 7 — potrebno je implementirati poslovnu logiku i frontend prikaz.
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 8
- **Napomena:** Implementirano: backend generisanje notifikacija za sve predviđene događaje (`TICKET_ASSIGNED`, `TICKET_FORWARDED`, `TICKET_RESPONSE`, `TICKET_CLOSED`, `STATUS_CHANGED`), real-time isporuka putem SignalR (`NotificationHub`), API endpointi za dohvat i označavanje pročitanog, frontend bell ikona s badge-om u headeru, dropdown s 5 najnovijih notifikacija, zasebna stranica `/notifications`, link u Sidebaru, klik na notifikaciju otvara odgovarajući tiket (role-based path), EF migracija `AddTicketIdToNotification`.

---

### PB-50

- **Naziv Stavke:** Prosječno vrijeme prvog odgovora — izvještaj za admina
- **Opis:** Implementirati mogućnost za izračunavanje i prikaz prosječnog vremena do prvog odgovora agenta ili tehničara po vremenskom periodu (dnevno, sedmično, mjesečno), kao metrika za admin dashboard. Podaci za izračun već postoje u bazi (`Comment.DateTime` i `Ticket.CreatedDate`) pa nema potrebe za dodatnom migracijom.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano: KPI `avgFirstResponseMinutes` na dashboardu + on-demand `FIRST_RESPONSE` izvještaj s agregatom i bucket trendom po pod-periodima (sedmica→dan, mjesec→sedmica, godina→mjesec, custom→auto).

---

### PB-51

- **Naziv Stavke:** Upravljanje korisničkim nalozima
- **Opis:** Implementirati administratorske funkcionalnosti za kreiranje, pregled, uređivanje, deaktivaciju i reaktivaciju korisničkih naloga za klijente, agente i tehničare. Sistem mora podržavati validaciju jedinstvenosti emaila i minimalne sigurnosne zahtjeve za lozinku, sprječavanje izmjene role postojećeg korisnika, evidenciju izmjena u audit log, te zabranu prijave deaktiviranim korisnicima.
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano: backend CRUD endpointi (`/api/users`, `/api/users/{id}`, deaktivacija/reaktivacija), role-based authorization, audit log evidencija svih izmjena, sigurnost (deaktivirani korisnici dobijaju 401 pri prijavi); frontend lista korisnika sa pretragom, filtrima i detaljnim prikazom; statistički prikaz za agente i tehničare umjesto paketa. Pokriveno automatskim testovima (68 backend + 10 frontend testova).

---

### PB-52

- **Naziv Stavke:** Upravljanje katalogom paketa i pretplata
- **Opis:** Implementirati administratorske funkcionalnosti za definisanje i uređivanje kataloga paketa (Internet, TV, mobilni) sa validacijom naziva i cijene, te dodjelu i ukidanje pretplata klijentima. Sistem mora sprječavati duplikatne aktivne pretplate na isti paket, evidentirati svaku promjenu u audit log i osigurati da klijent vidi ažurirane pakete na svom profilu odmah nakon promjene.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano: novi entiteti `CatalogPackage`, `ClientSubscription`, `SubscriptionAuditLog`; EF migracija `AddCatalogPackagesAndSubscriptions`; REST API kontroleri (`PackageCatalogController`, `ClientSubscriptionController`); administratorski UI (`PackageManagement.jsx`, `ClientSubscriptionsSection.jsx`); 409 Conflict za duplikate. Funkcionalnost je verifikovana manualno kroz UI; automatizovani testovi su evidentirani kao tehnički dug za naredni sprint.
- **Razlog za Partial:** Sva poslovna logika i UI su implementirani, ali nedostaju automatizovani unit/integracijski testovi. Acceptance kriterij testne pokrivenosti nije ispunjen.

---

### PB-53

- **Naziv Stavke:** Pregled audit log-a aktivnosti
- **Opis:** Implementirati administratorsku funkcionalnost za pregled historije ključnih akcija u sistemu (prijava korisnika, kreiranje/zatvaranje/prosljeđivanje tiketa, izmjena korisničkih naloga, izmjena paketa, dodjela pretplate). Audit log mora biti paginirano sortiran po vremenu, omogućavati filtriranje po tipu akcije, korisniku i vremenskom periodu, te pretragu po opisu. Sistem ne smije dozvoliti izmjenu ili brisanje zapisa.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano: backend `AuditLogService` + `AuditLogController` sa paginiranom listom, kombinovanim filterima i pretragom; frontend stranica audit log-a sa filterima, tabelom i modalom za detalje. Pristup ograničen na ADMINISTRATOR (403 za ostale role). Pokriveno automatskim testovima (10 backend + 52 frontend testa).

---

### PB-56

- **Naziv Stavke:** Prilozi na tiketima
- **Opis:** Implementirati upload i preuzimanje priloga (slika i dokumenata) na tikete i poruke. Sistem mora podržavati formate PNG, JPG, JPEG, PDF, DOCX i TXT, ograničiti veličinu pojedinačnog priloga na 5 MB i maksimalan broj priloga na 5 po tiketu/poruci, zabraniti upload izvršnih fajlova (.exe, .bat, .sh), sanitizirati nazive fajlova i prikazivati thumbnail za slike. Sistem ne smije dozvoliti brisanje priloga ni pristup prilozima korisnicima bez prava pregleda tiketa.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 9
- **Napomena:** Implementirano: backend entitet `Attachment`, `AttachmentService` sa whitelist validacijom, `AttachmentController` (upload/list/download) sa role-based autorizacijom, EF migracija `Pb56_AttachmentUserId`; frontend `FileUpload` (progress bar, error display) i `AttachmentList` (thumbnail + lightbox za slike, ikone i preuzimanje za dokumente) integrirani u kreiranje tiketa i poruke. Pokriveno automatskim testovima (27 backend + 16 frontend testova).

---

### PB-57

- **Naziv Stavke:** AI prijedlog odgovora za agente i tehničare
- **Opis:** Implementirati AI-potpomognutu funkcionalnost koja agentima i tehničarima predlaže odgovor na tiket na osnovu tipa problema i historije komunikacije. Sistem koristi internu knowledge base telekomunikacijskih rješenja (Internet, TV, mobilna mreža, naplata, tehnička podrška). Agent ili tehničar može prihvatiti, urediti ili odbaciti prijedlog — slanje ostaje eksplicitna radnja korisnika.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10
- **Napomena:** Implementirano: backend `IAIService`/`AIService` s metodom `GetAgentSuggestionAsync`, `AIController` (POST `/api/ai/agent-suggestion`), DTOs (`AgentSuggestionRequestDto`, `AgentSuggestionResponseDto`), interna knowledge base za 6 kategorija problema; frontend `AISuggestionModal.jsx` s dugmetom „Kopiraj u poruku", `aiService.js` API layer, integracija u `TicketDetail.jsx` (samo za AGENT i TECHNICIAN role).

---

### PB-58

- **Naziv Stavke:** AI uvidi za administratore
- **Opis:** Implementirati AI-generisane uvide o stanju helpdesk sistema za administratore na admin dashboardu. Uvidi moraju pružati inteligentnu analizu na osnovu trenutnih metrika (broj tiketa, statusi, prosječna rješavanja, ocjene korisnika, opterećenje agenata) i sadržavati sažetak stanja, identifikovane trendove i preporuke za akciju.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10
- **Napomena:** Implementirano: backend `AIService.GetAdminInsightsAsync`, `AIController` (POST `/api/ai/admin-insights`), DTOs (`AdminInsightsRequestDto`, `AdminInsightsResponseDto`); frontend `AIInsightsPanel.jsx` i `AIInsightsCard.jsx` komponente, „AI Uvidi" dugme u `Header.jsx` (vidljivo samo administratorima), inline panel ispod KPI kartica u `AdminDashboardSection.jsx`, `uiStore.js` Zustand store za dijeljeno stanje panela između Headera i dashboarda.

---

### PB-59

- **Naziv Stavke:** Redizajn korisničkog sučelja
- **Opis:** Izvršiti kompletni vizualni revamp korisničkog sučelja sistema s fokusom na moderan, pregledan dizajn inspirisan profesionalnim dashboard layoutima. Redizajn obuhvata: novi color scheme s tamnom navy paletom, world-class sidebar s navigacijom i statusnim chipom, redesigniran header s pretragom i notifikacijama, te poboljšan admin dashboard s trend indikatorima na stat karticama i key highlights sekcijom.
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** L
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10
- **Napomena:** Implementirano: `Sidebar.jsx` — `bg-[#f0f2f5]`, navy-800 logo i avatar, aktivne stavke s navy-50, status chip (amber/zeleni) s navigacijom na filtrirane tikete; `Header.jsx` — `bg-[#f4f6f9]`, desktop search bar, AI Uvidi dugme, notifikacijski dropdown; `AppLayout.jsx` — konzistentne boje i prošireni pageTitles; `AdminDashboardSection.jsx` — StatCard s ikonom/trendom/brojem/labelom, dismissabilni alert banner (u cijelosti klikabilan), inline AI panel, key highlights sekcija; `uiStore.js` — novi Zustand store za `aiPanelOpen` i `alertTicketCount/Url`; `index.css` — navy palette s realnim tamnim hex vrijednostima (navy-800: `#162d58`).

---

### PB-60

- **Naziv Stavke:** Interni komentari na tiketima
- **Opis:** Implementirati funkcionalnost za dodavanje internih komentara na tikete koji su vidljivi isključivo osoblju (agenti, tehničari, administratori), a potpuno skriveni od klijenata. Interni komentari prikazuju se u hronološkom toku razgovora uz jasnu vizualnu razliku od regularnih poruka (drugačija boja, labela „Interno").
- **Tip Stavke:** Feature
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10

---

### PB-61

- **Naziv Stavke:** Admin CRUD FAQ
- **Opis:** Implementirati administratorske funkcionalnosti za kreiranje, uređivanje i brisanje FAQ stavki direktno iz sučelja sistema, bez potrebe za intervencijom u kodu ili bazi podataka. Sistem mora validirati da pitanje i odgovor nisu prazni, zahtijevati potvrdu prije brisanja, te odmah prikazati promjene svim korisnicima. Pristup CRUD operacijama ograničen isključivo na administratore.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10

---

### PB-62

- **Naziv Stavke:** Assign to me — samodjelovanje tiketa
- **Opis:** Implementirati funkcionalnost koja agentu omogućuje preuzimanje nedodijeljenog tiketa jednim klikom, bez potrebe za manualnom dodjelom od strane administratora. Sistem evidentiuje dodjelu u historiji tiketa, šalje notifikacije relevantnim stranama i ne smije dozvoliti preuzimanje tiketa koji je već dodijeljen drugom agentu.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10

---

### PB-63

- **Naziv Stavke:** Agent availability status
- **Opis:** Implementirati funkcionalnost koja agentu omogućuje postavljanje vlastitog statusa dostupnosti (Dostupan, Zauzet, Nedostupan), vidljivog u timskom pregledu i profilu agenta. Kada agent postavi status „Nedostupan" (odmor, van radnog vremena), sistem automatski preraspodjeljuje sve njegove otvorene tikete — svaki zasebno po algoritmu automatske dodjele (najboljim dostupnim agentom). Sistem ne smije dodjeljivati nove tikete odsutnim agentima, status se resetuje na „Dostupan" pri ponovnoj prijavi, a administrator vidi statuse svih agenata s mogućnošću filtriranja.
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** S
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10

---

### PB-64

- **Naziv Stavke:** Linked Tickets — veza između tiketa
- **Opis:** Implementirati mogućnost povezivanja tiketa koji se odnose na isti problem ili su na drugi način međusobno zavisni. Agent ili tehničar može kreirati bidirekcionalnu vezu između tiketa (npr. „duplikat od", „nastavak od", „vezan uz"), pregledati linked tikete direktno iz detalja tiketa i ukloniti vezu. Sistem mora spriječiti ciklične veze i samopovezivanje tiketa.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Deferred
- **Veza sa sprintom ili release planom:** —
- **Napomena:** Odgođeno iz Sprinta 11 — nije implementirano, nije u scope završnog sprinta.

---

### PB-65

- **Naziv Stavke:** SLA praćenje i upozorenja
- **Opis:** Implementirati praćenje SLA rokova (Service Level Agreement) za tikete na osnovu prioriteta i tipa problema. Sistem mora izračunavati preostalo SLA vrijeme, vizualno upozoravati agente i administratore kada se SLA rok bliži ili je prekoračen, evidentirati SLA breacheve u statistici i spriječiti automatsku dodjelu tiketa agentima čiji bi workload prekršio SLA.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 11

---

### PB-66

- **Naziv Stavke:** Bulk akcije na tiketima
- **Opis:** Implementirati mogućnost masovnog upravljanja tiketima (zatvaranje, promjena prioriteta, dodjela agentu, prosljeđivanje timu) kroz checkboxe na listi tiketa. Bulk akcije dostupne su isključivo administratorima i agentima, sistem mora tražiti potvrdu za destruktivne operacije i prikazati sažetak rezultata nakon izvršavanja.
- **Tip Stavke:** Feature
- **Prioritet:** 3
- **Procjena složenosti ili napora:** M
- **Status:** Deferred
- **Veza sa sprintom ili release planom:** —
- **Napomena:** Odgođeno iz Sprinta 11 — nije implementirano, nije u scope završnog sprinta.

---

### PB-67

- **Naziv Stavke:** Login via broj telefona
- **Opis:** Proširiti autentikaciju klijenta na mogućnost prijave putem broja telefona umjesto emaila. Sistem mora podržavati unos broja u međunarodnom formatu, validirati jedinstvenost broja i dozvoliti korisniku da se prijavi s bilo kojim od dva identifikatora (email ili telefon) uz istu lozinku.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 11

---

### PB-70

- **Naziv Stavke:** MCP Admin Copilot
- **Opis:** Implementirati administratorski chat interfejs koji koristi Model Context Protocol (MCP) za kontrolisano čitanje živih podataka iz sistema. Administrator može postavljati pitanja slobodnim tekstom, a sistem preko MCP alata dohvaća relevantne podatke iz tiketa, timova, FAQ sadržaja i postojećih report/admin metrika. AI sloj ne smije izmišljati podatke, nego samo tumači strukturirane rezultate MCP alata, formatira odgovor, prikazuje izvore i predlaže narednu administratorsku akciju.
- **Tip Stavke:** Feature
- **Prioritet:** 2
- **Procjena složenosti ili napora:** XL
- **Status:** Done
- **Veza sa sprintom ili release planom:** Sprint 10
- **Napomena:** Implementirano (Sprint 10): MCP server (`Project/mcp-server`, TypeScript + zvanični `@modelcontextprotocol/sdk`, Streamable HTTP) s read-only alatima `ticket.search`/`ticket.analytics`/`team.workload`/`faq.search`; backend `AdminCopilotController` + `AdminCopilotService`; frontend `AdminCopilotPanel`/`AdminCopilotMessage`; Docker Compose integracija. Scope US-108, US-109, US-110, US-111.

---
