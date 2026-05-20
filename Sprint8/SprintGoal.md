# Sprint Goal – Sprint 8

## Sprint cilj

Cilj sprinta je implementirati sistem notifikacija koji pravovremeno obavještava sve korisničke role o događajima na tiketima, proširiti korisničko iskustvo kroz upravljanje profilom i pregled paketa te pretplata, omogućiti agentima i tehničarima uvid u vlastitu statistiku rada, te implementirati ocjenjivanje tiketa i ažuriranje statusa tiketa od strane tehničara.

Sprint je fokusiran na zaokruživanje korisničkog i tehničarskog iskustva unutar sistema podrške kako bi aplikacija postala funkcionalno kompletnija i bliža realnim poslovnim procesima customer support sistema. Poseban fokus stavljen je na unapređenje komunikacije između korisnika, agenata i tehničara kroz sistem notifikacija i proširenje postojećeg ticket workflow-a dodatnim funkcionalnostima koje omogućavaju bolju organizaciju rada i kvalitetnije korisničko iskustvo.

Korisnik kroz Sprint 8 dobija mogućnost ocjenjivanja usluge nakon zatvaranja tiketa, pregled aktivnih paketa i pretplata, upravljanje vlastitim profilom i primanje pravovremenih obavještenja o svim relevantnim promjenama unutar sistema bez potrebe za ručnim praćenjem statusa prijava. Time se unapređuje transparentnost rada sistema i povećava kvalitet interakcije između korisnika i support tima.

Tehničari i agenti kroz ovaj sprint dobivaju dodatne workflow funkcionalnosti koje uključuju ažuriranje statusa tiketa, pregled detaljnijih korisničkih informacija i historije prijava, kao i uvid u vlastitu statistiku rada kroz profilnu stranicu i dashboard prikaze. Statistički modul uključuje pregled broja aktivnih i zatvorenih tiketa, prosječnog vremena prvog odgovora, prosječnog vremena rješavanja problema i korisničkih ocjena, čime se postavlja osnova za budući izvještajni i analitički sistem planiran za naredne sprintove.

Poseban fokus stavljen je na implementaciju notifikacijskog modula čija je infrastruktura pripremljena u Sprintu 7, ali čija poslovna logika i frontend prikaz nisu bili implementirani. Sprint 8 donosi kompletnu implementaciju generisanja, prikaza i upravljanja notifikacijama za sve korisničke role koristeći SignalR real-time komunikaciju i backend event logiku povezanu sa ticket workflow sistemom.

Implementacijom notifikacijskog sistema omogućava se:
- automatsko obavještavanje korisnika kada agent odgovori na tiket,
- obavještavanje agenata i tehničara o novododijeljenim tiketima,
- generisanje notifikacija pri promjeni statusa tiketa,
- prikaz badge indikatora za nepročitane notifikacije,
- pregled historije notifikacija i označavanje notifikacija kao pročitanih.

Sprint također uključuje proširenje postojećeg chat i komunikacijskog sistema kroz sistemske poruke pri prosljeđivanju tiketa i generisanju workflow aktivnosti. Time se unapređuje pregled historije aktivnosti nad tiketima i omogućava kvalitetnije praćenje rada agenata i tehničara unutar sistema.

Poseban fokus stavljen je i na unapređenje lifecycle upravljanja tiketima kroz mogućnost ažuriranja statusa tiketa od strane tehničara, validaciju dozvoljenih tranzicija statusa i automatsko generisanje odgovarajućih workflow događaja i notifikacija. Cilj ovih funkcionalnosti je unaprijediti organizaciju rada support sistema i omogućiti preciznije praćenje stanja korisničkih prijava.

Dodatni cilj sprinta jeste implementacija funkcionalnosti ocjenjivanja zatvorenih tiketa kako bi korisnici mogli dati povratnu informaciju o kvalitetu rada agenata i tehničara. Implementacijom sistema ocjenjivanja omogućava se dugoročno praćenje kvaliteta support procesa i prikupljanje podataka potrebnih za buduće izvještaje i analitiku sistema.

Sprint također obuhvata unapređenje sigurnosti i organizacije korisničkih podataka kroz implementaciju funkcionalnosti za promjenu email adrese i lozinke korisnika, validaciju pristupa podacima prema korisničkim rolama i stabilizaciju autorizacijskog sistema za nove funkcionalnosti uvedene u ovom sprintu.

Kroz funkcionalnosti pregleda korisničkih profila agentima se omogućava pregled korisničkih podataka, historije tiketa i aktivnih paketa korisnika, čime se unapređuje efikasnost komunikacije sa korisnicima i omogućava kvalitetnija analiza prethodnih problema i zahtjeva.

