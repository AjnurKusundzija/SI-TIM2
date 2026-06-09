## Sprint cilj

Cilj Sprinta 11 je finalizirati sistem kroz implementaciju preostalih funkcionalnosti Product Backloga, dodatno unaprijediti ticket workflow, proširiti autentikacijske mogućnosti korisnika, implementirati SLA mehanizme za praćenje kvaliteta usluge, omogućiti masovne operacije nad tiketima te razviti AI chatbot za klijente koji će pružati podršku kroz postojeću bazu znanja i FAQ sadržaj.

Sprint 11 predstavlja završni razvojni sprint projekta i fokusiran je na funkcionalnosti koje sistem približavaju realnom produkcijskom okruženju. Nakon što su prethodni sprintovi uspostavili osnovnu arhitekturu sistema, ticket workflow, korisničke role, administrativne module, AI prijedloge odgovora i AI administrativne uvide, cilj ovog sprinta je dodatno unaprijediti operativnu efikasnost sistema i kvalitet korisničkog iskustva za sve učesnike procesa podrške.

Poseban fokus sprinta stavljen je na PB-46, odnosno export izvještaja u CSV formatu. Ova funkcionalnost omogućava administratorima jednostavno preuzimanje izvještaja radi daljnje analize, arhiviranja i dijeljenja sa menadžmentom. Implementacija je realizovana kroz client-side generisanje CSV fajla direktno u browseru bez potrebe za dodatnim backend endpointima, čime se smanjuje opterećenje serverske infrastrukture. Sistem podržava svih sedam tipova izvještaja i osigurava kompatibilnost sa Excel alatima korištenjem UTF-8 BOM formata.

Sprint također uvodi PB-64 Linked Tickets funkcionalnost kojom se omogućava povezivanje međusobno zavisnih ili povezanih tiketa. U realnim helpdesk okruženjima često se javlja više prijava koje se odnose na isti problem ili incident, zbog čega je važno omogućiti vezivanje tiketa kroz jasno definisane relacije poput „duplikat“, „nastavak“ ili „vezano uz“. Implementacija ove funkcionalnosti omogućava bolju organizaciju rada, lakše praćenje povezanih slučajeva i smanjenje dupliranja aktivnosti agenata i tehničara.

Jedan od najvažnijih dijelova sprinta predstavlja PB-65, implementacija SLA (Service Level Agreement) mehanizma. Sistem mora pratiti definisane rokove za obradu i rješavanje tiketa prema njihovom prioritetu, prikazivati upozorenja kada se SLA približava isteku te evidentirati sva prekoračenja. Implementacijom SLA logike omogućava se kvalitetnije upravljanje korisničkom podrškom i objektivno mjerenje performansi sistema i zaposlenika.

PB-66 uvodi podršku za masovne operacije nad tiketima. U okruženjima sa velikim brojem zahtjeva administratori i agenti često moraju izvršavati iste akcije nad više tiketa istovremeno. Omogućavanjem bulk zatvaranja, promjene prioriteta, dodjele agenata i prosljeđivanja timovima značajno se ubrzava svakodnevni rad i smanjuje broj ponavljajućih operacija.

Sprint uključuje i unapređenje autentikacijskog modula kroz PB-67 i PB-68. Korisnicima će biti omogućena prijava putem broja telefona kao alternative email adresi, čime se povećava fleksibilnost sistema i prilagođava različitim navikama korisnika. Dodatno, implementira se kompletan proces resetovanja lozinke putem emaila i vremenski ograničenih sigurnosnih tokena, čime se unapređuje sigurnost i samostalnost korisnika pri upravljanju vlastitim nalogom.

Poseban značaj u Sprintu 11 ima PB-69, AI Chatbot za klijente. Ova funkcionalnost predstavlja prirodan nastavak AI mogućnosti uvedenih u prethodnim sprintovima. Chatbot koristi Groq LLM integraciju i postojeću FAQ bazu znanja kako bi odgovarao na najčešća pitanja korisnika bez potrebe za uključivanjem agenata. Kada chatbot procijeni da problem zahtijeva ljudsku intervenciju, korisniku će ponuditi kreiranje tiketa uz automatsko prosljeđivanje prethodno prikupljenog konteksta, čime se dodatno ubrzava proces rješavanja zahtjeva.

Sprint također obuhvata dodatno testiranje, stabilizaciju sistema, uklanjanje preostalih tehničkih dugova, proširenje test coverage-a i završnu validaciju svih implementiranih funkcionalnosti. Posebna pažnja bit će posvećena kompatibilnosti novih modula sa postojećim sistemom, performansama aplikacije i kvalitetu korisničkog iskustva.

Kroz Sprint 11 tim završava razvoj svih planiranih funkcionalnosti projekta i priprema sistem za završnu demonstraciju, evaluaciju i potencijalnu upotrebu u realnom okruženju. Fokus sprinta nije samo na implementaciji novih mogućnosti, nego i na finalnom poliranju sistema, dokumentaciji, testiranju i osiguravanju visokog nivoa kvaliteta kompletne aplikacije.

---

## Fokus sprinta

- Implementacija CSV exporta za svih 7 tipova izvještaja (PB-46) — Done
- Client-side CSV generisanje s metadata headerom i UTF-8 BOM podrškom
- Export izvještaja kompatibilan sa Microsoft Excel alatima
- Implementacija Linked Tickets funkcionalnosti (PB-64)
- Kreiranje bidirekcionalnih veza između povezanih tiketa
- Prevencija cikličnih veza i samopovezivanja tiketa
- Implementacija SLA praćenja po prioritetu tiketa (PB-65)
- Vizualna SLA upozorenja i evidencija SLA breach događaja
- Praćenje vremena odgovora i vremena rješavanja tiketa
- Bulk odabir tiketa checkboxima i masovne operacije (PB-66)
- Bulk zatvaranje tiketa
- Bulk promjena prioriteta
- Bulk dodjela agenata
- Bulk prosljeđivanje timovima
- Potvrda za destruktivne bulk operacije i sažetak rezultata
- Proširenje autentikacije na login putem broja telefona (PB-67)
- Validacija međunarodnog formata telefonskih brojeva
- Reset lozinke putem emaila s vremenski ograničenim tokenom (PB-68)
- Sigurno upravljanje tokenima za reset lozinke
- AI Chatbot za klijente s Groq LLM integracijom (PB-69)
- Integracija chatbota sa FAQ sadržajem i internom bazom znanja
- Prebacivanje korisnika iz chatbota na kreiranje tiketa s kontekstom
- Stabilizacija svih AI funkcionalnosti implementiranih u prethodnim sprintovima
- Završno testiranje sistema i regresiono testiranje
- Proširenje unit, integracionih i end-to-end testova
- Finalizacija sistema i priprema za završnu demonstraciju

---

## Očekivani deliverable-i

- PB-46 CSV export izvještaja — Done
- PB-64 Linked Tickets implementacija
- PB-65 SLA praćenje i upozorenja
- PB-66 Bulk akcije na tiketima
- PB-67 Login putem broja telefona
- PB-68 Reset lozinke putem emaila
- PB-69 AI Chatbot za klijente (Groq)
- Linked Tickets backend i frontend integracija
- SLA monitoring i upozorenja na dashboardima
- Bulk operations modul za administratore i agente
- Nadograđeni autentikacijski sistem
- Chatbot integrisan sa FAQ i Knowledge Base modulima
- Prošireni unit testovi
- Prošireni integracioni testovi
- Završni Proof of Testing dokument
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Dokumentacija implementiranih funkcionalnosti
- Finalizovan sistem spreman za završnu prezentaciju
