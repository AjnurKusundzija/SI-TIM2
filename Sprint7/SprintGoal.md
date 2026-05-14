# Sprint Goal – Sprint 7

## Sprint cilj

Cilj sprinta je unaprijediti workflow upravljanja tiketima kroz implementaciju automatske dodjele tiketa, upravljanja prioritetima, proširenja funkcionalnosti za agente i tehničare, te unapređenje statusnog i životnog ciklusa tiketa.

Sprint je fokusiran na razvoj naprednijeg agent/tehničar workflow-a kako bi sistem omogućio efikasniju raspodjelu zadataka, bolju organizaciju rada tehničara i preciznije upravljanje prioritetima i statusima tiketa. Poseban fokus stavljen je na backend logiku automatske dodjele tiketa, validaciju dostupnosti tehničara i routing sistema zasnovanog na kategorijama problema i opterećenju timova.

Također, sprint uključuje proširenje funkcionalnosti za tehničare i agente kroz pregled dodijeljenih tiketa, mogućnost ažuriranja statusa, zatvaranja tiketa i prosljeđivanja tiketa između timova i korisnika sistema. Implementacijom ovih funkcionalnosti omogućava se potpuniji lifecycle upravljanja tiketima unutar sistema podrške.

Dodatni cilj sprinta je unapređenje organizacije ticket workflow-a kroz centralizaciju poslovne logike na backend sistemu, standardizaciju API endpointa i pripremu infrastrukture za buduće funkcionalnosti notifikacija i naprednog praćenja aktivnosti nad tiketima.

Sprint također obuhvata proširenje funkcionalnosti za pregled dodijeljenih tiketa tehničarima, validaciju pristupa podacima po korisničkim rolama, refaktorisanje postojećih ruta i unapređenje frontend prikaza za agente i tehničare.

Poseban fokus sprinta stavljen je na pripremu sistema za notifikacije i daljnje proširenje real-time funkcionalnosti u narednim sprintovima.

---

## Fokus sprinta

- Automatska dodjela tiketa
- Rule-based routing tiketa prema kategoriji problema
- Upravljanje prioritetima tiketa
- Load balancing između agenata i tehničara
- Validacija AvailabilityStatus logike
- Pregled dodijeljenih tiketa za tehničare
- Pregled tiketa za agente
- Prosljeđivanje tiketa između agenata i tehničara
- Ažuriranje statusa tiketa
- Zatvaranje tiketa
- Refaktorisanje backend workflow logike
- Unapređenje autorizacije i pristupa ticket podacima
- Integracija frontend i backend ticket funkcionalnosti
- Standardizacija API ruta i endpointa
- Priprema infrastrukture za sistem notifikacija
- Implementacija i proširenje unit testova
- Stabilizacija agent/tehničar workflow sistema

---

## Očekivani deliverable-i

- PB-25 Zatvaranje tiketa
- PB-28 Upravljanje prioritetima tiketa
- PB-30 Automatska dodjela tiketa
- PB-36 Ažuriranje statusa tiketa
- PB-37 Pregled dodijeljenih tiketa za tehničare
- PB-48 Prosljeđivanje tiketa i pregled historije aktivnosti
- Implementirana backend logika za automatski routing tiketa
- Implementiran load balancing sistem dodjele
- Implementirana validacija AvailabilityStatus logike
- Implementirani endpointi za upravljanje statusima tiketa
- Implementirane funkcionalnosti za agente i tehničare
- Refaktorisani API endpointi i frontend rute
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti
- Stabilizovan agent/tehničar ticket workflow modul

---

## Sprint Review kriterij

Ocjenjuje se funkcionalnost automatske dodjele tiketa, ispravnost routing logike, kvalitet implementacije agent/tehničar workflow-a i uspješnost testiranja sistema.

Posebna pažnja biće posvećena stabilnosti backend workflow logike, tačnosti dodjele tiketa prema prioritetima i dostupnosti tehničara, kvalitetu frontend-backend integracije i spremnosti sistema za buduće notifikacijske funkcionalnosti.

Review uključuje provjeru:
- Ispravnosti automatske dodjele tiketa
- Tačnosti routing logike i load balancing sistema
- Ispravnosti autorizacije po korisničkim rolama
- Funkcionalnosti pregleda dodijeljenih tiketa
- Funkcionalnosti prosljeđivanja tiketa
- Ispravnosti ažuriranja statusa i zatvaranja tiketa
- Stabilnosti backend workflow sistema
- Kvaliteta frontend i backend integracije
- Stabilnosti API endpointa
- Pokrivenosti testovima i stabilnosti aplikacije
- Spremnosti sistema za notifikacije i naredne sprint funkcionalnosti

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije workflow sistema
- Dogovor oko automatske dodjele tiketa
- Review sastanci za backend i frontend integraciju
- Sastanci vezani za routing i load balancing logiku
- Koordinacija između frontend i backend članova tima
- Review implementacije agent/tehničar funkcionalnosti
- Tehnički sastanci za organizaciju API endpointa
- Dogovor oko strukture ticket workflow logike
- Sastanci vezani za planiranje testiranja sistema
- Koordinacija implementacije prioriteta i statusa tiketa
