# Sprint Goal – Sprint 11

## Sprint cilj

Cilj Sprinta 11 je finalizirati sistem kroz implementaciju preostalih feature stavki Product Backloga, proširiti funkcionalnosti tiketa vezivanjem međuzavisnih slučajeva, uvesti SLA praćenje i upozorenja za agente i administratore, omogućiti masovne operacije nad tiketima, proširiti autentikacijske mogućnosti kroz login putem broja telefona i reset lozinke, te implementirati AI chatbot za klijente baziran na Groq integraciji.

Sprint 11 je završni sprint sistema i fokusiran je na podizanje kvalitete cjelokupnog iskustva za sve role: klijente, agente, tehničare i administratore. Implementacijom export funkcionalnosti za izvještaje, SLA mehanizama, bulk akcija i AI chatbota sistem postaje operativno spreman za realno korištenje u helpdesk okruženju.

Poseban fokus sprinta stavljen je na PB-46, odnosno export izvještaja u CSV formatu. Ova funkcionalnost je implementirana kao client-side generisanje CSV fajla direktno u browseru, bez potrebe za novim backend endpointom. Administrator može eksportovati izvještaj odabranog tipa i perioda klikom na uvijek aktivno Export dugme, a fajl sadrži metadata header (naziv izvještaja, period, datum exporta) i tabularni podaci. Implementacija podržava svih 7 tipova izvještaja i uključuje UTF-8 BOM za ispravno otvaranje u Excelu na Windows platformi.

Sprint također uključuje PB-64, implementaciju Linked Tickets funkcionalnosti koja agentima i tehničarima omogućuje kreiranje bidirekcionalnih veza između tiketa koji se odnose na isti problem ili su međusobno zavisni. Veza može biti tipa „duplikat", „nastavak" ili „vezano uz", a sistem mora spriječiti ciklične veze i samopovezivanje.

PB-65 uvodi SLA praćenje i upozorenja. Sistem mora pratiti SLA rokove po prioritetu tiketa, vizualno upozoravati agente i administratore kada se rok bliži ili je prekoračen, evidentirati SLA breacheve i ne dopuštati dodjeljivanje novih tiketa agentima čiji bi workload prekršio SLA.

PB-66 implementira bulk akcije na tiketima. Agenti i administratori mogu odabirati više tiketa checkboxima na listi i izvršavati masovne operacije: zatvaranje, promjenu prioriteta, dodjelu agentu i prosljeđivanje timu. Sistem zahtijeva potvrdu za destruktivne operacije i prikazuje sažetak rezultata.

PB-67 proširuje autentikaciju klijenta na mogućnost prijave putem broja telefona u međunarodnom formatu kao alternativu emailu. PB-68 implementira tok za oporavak lozinke: korisnik prima link putem emaila, otvara stranicu s vremenski ograničenim tokenom i unosi novu lozinku.

PB-69 implementira AI chatbot za klijente koristeći Groq LLM integraciju. Chatbot odgovara na česta pitanja koristeći FAQ sadržaj i internu bazu znanja, a ako ne može riješiti problem, nudi klijentu mogućnost kreiranja tiketa s prethodno prikupljenim kontekstom.

Sprint obuhvata dokumentovanje svih implementiranih promjena kroz Sprint Backlog, Decision Log, AI Usage Log i Proof of Testing.

---

## Fokus sprinta

- Implementacija CSV exporta za svih 7 tipova izvještaja (PB-46) — Done
- Client-side CSV generisanje s metadata headerom i UTF-8 BOM podrškom
- Implementacija Linked Tickets — bidirekciona veza između tiketa (PB-64)
- Prevencija cikličnih veza i samopovezivanja tiketa
- Implementacija SLA praćenja po prioritetu tiketa (PB-65)
- Vizualna SLA upozorenja i evidencija SLA breacheva
- Bulk odabir tiketa checkboxima i masovne operacije (PB-66)
- Potvrda za destruktivne bulk operacije i sažetak rezultata
- Proširenje autentikacije na login putem broja telefona (PB-67)
- Reset lozinke putem emaila s vremenski ograničenim tokenom (PB-68)
- AI Chatbot za klijente s Groq LLM integracijom (PB-69)
- Integracija chatbota s FAQ sadržajem i knowledge baseom
- Prebacivanje iz chatbota na kreiranje tiketa s kontekstom
- Proširenje test coverage-a za sve Sprint 11 funkcionalnosti
- Stabilizacija i finalizacija sistema

---

## Očekivani deliverable-i

- PB-46 CSV export izvještaja — Done
- PB-64 Linked Tickets implementacija
- PB-65 SLA praćenje i upozorenja
- PB-66 Bulk akcije na tiketima
- PB-67 Login via broj telefona
- PB-68 Reset lozinke putem emaila
- PB-69 AI Chatbot za klijente (Groq)
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalnost CSV exporta za sve tipove izvještaja, ispravnost Linked Tickets veza, tačnost SLA praćenja i upozorenja, stabilnost bulk operacija, ispravnost login putem telefona, funkcionalnost reset toka lozinke i kvalitet AI chatbot iskustva za klijente.

Review uključuje provjeru:

- Preuzimanja CSV fajla s ispravnim sadržajem za svih 7 tipova izvještaja
- Ispravnosti metadata headera u CSV fajlu (naziv, period, datum)
- Ispravnog otvaranja CSV fajla u Excelu (UTF-8 BOM)
- Kreiranje i prikaz bidirekcionalnih veza između tiketa
- Prevencije cikličnih veza i samopovezivanja
- Vizualnog prikaza SLA statusa na tiketima (boje, countdown)
- Notifikacija i upozorenja za SLA prekoračenja
- Checkboxa za odabir na listi tiketa
- Bulk zatvaranja, promjene prioriteta, dodjele i prosljeđivanja
- Login putem broja telefona u međunarodnom formatu
- Reset toka lozinke — email, token, nova lozinka
- AI chatbot odgovora na FAQ pitanja
- Prebacivanja iz chatbota na kreiranje tiketa s kontekstom
- Pokrivenosti testovima i stabilnosti aplikacije

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije CSV export funkcionalnosti
- Dogovor oko client-side vs server-side CSV generisanja
- Koordinacija implementacije Linked Tickets
- Dogovor oko SLA politika i rokova po prioritetu
- Tehnički sastanci za bulk akcije i performance razmatranja
- Koordinacija Groq AI chatbot integracije
- Dogovor oko autentikacijskih proširenja (telefon, reset)
- Review sastanci za finalizaciju sistema
- Koordinacija dokumentacije Sprint Backloga, Decision Loga i AI Usage Loga
