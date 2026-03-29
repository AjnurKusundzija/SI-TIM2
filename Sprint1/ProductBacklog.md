# Product Backlog

## Uvod

Ovaj dokument služi za praćenje i upravljanje zadacima u okviru razvoja proizvoda. Backlog treba biti **živ dokument i redovno ažuriran**.

Za svaki backlog item navodi se: ID, Naziv stavke, Kratak opis, Tip stavke, Prioritet, Procjena složenosti ili napora, Status i Veza sa sprintom ili release planom i po potrebi napisati i napomenu za backlog.

### Legenda za oznake

- **Statusi za backlog:** Backlog, To-Do, In Progress, Testing, Done
- **Procjena složenosti:** XS, S, M, L, XL
- **Oznaka prioriteta:** 1, 2, 3, 4, 5 (1 je najbitnije, 5 je najmanje bitno)
- **Tipovi stavki:** Feature, Bug, Dokumentacija, Research, Technical Task

---

## Tabelarni prikaz Backloga

| ID                | Naziv stavke                                   | Tip stavke                     | Prioritet | Složenost | Status  | Sprint   |
| :---------------- | :--------------------------------------------- | :----------------------------- | :-------: | :-------: | :------ | :------- |
| [SP2-01](#sp2-01) | Definisati Acceptance Criteria                 | Dokumentacija                  |     2     |     S     | To-Do   | Sprint 2 |
| [SP2-02](#sp2-02) | Definisanje User Stories                       | Dokumentacija                  |     1     |    XS     | To-Do   | Sprint 2 |
| [SP2-03](#sp2-03) | Definisati listu NFR zahtjeva                  | Dokumentacija                  |     2     |     M     | To-Do   | Sprint 2 |
| [SP3-01](#sp3-01) | Kreirati Risk Register                         | Dokumentacija                  |     1     |     S     | Backlog | Sprint 3 |
| [SP3-02](#sp3-02) | Izraditi Domain Model                          | Dokumentacija                  |     1     |     M     | Backlog | Sprint 3 |
| [SP3-03](#sp3-03) | Izraditi Use Case Model                        | Dokumentacija                  |     1     |     M     | Backlog | Sprint 3 |
| [SP3-04](#sp3-04) | Izraditi Architecture Overview                 | Dokumentacija                  |     1     |     M     | Backlog | Sprint 3 |
| [SP3-05](#sp3-05) | Definisati Test Strategy                       | Dokumentacija                  |     1     |    ML     | Backlog | Sprint 3 |
| [SP4-01](#sp4-01) | Definisati Definition of Done                  | Dokumentacija                  |     1     |     M     | Backlog | Sprint 4 |
| [SP4-02](#sp4-02) | Kreirati Initial Release Plan                  | Dokumentacija                  |     1     |     M     | Backlog | Sprint 4 |
| [SP4-03](#sp4-03) | Uspostaviti osnovni skeleton projekta          | Technical Task / Dokumentacija |     2     |     S     | Backlog | Sprint 4 |
| [SP4-04](#sp4-04) | Postaviti inicijalnu strukturu repozitorija... | Technical Task / Dokumentacija |     2     |     L     | Backlog | Sprint 4 |
| [SP5-01](#sp5-01) | Uspostava AI Usage Loga                        | Dokumentacija                  |     1     |    XS     | Backlog | Sprint 5 |
| [SP5-02](#sp5-02) | Uspostava Decision Loga                        | Dokumentacija                  |     1     |    XS     | Backlog | Sprint 5 |

---

## Detalji Backlog stavki

### SP2-01

- **Naziv Stavke:** Definisati Acceptance Criteria
- **Opis:** Za svaki User Story definisati jasne i mjerljive uslove koje funkcionalnost mora zadovoljiti kako bi bila smatrana gotovom
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 2

---

### SP2-02

- **Naziv Stavke:** Definisanje User Stories
- **Opis:** Kreirati strukturisanu listu User Stories, iz perspektive krajnjeg korisnika
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** To-Do
- **Veza sa sprintom ili release planom:** Sprint 2

---

### SP2-03

- **Naziv Stavke:** Definisati listu NFR zahtjeva
- **Opis:** Identifikovati i dokumentovati nefunkcionalne zahtjeve sistema (brzina učitavanja, broj istovremenih korisnika, sigurnost)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** M
- **Status:** To-Do -**Veza sa sprintom ili release planom:** Sprint 2

### SP3-01

- **Naziv Stavke:** Kreirati Risk Register
- **Opis:** Potrebno je napraviti dokument u kojem se procijenjuju sve prijetnje i rizici prije, tokom i poslije implementacije projekta
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-02

- **Naziv Stavke:** Izraditi Domain Model
- **Opis:** Potrebno je izradii dokument Domain model u kojem se nalazi reprezentacija specificnog problema u nasem domenu. Model se koristi kao most izmedju stakeholdera i developera. Definisati ključne entitete sistema njihove atribute, ponašanja i međusobne veze
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-03

- **Naziv Stavke:** Izraditi Use Case Model
- **Opis:** Definisati funkcionalne zahtjeve sistema kroz UML Use Case dijagrame
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-04

- **Naziv Stavke:** Izraditi Architecture Overview
- **Opis:** Dokumentovati arhitekturu sistema. Prikazati ključne komponente, njihove veze, i odgovornosti
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP3-05

- **Naziv Stavke:** Definisati Test Strategy
- **Opis:** Dokumentovati pristup testiranju (vrste testova, odgovornosti, alate)
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 3

---

### SP4-01

- **Naziv Stavke:** Definisati Definition of Done
- **Opis:** Kreirati i dokumentovati zajednički dogovoreni skup kriterija koje svaki product increment mora zadovoljiti prije nego što se smatra završenim
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP4-02

- **Naziv Stavke:** Kreirati Initial Release Plan
- **Opis:** Napraviti pregled planiranih isporuka funkcionalnosti
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** M
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP4-03

- **Naziv Stavke:** Uspostaviti osnovni skeleton projekta
- **Opis:** Kreirati minimalnu, ali funkcionalnu strukturu foldera
- **Tip Stavke:** Technical Task / Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** S
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP4-04

- **Naziv Stavke:** Postaviti inicijalnu strukturu repozitorija i osnovni tehnički setup
- **Opis:** Kreirati repozitorij i konfigurisati razvojno okruženje
- **Tip Stavke:** Technical Task / Dokumentacija
- **Prioritet:** 2
- **Procjena složenosti ili napora:** L
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 4

---

### SP5-01

- **Naziv Stavke:** Uspostava AI Usage Loga
- **Opis:** Kreirati i održavati dokument u kojem se bilježi svako korištenje AI alata tokom free AI usage faze razvoja softvera
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 5

---

### SP5-02

- **Naziv Stavke:** Uspostava Decision Loga
- **Opis:** Napraviti Decision Log dokument koji se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka. Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.
- **Tip Stavke:** Dokumentacija
- **Prioritet:** 1
- **Procjena složenosti ili napora:** XS
- **Status:** Backlog
- **Veza sa sprintom ili release planom:** Sprint 5