Sprint dodatno uključuje proširenje funkcionalnosti pregleda paketa i pretplata za korisnike kako bi korisnici imali centralizovan pregled aktivnih usluga, statusa pretplate i osnovnih informacija vezanih za njihove pakete unutar sistema.

Pored implementacije novih funkcionalnosti, sprint obuhvata refaktorisanje frontend komponenti, proširenje backend API endpointa, optimizaciju organizacije workflow logike i dodatnu stabilizaciju frontend-backend komunikacije kako bi sistem ostao skalabilan i spreman za naredna proširenja.

Poseban fokus sprinta stavljen je na kvalitet implementacije i testiranja novih funkcionalnosti. Sprint uključuje implementaciju dodatnih unit testova, validaciju novih workflow scenarija, testiranje autorizacije po korisničkim rolama i stabilizaciju notifikacijskog i statističkog sistema.

Kroz Sprint 8 tim radi na završavanju ključnih korisničkih i tehničarskih funkcionalnosti ticket sistema i pripremi infrastrukture za naredne sprintove koji uključuju naprednije dashboard funkcionalnosti, AI recommendation module, administratorske izvještaje, napredniju analitiku i dodatna proširenja support workflow sistema.

---

## Fokus sprinta

- Implementacija notifikacijskog sistema (generisanje, prikaz, upravljanje)
- Ocjenjivanje tiketa od strane korisnika nakon zatvaranja
- Ažuriranje statusa tiketa od strane tehničara
- Statistika rada na profilnoj stranici agenta i tehničara
- Upravljanje korisničkim profilom (email, lozinka)
- Pregled korisničkih profila i historije tiketa od strane agenta
- Prikaz aktivnih paketa i pretplata za korisnike
- Integracija notifikacija sa postojećim ticket workflow-om
- Proširenje autorizacije i pristupa po korisničkim rolama
- Implementacija i proširenje unit testova za nove funkcionalnosti

---

## Očekivani deliverable-i

- PB-49 Notifikacije — implementiran notifikacijski sistem za sve role s real-time SignalR isporukom
- PB-42 Statistika agenta i tehničara — lična statistika na profilnoj stranici i dashboardu
- SB-08 Sistemske poruke u chatu pri prosljeđivanju tiketa (proširenje PB-31)
- PB-36 Ažuriranje statusa tiketa — tehničar mijenja status dodijeljenih tiketa
- PB-26 Ocjenjivanje tiketa — korisnik ocjenjuje zatvoreni tiket (1–5, opcionalni komentar)
- PB-20 Upravljanje korisničkim profilom — promjena email adrese i lozinke
- PB-34 Pregled korisničkih profila (agent) — agent vidi profil, historiju tiketa i pakete korisnika
- PB-21 Prikaz paketa i pretplata — korisnik vidi svoje aktivne pakete i pretplate
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalna implementacija notifikacijskog sistema, ispravnost generisanja notifikacija po svim predviđenim događajima, kvalitet implementacije korisničkih profila i statistike, te uspješnost integracije ocjenjivanja tiketa i ažuriranja statusa.

Review uključuje provjeru:
- Ispravnosti generisanja notifikacija po svim tipovima (`TICKET_ASSIGNED`, `TICKET_FORWARDED`, `STATUS_CHANGED`, `TICKET_RESPONSE`, `TICKET_CLOSED`)
- Ispravnosti prikaza i upravljanja notifikacijama (badge, označavanje kao pročitano)
- Funkcionalnosti ocjenjivanja tiketa i ograničenja (jednom po tiketu, samo zatvoreni tiketi)
- Ispravnosti ažuriranja statusa tiketa od strane tehničara i generisanja odgovarajuće notifikacije
- Prikaza statistike na profilnoj stranici agenta i tehničara
- Funkcionalnosti promjene email adrese i lozinke korisnika
- Prikaza korisničkih profila i historije tiketa od strane agenta
- Prikaza paketa i pretplata korisnika
- Ispravnosti autorizacije po korisničkim rolama za sve nove funkcionalnosti
- Pokrivenosti testovima i stabilnosti aplikacije

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije notifikacijskog sistema
- Dogovor oko redefinisanja PB-42
- Review sastanci za backend i frontend integraciju
- Sastanci vezani za SignalR notifikacijsku infrastrukturu
- Koordinacija implementacije statističkog modula
- Review sastanci za profile i autorizaciju korisnika
- Tehnički sastanci za organizaciju API endpointa i event logike
- Dogovor oko strukture notifikacija i workflow događaja
- Sastanci vezani za planiranje testiranja novih funkcionalnosti
- Koordinacija implementacije ocjenjivanja tiketa i status workflow-a
- Review sastanci za statistiku i dashboard prikaze
- Koordinacija implementacije profilnih i korisničkih funkcionalnosti
