# Sprint Review Summary

## Sprint broj
- Sprint 9

## Planirani sprint goal
- Cilj Sprinta 9 bio je unaprijediti administratorske i korisničke funkcionalnosti sistema kroz implementaciju upravljanja korisničkim nalozima, kataloga paketa i pretplata, audit log sistema i podrške za priloge na tiketima. Fokus sprinta bio je na proširenju administratorskih mogućnosti, poboljšanju sigurnosti i praćenja aktivnosti sistema, te unapređenju rada sa tiketima i korisničkim podacima.

- Sprint je također uključivao proširenje sistema kroz upload i pregled priloga na tiketima, validaciju tipova fajlova, upravljanje pretplatama korisnika i implementaciju audit log funkcionalnosti za praćenje svih važnih aktivnosti unutar sistema.

- Poseban fokus sprinta stavljen je na stabilizaciju administratorskih funkcionalnosti, pravilnu autorizaciju po korisničkim rolama i proširenje frontend-backend integracije za nove module sistema.

---

## Sta je zavrseno
- Implementiran PB-51 Upravljanje korisničkim nalozima.
- Implementiran PB-52 Upravljanje katalogom paketa i pretplata.
- Implementiran PB-53 Pregled audit log-a aktivnosti.
- Implementiran PB-56 Prilozi na tiketima.
- Implementiran PB-29 Preraspodjela agenata po timovima.
- Implementirana administratorska kontrola aktivnih i deaktiviranih korisnika.
- Implementirana dodjela i ukidanje pretplata klijentima.
- Implementirana audit log evidencija ključnih aktivnosti sistema.
- Implementirana podrška za upload, pregled i download priloga na tiketima.
- Implementirana validacija tipova i veličine fajlova.
- Implementirani backend endpointi i frontend komponente za nove funkcionalnosti.
- Implementirana role-based autorizacija za administratorske funkcionalnosti.
- Implementirani unit i integracioni testovi za nove module.
- Ažurirani Sprint Backlog, Decision Log, AI Usage Log i Proof of Testing dokumenti.
- Stabilizovana frontend i backend integracija novih funkcionalnosti.

---

## Sta nije zavrseno
- Sve planirane funkcionalnosti Sprinta 9 su uspješno završene i demonstrirane tokom Sprint Review sastanka.
- Nije bilo preostalih PB ili US stavki koje su prebačene u naredni sprint.

---

## Demonstrirane funkcionalnosti ili artefakti
- Upravljanje korisničkim nalozima za administratore.
- Kreiranje, uređivanje, deaktivacija i reaktivacija korisnika.
- Upravljanje katalogom paketa i pretplata korisnika.
- Dodjela i ukidanje pretplata klijentima.
- Pregled aktivnih paketa i pretplata na korisničkom profilu.
- Audit log pregled i filtriranje aktivnosti sistema.
- Upload, pregled i download priloga na tiketima.
- Validacija tipova fajlova i ograničenja veličine priloga.
- Pregled rasporeda timova i preraspodjela agenata.
- Role-based autorizacija novih administratorskih funkcionalnosti.
- Frontend i backend integracija novih modula.
- Unit testovi, integracioni testovi i rezultati testiranja.
- Ažurirana projektna dokumentacija.

---

## Glavni problemi i blokeri
- Merge konflikti tokom integracije većih frontend i backend izmjena.
- Problemi sa migracijama baze podataka i usklađivanjem novih tabela.
- Povremeni bugovi vezani za upload i validaciju priloga.
- Dodatno vrijeme potrebno za stabilizaciju audit log sistema i pretplata korisnika.
- Potreba za dodatnim testiranjem administratorskih funkcionalnosti i role-based autorizacije.

---

## Kljucne odluke donesene u sprintu
- Odlučeno je da se katalog paketa i pretplata implementira kroz novi model odvojen od legacy SubscriptionPackages sistema.
- Audit log sistem implementiran je kao zaseban modul sa read-only pristupom za administratore.
- Administratorske funkcionalnosti ograničene su isključivo na odgovarajuće role kroz dodatnu backend i frontend validaciju.
- Odlučeno je da se upload priloga ograniči na dozvoljene formate i maksimalnu veličinu fajla radi sigurnosti sistema.
- Prikaz deaktiviranih korisnika i reaktivacija implementirani su kao posebna administratorska funkcionalnost.
- Sistem pretplata i korisničkih paketa integrisan je sa audit log sistemom radi praćenja svih promjena.

---

## Povratna informacija Product Ownera
- Product Owner je izrazio zadovoljstvo kvalitetom implementiranih funkcionalnosti i stabilnošću sistema.
- Posebno su pohvaljeni administratorski moduli, audit log funkcionalnosti i organizacija korisničkih pretplata.
- Pozitivno je ocijenjena frontend-backend integracija i kvalitet role-based autorizacije.
- Pohvaljena je i organizacija dokumentacije, testiranja i sprint artefakata.
- Sprint je ocijenjen maksimalnim brojem bodova (100%) bez dodatnih zahtjeva za doradu.

---

## Zakljucak za naredni sprint
- U narednom sprintu fokus će biti na AI funkcionalnostima, redizajnu korisničkog sučelja i dodatnom unapređenju administratorskih mogućnosti.
- Planirano je proširenje sistema AI prijedlozima odgovora, administratorskim AI uvidima i modernizacijom dashboard i layout komponenti.
- Tim će nastaviti sa stabilizacijom sistema, proširenjem test coverage-a i unapređenjem korisničkog iskustva kroz frontend redizajn i nove workflow funkcionalnosti.

Ovaj dokument se piše tek nakon sastanka sa PO.
