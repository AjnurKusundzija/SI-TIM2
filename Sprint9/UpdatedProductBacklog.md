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
| [PB-29](#pb-29) | Preraspodjela agenata po timovima                  | Feature        | 1         | M         | Backlog | Sprint 10 |
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
| [PB-46](#pb-46) | Export izvještaja                                  | Feature        | 5         | S         | Backlog | Sprint 11 |
| [PB-47](#pb-47) | FAQ segment                                        | Feature        | 3         | S         | Done    | Sprint 6  |
| [PB-48](#pb-48) | Pregled historije dodijeljenih tiketa za agente    | Feature        | 1         | M         | Done    | Sprint 7  |
| [PB-49](#pb-49) | Notifikacije                                       | Feature        | 1         | L         | Done    | Sprint 8  |
| [PB-50](#pb-50) | Prosječno vrijeme prvog odgovora (admin izvještaj) | Feature        | 2         | S         | Done    | Sprint 9  |


---

## Modul izvještaja — status zatvaranja PB-ova


| PB        | Status                                                                                                                |
| --------- | --------------------------------------------------------------------------------------------------------------------- |
| **PB-38** | ✅ Done — `TICKET_COUNT` vraća ukupan broj + bucket razbreak (dan/sedmica/mjesec) po periodu kreiranja                |
| **PB-39** | ✅ Done — `TICKET_STATUS` s breakdown po statusima, postocima, pie chartom i drill-down na `/tickets`                 |
| **PB-40** | ✅ Done — `PROBLEM_TYPE` s bar chartom i drill-down po kategoriji                                                     |
| **PB-41** | ✅ Done — `AVG_RESOLUTION` on-demand izvještaj s agregatom i bucket tabelom (dan/sedmica/mjesec)                      |
| **PB-43** | ✅ Done — `TEAM_WORKLOAD` vraća ukupne zbirove po agentu + pivot tabelu period × agent                                |
| **PB-44** | ✅ Done — `USER_RATINGS` s prosječnom ocjenom, distribucijom po zvjezdicama i bucket trendom po pod-periodima         |
| **PB-45** | ✅ Done — Admin Dashboard s KPI karticama, grafovima, globalnim filterom i drill-down                                 |
| **PB-50** | ✅ Done — `FIRST_RESPONSE` na dashboardu (agregat) + on-demand izvještaj s bucket trendom po pod-periodima            |
| **PB-46** | ⏳ Backlog (Sprint 11) — CSV export; disabled dugme placeholder je implementirano                                     |


**Napomena:** Izvještaji za **tehničara** (US-42, US-44, US-46, US-48) su označeni kao *out of scope* — po dogovoru u Sprint 9 modul izvještaja je isključivo za `ADMINISTRATOR`.

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
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 10

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
- **Napomena:** Implementirano: proširenje `/dashboard` za `ADMINISTRATOR`, `GET /api/admin/dashboard`, globalni filter (sedmica/mjesec/godina/custom), KPI kartice, grafovi, on-demand izvještaji (`POST /api/reports/generate`), drill-down na `/tickets`, disabled Export (čeka PB-46). Preostaje: formalno prihvatno testiranje (US-71–US-86), performans test < 5 s na većem datasetu.

---

### PB-46

- **Naziv Stavke:** Export izvještaja
- **Opis:** Implementirati mogućnost za izvoz izvještaja u CSV formatu
- **Tip Stavke:** Feature
- **Prioritet:** 5
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 11
- **Napomena:** Na dashboardu postoji samo disabled dugme „Export“ (placeholder). CSV generisanje i preuzimanje nije implementirano.

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

