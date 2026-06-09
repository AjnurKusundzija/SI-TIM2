## Sprint cilj

Cilj Sprinta 11 je finalizirati sistem kroz implementaciju preostalih funkcionalnosti Product Backloga, dodatno unaprijediti ticket workflow, proširiti autentikacijske mogućnosti korisnika i implementirati SLA mehanizme za praćenje kvaliteta usluge.

Sprint 11 predstavlja završni razvojni sprint projekta i fokusiran je na funkcionalnosti koje sistem približavaju realnom produkcijskom okruženju. Nakon što su prethodni sprintovi uspostavili osnovnu arhitekturu sistema, ticket workflow, korisničke role, administrativne module, AI prijedloge odgovora i AI administrativne uvide, cilj ovog sprinta je dodatno unaprijediti operativnu efikasnost sistema i kvalitet korisničkog iskustva za sve učesnike procesa podrške.

Poseban fokus sprinta stavljen je na PB-46, odnosno export izvještaja u CSV formatu. Ova funkcionalnost omogućava administratorima jednostavno preuzimanje izvještaja radi daljnje analize, arhiviranja i dijeljenja sa menadžmentom. Implementacija je realizovana kroz client-side generisanje CSV fajla direktno u browseru bez potrebe za dodatnim backend endpointima, čime se smanjuje opterećenje serverske infrastrukture. Sistem podržava svih sedam tipova izvještaja i osigurava kompatibilnost sa Excel alatima korištenjem UTF-8 BOM formata.

Jedan od najvažnijih dijelova sprinta predstavlja PB-65, implementacija SLA (Service Level Agreement) mehanizma. Sistem mora pratiti definisane rokove za obradu i rješavanje tiketa prema njihovom prioritetu, prikazivati upozorenja kada se SLA približava isteku te evidentirati sva prekoračenja. Implementacijom SLA logike omogućava se kvalitetnije upravljanje korisničkom podrškom i objektivno mjerenje performansi sistema i zaposlenika.

Sprint uključuje i unapređenje autentikacijskog modula kroz PB-67. Korisnicima će biti omogućena prijava putem broja telefona kao alternative email adresi, čime se povećava fleksibilnost sistema i prilagođava različitim navikama korisnika.

Sprint također obuhvata dodatno testiranje, stabilizaciju sistema, uklanjanje preostalih tehničkih dugova, proširenje test coverage-a i završnu validaciju svih implementiranih funkcionalnosti. Posebna pažnja bit će posvećena kompatibilnosti novih modula sa postojećim sistemom, performansama aplikacije i kvalitetu korisničkog iskustva.

Kroz Sprint 11 tim završava razvoj svih planiranih funkcionalnosti projekta i priprema sistem za završnu demonstraciju, evaluaciju i potencijalnu upotrebu u realnom okruženju. Fokus sprinta nije samo na implementaciji novih mogućnosti, nego i na finalnom poliranju sistema, dokumentaciji, testiranju i osiguravanju visokog nivoa kvaliteta kompletne aplikacije.

---

## Fokus sprinta

- Implementacija CSV exporta za svih 7 tipova izvještaja (PB-46) — Done
- Client-side CSV generisanje s metadata headerom i UTF-8 BOM podrškom
- Export izvještaja kompatibilan sa Microsoft Excel alatima
- Implementacija SLA praćenja po prioritetu tiketa (PB-65)
- Vizualna SLA upozorenja i evidencija SLA breach događaja
- Praćenje vremena odgovora i vremena rješavanja tiketa
- Proširenje autentikacije na login putem broja telefona (PB-67)
- Validacija međunarodnog formata telefonskih brojeva
- Završno testiranje sistema i regresiono testiranje
- Proširenje unit, integracionih i end-to-end testova
- Finalizacija sistema i priprema za završnu demonstraciju

---

## Očekivani deliverable-i

- PB-46 CSV export izvještaja — Done
- PB-65 SLA praćenje i upozorenja
- PB-67 Login via broj telefona
- SLA monitoring i upozorenja na dashboardima
- Nadograđeni autentikacijski sistem
- Prošireni unit testovi
- Prošireni integracioni testovi
- Završni Proof of Testing dokument
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Dokumentacija implementiranih funkcionalnosti
- Finalizovan sistem spreman za završnu prezentaciju
