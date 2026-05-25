# Sprint Goal – Sprint 9

## Sprint cilj

Cilj sprinta je implementirati administratorski dio sistema kroz razvoj centralizovanog admin dashboarda sa ključnim operativnim i analitičkim metrikama, omogućiti upravljanje korisničkim nalozima, upravljanje katalogom paketa i pretplata, audit log aktivnosti i preraspodjelu agenata po timovima, omogućiti dodavanje priloga na tikete, te implementirati sistem izvještaja za analizu rada helpdesk sistema.

Sprint je fokusiran na proširenje sistema administratorskim i analitičkim funkcionalnostima koje omogućavaju administratorima bolji pregled rada sistema, korisnika, agenata, tehničara i tiketa kroz dashboard i reporting module. Poseban fokus stavljen je na razvoj KPI metrika, dashboard prikaza i izvještaja koji omogućavaju detaljnu analizu stanja sistema kroz različite vremenske periode.

Implementacijom PB-45 Admin Dashboard funkcionalnosti planiran je razvoj centralizovanog administratorskog prikaza sa ključnim metrikama sistema, uključujući pregled ukupnog broja tiketa, distribucije statusa, prosječnog vremena odgovora i rješavanja, opterećenja agenata i najčešćih tipova problema. Dashboard treba omogućiti pregled podataka kroz grafove, KPI kartice i pregled aktivnih korisnika sistema.

Sprint također uključuje implementaciju globalnih vremenskih filtera, drill-down pregleda tiketa i generisanja izvještaja kroz PB-38, PB-39, PB-40, PB-41, PB-43, PB-44 i PB-50 kako bi administratori mogli analizirati podatke kroz različite vremenske periode i pratiti performanse sistema i timova.

Poseban fokus sprinta stavljen je na implementaciju administratorskih funkcionalnosti za upravljanje korisničkim nalozima kroz PB-51, uključujući kreiranje, pregled, uređivanje, deaktivaciju i reaktivaciju korisničkih naloga za agente, tehničare i klijente. Dodatno, sprint uključuje validaciju pristupa sistemu po korisničkim rolama i kontrolu pristupa administratorskim funkcionalnostima.

Sprint uključuje i implementaciju preraspodjele agenata po timovima kroz PB-29 kako bi administratori mogli upravljati organizacijom timova i raspodjelom agenata unutar sistema.

Dodatni fokus sprinta stavljen je na razvoj izvještaja o broju tiketa, statusima tiketa, tipovima problema, prosječnom vremenu rješavanja, prosječnom vremenu prvog odgovora, opterećenju agenata i korisničkim ocjenama kako bi sistem omogućio detaljniji pregled performansi i efikasnosti rada podrške.

Sprint također obuhvata integraciju frontend i backend administratorskih modula, optimizaciju dashboard i reporting endpointa, validaciju administratorskih privilegija i proširenje test coverage-a za nove funkcionalnosti sistema.

---

## Fokus sprinta

- Implementacija administratorskog dashboarda
- KPI metrike i pregled sistema
- Reporting i generisanje izvještaja
- Globalni vremenski filteri
- Drill-down pregled tiketa
- Prosječno vrijeme prvog odgovora
- Upravljanje korisničkim nalozima
- Aktivacija i deaktivacija korisnika
- Upravljanje agentima i tehničarima
- Preraspodjela agenata po timovima
- Validacija administratorskih privilegija
- Proširenje autorizacije po korisničkim rolama
- Integracija frontend i backend administratorskih modula
- Optimizacija dashboard i reporting endpointa
- Refaktorisanje backend logike
- Proširenje unit testova i test coverage-a

---

## Očekivani deliverable-i

- PB-45 Admin Dashboard sa ključnim metrikama
- PB-50 Prosječno vrijeme prvog odgovora
- PB-51 Upravljanje korisničkim nalozima
- PB-52 Upravljanje katalogom paketa i pretplata
- PB-53 Pregled audit log-a aktivnosti
- PB-56 Prilozi na tiketima
- PB-38 Izvještaj o broju tiketa
- PB-39 Izvještaj po statusu tiketa
- PB-40 Izvještaj po tipu problema
- PB-41 Prosječno vrijeme rješavanja tiketa
- PB-43 Izvještaj o opterećenju agenata
- PB-44 Izvještaj o ocjenama korisnika
- PB-29 Preraspodjela agenata po timovima
- Dashboard KPI prikazi
- Reporting endpointi i filteri
- Upravljanje korisnicima i administratorskim funkcionalnostima
- Validacija administratorskih ruta
- Refaktorisani API endpointi
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalnost administratorskog dashboarda, ispravnost izvještaja i KPI metrika, kvalitet administratorskih funkcionalnosti i stabilnost reporting sistema.

Review uključuje provjeru:
- Ispravnosti dashboard KPI metrika
- Funkcionalnosti dashboard prikaza i grafova
- Ispravnosti generisanja izvještaja
- Funkcionalnosti vremenskih filtera
- Funkcionalnosti drill-down pregleda
- Ispravnosti prosječnog vremena prvog odgovora
- Upravljanja korisničkim nalozima
- Upravljanja agentima i tehničarima
- Funkcionalnosti preraspodjele agenata po timovima
- Validacije administratorskih privilegija
- Stabilnosti frontend i backend integracije
- Stabilnosti reporting endpointa
- Pokrivenosti testovima i stabilnosti aplikacije

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije dashboard sistema
- Dogovor oko KPI metrika i reporting logike
- Review sastanci za frontend i backend integraciju
- Koordinacija implementacije administratorskih modula
- Tehnički sastanci za organizaciju API endpointa
- Sastanci vezani za dashboard i reporting funkcionalnosti
- Dogovor oko autorizacije administratorskih ruta
- Koordinacija implementacije izvještaja i filter sistema
- Review sastanci za dashboard i reporting UI
- Sastanci vezani za optimizaciju backend upita
