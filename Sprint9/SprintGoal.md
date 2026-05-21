# Sprint Goal – Sprint 9

## Sprint cilj

Cilj sprinta je implementirati administratorski dio sistema kroz razvoj centralizovanog admin dashboarda sa ključnim operativnim i analitičkim metrikama, omogućiti upravljanje korisničkim nalozima, katalogom paketa i pretplata, te implementirati audit log sistem za praćenje aktivnosti unutar aplikacije. Dodatno, sprint uključuje razvoj sistema priloga na tiketima kako bi se unaprijedila komunikacija između klijenata, agenata i tehničara.

Sprint je fokusiran na proširenje sistema prema administrativnim, analitičkim i nadzornim funkcionalnostima koje omogućavaju bolju kontrolu nad radom helpdesk sistema, pregled performansi timova i praćenje aktivnosti korisnika kroz centralizovane administratorske alate. Poseban fokus stavljen je na razvoj dashboard i reporting modula koji administratorima omogućavaju detaljan uvid u ključne KPI metrike, trendove rada sistema i stanje tiketa kroz različite vremenske periode.

Implementacijom administratorskog dashboarda sistem dobija centralizovan pregled stanja aplikacije, uključujući pregled otvorenih i zatvorenih tiketa, raspodjelu statusa, prosječna vremena odgovora i rješavanja, opterećenje agenata, kao i pregled najčešćih tipova problema. Dashboard funkcionalnosti omogućavaju administratorima donošenje bržih odluka i lakše praćenje performansi sistema kroz agregirane podatke i vizualne prikaze.

Poseban fokus sprinta stavljen je na implementaciju fleksibilnog reporting sistema koji omogućava generisanje izvještaja za različite vremenske periode, filtriranje podataka i drill-down pregled konkretnih tiketa povezanih sa određenim metrikama. Time se omogućava detaljnija analiza rada sistema i identifikacija potencijalnih problema ili uskih grla u radu agenata i tehničara.

Dodatni cilj sprinta jeste omogućiti administratorima upravljanje korisničkim nalozima i paketima bez direktnog pristupa bazi podataka, čime se unapređuje sigurnost, organizacija i održavanje sistema. Implementacijom administratorskih CRUD funkcionalnosti omogućava se jednostavnije upravljanje zaposlenicima, klijentima, paketima i pretplatama kroz centralizovani interfejs aplikacije.

Sprint također uvodi audit log mehanizam koji omogućava praćenje ključnih aktivnosti u sistemu radi sigurnosti, traceability-ja i lakše administracije. Audit log treba omogućiti pregled aktivnosti korisnika, praćenje izmjena nad tiketima, korisničkim nalozima i pretplatama, te pružiti osnovu za buduće sigurnosne i compliance funkcionalnosti.

Poseban dio sprinta odnosi se na razvoj funkcionalnosti priloga na tiketima i porukama kako bi korisnici, agenti i tehničari mogli razmjenjivati slike i dokumente prilikom prijave i rješavanja problema. Time se unapređuje kvalitet komunikacije između korisnika i podrške, olakšava dokumentovanje problema i omogućava efikasnije rješavanje tehničkih zahtjeva.

Sprint također obuhvata proširenje backend logike za dashboard i izvještaje, optimizaciju API endpointa, validaciju administratorskih privilegija i unapređenje autorizacije po korisničkim rolama. Fokus sprinta stavljen je i na stabilnost sistema, integraciju frontend i backend modula, kao i proširenje test coverage-a za nove administratorske i reporting funkcionalnosti.

Dodatno, sprint predstavlja pripremu sistema za buduće funkcionalnosti poput exporta izvještaja, naprednih analitičkih dashboarda, AI-driven preporuka i proširenih sigurnosnih mehanizama koji će biti implementirani u narednim sprintovima projekta.

---

## Fokus sprinta

- Implementacija administratorskog dashboarda
- KPI metrike i operativni pregled sistema
- Reporting i generisanje izvještaja
- Globalni vremenski filteri za dashboard i izvještaje
- Drill-down pregled povezanih tiketa
- Pregled prosječnog vremena prvog odgovora
- Upravljanje korisničkim nalozima
- Aktivacija i deaktivacija korisnika
- Upravljanje katalogom paketa i pretplata
- Dodjela i ukidanje pretplata klijentima
- Implementacija audit log sistema
- Praćenje aktivnosti i sigurnosnih događaja
- Upload priloga na tiketima i porukama
- Pregled i preuzimanje priloga
- Validacija pristupa administratorskim funkcionalnostima
- Refaktorisanje backend logike za izvještaje i dashboard
- Proširenje autorizacije po korisničkim rolama
- Integracija frontend i backend administratorskih modula
- Optimizacija dashboard i reporting upita
- Implementacija i proširenje unit testova
- Stabilizacija administratorskog i reporting sistema

---

## Očekivani deliverable-i

- PB-45 Admin Dashboard sa ključnim metrikama
- PB-50 Prosječno vrijeme prvog odgovora (admin izvještaj)
- PB-51 Upravljanje korisničkim nalozima
- PB-52 Upravljanje katalogom paketa i pretplata
- PB-53 Pregled audit log-a aktivnosti
- PB-56 Prilozi na tiketima
- Implementirani dashboard KPI prikazi
- Implementirani reporting endpointi i filteri
- Implementirani audit log mehanizmi
- Implementirano upravljanje korisnicima i pretplatama
- Implementiran upload i pregled priloga
- Implementirana autorizacija administratorskih ruta
- Refaktorisani API endpointi i dashboard servisi
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalnost administratorskog dashboarda, ispravnost izvještaja i metrika, kvalitet implementacije administratorskih funkcionalnosti, kao i stabilnost sistema priloga i audit log mehanizama.

Review uključuje provjeru:
- Ispravnosti dashboard KPI metrika
- Ispravnosti generisanja izvještaja
- Funkcionalnosti vremenskih filtera
- Funkcionalnosti drill-down pregleda tiketa
- Ispravnosti prosječnog vremena prvog odgovora
- Upravljanja korisničkim nalozima
- Upravljanja paketima i pretplatama
- Ispravnosti audit log sistema
- Funkcionalnosti upload-a i pregleda priloga
- Validacije pristupa administratorskim funkcionalnostima
- Stabilnosti frontend i backend integracije
- Stabilnosti dashboard i reporting API endpointa
- Pokrivenosti testovima i stabilnosti aplikacije
- Spremnosti sistema za buduće reporting i export funkcionalnosti

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije dashboard sistema
- Dogovor oko KPI metrika i reporting logike
- Review sastanci za frontend i backend integraciju
- Sastanci vezani za audit log i sigurnosne funkcionalnosti
- Koordinacija implementacije administratorskih modula
- Tehnički sastanci za organizaciju API endpointa
- Sastanci vezani za priloge i upload funkcionalnosti
- Dogovor oko autorizacije i pristupa administratorskim podacima
- Koordinacija implementacije izvještaja i filter sistema
- Review sastanci za dashboard i reporting UI
- Sastanci vezani za optimizaciju backend upita i performansi
- Koordinacija implementacije audit log praćenja aktivnosti
